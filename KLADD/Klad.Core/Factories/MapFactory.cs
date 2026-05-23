using System.Drawing;
using System.Numerics;
using Klad.Core.Entities;
using Klad.Core.Map;
using Klad.Core.Decorators;

namespace Klad.Core.Factories
{
    public class MapFactory
    {
        private readonly GamePrizeFactory _prizeFactory = new();

        public GameMap CreateMap(int width, int height, out IPlayer p1, out IPlayer p2, out List<Prize> prizes)
        {
            int mapIndex = new Random().Next(1, 6);
            string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", $"map_{mapIndex}.bmp");

            if (File.Exists(mapPath))
            {
                return LoadFromBitmap(mapPath, out p1, out p2, out prizes);
            }
            else
            {
                return GenerateMap(width, height, out p1, out p2, out prizes);
            }
        }

        private GameMap LoadFromBitmap(string path, out IPlayer p1, out IPlayer p2, out List<Prize> prizes)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                int w = bmp.Width;
                int h = bmp.Height;
                GameMap map = new GameMap(w, h);
                prizes = new List<Prize>();
                Point p1Pos = new Point(1, 1);
                Point p2Pos = new Point(w - 2, h - 2);

                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        if (c.ToArgb() == Color.Red.ToArgb())
                        {
                            p1Pos = new Point(x, y);
                            map.Grid[x, y] = new Floor();
                        }
                        else if (c.ToArgb() == Color.Green.ToArgb())
                        {
                            p2Pos = new Point(x, y);
                            map.Grid[x, y] = new Floor();
                        }
                        else
                        {
                            RawCellType raw = MapExporter.GetCellFromColor(c);
                            map.Grid[x, y] = CreateElement(raw);
                            
                            if (raw == RawCellType.TreasureMarker)
                            {
                                prizes.Add(CreatePrize(PrizeType.Treasure, x, y));
                            }
                            else if (raw == RawCellType.PowerUpMarker)
                            {
                                prizes.Add(CreatePrize(PrizeType.SpeedBoost, x, y));
                            }
                        }
                    }
                }

                p1 = new Player(1) { Position = new Vector2(p1Pos.X, p1Pos.Y), TextureId = TextureId.Player1 };
                p2 = new Player(2) { Position = new Vector2(p2Pos.X, p2Pos.Y), TextureId = TextureId.Player2 };

                AddRandomDebuffs(map, prizes, p1, p2);
                return map;
            }
        }

        private GameMap GenerateMap(int width, int height, out IPlayer p1, out IPlayer p2, out List<Prize> prizes)
        {
            MapGenerator gen = new MapGenerator(width, height);
            Point p1Pos, p2Pos;
            var grid = gen.Generate(out p1Pos, out p2Pos);
            
            GameMap map = new GameMap(width, height);
            prizes = new List<Prize>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.Grid[x, y] = CreateElement(grid[x, y]);
                }
            }

            p1 = new Player(1) { Position = new Vector2(p1Pos.X, p1Pos.Y), TextureId = TextureId.Player1 };
            p2 = new Player(2) { Position = new Vector2(p2Pos.X, p2Pos.Y), TextureId = TextureId.Player2 };

            AddRandomDebuffs(map, prizes, p1, p2);
            return map;
        }

        private IMazeElement CreateElement(RawCellType type)
        {
            return type switch
            {
                RawCellType.SolidWall => new Wall(),
                RawCellType.BreakableWall => new DestructibleDecorator(new Wall()),
                RawCellType.RecoverableWall => new RecoverableDecorator(new Wall()),
                _ => new Floor()
            };
        }

        private Prize CreatePrize(PrizeType type, int x, int y)
        {
            var p = _prizeFactory.CreatePrize(type);
            p.Position = new Vector2(x, y);
            return p;
        }

        private void AddRandomDebuffs(GameMap map, List<Prize> prizes, IPlayer p1, IPlayer p2)
        {
            Random rnd = new Random();
            int debuffsCount = rnd.Next(2, 4);
            int placed = 0;
            while (placed < debuffsCount)
            {
                int rx = rnd.Next(map.Width);
                int ry = rnd.Next(map.Height);
                if (map.Grid[rx, ry].IsWalkable && !prizes.Any(pr => (int)pr.X == rx && (int)pr.Y == ry)
                    && (Vector2.Distance(new Vector2(rx, ry), p1.Position) > 2)
                    && (Vector2.Distance(new Vector2(rx, ry), p2.Position) > 2))
                {
                    prizes.Add(CreatePrize(PrizeType.SpeedDebuff, rx, ry));
                    placed++;
                }
            }
        }
    }
}
