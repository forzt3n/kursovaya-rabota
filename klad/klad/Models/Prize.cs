namespace klad.Models
{
    public enum PrizeType
    {
        Treasure,
        SpeedBoost,
        SpeedDebuff
    }

    public abstract class Prize
    {
        public int TextureId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public abstract void Apply(Player player);
    }

    public class Treasure : Prize
    {
        public override void Apply(Player player)
        {
            player.Score++;
        }
    }

    public class SpeedBoost : Prize
    {
        public override void Apply(Player player)
        {
            player.SpeedMultiplier += 0.5f;
        }
    }

    public class SpeedDebuff : Prize
    {
        public override void Apply(Player player)
        {
            player.SpeedMultiplier -= 0.3f;
            if (player.SpeedMultiplier < 0.5f) player.SpeedMultiplier = 0.5f;
        }
    }
}
