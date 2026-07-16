# ダーツプロ総合アフィリエイトサイト 設計書

> プロジェクト名: **dart-portal**  
> 目的: 人気ダーツプロ・来店イベント・店舗・ギアを統合し、ファンが推しを逃さず、アフィリエイトで収益化する総合ポータル

---

## 1. 全体構成

```
┌─────────────────────────────────────────────────────────────────┐
│                        フロントエンド (Next.js)                    │
│  トップ / プロ一覧 / イベント / 店舗 / ギア / 特集 / マイ推し      │
└────────────────────────────┬────────────────────────────────────┘
                             │ REST API
┌────────────────────────────▼────────────────────────────────────┐
│                     API サーバー (FastAPI)                       │
│  /pros /events /shops /gear /recommendations /offers             │
└──────┬──────────────────┬──────────────────┬────────────────────┘
       │                  │                  │
┌──────▼──────┐  ┌────────▼────────┐  ┌─────▼──────────────────┐
│ PostgreSQL  │  │ Redis (cache)   │  │ 管理画面 CMS (React)    │
│ + JSON seed │  │ 通知キュー       │  │ プロ/イベント/店舗/ギア  │
└─────────────┘  └─────────────────┘  └────────────────────────┘
       ▲
┌──────┴──────────────────────────────────────────────────────────┐
│                    データ収集ワーカー (Cron / Celery)              │
│  スクレイパー: 自遊空間 / BAGUS / 店舗公式 / 六甲ボウル等          │
│  SNS: X API v2 / Instagram Graph API                            │
│  OCR: Google Vision / Tesseract (画像投稿から日程抽出)             │
└─────────────────────────────────────────────────────────────────┘
```

### 技術スタック（推奨）

| レイヤ | 技術 |
|--------|------|
| Frontend | Next.js 15, TypeScript, Tailwind CSS |
| API | FastAPI, Pydantic v2, SQLAlchemy |
| DB | PostgreSQL 16（本番）/ JSON seed（MVP） |
| Cache/Queue | Redis, Celery Beat |
| CMS | React Admin または Payload CMS |
| OCR | Google Cloud Vision API |
| Hosting | Vercel (FE) + Railway/Fly.io (API) |
| Analytics | Google Analytics 4, Search Console |

### 現状（MVP 実装済み）

- `scrape_events.py` — 神戸六甲ボウルイベント取得
- `main.py` — `POST /offers` レコメンド API
- `events.json` — スクレイプ結果キャッシュ

---

## 2. データベース設計（JSON スキーマ）

