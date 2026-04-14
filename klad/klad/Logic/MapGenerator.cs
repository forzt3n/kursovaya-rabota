using System;
using System.Collections.Generic;
using System.Drawing;

namespace klad.Logic
{
    public class MapGenerator
    {
        private Random _random = new Random();
        private int _width, _height;

        public MapGenerator(int width, int height)
        {
            // Размеры должны быть нечетными для корректной генерации сетки
            _width = width % 2 == 0 ? width + 1 : width;
            _height = height % 2 == 0 ? height + 1 : height;
        }

        public CellType[,] Generate(out Point p1, out Point p2)
        {
            CellType[,] grid = new CellType[_width, _height];

            // 1. Инициализация - всё забито стенами
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    grid[x, y] = CellType.IndestructibleWall;

            // 2. Алгоритм генерации проходов (Recursive Backtracking)
            GenerateMazeBranch(grid, 1, 1);

            // 3. Убираем тупики (делаем Braid Maze)
            RemoveDeadEnds(grid);

            // 4. Добавляем спец-стены (Коричневые и Красные) и сокровища
            RandomizeWallTypesAndItems(grid);

            p1 = new Point(1, 1);
            p2 = new Point(_width - 2, _height - 2);
            
            // Гарантируем пустоту на стартах
            grid[p1.X, p1.Y] = CellType.Floor;
            grid[p2.X, p2.Y] = CellType.Floor;

            return grid;
        }

        private void GenerateMazeBranch(CellType[,] grid, int x, int y)
        {
            grid[x, y] = CellType.Floor;
            var dirs = new List<(int dx, int dy)> { (0, 2), (0, -2), (2, 0), (-2, 0) };
            // Перемешиваем направления
            for (int i = dirs.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = dirs[i]; dirs[i] = dirs[j]; dirs[j] = temp;
            }

            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx > 0 && nx < _width - 1 && ny > 0 && ny < _height - 1 && grid[nx, ny] == CellType.IndestructibleWall)
                {
                    grid[x + dx / 2, y + dy / 2] = CellType.Floor;
                    GenerateMazeBranch(grid, nx, ny);
                }
            }
        }

        private void RemoveDeadEnds(CellType[,] grid)
        {
            for (int x = 1; x < _width - 1; x++)
            {
                for (int y = 1; y < _height - 1; y++)
                {
                    if (grid[x, y] == CellType.Floor)
                    {
                        int neighbors = 0;
                        if (grid[x + 1, y] == CellType.Floor) neighbors++;
                        if (grid[x - 1, y] == CellType.Floor) neighbors++;
                        if (grid[x, y + 1] == CellType.Floor) neighbors++;
                        if (grid[x, y - 1] == CellType.Floor) neighbors++;

                        if (neighbors == 1) // Это тупик
                        {
                            // Соединяем со случайной соседней стеной (не краем)
                            var wallDirs = new List<(int dx, int dy)> { (0, 1), (0, -1), (1, 0), (-1, 0) };
                            foreach (var (dx, dy) in wallDirs)
                            {
                                int wx = x + dx;
                                int wy = y + dy;
                                if (wx > 0 && wx < _width - 1 && wy > 0 && wy < _height - 1 && grid[wx, wy] == CellType.IndestructibleWall)
                                {
                                    grid[wx, wy] = CellType.Floor;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RandomizeWallTypesAndItems(CellType[,] grid)
        {
            for (int x = 1; x < _width - 1; x++)
            {
                for (int y = 1; y < _height - 1; y++)
                {
                    if (grid[x, y] == CellType.IndestructibleWall)
                    {
                        double r = _random.NextDouble();
                        if (r < 0.3) grid[x, y] = CellType.DestructibleWall; // Коричневая
                        else if (r < 0.5) grid[x, y] = CellType.PassageWall;  // Красная
                    }
                    else if (grid[x, y] == CellType.Floor)
                    {
                        if (_random.NextDouble() < 0.1) grid[x, y] = CellType.Prize; // Желтая
                    }
                }
            }
        }
    }
}
