namespace klad.Models
{
    public class Player
    {
        public int Id { get; }
        public float X { get; set; }
        public float Y { get; set; }
        public float BaseSpeed { get; set; } = 0.1f;
        public float SpeedMultiplier { get; set; } = 1.0f;
        public int Score { get; set; }
        public int TextureId { get; set; }

        public (int x, int y)? ActiveTempWall { get; set; }
        public (int x, int y)? ActiveTempPassage { get; set; }

        public Player(int id)
        {
            Id = id;
        }

        public float CurrentSpeed => BaseSpeed * SpeedMultiplier;
    }
}
