using Microsoft.Web.WebView2.Core;
using System;
using System.Data;
using System.Drawing.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;




namespace allbowl01
{
    public partial class MainForm : Form
    {
        private readonly DatabaseManager _db;
        private bool _webViewReady = false;

        private string _sortBy = "EventDate";
        private string _sortDir = "ASC";
        private string _filterChain = "";
        private string _filterPref = "";
        private string _searchPro = "";
        public MainForm()
        {
            InitializeComponent();

            _db = new DatabaseManager(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "prochallenge.db"));
            InitWebView2Async();
        }
        private bool _isInitialLoading = true;
        private async void InitWebView2Async()
        {
            try
            {
                await webView.EnsureCoreWebView2Async();
                _webViewReady = true;
                webView.CoreWebView2.WebMessageReceived += OnWebMessage;
                // 【重要】ここで NavigationCompleted 時に PushDataToPage() を呼んでいたなら、削除します。
                // 代わりに、portal.html 側の JavaScript の最後に 
                // window.chrome.webview.postMessage("ready:init"); 
                // と書くことで、C# 側の OnWebMessage 内の "ready" ケースが一度だけ発火するようにします。

                // 3. ページの読み込み開始
                _isInitialLoading = true;
                LoadPortal();
                /*webView.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    PushDataToPage();
                };
                LoadPortal();*/
                //UpdateStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 初期化エラー:\n{ex.Message}",
        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void LoadPortal()
        {
            if (!_webViewReady) return;
            var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portal.html");
            // デバッグ確認
            if (File.Exists(htmlPath))
            {
                var url = "file:///" + htmlPath.Replace("\\", "/");
                
                

                webView.CoreWebView2.Navigate("file:///" + htmlPath.Replace("\\", "/"));
            }
            else
            {
                webView.CoreWebView2.NavigateToString(
                    "<h1>portal.htmlが見つかりません</h1>");
            }

        }
        private void OnWebMessage(
            object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var msg = e.TryGetWebMessageAsString();
            var parts = msg.Split(':', 3);
            if (parts.Length < 1) return;
            bool shouldUpdate = false;


            switch (parts[0])
            {
                case "ready":
                    if (_isInitialLoading) {
                        shouldUpdate = true;
                        _isInitialLoading = false;
                    }
                    break;

                case "sort":
                    _sortBy = parts.Length > 1 ? parts[1] : "EventDate";
                    _sortDir = parts.Length > 2 ? parts[2] : "ASC";
                    shouldUpdate = true;
                    break;
                case "filterChain":
                    _filterChain = parts.Length > 1 ? parts[1] : "";
                    shouldUpdate = true;
                    break;
                case "filterPref":
                    _filterPref = parts.Length > 1 ? parts[1] : "";
                    shouldUpdate = true;
                    break;
                case "search":
                    _searchPro = parts.Length > 1 ? parts[1] : "";
                    shouldUpdate = true;
                    break;       
            }
            if (shouldUpdate)
            {
                PushDataToPage();
            }
            
        }

        private void PushDataToPage()
        {
            if (!_webViewReady) return;
            if (_db == null) return;
            var json = _db.GetAllAsJson(
                _sortBy, _sortDir, _filterChain, _filterPref, _searchPro);
            var chains = "[\"" + string.Join("\",\"", _db.GetDistinctChains()) + "\"]";
            var prefs = "[\"" + string.Join("\",\"", _db.GetDistinctPrefs()) + "\"]";
            
            
            webView.CoreWebView2.ExecuteScriptAsync(
                $"window.loadData({json}, {chains}, {prefs});");
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            lblStatus.Text = $"DB: {_db.GetTotalCount()} 件登録済";
        }

        private async void btnScrape_Click(object sender, EventArgs e)
        {
            btnScrape.Enabled = false;
            btnScrape.Text = "取得中...";
            progressBar.Value = 0;
            txtLog.Clear();

            using var scraper = new ProChallengeScraper(_db);
            scraper.OnProgress += p =>
            {
                Invoke(() =>
                {
                    txtLog.AppendText(p.Message + Environment.NewLine);
                    txtLog.ScrollToCaret();
                    if (p.Total > 0)
                        progressBar.Value =
                            (int)((double)p.Current / p.Total * 100);
                });
            };

            try
            {
                await scraper.RunAsync();
                MessageBox.Show(
                    $"スクレイピング完了！\n新規: {scraper.NewCount} 件",
                    "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PushDataToPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"エラー:\n{ex.Message}",
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnScrape.Enabled = true;
                btnScrape.Text = "スクレイピング開始";
                progressBar.Value = 100;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _filterChain = "";
            _filterPref = "";
            _searchPro = "";
            _sortBy = "EventDate";
            _sortDir = "ASC";
            
            PushDataToPage();
        }

        private void MainForm_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            webView?.Dispose();
        }
    }
}



