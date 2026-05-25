namespace Klad.Core.States
{
    public class GameOverState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly IInputService _input;

        public GameOverState(GameEngine engine, IInputService input)
        {
            _engine = engine;
            _input = input;
        }

        public void Update(float dt) { }

        public void HandleInput()
        {
            if (_input.IsKeyDown(GameKey.Enter) || _input.IsKeyDown(GameKey.Space) || _input.IsKeyDown(GameKey.Action1) || _input.IsKeyDown(GameKey.P2_Action1))
            {
                _engine.Initialize(21, 21);
                _engine.SetState(new MenuState(_engine, _input));
            }
        }
        public void Render(IRenderer renderer)
        {
            renderer.RenderGame(_engine);
            renderer.RenderGameOver(_engine.Map.Width, _engine.Map.Height, _engine.CurrentWinner);
        }
    }
}
