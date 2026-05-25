using System.Drawing;
using System.Numerics;
using Klad.Core.Entities;
using Klad.Core.Factories;
using Klad.Core.Map;
using Klad.Core.States;
using Klad.Core.Decorators;

namespace Klad.Core
{
    public class GameEngine
    {
        private IGameState _currentState = null!;
        public IGameState CurrentState => _currentState;

        public string CurrentWinner { get; private set; } = "";
        public GameMap Map { get; private set; } = null!;
        public IPlayer Player1 { get; private set; } = null!;
        public IPlayer Player2 { get; private set; } = null!;
        public List<Prize> Prizes { get; private set; } = new List<Prize>();
        private readonly MapFactory _mapFactory = new MapFactory();
        private readonly IInputService _input;

        public bool QuitRequested { get; set; }

        public GameEngine(IInputService input)
        {
            _input = input;
        }

        public void SetState(IGameState state)
        {
            _currentState = state;
        }

        public void Initialize(int width, int height)
        {
            IPlayer p1, p2;
            List<Prize> prizes;
            Map = _mapFactory.CreateMap(width, height, out p1, out p2, out prizes);
            Player1 = p1;
            Player2 = p2;
            Prizes = prizes;


            if (_currentState == null || _currentState is GameOverState)
            {
                SetState(new MenuState(this, _input));
            }
        }

        public void Update(float dt)
        {
            _currentState?.HandleInput();
            _currentState?.Update(dt);
        }

        public void Render(IRenderer renderer)
        {
            _currentState?.Render(renderer);
        }

        public bool IsGameOver()
        {

            if (!Prizes.Any(pr => pr is Treasure))
            {

                CurrentWinner = Player1.Score > Player2.Score ? "Игрок 1" : "Игрок 2";
                if (Player1.Score == Player2.Score) CurrentWinner = "Ничья";
                return true;
            }
            return false;
        }
    }
}
