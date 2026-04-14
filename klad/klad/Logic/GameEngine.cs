using System.Drawing;
using klad.Models;
using klad.Factories;

namespace klad.Logic
{
    public class GameEngine
    {
        public GameMap Map { get; private set; } = null!;
        public Player Player1 { get; private set; } = null!;
        public Player Player2 { get; private set; } = null!;
        public List<Prize> Prizes { get; private set; }
        private GamePrizeFactory _prizeFactory;
        private Random _random = new Random();

        public GameEngine()
        {
            Prizes = new List<Prize>();
            _prizeFactory = new GamePrizeFactory();
        }

        public void InitializeNewRandomGame(int width, int height, string bmpPath)
        {
            MapGenerator generator = new MapGenerator(width, height);
            Point p1, p2;
            var grid = generator.Generate(out p1, out p2);
            MapLoader.SaveToBmp(grid, p1, p2, bmpPath);
            InitializeFromBmp(bmpPath);
        }

        public void InitializeFromBmp(string path)
        {
            Point p1Start, p2Start;
            Prizes.Clear();
            var grid = MapLoader.LoadFromBmp(path, out p1Start, out p2Start);
            
            Map = new GameMap(grid.GetLength(0), grid.GetLength(1));
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    switch (grid[x, y])
                    {
                        case CellType.IndestructibleWall: Map[x, y] = new Wall(false); break;
                        case CellType.PassageWall: Map[x, y] = new Wall(true); break;
                        case CellType.DestructibleWall: Map[x, y] = new DestructibleWall(); break;
                        case CellType.Prize: 
                            Map[x, y] = new Floor(); 
                            var p = _prizeFactory.CreatePrize(PrizeType.Treasure);
                            p.X = x; p.Y = y; p.TextureId = 5;
                            Prizes.Add(p); 
                            break;
                        default: Map[x, y] = new Floor(); break;
                    }
                }
            }
            
            Player1 = new Player(1) { X = p1Start.X, Y = p1Start.Y, TextureId = 8 };
            Player2 = new Player(2) { X = p2Start.X, Y = p2Start.Y, TextureId = 9 };
        }

        public void Update(float deltaTime)
        {
            Map.Update(deltaTime);
            SpawnRandomPrize();
        }

        public void MovePlayer(int playerId, float dx, float dy)
        {
            Player p = playerId == 1 ? Player1 : Player2;
            float newX = p.X + dx * p.CurrentSpeed;
            float newY = p.Y + dy * p.CurrentSpeed;

            if (Map.IsWalkable(newX, newY))
            {
                p.X = newX;
                p.Y = newY;
                CheckPrizeCollision(p);
            }
        }

        private void CheckPrizeCollision(Player p)
        {
            for (int i = Prizes.Count - 1; i >= 0; i--)
            {
                var prize = Prizes[i];
                if (Math.Abs(prize.X - p.X) < 0.7f && Math.Abs(prize.Y - p.Y) < 0.7f)
                {
                    prize.Apply(p);
                    Prizes.RemoveAt(i);
                }
            }
        }

        public void ActionRemoveWall(int playerId)
        {
            Player p = playerId == 1 ? Player1 : Player2;
            int gx = (int)Math.Round(p.X);
            int gy = (int)Math.Round(p.Y);

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int nx = gx + dx[i];
                int ny = gy + dy[i];
                if (nx >= 0 && nx < Map.Width && ny >= 0 && ny < Map.Height)
                {
                    // Если это стена для временного прохода
                    if (Map[nx, ny].CanPassage)
                    {
                        Map.SetTemporaryPassage(nx, ny);
                        break;
                    }
                    // Если это разрушаемая стена
                    if (Map[nx, ny].IsDestructible)
                    {
                        Map[nx, ny] = new Floor();
                        break;
                    }
                }
            }
        }

        public void ActionPlaceWall(int playerId)
        {
            Player p = playerId == 1 ? Player1 : Player2;
            int gx = (int)Math.Round(p.X);
            int gy = (int)Math.Round(p.Y);
            if (Map[gx, gy] is Floor)
            {
                Map.SetTemporaryWall(gx, gy);
            }
        }

        public void SpawnRandomPrize()
        {
            if (_random.NextDouble() < 0.01) 
            {
                int x = _random.Next(Map.Width);
                int y = _random.Next(Map.Height);
                if (Map[x, y] is Floor)
                {
                    PrizeType type = _random.Next(10) < 5 ? PrizeType.SpeedBoost : PrizeType.SpeedDebuff;
                    var prize = _prizeFactory.CreatePrize(type);
                    prize.X = x; prize.Y = y; prize.TextureId = 5; 
                    Prizes.Add(prize);
                }
            }
        }

        public bool IsGameOver() => !Prizes.Any(p => p is Treasure);

        public string GetWinner()
        {
            if (Player1.Score > Player2.Score) return "Player 1";
            if (Player2.Score > Player1.Score) return "Player 2";
            return "Draw";
        }
    }
}
