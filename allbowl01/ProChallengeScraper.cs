using HtmlDocument = HtmlAgilityPack.HtmlDocument;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Linq.Expressions;
using HtmlAgilityPack;

namespace allbowl01
{


    public class ScraperProgress
    {
        public string Message { get; set; } = "";
        public int Current { get; set; }
        public int Total { get; set; }
        public bool IsError { get; set; }
    }
    public record StoreInfo(
        string Chain,
        string StoreName,
        string Prefecture,
        string Url);

    public class ProChallengeScraper : IDisposable
    {
        private readonly HttpClient _http;
        private readonly DatabaseManager _db;
        public event Action<ScraperProgress>? OnProgress;
        public int NewCount { get; private set; }
        public int TotalCount { get; private set; }

        private static readonly List<StoreInfo> Stores = new()
    {
            new("コロナ","小牧店",           "愛知","https://www.korona.co.jp/bowling/basic/kom/191"),
            new("コロナ","中川店",           "愛知","https://www.korona.co.jp/bowling/basic/nak/286"),
            new("コロナ","安城店",           "愛知","https://www.korona.co.jp/bowling/basic/anj/346"),
            new("コロナ","豊川店",           "愛知","https://www.korona.co.jp/bowling/basic/tok/434"),
            new("コロナ","半田店",           "愛知","https://www.korona.co.jp/bowling/challenge/han/310"),
            new("コロナ","大垣店",           "岐阜","https://www.korona.co.jp/bowling/challenge/ogk/512"),
            new("コロナ","ららぽーと沼津店", "静岡","https://www.korona.co.jp/bowling/basic/num/530"),
            // コロナ追加店舗
            new("コロナ","仙台店",   "宮城","https://www.korona.co.jp/bowling/basic/sen/350"),
            new("コロナ","小田原店", "神奈川","https://www.korona.co.jp/bowling/basic/odw/237"),
            new("コロナ","春日井店", "愛知","https://www.korona.co.jp/bowling/basic/kas/192"),
            new("コロナ","江南店",   "愛知","https://www.korona.co.jp/bowling/basic/kon/193"),
            new("コロナ","豊田店",   "愛知","https://www.korona.co.jp/bowling/basic/tyt/347"),
            new("コロナ","金沢店",   "石川","https://www.korona.co.jp/bowling/basic/knz/238"),
            new("コロナ","小松店",   "石川","https://www.korona.co.jp/bowling/basic/kmt/511"),
            new("コロナ","福井店",   "福井","https://www.korona.co.jp/bowling/basic/fui/239"),
            new("コロナ","福井春江店","福井","https://www.korona.co.jp/bowling/basic/fue/513"),
            new("コロナ","福山店",   "広島","https://www.korona.co.jp/bowling/basic/fuy/240"),
            new("コロナ","小倉店",   "福岡","https://www.korona.co.jp/bowling/basic/kok/241"),
            

            // グランドボウル
            new("グランドボウル","稲沢グランドボウル",        "愛知","https://www.grandbowl.jp/inazawa/news/category/tournament/"),
            new("グランドボウル","半田グランドボウル",        "愛知","https://www.grandbowl.jp/handa/news/category/tournament/"),
            new("グランドボウル","鈴鹿グランドボウル",        "三重","https://www.grandbowl.jp/suzuka/news/category/tournament/"),
            new("グランドボウル","川崎グランドボウル",        "神奈川","https://www.grandbowl.jp/kawasaki/news/category/tournament/"),
            new("グランドボウル","名古屋グランドボウル",      "愛知","https://www.grandbowl.jp/nagoya/news/category/tournament/"),
            new("グランドボウル","岡崎グランドボウル",        "愛知","https://www.grandbowl.jp/okazaki/news/category/tournament/"),
            new("グランドボウル","春日井グランドボウル",      "愛知","https://www.grandbowl.jp/kasugai/news/category/tournament/"),
            new("グランドボウル","高田馬場グランドボウル",    "東京","https://www.grandbowl.jp/takadanobaba/news/category/tournament/"),
            new("グランドボウル","東大和グランドボウル",      "東京","https://www.grandbowl.jp/higashiyamato/news/category/tournament/"),
            new("グランドボウル","新狭山グランドボウル",      "埼玉","https://www.grandbowl.jp/shinsayama/news/category/tournament/"),
            new("グランドボウル","藤枝グランドボウル",        "静岡","https://www.grandbowl.jp/fujieda/news/category/tournament/"),
            new("グランドボウル","袋井グランドボウル",        "静岡","https://www.grandbowl.jp/fukuroi/news/category/tournament/"),
            new("グランドボウル","浜岡グランドボウル",        "静岡","https://www.grandbowl.jp/hamaoka/news/category/tournament/"),
            new("グランドボウル","津グランドボウル",          "三重","https://www.grandbowl.jp/tsu/news/category/tournament/"),
            new("グランドボウル","心斎橋サンボウル",          "大阪","https://www.grandbowl.jp/shinsaibashi/news/category/tournament/"),
            new("グランドボウル","ジェームス山グランドボウル","兵庫","https://www.grandbowl.jp/jamesyama/news/category/tournament/"),
            // N&K
            new("N&K","幸田セントラルボウル",    "愛知","https://nandk.net/challenge-match/358071/"),
            new("N&K","狐ヶ崎ヤングランドボウル","静岡","https://nandk.net/challenge-match/357580/"),
            new("N&K","都留ファミリーボウル",    "山梨","https://nandk.net/challenge-match/358071/"),
            new("N&K","柿田川パークレーンズ",    "静岡","https://nandk.net/category/challenge-match/"),
            new("N&K","ヤングファラオ",          "長野","https://nandk.net/category/challenge-match/"),
            new("N&K","弘前ファミリーボウル",    "青森","https://nandk.net/category/challenge-match/"),
            // ゆりの木ボウル青森
            new("ゆりの木ボウル", "ゆりの木ボウル", "青森", "https://jabico.co.jp/wp-content/uploads/2026/01/af5835bb4f8f31085e405a0df8262fc7.pdf"),
            new("品川プリンスボウル", "品川プリンスボウル", "東京", "https://www.princehotels.co.jp/parktower/bowling/"),
            new("ニューパールレーン", "ニューパールレーン", "埼玉", "https://takesato-bowl.net/"),
            new("スプリングレーン", "川口スプリングレーン", "埼玉", "https://www.sp-bowl.com/k_game.php"),
            new("スプリングレーン", "浦和スプリングレーン", "埼玉", "https://www.sp-bowl.com/u_game.php"),

            new("スポルト", "スポルト八景", "神奈川", "https://www.sport-bowling.co.jp/schedule_hakkei"),

            new("サンコーボウル", "サンコーボウル", "北海道", "https://www.sanko-bowl.com/bowling/schedule/"),

            new("アイキョーボウル", "アイキョーボウル", "千葉", "https://www.aikyo-bowl.jp/schedule_a.html"),

            new("ヤングボウル", "横浜ヤングボウル", "神奈川", "http://www.youngbowl.com/shoplist/yokohama"),
            new("ヤングボウル", "柏ヤングボウル", "千葉", "http://www.youngbowl.com/shoplist/kashiwa"),
            new("ヤングボウル", "柏駅前ヤングボウル", "千葉", "http://www.youngbowl.com/shoplist/kashiwaekimae"),

            new("東京ポートボウル", "東京ポートボウルボウル", "東京", "https://www.tokyoportbowl.com/wpTB/wp-content/uploads/2026/03/2026%E5%B9%B4%E4%BC%9A%E5%A0%B14%E6%9C%88%E3%80%80.pdf"),
            new("新宿コパボウル", "新宿コパボウル", "東京", "https://copa-shinjuku.com/news/"),

            new("ヤングボウル", "岡山ヤングボウル", "岡山", "http://www.youngbowl.com/shoplist/okayama"),

            new("キスケボウル", "キスケボウル", "愛媛", "https://kisuke.com/kit/information/"),
            new("アルゴボウル", "アルゴボウル", "兵庫", "http://www.algo7.jp/news/sp/1235.html"),
            new("神戸六甲ボウル", "神戸六甲ボウル", "兵庫", "https://www.rokkobowl.co.jp/"),

            new("イーグルボウル", "イーグルボウル", "愛知", "https://eaglebowl.jp/event/"),
            new("サンボウル", "サンボウル", "愛知", "https://www.sunbowl300.sakura.ne.jp/"),


            new("パークレーン", "広島パークレーン", "広島", "https://parklane30.com/assets/document/schedule/taikai_2603.pdf"),
            new("パークレーン", "相模原パークレーン", "神奈川", "https://parklanes.jp/tournament_detail.php?id=2669"),


            new("東名ボール","東名ボール","愛知","https://www.tomei-bowl.com/category/pro-cha/"),

            new("イーボールトマト西宮", "イーボールトマト西宮", "兵庫", "https://www.e-bowltomato2438official.com/blank-6/"),
            new("アルゴボウル", "アルゴボウル", "兵庫", "http://www.algo7.jp/news/sp/1235.html"),

            new("ラウンドワン", "ラウンドワン", "オンライン", "https://www.round1.co.jp/service/r1live/challenge/"),
            new("平和島スターボウル", "平和島スターボウル", "", ""),
            // スターレーン
            new("スターレーン","盛岡スターレーン",   "岩手","https://www.starlanes.co.jp/morioka/"),
            new("スターレーン","本八幡スターレーン", "千葉","https://www.starlanes.co.jp/motoyawata/"),
            new("スターレーン","足利スターレーン",   "栃木","https://www.starlanes.co.jp/ashikaga/"),
            new("スターレーン","桐生スターレーン",   "群馬","https://www.starlanes.co.jp/kiryu/"),
            new("スターレーン","所沢スターレーン",   "埼玉","https://www.starlanes.co.jp/tokorozawa/"),
            new("スターレーン","冨津スターレーン",   "千葉","https://www.starlanes.co.jp/futtsu/"),
            new("スターレーン","立川スターレーン",   "東京","https://www.starlanes.co.jp/tachikawa/"),
            new("スターレーン", "高尾スターレーン", "東京", "https://www.starlanes.co.jp/tachikawa/"),
            new("スターレーン","折尾スターレーン",   "福岡","https://www.starlanes.co.jp/orio/"),
            // スプリングレーン
            new("スプリングレーン","浦和スプリングレーン","埼玉","https://www.sp-bowl.com/u_game.php"),
            new("スプリングレーン","川口スプリングレーン","埼玉","https://www.sp-bowl.com/k_game.php"),
            // パークレーン
            new("パークレーン","パークレーン高崎",   "群馬","https://www.parklane-takasaki.com/category/tournament/"),
            new("パークレーン","相模原パークレーン", "神奈川","https://parklanes.jp/tournament_detail.php"),
            new("パークレーン","広島パークレーン",   "広島","https://parklane30.com/tournament/"),
            // その他
            new("スポルト","スポルト八景",          "神奈川","https://www.sport-bowling.co.jp/schedule_hakkei"),
            new("ダイトー","ダイトースターレーン",   "山梨","https://www.daitho.co.jp/category/pro-challenge/"),
            


    };
        public ProChallengeScraper(DatabaseManager db)
        {
            _db = db;
            _http = new HttpClient();
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 Chrome/124.0 Safari/537.36");
            _http.Timeout = TimeSpan.FromSeconds(30);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public async Task RunAsync()
        {
            NewCount = 0;
            var targetStores = Stores
                .Where(s => !string.IsNullOrWhiteSpace(s.Url))
                .GroupBy(s => $"{s.Chain}|{s.StoreName}|{s.Url}")
                .Select(g => g.First())
                .ToList();

            TotalCount = targetStores.Count;
            int i = 0;
            var scraped = DateTime.Now;

            foreach (var store in targetStores)
            {
                i++;
                Notify($"[{i}/{TotalCount}] {store.StoreName} を取得中...", i, TotalCount);
                try
                {
                    List<ProChallengeEvent> events = store.Chain switch
                    {
                        "コロナ" => await ScrapeKorona(store, scraped),
                        "グランドボウル" => await ScrapeGrandBowl(store, scraped),
                        "N&K" => await ScrapeNandK(store, scraped),
                        "東名ボール" => await ScrapeTomei(store, scraped),
                        "スターレーン" => await ScrapeStarLane(store, scraped),
                        "六甲ボウル" => await ScrapeRokko(store, scraped),
                        "ラウンドワン" => await ScrapeRoundOne(store, scraped),
                        _ => await ScrapeGeneric(store, scraped)
                        
                    };
                    foreach (var ev in events)
                    {
                        int r = _db.Upsert(ev);
                        if (r > 0) NewCount++;
                    }
                    Notify($"  → {events.Count} 件取得", i, TotalCount);
                }
                catch (Exception ex)
                {
                    Notify($"  ⚠ {ex.Message}", i, TotalCount, true);
                }
                await Task.Delay(600);
            }
            Notify($"✅ 完了！新規 {NewCount} 件を登録", TotalCount, TotalCount);
        }

        private async Task<List<ProChallengeEvent>> ScrapeGeneric(
            StoreInfo store, DateTime scraped)
        {
            var events = new List<ProChallengeEvent>();
            if (IsPdfUrl(store.Url)) return events;

            var html = await FetchHtml(store.Url);
            events.AddRange(ParseGenericEventPage(html, store, store.Url, scraped));

            foreach (var link in ExtractCandidateLinks(html, store.Url).Take(12))
            {
                try
                {
                    if (IsPdfUrl(link)) continue;
                    var detail = await FetchHtml(link);
                    events.AddRange(ParseGenericEventPage(detail, store, link, scraped));
                    await Task.Delay(250);
                }
                catch { }
            }

            return Deduplicate(events);
        }
        private async Task<List<ProChallengeEvent>> ScrapeKorona(
            StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var events = ParseByRegex(html, store, scraped);
            if (events.Count == 0)
                events = ParseByHAP(html, store, scraped);
            return events;
        }
        private async Task<List<ProChallengeEvent>> ScrapeGrandBowl(
           StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var events = new List<ProChallengeEvent>();
            var baseUri = new Uri(store.Url.Replace("category/tournament/", ""));

            var links = doc.DocumentNode
                .SelectNodes("//a[contains(@href,'/news/')]")?
                .Select(n => n.GetAttributeValue("href", ""))
                .Where(h => Regex.IsMatch(h, @"/news/\d+"))
                .Select(h => new Uri(baseUri, h).ToString())
                .Distinct().Take(20).ToList() ?? new List<string>();

            foreach (var link in links)
            {
                try
                {
                    var detail = await FetchHtml(link);
                    var ev = ParseGrandBowlArticle(detail, store, link, scraped);
                    if (ev != null) events.Add(ev);
                    await Task.Delay(400);
                }
                catch { }
            }
            return events;
        }

        private ProChallengeEvent? ParseGrandBowlArticle(
            string html, StoreInfo store, string url, DateTime scraped)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var text = NormalizeJapaneseText(System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText));

