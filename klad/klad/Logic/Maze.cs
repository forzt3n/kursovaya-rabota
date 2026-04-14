using System.Drawing;
using klad.Models;
using klad.Decorators;

namespace klad.Logic
{
    public class Maze
    {
        private IMazeElement[,] _grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Maze(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new IMazeElement[width, height];
        }

        public IMazeElement this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        public static Maze FromGeneratedGrid(CellType[,] grid, List<Prize> prizes)
        {
            int w = grid.GetLength(0);
            int h = grid.GetLength(1);
            Maze maze = new Maze(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    switch (grid[x, y])
                    {
                        case CellType.IndestructibleWall:
                            maze[x, y] = new Wall(false);
                            break;
                        case CellType.PassageWall:
                            maze[x, y] = new Wall(true);
                            break;
                        case CellType.DestructibleWall:
                            maze[x, y] = new DestructibleWall();
                            break;
                        case CellType.Prize:
                            maze[x, y] = new Floor();
                            prizes.Add(new Treasure { X = x, Y = y, TextureId = 5 });
                            break;
                        default:
                            maze[x, y] = new Floor();
                            break;
                    }
                }
            }
            return maze;
        }

        public static Maze LoadFromBitmap(string filePath, out Point p1Start, out Point p2Start, List<Prize> initialPrizes)
        {
            using (Bitmap bmp = new Bitmap(filePath))
            {
                int w = bmp.Width;
                int h = bmp.Height;
                Maze maze = new Maze(w, h);
                p1Start = new Point(1, 1);
                p2Start = new Point(w - 2, h - 2);

                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        if (c.R == 0 && c.G == 0 && c.B == 0)
                            maze[x, y] = new Wall(false);
                        else if (c.R == 128 && c.G == 128 && c.B == 128)
                            maze[x, y] = new Wall(true);
                        else if (c.R == 150 && c.G == 75 && c.B == 0)
                            maze[x, y] = new DestructibleWall();
                        else if (c.R == 255 && c.G == 0 && c.B == 0)
                        {
                            maze[x, y] = new Floor();
                            p1Start = new Point(x, y);
                        }
                        else if (c.R == 0 && c.G == 0 && c.B == 255)
                        {
                            maze[x, y] = new Floor();
                            p2Start = new Point(x, y);
                        }
                        else if (c.R == 255 && c.G == 255 && c.B == 0)
                        {
                            maze[x, y] = new Floor();
                            initialPrizes.Add(new Treasure { X = x, Y = y });
                        }
                        else
                            maze[x, y] = new Floor();
                    }
                }
                return maze;
            }
        }

        public bool IsWalkable(float x, float y)
        {
            int gridX = (int)Math.Round(x);
            int gridY = (int)Math.Round(y);
            if (gridX < 0 || gridX >= Width || gridY < 0 || gridY >= Height) return false;
            return _grid[gridX, gridY].IsWalkable;
        }

        public void SetPassage(int x, int y)
        {
             if (_grid[x, y].CanPassage)
                 _grid[x, y] = new TemporaryPassageDecorator(_grid[x, y]);
        }

        public void SetTempWall(int x, int y)
        {
             if (_grid[x, y].IsWalkable && _grid[x, y] is Floor)
                 _grid[x, y] = new TemporaryWallDecorator(_grid[x, y]);
        }

        public void RestoreElement(int x, int y)
        {
             if (_grid[x, y] is ElementDecorator decorator)
             {
                  _grid[x, y] = decorator.DecoratedElement;
             }
        }
    }
}
