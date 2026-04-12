using System.Drawing;

namespace klad.Logic
{
    public enum CellType
    {
        Floor,               
        IndestructibleWall,   
        DestructibleWall,    
        PassageWall,          
        Prize,                
        TemporaryWall         
    }

    public class MapLoader
    {
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
                
                        if (c.G > c.R && c.G > c.B && c.R < 100 && c.B < 100)
                            grid[x, y] = CellType.IndestructibleWall;       
                        else if (c.R > 200 && c.G < 100 && c.B < 100)
                            grid[x, y] = CellType.PassageWall;
                        else if (c.R > 100 && c.G > 50 && c.G < 150 && c.B < 50)
                            grid[x, y] = CellType.DestructibleWall;
                        else if (c.R > 200 && c.G > 200 && c.B < 100)
                            grid[x, y] = CellType.Prize;
                        else
                            grid[x, y] = CellType.Floor;
                        if (c.R > 200 && c.G < 50 && c.B > 200) p1Start = new Point(x, y); 
                        if (c.R < 50 && c.G > 200 && c.B > 200) p2Start = new Point(x, y); 
                    }
                }
                return grid;
            }
        }
    }
}
