using System;
using System.Collections.Generic;
using System.Drawing;

namespace Klad.Core.Map
{
    public class MapGenerator
    {
        private Random _random = new Random();
        private int _width, _height;
        public MapGenerator(int width, int height)
        {
            _width = width % 2 == 0 ? width + 1 : width;
            _height = height % 2 == 0 ? height + 1 : height;
        }

        public RawCellType[,] Generate(out Point p1, out Point p2)
        {
            RawCellType[,] grid = new RawCellType[_width, _height];
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    grid[x, y] = RawCellType.SolidWall;

            GeneratePath(grid, 1, 1);

            p1 = new Point(1, 1);
            p2 = new Point(_width - 2, _height - 2);
            grid[p1.X, p1.Y] = RawCellType.Empty;
            grid[p2.X, p2.Y] = RawCellType.Empty;

            PopulateObjects(grid);
            return grid;
        }

        private void GeneratePath(RawCellType[,] grid, int x, int y)
        {
            grid[x, y] = RawCellType.Empty;
            var dirs = new List<(int dx, int dy)> { (0, 2), (0, -2), (2, 0), (-2, 0) };
            for (int i = dirs.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var t = dirs[i]; dirs[i] = dirs[j]; dirs[j] = t;
            }

            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx, ny = y + dy;
                if (nx > 0 && nx < _width - 1 && ny > 0 && ny < _height - 1 && grid[nx, ny] == RawCellType.SolidWall)
                {
                    grid[x + dx / 2, y + dy / 2] = RawCellType.Empty;
                    GeneratePath(grid, nx, ny);
                }
            }
        }

        private void PopulateObjects(RawCellType[,] grid)
        {
            List<Point> floorCells = new List<Point>();
            for (int x = 1; x < _width - 1; x++)
            {
                for (int y = 1; y < _height - 1; y++)
                {
                    if (grid[x, y] == RawCellType.Empty)
                        floorCells.Add(new Point(x, y));
                    else if (grid[x, y] == RawCellType.SolidWall)
                    {
                        double r = _random.NextDouble();
                        if (r < 0.15) grid[x, y] = RawCellType.BreakableWall;
                        else if (r < 0.30) grid[x, y] = RawCellType.RecoverableWall;
                    }
                }
            }

            int treasureCount = 15;
            for (int i = 0; i < treasureCount && floorCells.Count > 0; i++)
            {
                int idx = _random.Next(floorCells.Count);
                var p = floorCells[idx];
                if ((p.X == 1 && p.Y == 1) || (p.X == _width-2 && p.Y == _height-2)) { i--; floorCells.RemoveAt(idx); continue; }
                grid[p.X, p.Y] = RawCellType.TreasureMarker;
                floorCells.RemoveAt(idx);
            }

            for (int i = 0; i < 5 && floorCells.Count > 0; i++)
            {
                int idx = _random.Next(floorCells.Count);
                grid[floorCells[idx].X, floorCells[idx].Y] = RawCellType.PowerUpMarker;
                floorCells.RemoveAt(idx);
            }
        }
    }
}
