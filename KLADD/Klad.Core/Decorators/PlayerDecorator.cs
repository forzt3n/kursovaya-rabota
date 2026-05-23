using System.Numerics;
using Klad.Core.Entities;

namespace Klad.Core.Decorators
{
    public abstract class PlayerDecorator : IPlayer
    {
        protected IPlayer _inner;

        protected PlayerDecorator(IPlayer inner)
        {
            _inner = inner;
        }

        public virtual int Id => _inner.Id;
        public virtual Vector2 Position { get => _inner.Position; set => _inner.Position = value; }
        public virtual Vector2 Direction { get => _inner.Direction; set => _inner.Direction = value; }
        public virtual float SpeedMultiplier { get => _inner.SpeedMultiplier; set => _inner.SpeedMultiplier = value; }
        public virtual float CurrentSpeed => _inner.CurrentSpeed;
        public virtual int Score { get => _inner.Score; set => _inner.Score = value; }
        public virtual TextureId TextureId { get => _inner.TextureId; set => _inner.TextureId = value; }

        public virtual void Move(IMap map, Vector2 moveDir) => _inner.Move(map, moveDir);
        public virtual void CheckCollisions(List<Prize> prizes) => _inner.CheckCollisions(prizes);
        public virtual void ActionPlaceWall(IMap map) => _inner.ActionPlaceWall(map);
        public virtual void ActionBreakWall(IMap map) => _inner.ActionBreakWall(map);
    }

    public class BuffDecorator : PlayerDecorator
    {
        private float _speedBonus;

        public BuffDecorator(IPlayer inner, float speedBonus) : base(inner)
        {
            _speedBonus = speedBonus;
        }

        public override float CurrentSpeed => base.CurrentSpeed + _speedBonus;
    }
}
