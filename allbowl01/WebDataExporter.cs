using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace allbowl01
{
    public static class WebDataExporter
    {
        public static void Export(DatabaseManager db, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            var events = db.GetEvents()
                .Select(ev =>
                {
                    var pros = SplitPros(ev.ProNames);
                    return new
                {
                    id = ev.Id,
                    date = ev.EventDate.ToString("yyyy-MM-dd"),
                    chain = ev.ChainName,
                    venue = ev.StoreName,
                    prefecture = ev.Prefecture,
                    pros,
                    proText = pros.Length > 0 ? string.Join("、", pros) : "",
                    timeSlots = new[] { ev.TimeSlot1, ev.TimeSlot2 }
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToArray(),
                    sourceUrl = ev.SourceUrl,
                    scrapedAt = ev.ScrapedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };
                })
                .ToArray();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            File.WriteAllText(
                Path.Combine(outputDir, "events.json"),
                JsonSerializer.Serialize(events, options));

            var facets = new
            {
                generatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                eventCount = events.Length,
                chains = events.Select(e => e.chain).Where(NotBlank).Distinct().Order().ToArray(),
                prefectures = events.Select(e => e.prefecture).Where(NotBlank).Distinct().Order().ToArray(),
                venues = events.Select(e => e.venue).Where(NotBlank).Distinct().Order().ToArray(),
                pros = events.SelectMany(e => e.pros).Where(NotBlank).Distinct().Order().ToArray()
            };

            File.WriteAllText(
                Path.Combine(outputDir, "facets.json"),
                JsonSerializer.Serialize(facets, options));
        }

        private static string[] SplitPros(string proNames)
        {
            var matches = Regex.Matches(
                NormalizeProName(proNames).Replace(" ", ""),
                @"[一-龯ぁ-んァ-ヶー々〆〇髙﨑]+?プロ");

            if (matches.Count > 0)
            {
                return matches
                    .Select(m => NormalizeProName(m.Value))
                    .Where(IsLikelyProName)
                    .Distinct()
                    .ToArray();
            }

            return proNames
                .Split(new[] { '/', '／', ',', '、', '&', '＆' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeProName)
                .Where(IsLikelyProName)
                .Distinct()
                .ToArray();
        }

        private static string NormalizeProName(string value) =>
            value.Replace("　", " ").Trim();

        private static bool IsLikelyProName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (!value.Contains("プロ")) return false;
            if (value.Length < 4 || value.Length > 28) return false;
            if (value.Contains("紹介") || value.Contains("料金") || value.Contains("施設")) return false;
            if (value.Contains("PLAN") || value.Contains("PRICE") || value.Contains("NEWS")) return false;
            if (value == "専属プロ") return false;
            if (value == "年プロ" || value.Contains("スペシャルプロ")) return false;
            return true;
        }

        private static bool NotBlank(string value) => !string.IsNullOrWhiteSpace(value);
    }
}
