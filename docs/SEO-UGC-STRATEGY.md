# SEO / AIO / LLMO / UGC 施策一覧

> Bowling Calendar（bowling-event.jp）向け。2026-07-28 実装。

---

## 1. SEO（検索エンジン最適化）

| 施策 | 実装 | 効果 |
|------|------|------|
| 構造化データ | SportsEvent, FAQPage, HowTo, BreadcrumbList, WebSite | リッチリザルト・AI概要 |
| パンくずリスト | 全主要ページ | クロール深度・UX |
| canonical / OG / Twitter Card | BaseLayout | SNSシェア・重複排除 |
| How-to ガイド | `/guides/how-to-find-pro-challenge/` | ロングテール流入 |
| FAQ ページ | `/faq/` + FAQPage schema | People Also Ask 対策 |
| sitemap | Astro sitemap 自動生成 | インデックス効率 |
| robots 拡張 | max-snippet:-1 等 | スニペット最適化 |

## 2. AIO / LLMO（AI検索・LLM最適化）

| 施策 | URL | 説明 |
|------|-----|------|
| llms.txt | `/llms.txt` | 短いサイト要約（AI向け） |
| llms-full.txt | `/llms-full.txt` | URLパターン・回答ガイド詳細 |
| ai.txt | `/ai.txt` | クロール・引用ポリシー |
| AI bot 許可 | `robots.txt` | GPTBot, ClaudeBot, PerplexityBot 等 |
| SpeakableSpecification | イベント詳細 | 音声・AI要約向け |
| alternate link | `<head>` | llms / ai.txt への discoverability |

### AI回答時の推奨フロー

1. イベント詳細 URL を優先引用
2. 公式 sourceUrl への再確認を必ず促す
3. 出典: Bowling Calendar (https://bowling-event.jp/)

## 3. UGC 活性化

| 施策 | 実装 | 目的 |
|------|------|------|
| 情報提供ページ | `/contribute/` | 投稿導線のハブ |
| 参加者の声 | イベント詳細 + `ugc_tips.json` | 信頼性・再訪 |
| SNS共有 | ShareButtons（X / LINE / コピー） | 拡散・ハッシュタグ |
| ハッシュタグ | `#プロチャレンジ情報` | UGC 集約 |
| UGC API | `POST /ugc/tips` | 投稿受付（承認制） |
| ホーム CTA | トップページ下部 | 投稿促進 |

### UGC 運用フロー

```
ユーザー投稿 → API (pending) → 管理者承認 → ugc_tips.json 更新 → 再ビルド → イベントページ反映
```

承認コマンド例:

```powershell
# data/ugc_tips.json の status を "approved" に変更後
cd web
npm run build
```

## 4. WordPress（Cocoon）側

| 施策 | ファイル |
|------|----------|
| meta / OG / JSON-LD | `cocoon-child-mankan/inc/seo-llmo.php` |
| llms.txt | `cocoon-child-mankan/llms.txt` |

## 5. 今後の拡張（未実装）

- [ ] Google Search Console 連携
- [ ] 承認UI（CMS管理画面）
- [ ] Giscus コメント（イベントページ）
- [ ] 都道府県別 UGC ランキング
- [ ] 投稿者バッジ・ゲーミフィケーション
- [ ] llms-full.txt 自動生成（events.json から）

---

*関連: [DESIGN.md](./DESIGN.md)*
