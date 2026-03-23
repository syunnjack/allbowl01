namespace allbowl01
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>


        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelLeft;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.Button btnScrape;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.RichTextBox txtLog;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnScrape = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblLogTitle = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.SuspendLayout();

            // ── panelTop ──────────────────────────────────────
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.panelTop.Controls.Add(this.lblStatus);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.panelTop.Size = new System.Drawing.Size(1400, 52);
            this.panelTop.TabIndex = 0;

            // ── lblTitle ──────────────────────────────────────
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Yu Gothic UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(232, 0, 45);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "全国プロチャレンジ情報ポータル";

            // ── lblStatus ─────────────────────────────────────
            this.lblStatus.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.lblStatus.Location = new System.Drawing.Point(1180, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 15);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "DB: 0 件登録済";


            // ── panelLeft ─────────────────────────────────────
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.panelLeft.Controls.Add(this.txtLog);
            this.panelLeft.Controls.Add(this.lblLogTitle);
            this.panelLeft.Controls.Add(this.progressBar);
            this.panelLeft.Controls.Add(this.btnRefresh);
            this.panelLeft.Controls.Add(this.btnScrape);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 52);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(12);
            this.panelLeft.Size = new System.Drawing.Size(220, 848);
            this.panelLeft.TabIndex = 1;


            // ── btnScrape ─────────────────────────────────────
            this.btnScrape.BackColor = System.Drawing.Color.FromArgb(232, 0, 45);
            this.btnScrape.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnScrape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            
            this.btnScrape.Font = new System.Drawing.Font("Yu Gothic UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnScrape.ForeColor = System.Drawing.Color.White;
            this.btnScrape.Location = new System.Drawing.Point(12, 16);
            this.btnScrape.Name = "btnScrape";
            this.btnScrape.Size = new System.Drawing.Size(196, 42);
            this.btnScrape.TabIndex = 0;
            this.btnScrape.Text = "スクレイピング開始";
            this.btnScrape.UseVisualStyleBackColor = false;
            this.btnScrape.FlatAppearance.BorderSize = 0;
            this.btnScrape.Click += new System.EventHandler(this.btnScrape_Click);
            

            this.btnScrape.Size = new System.Drawing.Size(196, 42);
            this.btnScrape.Location = new System.Drawing.Point(12, 16);
            this.btnScrape.BackColor = System.Drawing.Color.FromArgb(232, 0, 45);
            this.btnScrape.ForeColor = System.Drawing.Color.White;
            this.btnScrape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrape.Click += new System.EventHandler(this.btnScrape_Click);

            // ── btnRefresh ────────────────────────────────────
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            
            this.btnRefresh.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.btnRefresh.Location = new System.Drawing.Point(12, 66);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(196, 36);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "表示を更新";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.FlatAppearance.BorderSize = 1;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // ── progressBar ───────────────────────────────────
            this.progressBar.Location = new System.Drawing.Point(12, 112);
            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(196, 10);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 2;

            // ── lblLogTitle ───────────────────────────────────
            this.lblLogTitle.AutoSize = true;
            this.lblLogTitle.Font = new System.Drawing.Font("Consolas", 8F);
            this.lblLogTitle.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblLogTitle.Location = new System.Drawing.Point(12, 132);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(28, 13);
            this.lblLogTitle.TabIndex = 3;
            this.lblLogTitle.Text = "LOG";

            // ── txtLog ────────────────────────────────────────
            this.txtLog.Anchor = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Bottom
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(12, 12, 12);
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(130, 200, 130);
            this.txtLog.Location = new System.Drawing.Point(12, 152);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(196, 650);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";


            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(220, 52);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1180, 848);
            this.webView.TabIndex = 2;
            this.webView.ZoomFactor = 1D;



            // Step6: フォームにコントロールを追加（順序重要）
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Yu Gothic", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(240, 240, 235);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "全国プロチャレンジ情報 2026";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);


            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);   // レイアウト計算を再開
        }
    }
}


