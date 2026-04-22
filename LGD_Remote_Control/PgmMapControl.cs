using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace LGD_Remote_Control
{
    /// <summary>
    /// PGM 二维栅格地图显示控件
    /// 支持左键拖拽、滚轮缩放，鼠标悬停显示世界坐标（米）
    /// </summary>
    public class PgmMapControl : UserControl
    {
        // ===== 地图数据 =====
        private Bitmap _bitmap;        // 解码后的位图
        private float _resolution;    // 米/像素
        private float _originX;       // 地图左下角世界坐标 X
        private float _originY;       // 地图左下角世界坐标 Y

        // ===== 视图变换 =====
        private float _viewScale = 1f;      // 屏幕像素/地图像素
        private float _viewOffsetX = 0f;    // 平移 X（屏幕像素）
        private float _viewOffsetY = 0f;    // 平移 Y（屏幕像素）
        private Point _dragStart;
        private bool _dragging = false;
        private float _mouseX = -1f;  // 鼠标在面板内的屏幕坐标，-1 表示不在面板内
        private float _mouseY = -1f;

        // ===== 子控件 =====
        private DoubleBufferedPanel _mapPanel;
        private Label _lblStatus;
        private Button _btnZoomIn;
        private Button _btnZoomOut;

        public PgmMapControl()
        {
            // 状态栏
            _lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Text = "未加载地图",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 40, 40),
                Padding = new Padding(6, 0, 0, 0)
            };

            // 地图绘制面板（双缓冲）
            _mapPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DimGray
            };
            _mapPanel.Paint += MapPanel_Paint;
            _mapPanel.MouseDown += MapPanel_MouseDown;
            _mapPanel.MouseMove += MapPanel_MouseMove;
            _mapPanel.MouseUp += MapPanel_MouseUp;
            _mapPanel.MouseWheel += MapPanel_MouseWheel;
            _mapPanel.MouseLeave += (s, e) => { _mouseX = -1f; _mouseY = -1f; _mapPanel.Invalidate(); };
            _mapPanel.Resize += (s, e) => _mapPanel.Invalidate();

            // 右上角缩放按钮
            _btnZoomIn = new Button
            {
                Text = "+",
                Size = new Size(36, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            _btnZoomOut = new Button
            {
                Text = "-",
                Size = new Size(36, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };

            _btnZoomIn.Click += (s, e) => { ZoomAt(_mapPanel.Width / 2f, _mapPanel.Height / 2f, 1.25f); };
            _btnZoomOut.Click += (s, e) => { ZoomAt(_mapPanel.Width / 2f, _mapPanel.Height / 2f, 0.8f); };

            _mapPanel.Controls.Add(_btnZoomIn);
            _mapPanel.Controls.Add(_btnZoomOut);

            Controls.Add(_mapPanel);
            Controls.Add(_lblStatus);

            _mapPanel.SizeChanged += (s, e) =>
            {
                _btnZoomIn.Location = new Point(_mapPanel.Width - 46, 10);
                _btnZoomOut.Location = new Point(_mapPanel.Width - 46, 50);
            };
        }

        /// <summary>
        /// 加载 PGM 文件及 YAML 配置
        /// </summary>
        public void LoadMap(string pgmPath, string yamlPath)
        {
            _bitmap?.Dispose();
            _bitmap = null;

            // 解析 yaml
            _resolution = 0.05f;
            _originX = 0f;
            _originY = 0f;

            if (File.Exists(yamlPath))
            {
                foreach (var line in File.ReadAllLines(yamlPath))
                {
                    var parts = line.Split(':');
                    if (parts.Length < 2) continue;
                    string key = parts[0].Trim().ToLower();
                    string val = parts[1].Trim();

                    if (key == "resolution" && float.TryParse(val,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out float res))
                    {
                        _resolution = res;
                    }
                    else if (key == "origin")
                    {
                        // 格式: [x, y, z]
                        val = val.Trim('[', ']');
                        var nums = val.Split(',');
                        if (nums.Length >= 2)
                        {
                            float.TryParse(nums[0].Trim(),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out _originX);
                            float.TryParse(nums[1].Trim(),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out _originY);
                        }
                    }
                }
            }

            // 解码 PGM
            if (File.Exists(pgmPath))
            {
                _bitmap = LoadPgm(pgmPath);
            }

            if (_bitmap != null)
            {
                // 初始视图：居中适应
                FitToView();
                _lblStatus.Text = $"地图: {_bitmap.Width}×{_bitmap.Height} px  " +
                                  $"分辨率: {_resolution} m/px  " +
                                  $"原点: ({_originX}, {_originY})";
            }
            else
            {
                _lblStatus.Text = "PGM 文件加载失败";
            }

            _mapPanel.Invalidate();
        }

        private void FitToView()
        {
            if (_bitmap == null) return;
            float scaleX = (float)_mapPanel.Width / _bitmap.Width;
            float scaleY = (float)_mapPanel.Height / _bitmap.Height;
            _viewScale = Math.Min(scaleX, scaleY) * 0.9f;
            _viewOffsetX = (_mapPanel.Width - _bitmap.Width * _viewScale) / 2f;
            _viewOffsetY = (_mapPanel.Height - _bitmap.Height * _viewScale) / 2f;
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            if (_bitmap != null)
            {
                float w = _bitmap.Width * _viewScale;
                float h = _bitmap.Height * _viewScale;

                // PGM 第 0 行 = 图像最上方 = NORTH（高 Y），直接绘制即为标准北在上方向
                g.TranslateTransform(_viewOffsetX, _viewOffsetY);
                g.DrawImage(_bitmap, 0, 0, w, h);
                g.ResetTransform();
            }

            // 坐标叠加层：直接画在双缓冲面板上，不触发 Label 重绘
            if (_bitmap != null && _mouseX >= 0f && _mouseY >= 0f)
            {
                var world = ScreenToWorld(_mouseX, _mouseY);
                string text = $"X: {world.X:F3} m   Y: {world.Y:F3} m";
                using (var bgBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                using (var fgBrush = new SolidBrush(Color.White))
                {
                    SizeF sz = g.MeasureString(text, Font);
                    g.FillRectangle(bgBrush, 4f, _mapPanel.Height - sz.Height - 6f, sz.Width + 8f, sz.Height + 4f);
                    g.DrawString(text, Font, fgBrush, 8f, _mapPanel.Height - sz.Height - 4f);
                }
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                _viewOffsetX += e.X - _dragStart.X;
                _viewOffsetY += e.Y - _dragStart.Y;
                _dragStart = e.Location;
            }

            // 记录鼠标位置，在 Paint 叠加层中绘制坐标（双缓冲，无闪烁）
            _mouseX = e.X;
            _mouseY = e.Y;
            _mapPanel.Invalidate();
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _dragging = false;
        }

        private void MapPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            float factor = e.Delta > 0 ? 1.2f : (1f / 1.2f);
            ZoomAt(e.X, e.Y, factor);
        }

        private void ZoomAt(float cx, float cy, float factor)
        {
            _viewOffsetX = cx + (_viewOffsetX - cx) * factor;
            _viewOffsetY = cy + (_viewOffsetY - cy) * factor;
            _viewScale *= factor;
            _viewScale = Math.Max(0.01f, Math.Min(_viewScale, 500f));
            _mapPanel.Invalidate();
        }

        /// <summary>
        /// 将屏幕坐标转换为世界坐标（米）
        /// 注意 PGM 的像素行从图像顶部开始，但 origin 是左下角
        /// </summary>
        private PointF ScreenToWorld(float sx, float sy)
        {
            if (_bitmap == null) return PointF.Empty;

            // 屏幕 → 位图像素坐标（行号从最上计）
            float px = (sx - _viewOffsetX) / _viewScale;
            float py_top = (sy - _viewOffsetY) / _viewScale;

            // ROS 约定：origin 是左下角（SOUTH），第 0 行为 NORTH
            // wy = originY + (height - 1 - row_from_top) * resolution
            float wx = _originX + px * _resolution;
            float wy = _originY + (_bitmap.Height - 1 - py_top) * _resolution;
            return new PointF(wx, wy);
        }

        /// <summary>
        /// 解析 PGM 文件（P2 ASCII / P5 Binary）
        /// </summary>
        private static Bitmap LoadPgm(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // 读取头部
                string magic = ReadToken(fs);
                bool isBinary = magic == "P5";

                string widthStr = ReadToken(fs);
                string heightStr = ReadToken(fs);
                string maxValStr = ReadToken(fs);

                int width = int.Parse(widthStr);
                int height = int.Parse(heightStr);
                int maxVal = int.Parse(maxValStr);

                var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, width, height);
                var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                int stride = bmpData.Stride;
                byte[] buf = new byte[stride * height];

                if (isBinary)
                {
                    // P5: 二进制灰度
                    if (maxVal <= 255)
                    {
                        byte[] raw = new byte[width * height];
                        int read = 0;
                        while (read < raw.Length)
                        {
                            int r = fs.Read(raw, read, raw.Length - read);
                            if (r == 0) break;
                            read += r;
                        }
                        for (int y = 0; y < height; y++)
                            for (int x = 0; x < width; x++)
                            {
                                byte v = (byte)(raw[y * width + x] * 255 / maxVal);
                                int idx = y * stride + x * 4;
                                buf[idx] = v; buf[idx + 1] = v; buf[idx + 2] = v; buf[idx + 3] = 255;
                            }
                    }
                }
                else
                {
                    // P2: ASCII
                    using (var sr = new StreamReader(fs))
                    {
                        for (int y = 0; y < height; y++)
                            for (int x = 0; x < width; x++)
                            {
                                string tok = ReadTokenFromReader(sr);
                                if (!int.TryParse(tok, out int val)) val = 0;
                                byte v = (byte)(val * 255 / maxVal);
                                int idx = y * stride + x * 4;
                                buf[idx] = v; buf[idx + 1] = v; buf[idx + 2] = v; buf[idx + 3] = 255;
                            }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(buf, 0, bmpData.Scan0, buf.Length);
                bmp.UnlockBits(bmpData);
                return bmp;
            }
        }

        private static string ReadToken(Stream s)
        {
            var sb = new System.Text.StringBuilder();
            int b;
            // 跳过空白和注释
            while ((b = s.ReadByte()) != -1)
            {
                char c = (char)b;
                if (c == '#')
                {
                    // 跳过注释行
                    while ((b = s.ReadByte()) != -1 && (char)b != '\n') { }
                    continue;
                }
                if (!char.IsWhiteSpace(c)) { sb.Append(c); break; }
            }
            while ((b = s.ReadByte()) != -1)
            {
                char c = (char)b;
                if (char.IsWhiteSpace(c)) break;
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static string ReadTokenFromReader(StreamReader sr)
        {
            var sb = new System.Text.StringBuilder();
            int b;
            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (c == '#') { while ((b = sr.Read()) != -1 && (char)b != '\n') { } continue; }
                if (!char.IsWhiteSpace(c)) { sb.Append(c); break; }
            }
            while ((b = sr.Peek()) != -1)
            {
                if (char.IsWhiteSpace((char)b)) break;
                sb.Append((char)sr.Read());
            }
            return sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _bitmap?.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// 启用双缓冲的自定义面板，防止重绘闪烁
    /// </summary>
    internal class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);
            UpdateStyles();
        }
    }
}
