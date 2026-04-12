using System;
using System.IO;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace LGD_Remote_Control
{
    /// <summary>
    /// 本地瓦片地图提供者 - 从本地文件夹加载百度瓦片
    /// 瓦片存放路径格式: tiles/{z}/{x}/{y}.png
    /// </summary>
    public class LocalTileProvider : GMapProvider
    {
        public static readonly LocalTileProvider Instance = new LocalTileProvider();

        private readonly Guid _id = new Guid("E1E4E5F0-1234-4ABC-9DEF-AABBCCDDEEFF");

        // 瓦片根目录（可在外部设置）
        public string TilesRootPath { get; set; }

        public override Guid Id => _id;
        public override string Name => "LocalTileProvider";
        public override PureProjection Projection => GMap.NET.Projections.MercatorProjection.Instance;

        private GMapProvider[] _overlays;
        public override GMapProvider[] Overlays => _overlays ?? (_overlays = new GMapProvider[] { this });

        public LocalTileProvider()
        {
            // 默认瓦片路径：项目目录下的 Map\Tiles_BIGEMAP 文件夹
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            TilesRootPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\Map\Tiles_BIGEMAP"));
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            // 瓦片文件路径: Tiles_BIGEMAP/{z}/{x}/{y}.png
            string tilePath = Path.Combine(TilesRootPath, zoom.ToString(), pos.X.ToString(), pos.Y.ToString() + ".png");

            if (File.Exists(tilePath))
            {
                try
                {
                    byte[] data = File.ReadAllBytes(tilePath);
                    if (data.Length > 0)
                    {
                        return GMapImageProxy.Instance.FromArray(data);
                    }
                }
                catch
                {
                    // 读取失败返回 null
                }
            }

            return null;
        }
    }
}