            if (!Regex.IsMatch(text,
                @"(プロ.{0,20}(チャレンジ|Birthday|誕生|バースデー|対決)|チャレンジ.{0,20}プロ)"))
                return null;

            var dm = Regex.Match(text,
                @"(?:(20\d{2})\s*[年/]\s*)?(\d{1,2})\s*[月/]\s*(\d{1,2})\s*(?:日)?");
            if (!dm.Success || !TryReadEventDate(dm, out var dt)) return null;
            if (dt < DateTime.Today || dt > DateTime.Today.AddYears(2)) return null;

            var title = NormalizeJapaneseText(System.Net.WebUtility.HtmlDecode(
                doc.DocumentNode.SelectSingleNode("//h1")?.InnerText ?? ""));
            var start = Math.Max(0, dm.Index - 120);
            var length = Math.Min(text.Length - start, 900);
            var focusedText = $"{title} {text.Substring(start, length)}";
            var pros = ExtractProNames(focusedText).Take(4).ToList();
            if (pros.Count == 0) return null;

            var times = Regex.Matches(text, @"(\d{1,2}:\d{2})(?:スタート|開始|～)?")
                .Cast<Match>().Select(m => m.Groups[1].Value)
                .Distinct().Take(2).ToList();

            return new ProChallengeEvent
            {
                ChainName = store.Chain,
                StoreName = store.StoreName,
                Prefecture = store.Prefecture,
                EventDate = dt,
                ProNames = string.Join("、", pros),
                TimeSlot1 = times.Count > 0 ? times[0] : "",
                TimeSlot2 = times.Count > 1 ? times[1] : "",
                SourceUrl = url,
                ScrapedAt = scraped
            };
        }

