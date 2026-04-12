namespace LGD_Remote_Control
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelVideoTop = new System.Windows.Forms.Panel();
            this.videoView = new LibVLCSharp.WinForms.VideoView();
            this.panelRtspBar = new System.Windows.Forms.Panel();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtRtspUrl = new System.Windows.Forms.TextBox();
            this.lblRtspUrl = new System.Windows.Forms.Label();
            this.panelMapTop = new System.Windows.Forms.Panel();
            this.gMapControl = new GMap.NET.WindowsForms.GMapControl();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.panelMapBar = new System.Windows.Forms.Panel();
            this.lblMapInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelVideoTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoView)).BeginInit();
            this.panelRtspBar.SuspendLayout();
            this.panelMapTop.SuspendLayout();
            this.gMapControl.SuspendLayout();
            this.panelMapBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.panelVideoTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panelMapTop);
            this.splitContainerMain.Size = new System.Drawing.Size(1200, 700);
            this.splitContainerMain.SplitterDistance = 727;
            this.splitContainerMain.TabIndex = 0;
            // 
            // panelVideoTop
            // 
            this.panelVideoTop.Controls.Add(this.videoView);
            this.panelVideoTop.Controls.Add(this.panelRtspBar);
            this.panelVideoTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVideoTop.Location = new System.Drawing.Point(0, 0);
            this.panelVideoTop.Name = "panelVideoTop";
            this.panelVideoTop.Size = new System.Drawing.Size(727, 700);
            this.panelVideoTop.TabIndex = 0;
            // 
            // videoView
            // 
            this.videoView.BackColor = System.Drawing.Color.Black;
            this.videoView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoView.Location = new System.Drawing.Point(0, 40);
            this.videoView.MediaPlayer = null;
            this.videoView.Name = "videoView";
            this.videoView.Size = new System.Drawing.Size(727, 660);
            this.videoView.TabIndex = 0;
            // 
            // panelRtspBar
            // 
            this.panelRtspBar.Controls.Add(this.btnDisconnect);
            this.panelRtspBar.Controls.Add(this.btnConnect);
            this.panelRtspBar.Controls.Add(this.txtRtspUrl);
            this.panelRtspBar.Controls.Add(this.lblRtspUrl);
            this.panelRtspBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRtspBar.Location = new System.Drawing.Point(0, 0);
            this.panelRtspBar.Name = "panelRtspBar";
            this.panelRtspBar.Size = new System.Drawing.Size(727, 40);
            this.panelRtspBar.TabIndex = 1;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(460, 7);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(70, 27);
            this.btnDisconnect.TabIndex = 0;
            this.btnDisconnect.Text = "断开";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(385, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(70, 27);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtRtspUrl
            // 
            this.txtRtspUrl.Location = new System.Drawing.Point(75, 9);
            this.txtRtspUrl.Name = "txtRtspUrl";
            this.txtRtspUrl.Size = new System.Drawing.Size(300, 21);
            this.txtRtspUrl.TabIndex = 2;
            this.txtRtspUrl.Text = "rtsp://";
            // 
            // lblRtspUrl
            // 
            this.lblRtspUrl.AutoSize = true;
            this.lblRtspUrl.Location = new System.Drawing.Point(8, 12);
            this.lblRtspUrl.Name = "lblRtspUrl";
            this.lblRtspUrl.Size = new System.Drawing.Size(59, 12);
            this.lblRtspUrl.TabIndex = 3;
            this.lblRtspUrl.Text = "RTSP地址:";
            // 
            // panelMapTop
            // 
            this.panelMapTop.Controls.Add(this.gMapControl);
            this.panelMapTop.Controls.Add(this.panelMapBar);
            this.panelMapTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMapTop.Location = new System.Drawing.Point(0, 0);
            this.panelMapTop.Name = "panelMapTop";
            this.panelMapTop.Size = new System.Drawing.Size(469, 700);
            this.panelMapTop.TabIndex = 0;
            // 
            // gMapControl
            // 
            this.gMapControl.Bearing = 0F;
            this.gMapControl.CanDragMap = true;
            this.gMapControl.Controls.Add(this.btnZoomIn);
            this.gMapControl.Controls.Add(this.btnZoomOut);
            this.gMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl.EmptyTileColor = System.Drawing.Color.LightGray;
            this.gMapControl.GrayScaleMode = false;
            this.gMapControl.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl.LevelsKeepInMemmory = 5;
            this.gMapControl.Location = new System.Drawing.Point(0, 40);
            this.gMapControl.MarkersEnabled = true;
            this.gMapControl.MaxZoom = 18;
            this.gMapControl.MinZoom = 1;
            this.gMapControl.MouseWheelZoomEnabled = true;
            this.gMapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl.Name = "gMapControl";
            this.gMapControl.NegativeMode = false;
            this.gMapControl.PolygonsEnabled = true;
            this.gMapControl.RetryLoadTile = 0;
            this.gMapControl.RoutesEnabled = true;
            this.gMapControl.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl.ShowTileGridLines = false;
            this.gMapControl.Size = new System.Drawing.Size(469, 660);
            this.gMapControl.TabIndex = 0;
            this.gMapControl.Zoom = 10D;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.Location = new System.Drawing.Point(508, 10);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(36, 36);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.Location = new System.Drawing.Point(508, 50);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(36, 36);
            this.btnZoomOut.TabIndex = 0;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // panelMapBar
            // 
            this.panelMapBar.Controls.Add(this.lblMapInfo);
            this.panelMapBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMapBar.Location = new System.Drawing.Point(0, 0);
            this.panelMapBar.Name = "panelMapBar";
            this.panelMapBar.Size = new System.Drawing.Size(469, 40);
            this.panelMapBar.TabIndex = 1;
            // 
            // lblMapInfo
            // 
            this.lblMapInfo.AutoSize = true;
            this.lblMapInfo.Location = new System.Drawing.Point(8, 12);
            this.lblMapInfo.Name = "lblMapInfo";
            this.lblMapInfo.Size = new System.Drawing.Size(53, 12);
            this.lblMapInfo.TabIndex = 2;
            this.lblMapInfo.Text = "瓦片地图";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LGD 远程控制系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelVideoTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.videoView)).EndInit();
            this.panelRtspBar.ResumeLayout(false);
            this.panelRtspBar.PerformLayout();
            this.panelMapTop.ResumeLayout(false);
            this.gMapControl.ResumeLayout(false);
            this.panelMapBar.ResumeLayout(false);
            this.panelMapBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panelVideoTop;
        private System.Windows.Forms.Panel panelRtspBar;
        private System.Windows.Forms.Label lblRtspUrl;
        private System.Windows.Forms.TextBox txtRtspUrl;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private LibVLCSharp.WinForms.VideoView videoView;
        private System.Windows.Forms.Panel panelMapTop;
        private System.Windows.Forms.Panel panelMapBar;
        private System.Windows.Forms.Label lblMapInfo;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private GMap.NET.WindowsForms.GMapControl gMapControl;
    }
}

