using System.Drawing;

namespace klad.Logic
{
    public enum CellType
    {
        Floor,                // Салатовый (проходимая)
        IndestructibleWall,   // Зелёный (нельзя разрушить)
        DestructibleWall,     // Коричневый (можно уничтожить)
        PassageWall,          // Красный (можно сделать временный проход)
        Prize,                // Жёлтый (улучшение)
        TemporaryWall         // Временная стена (создается игроком)
    }

    public class MapLoader
    {
        // Константы цветов для ТЗ
        public static readonly Color ColorFloor = Color.FromArgb(144, 238, 144); // Салатовый (LightGreen)
        public static readonly Color ColorIndestructible = Color.FromArgb(0, 128, 0); // Зеленый
        public static readonly Color ColorDestructible = Color.FromArgb(150, 75, 0); // Коричневый
        public static readonly Color ColorPassage = Color.FromArgb(255, 0, 0);       // Красный
        public static readonly Color ColorPrize = Color.FromArgb(255, 255, 0);       // Желтый
        public static readonly Color ColorP1 = Color.FromArgb(255, 0, 255);          // Маджента
        public static readonly Color ColorP2 = Color.FromArgb(0, 255, 255);          // Циан

        public static CellType[,] LoadFromBmp(string path, out Point p1Start, out Point p2Start)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                int w = bmp.Width;
                int h = bmp.Height;
                CellType[,] grid = new CellType[w, h];
                p1Start = new Point(1, 1);
                p2Start = new Point(w - 2, h - 2);

                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        int argb = c.ToArgb();

                        if (argb == ColorIndestructible.ToArgb())
                            grid[x, y] = CellType.IndestructibleWall;
                        else if (argb == ColorDestructible.ToArgb())
                            grid[x, y] = CellType.DestructibleWall;
                        else if (argb == ColorPassage.ToArgb())
                            grid[x, y] = CellType.PassageWall;
                        else if (argb == ColorPrize.ToArgb())
                            grid[x, y] = CellType.Prize;
                        else if (argb == ColorP1.ToArgb())
                        {
                            grid[x, y] = CellType.Floor;
                            p1Start = new Point(x, y);
                        }
                        else if (argb == ColorP2.ToArgb())
                        {
                            grid[x, y] = CellType.Floor;
                            p2Start = new Point(x, y);
                        }
                        else
                            grid[x, y] = CellType.Floor; // Салатовый и остальные
                    }
                }
                return grid;
            }
        }

        public static void SaveToBmp(CellType[,] grid, Point p1, Point p2, string path)
        {
            int w = grid.GetLength(0);
            int h = grid.GetLength(1);
            using (Bitmap bmp = new Bitmap(w, h))
            {
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        Color c = grid[x, y] switch
                        {
                            CellType.IndestructibleWall => ColorIndestructible,
                            CellType.DestructibleWall => ColorDestructible,
                            CellType.PassageWall => ColorPassage,
                            CellType.Prize => ColorPrize,
                            _ => ColorFloor
                        };
                        bmp.SetPixel(x, y, c);
                    }
                }
                bmp.SetPixel(p1.X, p1.Y, ColorP1);
                bmp.SetPixel(p2.X, p2.Y, ColorP2);
                bmp.Save(path);
            }
        }
    }
}
