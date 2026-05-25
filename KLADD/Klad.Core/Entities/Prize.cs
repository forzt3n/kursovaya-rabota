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
}
