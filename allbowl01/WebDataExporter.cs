using System.Text.Encodings.Web;
using System.Text.Json;

namespace allbowl01
{
    public static class WebDataExporter
    {
        public static void Export(DatabaseManager db, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            var events = db.GetEvents()
                .Select(ev => new
                {
                    id = ev.Id,
                    date = ev.EventDate.ToString("yyyy-MM-dd"),
                    chain = ev.ChainName,
                    venue = ev.StoreName,
                    prefecture = ev.Prefecture,
                    pros = SplitPros(ev.ProNames),
                    proText = ev.ProNames,
                    timeSlots = new[] { ev.TimeSlot1, ev.TimeSlot2 }
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToArray(),
                    sourceUrl = ev.SourceUrl,
                    scrapedAt = ev.ScrapedAt.ToString("yyyy-MM-dd HH:mm:ss")
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

        private static string[] SplitPros(string proNames) =>
            proNames
                .Split(new[] { '/', '／', ',', '、', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(NotBlank)
                .Distinct()
                .ToArray();

        private static bool NotBlank(string value) => !string.IsNullOrWhiteSpace(value);
    }
}
