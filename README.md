# allbowl01

全国のプロボウリングチャレンジマッチ開催情報を収集し、日付・会場・都道府県・出演プロボウラー名で検索するための Windows デスクトップアプリです。

## 現在の構成

- `allbowl01`: .NET 8 / Windows Forms アプリ
- `WebView2`: アプリ内ポータル画面の表示
- `SQLite`: 取得したイベント情報の保存
- `HtmlAgilityPack`: 公式サイトからのHTML解析
- `portal.html`: 検索・フィルタ・一覧表示UI

## できること

- ボウリング場公式サイトからチャレンジマッチ情報を取得
- イベント日、チェーン、会場、都道府県、出演プロ名を保存
- WebView2上で一覧・カード表示を切り替え
- チェーン、都道府県、プロ名で絞り込み

## SEO / AIO / LLMO 方針

WinForms/WebView2 はデスクトップアプリ内でHTMLを表示する構成のため、それ単体では検索エンジンやAIクローラーに見つけてもらいにくいです。検索流入を狙う場合は、公開URLを持つWebページとして次の情報を提供する必要があります。

- 初期HTMLで読める本文
- title / description / canonical / OGP
- 構造化データ
- `robots.txt`
- `sitemap.xml`
- `llms.txt`
- イベント別、都道府県別、会場別、プロ別のクロール可能なページ

このリポジトリでは短期対応として `portal.html`、`robots.txt`、`sitemap.xml`、`llms.txt` を整備しています。

## 推奨する次の構成

SEO / AIO / LLMO を本格的に強化する場合は、データ収集と公開フロントを分離する構成を推奨します。

1. C# アプリまたはバッチで公式サイトからデータを収集する
2. SQLite の内容から `events.json` を生成する
3. Next.js、Astro、または SvelteKit で静的ページを生成する
4. GitHub Pages、Cloudflare Pages、Vercel などで公開する

生成したいページ例:

- `/events/`
- `/prefectures/aichi/`
- `/venues/inazawa-grandbowl/`
- `/pros/example-pro/`
- `/chains/grandbowl/`

## 注意

掲載情報は各ボウリング場・運営会社の公式サイトをもとに整理します。開催日時、参加条件、料金、空き状況は変更される場合があるため、参加前には必ず公式ページで最新情報を確認してください。

## 開発

```powershell
dotnet restore
dotnet build
```

## ライセンス

未設定です。公開・再利用方針に合わせてライセンスファイルを追加してください。
