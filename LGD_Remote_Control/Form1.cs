using System;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace LGD_Remote_Control
{
    public partial class Form1 : Form
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ========== 初始化 VLC ==========
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = _mediaPlayer;

            // ========== 初始化地图 ==========
            // 启用WinForms图片代理
            GMap.NET.WindowsForms.GMapImageProxy.Enable();

            gMapControl.MapProvider = LocalTileProvider.Instance;
            gMapControl.Manager.Mode = AccessMode.ServerOnly; // 仅从Provider加载（本地文件）
            gMapControl.ShowCenter = false;
            gMapControl.DragButton = System.Windows.Forms.MouseButtons.Left; // 左键拖拽地图

            // 中心点和缩放范围（来自 map.ini）
            gMapControl.Position = new PointLatLng(37.14627525, 117.10391521);
            gMapControl.Zoom = 16;
            gMapControl.MinZoom = 13;
            gMapControl.MaxZoom = 20;

            // 鼠标移动时显示经纬度
            gMapControl.MouseMove += (ms, me) =>
            {
                var latLng = gMapControl.FromLocalToLatLng(me.X, me.Y);
                lblMapInfo.Text = $"经度: {latLng.Lng:F6}  纬度: {latLng.Lat:F6}  缩放: {gMapControl.Zoom}";
            };

            // 缩放按钮定位到地图右上角
            btnZoomIn.Location = new System.Drawing.Point(gMapControl.Width - 46, 10);
            btnZoomOut.Location = new System.Drawing.Point(gMapControl.Width - 46, 50);
            btnZoomIn.BringToFront();
            btnZoomOut.BringToFront();

            // ========== 初始化点云 ==========
            string pcdPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\PCD\scans.pcd"));
            if (System.IO.File.Exists(pcdPath))
            {
                try
                {
                    var pts = PcdParser.Load(pcdPath);
                    pointCloudControl.LoadPoints(pts);
                }
                catch (Exception ex)
                {
                    pointCloudControl.LoadPoints(null);
                    System.Diagnostics.Debug.WriteLine("PCD加载失败: " + ex.Message);
                }
            }

            // ========== 初始化PGM地图 ==========
            string pgmPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\PGM\map.pgm"));
            string yamlPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\PGM\map.yaml"));
            pgmMapControl.LoadMap(pgmPath, yamlPath);
        }

        /// <summary>
        /// 连接RTSP视频流
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            string rtspUrl = txtRtspUrl.Text.Trim();
            if (string.IsNullOrEmpty(rtspUrl))
            {
                MessageBox.Show("请输入RTSP地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 停止之前的播放
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }

                var media = new Media(_libVLC, new Uri(rtspUrl));
                // 设置网络缓存（毫秒），降低延迟
                media.AddOption(":network-caching=300");
                media.AddOption(":rtsp-tcp");

                _mediaPlayer.Play(media);

                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 断开RTSP视频流
        /// </summary>
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("断开失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 地图放大
        /// </summary>
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom < gMapControl.MaxZoom)
            {
                gMapControl.Zoom += 1;
            }
        }

        /// <summary>
        /// 地图缩小
        /// </summary>
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (gMapControl.Zoom > gMapControl.MinZoom)
            {
                gMapControl.Zoom -= 1;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 释放VLC资源
            try
            {
                _mediaPlayer?.Stop();
                _mediaPlayer?.Dispose();
                _libVLC?.Dispose();
            }
            catch { }
        }
    }
}
