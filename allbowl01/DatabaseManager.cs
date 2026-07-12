using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

namespace allbowl01
{
    public class ProChallengeEvent
    {
        public int Id { get; set; }
        public string ChainName { get; set; } = "";
        public string StoreName { get; set; } = "";
        public string Prefecture { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string ProNames { get; set; } = "";
        public string TimeSlot1 { get; set; } = "";
        public string TimeSlot2 { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public DateTime ScrapedAt { get; set; }
    }
    public class DatabaseManager
    {
        private readonly string _dbPath;

        public DatabaseManager(string dbPath = "prochallenge.db")
        {
            _dbPath = dbPath;
            Initialize();
        }

        private void Initialize()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ProChallengeEvents (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    ChainName   TEXT NOT NULL DEFAULT '',
                    StoreName   TEXT NOT NULL,
                    Prefecture  TEXT NOT NULL DEFAULT '',
                    EventDate   TEXT NOT NULL,
                    ProNames    TEXT NOT NULL,
                    TimeSlot1   TEXT NOT NULL DEFAULT '',
                    TimeSlot2   TEXT NOT NULL DEFAULT '',
                    SourceUrl   TEXT NOT NULL DEFAULT '',
                    ScrapedAt   TEXT NOT NULL,
                    UNIQUE(StoreName, EventDate, ProNames)
                );
                CREATE INDEX IF NOT EXISTS idx_date  ON ProChallengeEvents(EventDate);
                CREATE INDEX IF NOT EXISTS idx_store ON ProChallengeEvents(StoreName);
                CREATE INDEX IF NOT EXISTS idx_chain ON ProChallengeEvents(ChainName);
                CREATE INDEX IF NOT EXISTS idx_pref  ON ProChallengeEvents(Prefecture);
            ";
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            return conn;
        }

        public int Upsert(ProChallengeEvent ev)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ProChallengeEvents
                    (ChainName, StoreName, Prefecture, EventDate,
                     ProNames, TimeSlot1, TimeSlot2, SourceUrl, ScrapedAt)
                VALUES
                    ($chain, $store, $pref, $date,
                     $pro, $t1, $t2, $url, $scraped)
                ON CONFLICT(StoreName, EventDate, ProNames) DO UPDATE SET
                    ChainName  = excluded.ChainName,
                    Prefecture = excluded.Prefecture,
                    TimeSlot1  = excluded.TimeSlot1,
                    TimeSlot2  = excluded.TimeSlot2,
                    SourceUrl  = excluded.SourceUrl,
                    ScrapedAt  = excluded.ScrapedAt;
            ";
            cmd.Parameters.AddWithValue("$chain", ev.ChainName);
            cmd.Parameters.AddWithValue("$store", ev.StoreName);
            cmd.Parameters.AddWithValue("$pref", ev.Prefecture);
            cmd.Parameters.AddWithValue("$date", ev.EventDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$pro", ev.ProNames);
            cmd.Parameters.AddWithValue("$t1", ev.TimeSlot1);
            cmd.Parameters.AddWithValue("$t2", ev.TimeSlot2);
            cmd.Parameters.AddWithValue("$url", ev.SourceUrl);
            cmd.Parameters.AddWithValue("$scraped", ev.ScrapedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            return cmd.ExecuteNonQuery();
        }

        public string GetAllAsJson(
            string sortBy = "EventDate",
            string sortDir = "ASC",
            string filterChain = "",
            string filterPref = "",
            string searchPro = "")
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();

            var where = new List<string>();
            if (!string.IsNullOrEmpty(filterChain))
            {
                where.Add("ChainName = $chain");
                cmd.Parameters.AddWithValue("$chain", filterChain);
            }
            if (!string.IsNullOrEmpty(filterPref))
            {
                where.Add("Prefecture = $pref");
                cmd.Parameters.AddWithValue("$pref", filterPref);
            }
            if (!string.IsNullOrEmpty(searchPro))
            {
                where.Add("ProNames LIKE $pro");
                cmd.Parameters.AddWithValue("$pro", $"%{searchPro}%");
            }

            var allowed = new System.Collections.Generic.HashSet<string>
                { "EventDate","StoreName","ChainName","Prefecture","ProNames" };
            if (!allowed.Contains(sortBy)) sortBy = "EventDate";
            if (sortDir != "DESC") sortDir = "ASC";

            var wc = where.Count > 0
                ? "WHERE " + string.Join(" AND ", where) : "";

            cmd.CommandText = $@"
                SELECT Id, ChainName, StoreName, Prefecture, EventDate,
                       ProNames, TimeSlot1, TimeSlot2, SourceUrl
                FROM ProChallengeEvents
                {wc}
                ORDER BY {sortBy} {sortDir}
            ";

            var sb = new System.Text.StringBuilder("[");
            bool first = true;
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append('{');
                sb.Append($"\"id\":{r.GetInt32(0)},");
                sb.Append($"\"chain\":{J(r.GetString(1))},");
                sb.Append($"\"store\":{J(r.GetString(2))},");
                sb.Append($"\"pref\":{J(r.GetString(3))},");
                sb.Append($"\"date\":{J(r.GetString(4))},");
                sb.Append($"\"pro\":{J(r.GetString(5))},");
                sb.Append($"\"t1\":{J(r.GetString(6))},");
                sb.Append($"\"t2\":{J(r.GetString(7))},");
                sb.Append($"\"url\":{J(r.GetString(8))}");
                sb.Append('}');
            }
            sb.Append(']');
            return sb.ToString();
        }

        public List<string> GetDistinctChains()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT DISTINCT ChainName FROM ProChallengeEvents " +
                "WHERE ChainName != '' ORDER BY ChainName";
            var list = new List<string>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));
            return list;
        }

        public List<string> GetDistinctPrefs()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT DISTINCT Prefecture FROM ProChallengeEvents " +
                "WHERE Prefecture != '' ORDER BY Prefecture";
            var list = new List<string>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(r.GetString(0));
            return list;
        }

        public int GetTotalCount()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM ProChallengeEvents";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<ProChallengeEvent> GetEvents(
            string sortBy = "EventDate",
            string sortDir = "ASC")
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();

            var allowed = new System.Collections.Generic.HashSet<string>
                { "EventDate","StoreName","ChainName","Prefecture","ProNames" };
            if (!allowed.Contains(sortBy)) sortBy = "EventDate";
            if (sortDir != "DESC") sortDir = "ASC";

            cmd.CommandText = $@"
                SELECT Id, ChainName, StoreName, Prefecture, EventDate,
                       ProNames, TimeSlot1, TimeSlot2, SourceUrl, ScrapedAt
                FROM ProChallengeEvents
                ORDER BY {sortBy} {sortDir}
            ";

            var events = new List<ProChallengeEvent>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                events.Add(new ProChallengeEvent
                {
                    Id = r.GetInt32(0),
                    ChainName = r.GetString(1),
                    StoreName = r.GetString(2),
                    Prefecture = r.GetString(3),
                    EventDate = DateTime.Parse(r.GetString(4)),
                    ProNames = r.GetString(5),
                    TimeSlot1 = r.GetString(6),
                    TimeSlot2 = r.GetString(7),
                    SourceUrl = r.GetString(8),
                    ScrapedAt = DateTime.Parse(r.GetString(9))
                });
            }

            return events;
        }

        private static string J(string s) =>
            "\"" + s.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "")
                    .Replace("\n", " ") + "\"";
    }
}
