using Klad.Core;

namespace Klad.Infrastructure.Input
{
    public class InputService : IInputService
    {
        private readonly Dictionary<GameKey, List<Keys>> _keyMapping = new()
        {
            { GameKey.Up, new() { Keys.W } },
            { GameKey.Down, new() { Keys.S } },
            { GameKey.Left, new() { Keys.A } },
            { GameKey.Right, new() { Keys.D } },
            { GameKey.Action1, new() { Keys.F } },
            { GameKey.Action2, new() { Keys.G } },

            { GameKey.P2_Up, new() { Keys.Up } },
            { GameKey.P2_Down, new() { Keys.Down } },
            { GameKey.P2_Left, new() { Keys.Left } },
            { GameKey.P2_Right, new() { Keys.Right } },
            { GameKey.P2_Action1, new() { Keys.Subtract } },
            { GameKey.P2_Action2, new() { Keys.Add } },

            { GameKey.Enter, new() { Keys.Enter, Keys.Return } },
            { GameKey.Escape, new() { Keys.Escape } },
            { GameKey.Space, new() { Keys.Space } }
        };

        private readonly Dictionary<Keys, bool> _currentKeys = new();
        private readonly Dictionary<Keys, bool> _previousKeys = new();

        public void UpdateKey(Keys key, bool isDown)
        {
            _currentKeys[key] = isDown;
        }

        public void Tick()
        {
            _previousKeys.Clear();
            foreach (var kvp in _currentKeys)
            {
                _previousKeys[kvp.Key] = kvp.Value;
            }
        }

        public bool IsKeyPressed(GameKey key)
        {
            if (_keyMapping.TryGetValue(key, out var winKeys))
            {
                foreach (var k in winKeys)
                {
                    if (_currentKeys.TryGetValue(k, out bool isDown) && isDown)
                        return true;
                }
            }
            return false;
        }

        public bool IsKeyDown(GameKey key)
        {
            if (_keyMapping.TryGetValue(key, out var winKeys))
            {
                foreach (var k in winKeys)
                {
                    bool current = _currentKeys.TryGetValue(k, out bool c) && c;
                    bool previous = _previousKeys.TryGetValue(k, out bool p) && p;
                    if (current && !previous) return true;
                }
            }
            return false;
        }
    }
}
