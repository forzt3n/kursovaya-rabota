using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Klad.Core.Map
{
    public static class MapExporter
    {
        public static Color GetColorForCell(RawCellType cell)
        {
            return cell switch
            {
                RawCellType.SolidWall => Color.Black,
                RawCellType.BreakableWall => Color.Gray,
                RawCellType.RecoverableWall => Color.DarkGray,
                RawCellType.Empty => Color.White,
                RawCellType.TreasureMarker => Color.Yellow,
                RawCellType.PowerUpMarker => Color.Blue,
                _ => Color.White
            };
        }

        public static RawCellType GetCellFromColor(Color color)
        {
            if (color.ToArgb() == Color.Black.ToArgb()) return RawCellType.SolidWall;
            if (color.ToArgb() == Color.Gray.ToArgb()) return RawCellType.BreakableWall;
            if (color.ToArgb() == Color.DarkGray.ToArgb()) return RawCellType.RecoverableWall;
            if (color.ToArgb() == Color.Yellow.ToArgb()) return RawCellType.TreasureMarker;
            if (color.ToArgb() == Color.Blue.ToArgb()) return RawCellType.PowerUpMarker;
            return RawCellType.Empty;
        }

        public static void ExportRandomMaps(string targetFolder, int count, int width, int height)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            MapGenerator gen = new MapGenerator(width, height);

            for (int i = 1; i <= count; i++)
            {
                Point p1, p2;
                var grid = gen.Generate(out p1, out p2);
                int w = grid.GetLength(0);
                int h = grid.GetLength(1);

                using (Bitmap bmp = new Bitmap(w, h))
                {
                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < h; y++)
                        {
                            bmp.SetPixel(x, y, GetColorForCell(grid[x, y]));
                        }
                    }
                    
                    bmp.SetPixel(p1.X, p1.Y, Color.Red);
                    bmp.SetPixel(p2.X, p2.Y, Color.Green);

                    string filePath = Path.Combine(targetFolder, $"map_{i}.bmp");
                    bmp.Save(filePath, ImageFormat.Bmp);
                }
            }
        }
    }
}
