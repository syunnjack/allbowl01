import { defineConfig } from "astro/config";
import sitemap from "@astrojs/sitemap";

export default defineConfig({
  site: "https://bowlingcalendar.jp",
  integrations: [sitemap()],
  output: "static"
});