### 2.1 Pro（ダーツプロ）

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "type": "object",
  "required": ["id", "name", "league", "popularity_score"],
  "properties": {
    "id": { "type": "integer" },
    "name": { "type": "string", "example": "熊谷麻音" },
    "name_kana": { "type": "string" },
    "league": { "type": "string", "enum": ["JAPAN", "PERFECT", "OTHER"] },
    "gender": { "type": "string", "enum": ["female", "male"] },
    "profile_image_url": { "type": "string", "format": "uri" },
    "bio": { "type": "string" },
    "sns": {
      "type": "object",
      "properties": {
        "x": { "type": "string" },
        "instagram": { "type": "string" },
        "youtube": { "type": "string" },
        "tiktok": { "type": "string" }
      }
    },
    "followers": {
      "type": "object",
      "properties": {
        "x": { "type": "integer" },
        "instagram": { "type": "integer" }
      }
    },
    "media_appearances": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "title": { "type": "string" },
          "media_type": { "type": "string", "enum": ["gravure", "tv", "web", "magazine"] },
          "url": { "type": "string" },
          "published_at": { "type": "string", "format": "date" }
        }
      }
    },
    "gear_ids": { "type": "array", "items": { "type": "integer" } },
    "popularity_score": { "type": "number", "description": "SNS + 検索 + イベント頻度の合成スコア" },
    "tags": { "type": "array", "items": { "type": "string" }, "example": ["グラビア", "人気女子プロ"] }
  }
}
```

### 2.2 Event（来店・大会イベント）

```json
{
  "type": "object",
  "required": ["id", "title", "pro_ids", "shop_id", "start_date", "source"],
  "properties": {
    "id": { "type": "integer" },
    "title": { "type": "string" },
    "pro_ids": { "type": "array", "items": { "type": "integer" } },
    "shop_id": { "type": "integer" },
    "area": { "type": "string", "example": "Hyogo" },
    "prefecture": { "type": "string", "example": "兵庫県" },
    "start_date": { "type": "string", "format": "date" },
    "end_date": { "type": "string", "format": "date" },
    "start_time": { "type": "string", "example": "13:00" },
    "event_type": { "type": "string", "enum": ["visit", "challenge", "tournament", "exhibition"] },
    "ticket_url": { "type": "string", "format": "uri" },
    "ticket_aff": { "type": "boolean" },
    "detail_url": { "type": "string", "format": "uri" },
    "source": { "type": "string", "enum": ["scraper", "sns_ocr", "cms", "api"] },
    "source_url": { "type": "string" },
    "status": { "type": "string", "enum": ["scheduled", "cancelled", "finished"] },
    "image_url": { "type": "string" }
  }
}
```

### 2.3 Shop（店舗）

```json
{
  "type": "object",
  "required": ["id", "name", "chain", "prefecture"],
  "properties": {
    "id": { "type": "integer" },
    "name": { "type": "string" },
    "chain": { "type": "string", "enum": ["自遊空間", "BAGUS", "ダーツバー", "その他"] },
    "prefecture": { "type": "string" },
    "city": { "type": "string" },
    "address": { "type": "string" },
    "lat": { "type": "number" },
    "lng": { "type": "number" },
    "official_url": { "type": "string" },
    "event_page_url": { "type": "string" },
    "scraper_key": { "type": "string", "description": "スクレイパー識別子" }
  }
}
```

### 2.4 Gear（ギア）

```json
{
  "type": "object",
  "required": ["id", "name", "category", "price", "affiliate_links"],
  "properties": {
    "id": { "type": "integer" },
    "name": { "type": "string" },
    "category": { "type": "string", "enum": ["barrel", "flight", "shaft", "case", "goods"] },
    "brand": { "type": "string" },
    "pro_id": { "type": "integer" },
    "price": { "type": "integer" },
    "image_url": { "type": "string" },
    "affiliate_links": {
      "type": "object",
      "properties": {
        "amazon": { "type": "string" },
        "rakuten": { "type": "string" },
        "dartshop": { "type": "string" }
      }
    },
    "is_signature_model": { "type": "boolean" }
  }
}
```

### 2.5 Fan（ユーザー / 推し設定）

```json
{
  "type": "object",
  "properties": {
    "user_id": { "type": "string" },
    "area": { "type": "string" },
    "favorite_pro_ids": { "type": "array", "items": { "type": "integer" } },
    "budget": { "type": "integer" },
    "notify_channels": {
      "type": "object",
      "properties": {
        "email": { "type": "boolean" },
        "line": { "type": "boolean" },
        "web_push": { "type": "boolean" }
      }
    }
  }
}
```

---

## 3. API 設計

### 3.1 エンドポイント一覧

| Method | Path | 説明 |
|--------|------|------|
| GET | `/pros` | プロ一覧（ランキング順） |
| GET | `/pros/{id}` | プロ詳細 |
| GET | `/events` | イベント一覧（フィルタ: area, pro_id, date_from） |
| GET | `/events/{id}` | イベント詳細 |
| GET | `/shops` | 店舗一覧 |
| GET | `/shops/{id}` | 店舗詳細 + 関連イベント |
| GET | `/gear` | ギア一覧（フィルタ: pro_id, category） |
| GET | `/gear/{id}` | ギア詳細 |
| POST | `/recommendations` | 推し × イベント × ギア 総合レコメンド |
| POST | `/offers` | **実装済み** — 予算ベースのオファー生成 |
| POST | `/fans/register` | 推し登録 + 通知設定 |
| GET | `/rankings/pros` | 人気ランキング（週次/月次） |

### 3.2 レスポンス例

**GET /pros?sort=popularity&limit=5**

```json
[
  {
    "id": 101,
    "name": "熊谷麻音",
    "league": "JAPAN",
    "popularity_score": 92.5,
    "followers": { "x": 45000, "instagram": 82000 },
    "tags": ["グラビア", "人気女子プロ"],
    "upcoming_events_count": 3
  }
]
```

**GET /events?area=Hyogo&pro_id=101&date_from=2026-07-17**

```json
[
  {
    "id": 1,
    "title": "熊谷麻音プロ チャレンジマッチ",
    "pro_ids": [101],
    "shop_id": 12,
    "shop_name": "自遊空間 神戸店",
    "start_date": "2026-08-15",
    "event_type": "challenge",
    "detail_url": "https://...",
    "ticket_aff": true
  }
]
```

**POST /recommendations**

```json
// Request
{
  "area": "Hyogo",
  "favorite_pro_ids": [101, 205],
  "budget": 25000
}