        private async Task<List<ProChallengeEvent>> ScrapeRoundOne(
            StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var text = ToPlainText(html);
            var events = new List<ProChallengeEvent>();
            var rx = new Regex(
                @"(?<pro>[一-龯ぁ-んァ-ヶー々〆〇髙﨑 ]{2,18}プロ)\s*[:：]\s*" +
                @"(?:(?<year>20\d{2})年)?(?<month>\d{1,2})月(?<day>\d{1,2})日" +
                @"[\s\S]{0,140}?開催時間\s*(?<times>(?:\d{1,2}:\d{2}\s*/?\s*){1,4})" +
                @"[\s\S]{0,100}?配信店舗\s*(?<venue>[^\s。]+?店)");

            foreach (Match match in rx.Matches(text))
            {
                if (!TryReadRoundOneDate(match, out var eventDate)) continue;
                if (eventDate < DateTime.Today || eventDate > DateTime.Today.AddYears(2)) continue;

                var venue = NormalizeRoundOneVenue(match.Groups["venue"].Value);
                var prefecture = RoundOnePrefecture(venue);
                var pro = Regex.Replace(match.Groups["pro"].Value, @"\s+", "");
                if (!IsLikelyGenericProName(pro)) continue;

                var times = Regex.Matches(match.Groups["times"].Value, @"\d{1,2}:\d{2}")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .Where(IsLikelyTime)
                    .Distinct()
                    .Take(2)
                    .ToList();

                events.Add(new ProChallengeEvent
                {
                    ChainName = store.Chain,
                    StoreName = string.IsNullOrWhiteSpace(venue) ? store.StoreName : venue,
                    Prefecture = prefecture,
                    EventDate = eventDate,
                    ProNames = pro,
                    TimeSlot1 = times.Count > 0 ? times[0] : "",
                    TimeSlot2 = times.Count > 1 ? times[1] : "",
                    SourceUrl = store.Url,
                    ScrapedAt = scraped
                });
            }

            return Deduplicate(events);
        }

