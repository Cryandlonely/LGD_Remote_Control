using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

namespace LGD_Remote_Control
{
    public struct PointXYZ
    {
        public float X, Y, Z;
        public PointXYZ(float x, float y, float z) { X = x; Y = y; Z = z; }
    }

    public static class PcdParser
    {
        public static PointXYZ[] Load(string filePath)
        {
            var points = new List<PointXYZ>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // ===== 逐字节读取头部，直到 DATA 行 =====
                var headerLines = new List<string>();
                var sb = new StringBuilder();
                int b;
                while ((b = fs.ReadByte()) != -1)
                {
                    if (b == '\n')
                    {
                        string hline = sb.ToString().Trim();
                        sb.Clear();
                        headerLines.Add(hline);
                        if (hline.StartsWith("DATA", StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                    else if (b != '\r')
                    {
                        sb.Append((char)b);
                    }
                }

                long dataOffset = fs.Position;

                // ===== 解析头部字段 =====
                string[] fields = new string[0];
                int[] sizes = new int[0];
                string[] types = new string[0];
                int pointCount = 0;
                string dataType = "ascii";
                int xIdx = 0, yIdx = 1, zIdx = 2;

                foreach (var hline in headerLines)
                {
                    var parts = hline.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2) continue;

                    switch (parts[0].ToUpper())
                    {
                        case "FIELDS":
                            fields = new string[parts.Length - 1];
                            for (int i = 1; i < parts.Length; i++)
                                fields[i - 1] = parts[i].ToLower();
                            xIdx = 0; yIdx = 1; zIdx = 2;
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (fields[i] == "x") xIdx = i;
                                else if (fields[i] == "y") yIdx = i;
                                else if (fields[i] == "z") zIdx = i;
                            }
                            break;
                        case "SIZE":
                            sizes = new int[parts.Length - 1];
                            for (int i = 1; i < parts.Length; i++)
                                int.TryParse(parts[i], out sizes[i - 1]);
                            break;
                        case "TYPE":
                            types = new string[parts.Length - 1];
                            for (int i = 1; i < parts.Length; i++)
                                types[i - 1] = parts[i].ToUpper();
                            break;
                        case "POINTS":
                            int.TryParse(parts[1], out pointCount);
                            break;
                        case "DATA":
                            if (parts.Length > 1)
                                dataType = parts[1].Trim().ToLower();
                            break;
                    }
                }

                int fieldCount = fields.Length > 0 ? fields.Length : 4;

                // 补齐 sizes/types
                if (sizes.Length < fieldCount)
                {
                    var tmp = new int[fieldCount];
                    for (int i = 0; i < fieldCount; i++)
                        tmp[i] = i < sizes.Length ? sizes[i] : 4;
                    sizes = tmp;
                }
                if (types.Length < fieldCount)
                {
                    var tmp = new string[fieldCount];
                    for (int i = 0; i < fieldCount; i++)
                        tmp[i] = i < types.Length ? types[i] : "F";
                    types = tmp;
                }

                int rowSize = 0;
                foreach (var s in sizes) rowSize += s;

                // ===== 读取点数据 =====
                if (dataType == "ascii")
                {
                    fs.Seek(dataOffset, SeekOrigin.Begin);
                    using (var reader = new StreamReader(fs))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (string.IsNullOrEmpty(line)) continue;
                            var p = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (p.Length <= Math.Max(xIdx, Math.Max(yIdx, zIdx))) continue;

                            if (float.TryParse(p[xIdx], NumberStyles.Any, CultureInfo.InvariantCulture, out float x) &&
                                float.TryParse(p[yIdx], NumberStyles.Any, CultureInfo.InvariantCulture, out float y) &&
                                float.TryParse(p[zIdx], NumberStyles.Any, CultureInfo.InvariantCulture, out float z))
                            {
                                points.Add(new PointXYZ(x, y, z));
                            }
                        }
                    }
                }
                else if (dataType == "binary")
                {
                    fs.Seek(dataOffset, SeekOrigin.Begin);
                    using (var reader = new BinaryReader(fs))
                    {
                        for (int i = 0; i < pointCount; i++)
                        {
                            byte[] row = reader.ReadBytes(rowSize);
                            if (row.Length < rowSize) break;

                            float x = ReadField(row, xIdx, sizes, types);
                            float y = ReadField(row, yIdx, sizes, types);
                            float z = ReadField(row, zIdx, sizes, types);
                            points.Add(new PointXYZ(x, y, z));
                        }
                    }
                }
            }

            return points.ToArray();
        }

        private static float ReadField(byte[] row, int fieldIdx, int[] sizes, string[] types)
        {
            int offset = 0;
            for (int i = 0; i < fieldIdx; i++) offset += sizes[i];

            int size = fieldIdx < sizes.Length ? sizes[fieldIdx] : 4;
            string type = fieldIdx < types.Length ? types[fieldIdx] : "F";

            switch (type)
            {
                case "F":
                    if (size == 4) return BitConverter.ToSingle(row, offset);
                    if (size == 8) return (float)BitConverter.ToDouble(row, offset);
                    break;
                case "I":
                    if (size == 4) return BitConverter.ToInt32(row, offset);
                    if (size == 2) return BitConverter.ToInt16(row, offset);
                    if (size == 1) return (sbyte)row[offset];
                    break;
                case "U":
                    if (size == 4) return BitConverter.ToUInt32(row, offset);
                    if (size == 2) return BitConverter.ToUInt16(row, offset);
                    if (size == 1) return row[offset];
                    break;
            }
            return 0f;
        }
    }
}
