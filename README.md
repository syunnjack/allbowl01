# Bowling Event

Bowling Event is a Japanese professional bowling event directory. It collects official pro challenge, pro-am, and selected tournament schedule information, then publishes it in two forms:

- Web version: a static Astro site for SEO, AIO, and LLMO visibility.
- Windows app version: a WinForms/WebView2 tool for scraping, local review, SQLite storage, and web data export.

## Production Domain

The public domain is:

```text
https://bowling-event.jp
```

The web site uses `web/public/CNAME` for GitHub Pages custom domain support.

Recommended DNS for GitHub Pages:

- `A @ 185.199.108.153`
- `A @ 185.199.109.153`
- `A @ 185.199.110.153`
- `A @ 185.199.111.153`
- `CNAME www syunnjack.github.io`

After DNS propagation, set the GitHub Pages custom domain to `bowling-event.jp` and enable `Enforce HTTPS`.

## Repository Structure

- `allbowl01/`: .NET 8 Windows Forms app with WebView2 and SQLite.
- `web/`: Astro static website.
- `.github/workflows/deploy-web.yml`: builds and deploys the web version to GitHub Pages.
- `.github/workflows/build-app.yml`: builds the Windows app and uploads it as a GitHub Actions artifact.

## Web Version

The web version is the public search and discovery surface.

Generated sections:

- `/events/`
- `/prefectures/`
- `/venues/`
- `/pros/`
- `/chains/`

SEO / AIO / LLMO features:

- crawlable static HTML
- canonical URLs
- metadata and Open Graph tags
- structured data
- `robots.txt`
- `sitemap.xml`
- `llms.txt`

Development:

```powershell
cd web
npm install
npm run dev
npm run build
```

## Windows App Version

The Windows app is the operation tool. It is not the main search surface, but it feeds the public web site.

Main responsibilities:

- scrape official bowling venue, operator, and JPBA pages
- store events in SQLite
- review local data through WebView2
- filter by chain, prefecture, and pro name
- export JSON data for the Astro web site

Development:

```powershell
dotnet restore
dotnet build
dotnet run --project allbowl01
```

Export web data:

```powershell
dotnet run --project allbowl01 -- --export-web web/src/data
```

Scrape and export web data:

```powershell
dotnet run --project allbowl01 -- --scrape-export web/src/data
```

## Operation Flow

1. Run scraping from the Windows app or CLI.
2. Export `web/src/data/events.json` and `web/src/data/facets.json`.
3. Run `npm run build` in `web/`.
4. Commit the updated data and source files.
5. Push to `master`.
6. GitHub Pages publishes the updated web version.

## Source Policy

Event information should be based on official venue, operator, and JPBA pages whenever possible. Users should verify date, time, fee, eligibility, and availability on the linked official source before attending.
