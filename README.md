# allbowl01 / Bowling Event

日本のプロボウリング来店イベントを集約するポータル。  
公開面は **Astro 静的サイト** と **WordPress（Cocoon 子テーマ）** の2系統を並行保持し、パフォーマンス比較ができる構成です。

## Production Domain

```text
https://bowling-event.jp
```

GitHub Pages カスタムドメイン: `web/public/CNAME`

## Repository Structure

| パス | 用途 |
|------|------|
| `web/` | **Astro** 静的サイト（本番: bowling-event.jp） |
| `cocoon-child-mankan/` | **WordPress** Cocoon 子テーマ（Mankan トップ LP） |
| `allbowl01/` | Windows アプリ（C# / スクレイピング・データ出力） |
| `main.py` 他 | Python MVP API（ダーツレコメンド・六甲ボウルスクレイパー） |
| `docs/DESIGN.md` | ダーツポータル全体設計書 |

## Astro（静的サイト）

SEO / AIO / LLMO 向けの公開サイト。

```powershell
cd web
npm install
npm run dev
npm run build
```

主なページ: `/events/`, `/prefectures/`, `/venues/`, `/pros/`, `/chains/`

デプロイ: `.github/workflows/deploy-web.yml` → GitHub Pages

## WordPress（Cocoon 子テーマ）

`cocoon-child-mankan/` を WordPress の `wp-content/themes/` に配置して有効化。

- テンプレート: `page-mankan-top.php`（固定ページ「Mankan Top」用）
- スタイル: `assets/css/mankan-style.css`
- ローカル確認: `preview.html` をブラウザで開く

Astro 版と同等コンテンツの WP 実装として、表示速度・Core Web Vitals を比較できます。

## Windows App

```powershell
dotnet restore
dotnet build
dotnet run --project allbowl01
```

スクレイプ → SQLite → `web/src/data/events.json` へエクスポート:

```powershell
dotnet run --project allbowl01 -- --scrape-export web/src/data
```

## Python MVP API（任意）

```powershell
python -m venv .venv
.\.venv\Scripts\pip.exe install -r requirements.txt
.\.venv\Scripts\python.exe scrape_events.py
.\.venv\Scripts\python.exe main.py
```

- Swagger UI: http://127.0.0.1:8000/docs
- レコメンド: `POST /offers`

## Operation Flow

1. Windows アプリまたは CLI でスクレイプ
2. `web/src/data/events.json` を更新
3. Astro: `npm run build` → push → GitHub Pages 公開
4. WordPress: テーマをデプロイし固定ページを公開
5. 両方の Lighthouse / PageSpeed を比較

## Source Policy

イベント情報は公式サイト（店舗・運営会社・JPBA）を優先。参加前に公式ページで日時・料金を確認してください。