// Response
{
  "events": [...],
  "gear_bundles": [
    {
      "pro_id": 101,
      "pro_name": "熊谷麻音",
      "items": [...],
      "total_price": 22000,
      "affiliate_total_commission_est": 1100
    }
  ],
  "featured_media": [...],
  "score": 94.2
}
```

**POST /offers（実装済み）**

```json
// Request
{ "area": "Hyogo", "favorite_pro_ids": [101], "budget": 20000 }

// Response
[
  {
    "event": { "id": 1, "title": "寺下智香プロ７月度日程", "pro_id": 101, "area": "Hyogo", "date": "2026-07-31" },
    "bundle": [{ "id": 1, "name": "プロモデルバレル", "price": 12000 }],
    "total": 17000,
    "score": 86.0
  }
]
```

---

## 4. 管理画面（CMS）設計

### 4.1 画面構成

```
/admin
├── ダッシュボード（KPI: PV, アフィクリック, 新規イベント数）
├── プロ管理
│   ├── 一覧（検索・ソート・人気スコア編集）
│   ├── 新規/編集（SNS, メディア出演, ギア紐付け）
│   └── ランキングプレビュー
├── イベント管理
│   ├── 一覧（ソース: scraper / sns / cms フィルタ）
│   ├── 新規/編集
│   └── スクレイプ結果の承認キュー（要レビュー）
├── 店舗管理
│   ├── 一覧 / 新規 / 編集
│   └── スクレイパー設定（URL, セレクタ）
├── ギア管理
│   ├── 一覧 / 新規 / 編集
│   └── アフィリエイトリンク一括更新
├── 特集ページ管理（SEO用）
└── 設定（通知テンプレ, API キー, Cron スケジュール）
```

### 4.2 権限

| ロール | 権限 |
|--------|------|
| admin | 全操作 |
| editor | プロ/イベント/ギア編集 |
| reviewer | スクレイプ結果の承認のみ |

---

## 5. UI/UX 設計（トップページ）

### 5.1 セクション構成

```
[ヘッダー] ロゴ | プロ | イベント | 店舗 | ギア | 特集 | 🔍 | マイ推し

[ヒーロー]
  「推しプロのイベント、もう逃さない」
  エリア選択 + 推しプロ選択 → 即レコメンド表示

[人気プロランキング TOP10]
  カード: 写真 / 名前 / リーグ / 次回イベント / SNS

[今週の来店イベント]
  地域タブ（関東/関西/...）+ 日付ソート
  カード: プロ名 / 店舗 / 日時 / 詳細リンク

[グラビア・メディア特集]
  「話題の女子プロ特集」→ SEO ランディング

[推しプロのギア]
  プロ別タブ + Amazon/楽天 CTA

[店舗マップ]
  自遊空間 / BAGUS ピン表示

[フッター] SNS / プライバシー / アフィリエイト表記
```

### 5.2 UX 原則

- **3タップ以内**で推しプロの次回イベントに到達
- イベントカードに **「通知を受け取る」** ワンクリック
- ギアは **イベント詳細ページ内** に関連商品を表示（コンバージョン最大化）
- モバイルファースト（ダーツファンの SNS 流入を想定）

---

## 6. 拡散戦略（SEO / SNS）

### SEO

| 施策 | 内容 |
|------|------|
| 特集ページ | `/features/gravure-pros`, `/features/{pro-name}` |
| 構造化データ | `Event`, `Person`, `Product` の JSON-LD |
| 地域 LP | `/events/osaka`, `/events/tokyo` |
| 更新頻度 | イベントページを毎日自動更新 → クロール促進 |
| 内部リンク | プロ ↔ イベント ↔ 店舗 ↔ ギア の相互リンク |

### SNS

| プラットフォーム | 施策 |
|------------------|------|
| X | 新規イベント自動投稿（プロタグ付き）、週間ランキング画像 |
| Instagram | プロ別イベントカード画像の自動生成（OGP） |
| LINE | 推し登録ユーザへの前日リマインド（将来） |

### バズ設計

- **週間人気プロランキング** → X で引用されやすい形式
- **「来週会えるプロ」** 定期投稿シリーズ
- グラビア特集 → 検索流入 + SNS シェア

---

## 7. アフィリエイト戦略

### 7.1 収益導線

```
イベント詳細 → チケット/予約リンク（店舗アフィ / 自遊空間）
     ↓