        private async Task<List<ProChallengeEvent>> ScrapeNandK(
            StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var events = new List<ProChallengeEvent>();
            var base2 = new Uri("https://nandk.net/");

            var links = doc.DocumentNode
                .SelectNodes("//a[contains(@href,'challenge-match')]")?
                .Select(n => n.GetAttributeValue("href", ""))
                .Where(h => h.Contains("challenge-match"))
                .Select(h => new Uri(base2, h).ToString())
                .Distinct().Take(20).ToList() ?? new List<string>();

            foreach (var link in links)
            {
                try
                {
                    var detail = await FetchHtml(link);
                    var ev = ParseNandKArticle(detail, store, link, scraped);
                    if (ev != null) events.Add(ev);
                    await Task.Delay(400);
                }
                catch { }
            }
            return events;
        }

        private ProChallengeEvent? ParseNandKArticle(
            string html, StoreInfo store, string url, DateTime scraped)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var title = doc.DocumentNode.SelectSingleNode("//h1")?.InnerText ?? "";
            var text = System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText);

            DateTime dt;
            var today = DateTime.Today;
            var dm4 = Regex.Match(title + text,
                @"(\d{4})[年/](\d{1,2})[月/](\d{1,2})");
            var dm2 = Regex.Match(title, @"(\d{1,2})/(\d{1,2})");

            if (dm4.Success)
            {
                if (!DateTime.TryParse(
                    $"{dm4.Groups[1].Value}-{dm4.Groups[2].Value}-{dm4.Groups[3].Value}",
                    out dt)) return null;
            }
            else if (dm2.Success)
            {
                int mo = int.Parse(dm2.Groups[1].Value);
                int dy = int.Parse(dm2.Groups[2].Value);
                dt = new DateTime(today.Year, mo, dy);
                if (dt < today) dt = dt.AddYears(1);
            }
            else return null;

            if (dt < DateTime.Today || dt > DateTime.Today.AddYears(2)) return null;

            var proM = Regex.Match(title, @"・\s*(.+プロ)");
            var pro = proM.Success ? proM.Groups[1].Value.Trim()
                       : Regex.Match(text,
                           @"([\p{L}\p{Lo}　・＆& ]+プロ[^\r\n。]{0,30})")
                           .Value.Trim();
            if (string.IsNullOrEmpty(pro)) return null;

            var times = Regex.Matches(text,
                @"(\d{1,2}:\d{2})(?:スタート|開始|～)?")
                .Cast<Match>().Select(m => m.Groups[1].Value)
                .Distinct().Take(2).ToList();

            return new ProChallengeEvent
            {
                ChainName = store.Chain,
                StoreName = store.StoreName,
                Prefecture = store.Prefecture,
                EventDate = dt,
                ProNames = pro,
                TimeSlot1 = times.Count > 0 ? times[0] : "",
                TimeSlot2 = times.Count > 1 ? times[1] : "",
                SourceUrl = url,
                ScrapedAt = scraped
            };
        }
        private async Task<List<ProChallengeEvent>> ScrapeRokko(
    StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var events = new List<ProChallengeEvent>();

            var posts = doc.DocumentNode.SelectNodes("//div[contains(@class,'post')]");
            if (posts == null) return events;

            foreach (var post in posts)
            {
                var titleNode = post.SelectSingleNode(".//h2[@class='title']/a");
                if (titleNode == null) continue;

                var detailUrl = titleNode.GetAttributeValue("href", "");
                var detail = await FetchHtml(detailUrl);

                var text = System.Net.WebUtility.HtmlDecode(detail);

                if (!Regex.IsMatch(text, @"プロ.*(チャレンジ|マッチ|投げよう)"))
                    continue;

                var dm = Regex.Match(text, @"(\d{1,2})月(\d{1,2})日");
                if (!dm.Success) continue;

                var dt = new DateTime(DateTime.Now.Year,
                    int.Parse(dm.Groups[1].Value),
                    int.Parse(dm.Groups[2].Value));

                var pro = Regex.Match(text, @"([^\s]+プロ)").Value;
                var time = Regex.Match(text, @"(\d{1,2}:\d{2})").Value;

                events.Add(new ProChallengeEvent
                {
                    ChainName = store.Chain,
                    StoreName = store.StoreName,
                    Prefecture = store.Prefecture,
                    EventDate = dt,
                    ProNames = pro,
                    TimeSlot1 = time,
                    SourceUrl = detailUrl,
                    ScrapedAt = scraped
                });
            }

            return events;
        }


        private async Task<List<ProChallengeEvent>> ScrapeTomei(
            StoreInfo store, DateTime scraped)
        {
            var html = await FetchHtml(store.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var events = new List<ProChallengeEvent>();
            var base3 = new Uri("https://www.tomei-bowl.com/");

            var links = doc.DocumentNode
                .SelectNodes("//a[@href]")?
                .Select(n => n.GetAttributeValue("href", ""))
                .Where(h => Regex.IsMatch(h, @"/20\d{2}/\d{2}/"))
                .Select(h => new Uri(base3, h).ToString())
                .Distinct().Take(20).ToList() ?? new List<string>();

            foreach (var link in links)
            {
                try
                {
                    var detail = await FetchHtml(link);
                    var d2 = new HtmlDocument();
                    d2.LoadHtml(detail);
                    var text = System.Net.WebUtility.HtmlDecode(
                        d2.DocumentNode.InnerText);

                    if (!Regex.IsMatch(text,
                        @"プロ.{0,15}(チャレンジ|投げよう|マッチ)")) continue;

                    var titleNode = d2.DocumentNode.SelectSingleNode("//h1");
                    var titleText = System.Net.WebUtility.HtmlDecode(titleNode?.InnerText ?? "");
                    var dmTitle = Regex.Match(titleText, @"(\d{4})[\.年](\d{1,2})月");
                    DateTime dt;
                    if (dmTitle.Success)
                    {
                        int yr = int.Parse(dmTitle.Groups[1].Value);
                        int mo = int.Parse(dmTitle.Groups[2].Value);
                        dt = new DateTime(yr, mo, 1);
                    }
                    else
                    {
                        var dm = Regex.Match(text, @"(\d{4})[年/](\d{1,2})[月/](\d{1,2})");
                        if (!dm.Success) continue;
                        if (!DateTime.TryParse(
                            $"{dm.Groups[1].Value}-{dm.Groups[2].Value}-{dm.Groups[3].Value}",
                            out dt)) continue;

                    }
                    if (dt < DateTime.Today || dt > DateTime.Today.AddYears(2)) continue;

                    var pro = Regex.Match(text,
                        @"([\p{L}\p{Lo}　・＆& ]+プロ(?:[＆&\s]+[\p{L}\p{Lo}　・ ]+プロ)?)")
                        .Value.Trim();
                    if (string.IsNullOrEmpty(pro)) continue;
                    var imgNames = d2.DocumentNode
                        .SelectNodes("//img[@src]")?
                        .Select(n => System.Net.WebUtility.UrlDecode(n.GetAttributeValue("src", "")))
                        .ToList() ?? new List<string>();
                    foreach (var img in imgNames)
                    {
                        var m2 = Regex.Match(img, @"([\p{L}\p{Lo}]+プロ)");
                        if (m2.Success)
                        {
                            pro = m2.Groups[1].Value.Trim(); break;
                        }


                        var times = Regex.Matches(text,
                            @"(\d{1,2}:\d{2})(?:スタート|開始)?")
                            .Cast<Match>().Select(m => m.Groups[1].Value)
                            .Distinct().Take(2).ToList();

                        events.Add(new ProChallengeEvent
                        {
                            ChainName = store.Chain,
                            StoreName = store.StoreName,
                            Prefecture = store.Prefecture,
                            EventDate = dt,
                            ProNames = pro,
                            TimeSlot1 = times.Count > 0 ? times[0] : "",
                            TimeSlot2 = times.Count > 1 ? times[1] : "",
                            SourceUrl = link,
                            ScrapedAt = scraped
                        });
                        await Task.Delay(400);
                    }
                }

                catch { }
            }
            return events;
        }


        private async Task<List<ProChallengeEvent>> ScrapeStarLane(
    StoreInfo store, DateTime scraped)
        {

            var html = await FetchHtml(store.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var baseUri = new Uri(store.Url);
            var events = new List<ProChallengeEvent>();

            var links = doc.DocumentNode
                .SelectNodes("//a[@href]")?
                .Select(n => n.GetAttributeValue("href", ""))
                .Where(h => Regex.IsMatch(h, @"20\d{2}/\d{2}/\d{2}/post"))
                .Select(h => new Uri(baseUri, h).ToString())
                .Distinct().Take(15).ToList() ?? new List<string>();


            foreach (var link in links)
            {
                try
                {
                    var detail = await FetchHtml(link);
                    var d2 = new HtmlDocument();
                    d2.LoadHtml(detail);
                    //var text = System.Net.WebUtility.HtmlDecode(d2.DocumentNode.InnerText);

                    var h1 = System.Net.WebUtility.HtmlDecode(
                        d2.DocumentNode.SelectSingleNode("//h1")?.InnerText ?? "");
                    h1 = h1.Replace("０", "0").Replace("１", "1").Replace("２", "2")
                   .Replace("３", "3").Replace("４", "4").Replace("５", "5")
                   .Replace("６", "6").Replace("７", "7").Replace("８", "8")
                   .Replace("９", "9");

                    if (!Regex.IsMatch(h1, @"(チャレンジ|投げよう|マッチ|プロ)")) continue;

                    var text = System.Net.WebUtility.HtmlDecode(d2.DocumentNode.InnerText);

                    // 全角数字を半角に変換
                    text = text
                        .Replace("０", "0").Replace("１", "1").Replace("２", "2")
                        .Replace("３", "3").Replace("４", "4").Replace("５", "5")
                        .Replace("６", "6").Replace("７", "7").Replace("８", "8")
                        .Replace("９", "9").Replace("　", " ");

                    if (!Regex.IsMatch(text, @"プロ.{0,20}(チャレンジ|投げよう|マッチ|バースデー)"))
                        continue;
                    var dm = Regex.Match(text, @"(\d{4})[年/](\d{1,2})[月/](\d{1,2})");
                    if (!dm.Success) continue;


                    if (!DateTime.TryParse(
                        $"{dm.Groups[1].Value}-{dm.Groups[2].Value}-{dm.Groups[3].Value}",
                        out var dt)) continue;
                    if (dt < DateTime.Today || dt > DateTime.Today.AddYears(2)) continue;

                    var proM = Regex.Match(h1, @"([\p{L}\p{Lo}]+プロ)");
                    var pro = proM.Success ? proM.Groups[1].Value.Trim()
                        : Regex.Match(text,
                        @"([\p{L}\p{Lo}　・＆& ]+プロ(?:[＆&\s]+[\p{L}\p{Lo}　・ ]+プロ)?)")
                        .Value.Trim();
                    if (string.IsNullOrEmpty(pro)) continue;
                    var times = Regex.Matches(text, @"(\d{1,2}:\d{2})(?:スタート|開始|～)?")
                        .Cast<Match>().Select(m => m.Groups[1].Value)
                        .Distinct().Take(2).ToList();
                    events.Add(new ProChallengeEvent
                    {
                        ChainName = store.Chain,
                        StoreName = store.StoreName,
                        Prefecture = store.Prefecture,
                        EventDate = dt,
                        ProNames = pro,
                        TimeSlot1 = times.Count > 0 ? times[0] : "",
                        TimeSlot2 = times.Count > 1 ? times[1] : "",
                        SourceUrl = link,
                        ScrapedAt = scraped
                    });
                    await Task.Delay(300);
                }

                catch { }
            }
            return events;
        }


        private static List<ProChallengeEvent> ParseByRegex(
            string html, StoreInfo store, DateTime scraped)
        {
            var events = new List<ProChallengeEvent>();
            var text = Regex.Replace(html, @"<[^>]+>", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            text = text.Replace("：", ":").Replace("　", " ");

            var dateRx = new Regex(@"(\d{4})[/年](\d{1,2})[/月](\d{1,2})");
            var proRx = new Regex(
                @"参加プロ(?:ボウラー)?[\s\S]{0,40}?" +
                @"([\p{L}\p{Lo} 　・＆&（）]+プロ[^\r\n【】。]{0,40})");
            var timeRx = new Regex(@"(\d{1,2}:\d{2})(?:スタート|開始)?");

            var segs = Regex.Split(text, @"(?=\d{4}[/年]\d{1,2}[/月])");
            foreach (var seg in segs)
            {
                var dm = dateRx.Match(seg);
                if (!dm.Success) continue;
                if (!DateTime.TryParse(
                    $"{dm.Groups[1].Value}-{dm.Groups[2].Value}-{dm.Groups[3].Value}",
                    out var dt)) continue;
                if (dt < DateTime.Today || dt > DateTime.Today.AddYears(2)) continue;

                var pm = proRx.Match(seg);
                if (!pm.Success) continue;

                var times = timeRx.Matches(seg).Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct().Take(2).ToList();

                events.Add(new ProChallengeEvent
                {
                    ChainName = store.Chain,
                    StoreName = store.StoreName,
                    Prefecture = store.Prefecture,
                    EventDate = dt,
                    ProNames = pm.Groups[1].Value.Trim(),
                    TimeSlot1 = times.Count > 0 ? times[0] : "",
                    TimeSlot2 = times.Count > 1 ? times[1] : "",
                    SourceUrl = store.Url,
                    ScrapedAt = scraped
                });
            }
            return events;
        }

        private static List<ProChallengeEvent> ParseByHAP(
            string html, StoreInfo store, DateTime scraped)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var text = System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
            return ParseByRegex(text, store, scraped);
        }

        private static List<ProChallengeEvent> ParseGenericEventPage(
            string html, StoreInfo store, string sourceUrl, DateTime scraped)
        {
            var text = ToPlainText(html);
            if (!text.Contains("プロ")) return new List<ProChallengeEvent>();
            if (!Regex.IsMatch(text, @"(チャレンジ|プロチャレ|大会|トーナメント|投げよう|スケジュール|予定|リーグ|マッチ|イベント)"))
                return new List<ProChallengeEvent>();

            var events = new List<ProChallengeEvent>();
            var dateMatches = Regex.Matches(text,
                @"(?:(20\d{2})\s*[年/\.-]\s*)?(\d{1,2})\s*[月/\.-]\s*(\d{1,2})\s*(?:日)?");

            for (var i = 0; i < dateMatches.Count; i++)
            {
                var match = dateMatches[i];
                var start = Math.Max(0, match.Index - 120);
                var end = i + 1 < dateMatches.Count
                    ? Math.Min(text.Length, dateMatches[i + 1].Index + 120)
                    : Math.Min(text.Length, match.Index + 700);
                var segment = text.Substring(start, end - start);

                if (!TryReadEventDate(match, out var eventDate)) continue;
                if (eventDate < DateTime.Today || eventDate > DateTime.Today.AddYears(2)) continue;
                if (!Regex.IsMatch(segment, @"(チャレンジ|プロチャレ|大会|投げよう|マッチ|イベント|リーグ)")) continue;

                var pros = ExtractProNames(segment).Take(4).ToList();
                if (pros.Count == 0) continue;

                var times = Regex.Matches(segment, @"(?<!\d)(\d{1,2}:\d{2})(?!\d)")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Where(IsLikelyTime)
                    .Distinct()
                    .Take(2)
                    .ToList();

                events.Add(new ProChallengeEvent
                {
                    ChainName = store.Chain,
                    StoreName = store.StoreName,
                    Prefecture = store.Prefecture,
                    EventDate = eventDate,
                    ProNames = string.Join("、", pros),
                    TimeSlot1 = times.Count > 0 ? times[0] : "",
                    TimeSlot2 = times.Count > 1 ? times[1] : "",
                    SourceUrl = sourceUrl,
                    ScrapedAt = scraped
                });
            }

            return Deduplicate(events);
        }

        private static IEnumerable<string> ExtractCandidateLinks(string html, string pageUrl)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var baseUri = new Uri(pageUrl);
            var host = baseUri.Host;

            return doc.DocumentNode
                .SelectNodes("//a[@href]")?
                .Select(a =>
                {
                    var href = a.GetAttributeValue("href", "");
                    var label = System.Net.WebUtility.HtmlDecode(a.InnerText ?? "");
                    return new { href, label };
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.href))
                .Where(x => !x.href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                .Where(x => !x.href.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    try
                    {
                        var uri = new Uri(baseUri, x.href);
                        return new
                        {
                            Url = uri.ToString(),
                            uri.Host,
                            Text = System.Net.WebUtility.UrlDecode(x.href + " " + x.label)
                        };
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null && x.Host == host)
                .Where(x => Regex.IsMatch(x!.Text,
                    @"(challenge|pro|event|schedule|tournament|game|news|information|チャレンジ|プロ|大会|予定|スケジュール|イベント|トーナメント)",
                    RegexOptions.IgnoreCase))
                .Where(x => !Regex.IsMatch(x!.Url,
                    @"\.(jpg|jpeg|png|gif|webp|svg|css|js|zip)(\?|$)",
                    RegexOptions.IgnoreCase))
                .Where(x => !Regex.IsMatch(x!.Url,
                    @"(result|staff|profile|ranking|record)",
                    RegexOptions.IgnoreCase))
                .Select(x => x!.Url)
                .Distinct()
                .ToList() ?? Enumerable.Empty<string>();
        }

        private static string ToPlainText(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var text = System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
            return NormalizeJapaneseText(text);
        }

        private static string NormalizeJapaneseText(string value)
        {
            var normalized = value
                .Replace("０", "0").Replace("１", "1").Replace("２", "2")
                .Replace("３", "3").Replace("４", "4").Replace("５", "5")
                .Replace("６", "6").Replace("７", "7").Replace("８", "8")
                .Replace("９", "9").Replace("　", " ")
                .Replace("：", ":");
            return Regex.Replace(normalized, @"\s+", " ");
        }

        private static bool TryReadEventDate(Match match, out DateTime eventDate)
        {
            var year = match.Groups[1].Success
                ? int.Parse(match.Groups[1].Value)
                : DateTime.Today.Year;
            var month = int.Parse(match.Groups[2].Value);
            var day = int.Parse(match.Groups[3].Value);

            eventDate = DateTime.MinValue;
            if (month < 1 || month > 12 || day < 1 || day > 31) return false;

            try
            {
                eventDate = new DateTime(year, month, day);
                if (!match.Groups[1].Success && eventDate < DateTime.Today)
                    eventDate = eventDate.AddYears(1);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryReadRoundOneDate(Match match, out DateTime eventDate)
        {
            var year = match.Groups["year"].Success
                ? int.Parse(match.Groups["year"].Value)
                : DateTime.Today.Year;
            var month = int.Parse(match.Groups["month"].Value);
            var day = int.Parse(match.Groups["day"].Value);

            eventDate = DateTime.MinValue;
            try
            {
                eventDate = new DateTime(year, month, day);
                if (!match.Groups["year"].Success && eventDate < DateTime.Today)
                    eventDate = eventDate.AddYears(1);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<string> ExtractProNames(string value)
        {
            var compact = Regex.Replace(
                value,
                @"(?<=[一-龯ぁ-んァ-ヶー々〆〇髙﨑])\s+(?=[一-龯ぁ-んァ-ヶー々〆〇髙﨑])",
                "");

            return Regex.Matches(compact, @"[一-龯ぁ-んァ-ヶー々〆〇髙﨑]{2,16}\s*プロ")
                .Cast<Match>()
                .Select(m => NormalizeProCandidate(Regex.Replace(m.Value, @"\s+", "")))
                .Where(IsLikelyGenericProName)
                .Distinct();
        }

        private static string NormalizeProCandidate(string value)
        {
            string[] prefixes =
            {
                "カップ", "ちらから", "らから", "より", "には", "海の日企画", "月の"
            };

            foreach (var prefix in prefixes)
            {
                if (value.StartsWith(prefix))
                    value = value[prefix.Length..];
            }

            if ((value.StartsWith("日") || value.StartsWith("年"))
                && value.Replace("プロ", "").Length > 3)
            {
                value = value[1..];
            }

            return value;
        }

        private static bool IsLikelyGenericProName(string value)
        {
            var baseName = value.Replace("プロ", "");
            if (value.Length < 4 || value.Length > 20) return false;
            if (baseName.Length < 3 || baseName.Length > 10) return false;
            if (value.Contains("専属プロ") || value.Contains("女子プロ") || value.Contains("男子プロ")) return false;
            if (value.Contains("プロショップ") || value.Contains("プロチャレ")) return false;
            if (value.Contains("スケジュール") || value.Contains("お知らせ")) return false;
            if (value.Contains("チャレンジ") || value.Contains("チャリティー")) return false;
            if (value.Contains("スタッフ") || value.Contains("所属") || value.Contains("出場")) return false;
            if (value.Contains("選手") || value.Contains("優勝") || value.Contains("月度")) return false;
            if (value.Contains("公益") || value.Contains("社団") || value.Contains("日本プロ")) return false;
            if (value.Contains("地区") || value.Contains("期生") || value.Contains("競技会")) return false;
            if (value.Contains("ゲーム") || value.Contains("オンライン") || value.Contains("卓球")) return false;
            if (value.Contains("県出身") || value.Contains("戦績") || value.Contains("開催")) return false;
            if (value.Contains("できる") || value.Contains("知りたく")) return false;
            if (Regex.IsMatch(value, @"(詳しく|こちら|見る|その他|コンペ|案内|団体|大会|イベント|シフト|表彰|トータル|ピン|限定|抽選|希望|必ず|内容|進呈|投目|カウント|センター|毎週|令和|最初|新人|専属|杯)")) return false;
            if (value.StartsWith("の") || value.StartsWith("と")) return false;
            if (IsPrefectureLabel(value)) return false;
            return true;
        }

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

        private static string NormalizeRoundOneVenue(string value) =>
            value.Trim()
                .Replace("博多・半道橋店", "博多・半道橋店")
                .Replace("中川1号線店", "中川1号線店");

        private static string RoundOnePrefecture(string venue) =>
            venue switch
            {
                "吉祥寺店" => "東京",
                "南砂店" => "東京",
                "府中本町駅前店" => "東京",
                "堺中央環状店" => "大阪",
                "博多・半道橋店" => "福岡",
                "中川1号線店" => "愛知",
                _ => ""
            };

        private static bool IsLikelyTime(string value) =>
            TimeSpan.TryParse(value, out var time)
            && time.Hours >= 6
            && time.Hours <= 23;

        private static bool IsPdfUrl(string value) =>
            value.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            || value.Contains(".pdf?", StringComparison.OrdinalIgnoreCase);

        private static List<ProChallengeEvent> Deduplicate(IEnumerable<ProChallengeEvent> events) =>
            events
                .GroupBy(e => $"{e.StoreName}|{e.EventDate:yyyy-MM-dd}|{e.ProNames}|{e.TimeSlot1}|{e.SourceUrl}")
                .Select(g => g.First())
                .OrderBy(e => e.EventDate)
                .ToList();

        private async Task<string> FetchHtml(string url)
        {
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var bytes = await resp.Content.ReadAsByteArrayAsync();
            var cs = resp.Content.Headers.ContentType?.CharSet?.ToLower() ?? "";
            Encoding enc = (cs.Contains("shift") || cs.Contains("sjis"))
                ? Encoding.GetEncoding("shift_jis") : Encoding.UTF8;
            var html = enc.GetString(bytes);
            var m = Regex.Match(html,
                @"charset\s*=\s*[""']?([^""'\s;>]+)", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                var c2 = m.Groups[1].Value.ToLower();
                if (c2.Contains("shift") || c2.Contains("sjis"))
                    html = Encoding.GetEncoding("shift_jis").GetString(bytes);
                else if (c2.Contains("euc"))
                    html = Encoding.GetEncoding("euc-jp").GetString(bytes);
            }
            return html;
        }
        private void Notify(string msg, int cur, int total, bool isError = false) =>
            OnProgress?.Invoke(new ScraperProgress
            { Message = msg, Current = cur, Total = total, IsError = isError });

        public void Dispose() => _http.Dispose();
    }
}
   



        
    
    

        
