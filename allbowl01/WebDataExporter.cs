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
                .Where(ev => ev.pros.Length > 0)
                .Where(ev => !IsLowConfidenceSource(ev.sourceUrl))
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
                return RemoveCoveredShortNames(matches
                    .Select(m => NormalizeProName(m.Value))
                    .Where(IsLikelyProName)
                    .Distinct()
                    .ToArray());
            }

            return RemoveCoveredShortNames(proNames
                .Split(new[] { '/', '／', ',', '、', '&', '＆' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeProName)
                .Where(IsLikelyProName)
                .Distinct()
                .ToArray());
        }

        private static string NormalizeProName(string value) =>
            value.Replace("　", " ").Trim();

        private static bool IsLikelyProName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (!value.Contains("プロ")) return false;
            if (value.Length < 4 || value.Length > 28) return false;
            if (value.Replace("プロ", "").Length < 3) return false;
            if (value.Contains("紹介") || value.Contains("料金") || value.Contains("施設")) return false;
            if (value.Contains("PLAN") || value.Contains("PRICE") || value.Contains("NEWS")) return false;
            if (value.Contains("スケジュール") || value.Contains("お知らせ")) return false;
            if (value.Contains("チャレンジ") || value.Contains("チャリティー")) return false;
            if (value.Contains("スタッフ") || value.Contains("所属") || value.Contains("出場")) return false;
            if (value.Contains("選手") || value.Contains("優勝") || value.Contains("月度")) return false;
            if (value.Contains("公益") || value.Contains("社団") || value.Contains("日本プロ")) return false;
            if (value.Contains("地区") || value.Contains("期生") || value.Contains("競技会")) return false;
            if (value.Contains("ゲーム") || value.Contains("オンライン") || value.Contains("卓球")) return false;
            if (value.Contains("県出身") || value.Contains("戦績") || value.Contains("開催")) return false;
            if (value.Contains("できる") || value.Contains("知りたく")) return false;
            if (value.StartsWith("の") || value.StartsWith("と")) return false;
            if (IsPrefectureLabel(value)) return false;
            if (value == "専属プロ") return false;
            if (value == "年プロ" || value.Contains("スペシャルプロ")) return false;
            return true;
        }

        private static string[] RemoveCoveredShortNames(string[] values) =>
            values
                .Where(value =>
                {
                    var baseName = value.Replace("プロ", "");
                    return !values.Any(other =>
                    {
                        if (other == value) return false;
                        var otherBase = other.Replace("プロ", "");
                        return otherBase.Length > baseName.Length
                            && otherBase.Contains(baseName);
                    });
                })
                .ToArray();

        private static bool IsLowConfidenceSource(string value) =>
            value.Contains("result", StringComparison.OrdinalIgnoreCase)
            || value.Contains("staff", StringComparison.OrdinalIgnoreCase)
            || value.Contains("profile", StringComparison.OrdinalIgnoreCase)
            || value.Contains("ranking", StringComparison.OrdinalIgnoreCase);

        private static bool IsPrefectureLabel(string value)
        {
            var baseName = value.Replace("プロ", "");
            string[] prefectures =
            {
                "北海道", "青森", "岩手", "宮城", "秋田", "山形", "福島",
                "茨城", "栃木", "群馬", "埼玉", "千葉", "東京", "神奈川",
                "新潟", "富山", "石川", "福井", "山梨", "長野", "岐阜",
                "静岡", "愛知", "三重", "滋賀", "京都", "大阪", "兵庫",
                "奈良", "和歌山", "鳥取", "島根", "岡山", "広島", "山口",
                "徳島", "香川", "愛媛", "高知", "福岡", "佐賀", "長崎",
                "熊本", "大分", "宮崎", "鹿児島", "沖縄"
            };
            return prefectures.Contains(baseName);
        }

        private static bool NotBlank(string value) => !string.IsNullOrWhiteSpace(value);
    }
}
