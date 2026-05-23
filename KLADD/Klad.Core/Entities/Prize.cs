using System.Numerics;

namespace Klad.Core.Entities
{
    public enum PrizeType
    {
        Treasure,
        SpeedBoost,
        SpeedDebuff
    }

    public abstract class Prize
    {
        public TextureId TextureId { get; set; }
        public Vector2 Position { get; set; }
        public float X => Position.X;
        public float Y => Position.Y;
        public abstract void Apply(IPlayer player);
    }

    public class Treasure : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.Score++;
        }
    }

    public class SpeedBoost : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.SpeedMultiplier += 0.2f;
        }
    }

    public class SpeedDebuff : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.SpeedMultiplier -= 0.15f;
            if (player.SpeedMultiplier < 0.5f) player.SpeedMultiplier = 0.5f;
        }
    }
}


