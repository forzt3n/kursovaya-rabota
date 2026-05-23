using System;
using System.Numerics;

namespace Klad.Core.Entities
{
    public class Player : IPlayer
    {
        public int Id { get; }
        public Vector2 Position { get; set; }
        public float BaseSpeed { get; set; } = 0.08f;
        public float SpeedMultiplier { get; set; } = 1.0f;
        public int Score { get; set; }
        public TextureId TextureId { get; set; }

        public Vector2 Direction { get; set; } = new Vector2(0, 1);

        public Player(int id)
        {
            Id = id;
        }

        public virtual float CurrentSpeed => BaseSpeed * SpeedMultiplier;

        public virtual void Move(IMap map, Vector2 moveDir)
        {
            if (moveDir != Vector2.Zero) Direction = moveDir;

            float speed = CurrentSpeed;

            // Пытаемся подвинуться только по X
            Vector2 nextX = new Vector2(Position.X + moveDir.X * speed, Position.Y);
            if (map.IsPassable(nextX.X, nextX.Y))
            {
                Position = nextX;
            }

            // Пытаемся подвинуться только по Y
            Vector2 nextY = new Vector2(Position.X, Position.Y + moveDir.Y * speed);
            if (map.IsPassable(nextY.X, nextY.Y))
            {
                Position = nextY;
            }
        }

        public virtual void CheckCollisions(List<Prize> prizes)
        {
            for (int i = prizes.Count - 1; i >= 0; i--)
            {
                Vector2 prizePos = new Vector2(prizes[i].X, prizes[i].Y);
                if (Vector2.Distance(prizePos, Position) < 0.6f)
                {
                    prizes[i].Apply(this);
                    prizes.RemoveAt(i);
                }
            }
        }

        public virtual void ActionPlaceWall(IMap map)
        {
            map.TryPlaceWall(Id, (int)Math.Round(Position.X + Direction.X), (int)Math.Round(Position.Y + Direction.Y));
        }

        public virtual void ActionBreakWall(IMap map)
        {
            map.OnInteract((int)Math.Round(Position.X + Direction.X), (int)Math.Round(Position.Y + Direction.Y));
        }
    }
}
