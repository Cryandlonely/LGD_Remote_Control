using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LGD_Remote_Control
{
    public class PointCloudControl : UserControl
    {
        private OpenTK.GLControl _glControl;
        private Label _lblStatus;
        private PointXYZ[] _points;
        private bool _glReady = false;

        // 相机旋转与缩放
        private float _rotX = 30f;
        private float _rotY = 0f;
        private float _zoom = 1f;
        private Point _lastMouse;
        private bool _dragging = false;

        // 点云包围盒
        private float _centerX, _centerY, _centerZ;
        private float _scale = 1f;
        private float _zMin, _zMax;

        public PointCloudControl()
        {
            _lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Text = "未加载点云文件",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 40, 40),
                Padding = new Padding(6, 0, 0, 0)
            };

            _glControl = new OpenTK.GLControl(new GraphicsMode(32, 24, 0, 4));
            _glControl.Dock = DockStyle.Fill;
            _glControl.Load += GlControl_Load;
            _glControl.Paint += GlControl_Paint;
            _glControl.Resize += GlControl_Resize;
            _glControl.MouseDown += GlControl_MouseDown;
            _glControl.MouseMove += GlControl_MouseMove;
            _glControl.MouseUp += GlControl_MouseUp;
            _glControl.MouseWheel += GlControl_MouseWheel;

            Controls.Add(_glControl);
            Controls.Add(_lblStatus);
            BackColor = Color.Black;
        }

        public void LoadPoints(PointXYZ[] points)
        {
            _points = points;
            if (points == null || points.Length == 0)
            {
                _lblStatus.Text = "点云为空";
                return;
            }

            // 计算包围盒与中心
            float xMin = float.MaxValue, xMax = float.MinValue;
            float yMin = float.MaxValue, yMax = float.MinValue;
            _zMin = float.MaxValue; _zMax = float.MinValue;

            foreach (var p in points)
            {
                if (p.X < xMin) xMin = p.X;
                if (p.X > xMax) xMax = p.X;
                if (p.Y < yMin) yMin = p.Y;
                if (p.Y > yMax) yMax = p.Y;
                if (p.Z < _zMin) _zMin = p.Z;
                if (p.Z > _zMax) _zMax = p.Z;
            }

            _centerX = (xMin + xMax) / 2f;
            _centerY = (yMin + yMax) / 2f;
            _centerZ = (_zMin + _zMax) / 2f;

            float maxRange = Math.Max(Math.Max(xMax - xMin, yMax - yMin), _zMax - _zMin);
            _scale = maxRange > 0.001f ? 2f / maxRange : 1f;

            _lblStatus.Text = $"点数: {points.Length:N0}    " +
                              $"X:[{xMin:F2}, {xMax:F2}]  " +
                              $"Y:[{yMin:F2}, {yMax:F2}]  " +
                              $"Z:[{_zMin:F2}, {_zMax:F2}]";

            if (_glReady)
                _glControl.Invalidate();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            _glReady = true;
            GL.ClearColor(0.08f, 0.08f, 0.08f, 1f);
            GL.Enable(EnableCap.DepthTest);
            GL.PointSize(2f);
            SetupViewport();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            if (!_glReady) return;
            SetupViewport();
            _glControl.Invalidate();
        }

        private void SetupViewport()
        {
            int w = _glControl.Width;
            int h = _glControl.Height > 0 ? _glControl.Height : 1;
            GL.Viewport(0, 0, w, h);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f), (float)w / h, 0.01f, 1000f);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            if (!_glReady) return;
            _glControl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            float dist = 3f / _zoom;
            GL.Translate(0f, 0f, -dist);
            GL.Rotate(_rotX, 1f, 0f, 0f);
            GL.Rotate(_rotY, 0f, 0f, 1f);

            DrawGrid();
            DrawPoints();

            _glControl.SwapBuffers();
        }

        private void DrawPoints()
        {
            if (_points == null || _points.Length == 0) return;

            float zRange = _zMax - _zMin;
            if (zRange < 0.001f) zRange = 1f;

            GL.Begin(PrimitiveType.Points);
            foreach (var p in _points)
            {
                // 高度颜色映射: 低=蓝, 中=绿, 高=红
                float t = (p.Z - _zMin) / zRange;
                float r = Math.Max(0f, t * 2f - 1f);
                float g = 1f - Math.Abs(t * 2f - 1f);
                float b = Math.Max(0f, 1f - t * 2f);
                GL.Color3(r, g, b);
                GL.Vertex3(
                    (p.X - _centerX) * _scale,
                    (p.Y - _centerY) * _scale,
                    (p.Z - _centerZ) * _scale);
            }
            GL.End();
        }

        private void DrawGrid()
        {
            GL.Color3(0.25f, 0.25f, 0.25f);
            GL.Begin(PrimitiveType.Lines);
            for (float i = -2f; i <= 2.01f; i += 0.5f)
            {
                GL.Vertex3(i, -2f, -1f);
                GL.Vertex3(i, 2f, -1f);
                GL.Vertex3(-2f, i, -1f);
                GL.Vertex3(2f, i, -1f);
            }
            GL.End();
        }


        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _lastMouse = e.Location;
            }
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                _rotY += (e.X - _lastMouse.X) * 0.5f;
                _rotX += (e.Y - _lastMouse.Y) * 0.5f;
                _lastMouse = e.Location;
                _glControl.Invalidate();
            }
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _dragging = false;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            _zoom *= e.Delta > 0 ? 1.15f : (1f / 1.15f);
            _zoom = Math.Max(0.05f, Math.Min(_zoom, 200f));
            _glControl.Invalidate();
        }
    }
}
