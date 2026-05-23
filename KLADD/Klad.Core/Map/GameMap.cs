using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Klad.Core.Entities;
using Klad.Core.Decorators;

namespace Klad.Core.Map
{
    public class TempEffect
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float TimeLeft { get; set; }
        public bool Visible => TimeLeft > 1.0f || (int)(TimeLeft * 10) % 2 == 0;
    }

    public class GameMap : IMap
    {
        public int Width { get; }
        public int Height { get; }
        public IMazeElement[,] Grid { get; }
        
        private readonly List<TempEffect> _effects = new();

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            Grid = new IMazeElement[width, height];
        }

        public void Update(float deltaTime, params IPlayer[] players)
        {
            if (deltaTime <= 0) return;

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                effect.TimeLeft -= deltaTime;
                if (effect.TimeLeft <= 0)
                {
                    if (Grid[effect.X, effect.Y] is ElementDecorator decorator)
                    {
                        Grid[effect.X, effect.Y] = decorator.GetInner();
                        
                        if (!Grid[effect.X, effect.Y].IsWalkable)
                        {
                            foreach (var p in players)
                            {
                                if (Vector2.Distance(p.Position, new Vector2(effect.X, effect.Y)) < 0.8f)
                                {
                                    PushPlayerOut(p, effect.X, effect.Y);
                                }
                            }
                        }
                    }
                    _effects.RemoveAt(i);
                }
            }
        }

        private void PushPlayerOut(IPlayer p, int wx, int wy)
        {
            Vector2[] dirs = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
            foreach (var d in dirs)
            {
                int nx = wx + (int)d.X;
                int ny = wy + (int)d.Y;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Grid[nx, ny].IsWalkable)
                {
                    p.Position = new Vector2(nx, ny);
                    return;
                }
            }
        }

        public bool IsAnyEffectActive<T>() where T : IMazeElement
        {
            return _effects.Any(e => Grid[e.X, e.Y] is T);
        }

        public bool IsPassable(float x, float y)
        {
            float margin = 0.2f;
            int x1 = (int)Math.Floor(x + margin);
            int y1 = (int)Math.Floor(y + margin);
            int x2 = (int)Math.Floor(x + 1 - margin);
            int y2 = (int)Math.Floor(y + 1 - margin);

            return IsCellWalkable(x1, y1) && IsCellWalkable(x2, y1) && 
                   IsCellWalkable(x1, y2) && IsCellWalkable(x2, y2);
        }

        private bool IsCellWalkable(int gx, int gy)
        {
            if (gx < 0 || gx >= Width || gy < 0 || gy >= Height) return false;
            return Grid[gx, gy].IsWalkable;
        }

        public void TryPlaceWall(int ownerId, int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            if (HasEffectAt(x, y)) return;

            if (_effects.Any(e => Grid[e.X, e.Y] is TemporaryWallDecorator)) return;

            if (Grid[x, y].IsWalkable)
            {
                ApplyTemporaryEffect(x, y, 4.0f, inner => new TemporaryWallDecorator(inner));
            }
        }

        public void OnInteract(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            Grid[x, y].OnInteract(this, x, y);
        }

        public void SetCell(int x, int y, IMazeElement element)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            Grid[x, y] = element;
        }

        public void ApplyTemporaryEffect(int x, int y, float duration, Func<IMazeElement, IMazeElement> wrap)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            Grid[x, y] = wrap(Grid[x, y]);
            _effects.Add(new TempEffect { X = x, Y = y, TimeLeft = duration });
        }

        public bool HasEffectAt(int x, int y)
        {
            return _effects.Any(e => e.X == x && e.Y == y);
        }

        public bool IsVisible(int x, int y)
        {
            var effect = _effects.FirstOrDefault(e => e.X == x && e.Y == y);
            return effect == null || effect.Visible;
        }
    }
}
