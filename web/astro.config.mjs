import { defineConfig } from "astro/config";
import sitemap from "@astrojs/sitemap";

export default defineConfig({
  site: "https://bowling-event.jp",
  integrations: [sitemap()],
  output: "static"
});