プロ詳細 → 使用ギア → Amazon / 楽天 / ダーツショップ
     ↓
レコメンド → 予算内ギアバンドル → まとめ買い CTA
```

### 7.2 最適化

- ギアバンドルは **予算の 70〜110%** に収める（`/offers` ロジック活用）
- イベントページ下部に **「このプロの使用ギア」** 固定表示
- クリック計測: UTM + サーバーサイドリダイレクト `/go/{gear_id}`
- ASP: Amazon アソシエイト, 楽天アフィリエイト, A8.net（ダーツ専門店）

### 7.3 表記

- フッター + 各アフィリページに **PR表記** を明記（景表法・ステマ規制対応）

---

## 8. イベント自動収集ロジック

### 8.1 スクレイピング（店舗公式）

| ソース | URL 例 | 方式 |
|--------|--------|------|
| 神戸六甲ボウル | rokkobowl.co.jp/archives/category/event | **実装済み** |
| 自遊空間 | jiqoo.jp 各店舗ページ | HTML パース + 店舗ループ |
| BAGUS | bagus-99.com イベントページ | HTML パース |
| ダーツバー | 各店舗サイト | テンプレートベース |

**パイプライン:**

```
Cron (6h) → Scraper → RawEvent[] → 正規化 → 重複排除 → 要レビューキュー → CMS承認 → DB
```

**重複排除キー:** `{shop_id, pro_id, start_date, title_normalized}`

### 8.2 SNS + OCR

```
X API v2 (ユーザタイムライン)
  → 画像付き投稿フィルタ
  → Google Vision OCR
  → 日付・店舗名・プロ名を NER 抽出
  → confidence < 0.8 は要レビュー
```

**OCR 抽出パターン:** `\d{1,2}月\d{1,2}日`, `\d{1,2}/\d{1,2}`, 店舗名辞書マッチ

### 8.3 大会（JAPAN LADIES / PERFECT）

- 公式サイト or プレスリリース RSS
- 大会は `event_type: tournament` として区別

---

## 9. 実装ロードマップ

| Step | 内容 | 期間目安 |
|------|------|----------|
| **1** | MVP API 完成（`/offers`, 六甲ボウルスクレイパー） | ✅ 完了 |
| **2** | JSON seed → PostgreSQL 移行、CRUD API (`/pros`, `/events`, `/shops`, `/gear`) | 1週 |
| **3** | 自遊空間・BAGUS スクレイパー追加 | 1週 |
| **4** | Next.js フロント（トップ + プロ一覧 + イベント一覧） | 2週 |
| **5** | 管理画面 CMS（プロ/イベント/店舗/ギア CRUD） | 2週 |
| **6** | 人気ランキング自動計算 + 特集ページ | 1週 |
| **7** | X API 連携 + OCR パイプライン | 2週 |
| **8** | 推し登録 + 通知（Web Push / Email） | 1週 |
| **9** | SEO（構造化データ, サイトマップ, 地域 LP） | 1週 |
| **10** | アフィリエイト計測 + A/B テスト + 本番デプロイ | 1週 |

---

## 10. ディレクトリ構成（目標）

```
dart-portal/
├── main.py                 # FastAPI エントリ（MVP）
├── scrape_events.py        # 六甲ボウルスクレイパー
├── requirements.txt
├── events.json             # スクレイプキャッシュ
├── docs/
│   └── DESIGN.md           # 本設計書
├── api/
│   ├── routes/             # pros, events, shops, gear, recommendations
│   ├── models/             # Pydantic / SQLAlchemy
│   └── services/           # レコメンド, ランキング
├── scrapers/
│   ├── rokkobowl.py
│   ├── jiqoo.py
│   ├── bagus.py
│   └── base.py
├── workers/
│   ├── sns_poller.py
│   └── ocr_pipeline.py
├── data/
│   ├── pros.json
│   ├── shops.json
│   └── gear.json
├── frontend/               # Next.js
└── admin/                  # CMS
```

---

*最終更新: 2026-07-17*
