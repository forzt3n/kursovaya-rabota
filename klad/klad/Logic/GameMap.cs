using System;
using System.Collections.Generic;
using klad.Models;
using klad.Decorators;

namespace klad.Logic
{
    public class GameMap
    {
        private IMazeElement[,] _grid;
        public int Width { get; }
        public int Height { get; }

        private List<TempEffect> _tempEffects = new List<TempEffect>();

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new IMazeElement[width, height];
        }

        public IMazeElement this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        public void Update(float deltaTime)
        {
            for (int i = _tempEffects.Count - 1; i >= 0; i--)
            {
                var effect = _tempEffects[i];
                effect.TimeLeft -= deltaTime;

                if (effect.TimeLeft <= 0)
                {
                    RestoreElement(effect.X, effect.Y);
                    _tempEffects.RemoveAt(i);
                }
            }
        }

        public void SetTemporaryWall(int x, int y)
        {
            if (_grid[x, y] is Floor)
            {
                _grid[x, y] = new TemporaryWallDecorator(_grid[x, y]);
                _tempEffects.Add(new TempEffect { X = x, Y = y, TimeLeft = 7.0f, IsBlinking = false });
            }
        }

        public void SetTemporaryPassage(int x, int y)
        {
            if (_grid[x, y].CanPassage)
            {
                _grid[x, y] = new TemporaryPassageDecorator(_grid[x, y]);
                _tempEffects.Add(new TempEffect { X = x, Y = y, TimeLeft = 7.0f, IsBlinking = false });
            }
        }

        public void RestoreElement(int x, int y)
        {
            if (_grid[x, y] is ElementDecorator decorator)
            {
                _grid[x, y] = decorator.DecoratedElement;
            }
        }

        public bool IsBlinking(int x, int y)
        {
            var effect = _tempEffects.Find(e => e.X == x && e.Y == y);
            if (effect != null && effect.TimeLeft < 3.0f)
            {
                return (int)(effect.TimeLeft * 10) % 4 < 2;
            }
            return false;
        }

        public bool IsWalkable(float x, float y)
        {
            int gx = (int)Math.Round(x);
            int gy = (int)Math.Round(y);
            if (gx < 0 || gx >= Width || gy < 0 || gy >= Height) return false;
            return _grid[gx, gy].IsWalkable;
        }
    }

    class TempEffect
    {
        public int X, Y;
        public float TimeLeft;
        public bool IsBlinking;
    }
}
