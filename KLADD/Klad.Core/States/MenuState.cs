namespace Klad.Core.States
{
    public class MenuState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly IInputService _input;
        public int Selection { get; private set; } = 0;

        public MenuState(GameEngine engine, IInputService input)
        {
            _engine = engine;
            _input = input;
        }

        public void Update(float dt) { }

        public void HandleInput()
        {
            if (_input.IsKeyDown(GameKey.Up) || _input.IsKeyDown(GameKey.P2_Up)) 
                Selection = 0;
            if (_input.IsKeyDown(GameKey.Down) || _input.IsKeyDown(GameKey.P2_Down)) 
                Selection = 1;

            if (_input.IsKeyDown(GameKey.Enter) || _input.IsKeyDown(GameKey.Space) || _input.IsKeyDown(GameKey.Action1) || _input.IsKeyDown(GameKey.P2_Action1))
            {
                if (Selection == 0)
                {
                    _engine.SetState(new GameplayState(_engine, _input));
                }
                else
                {
                    _engine.QuitRequested = true;
                }
            }
        }

        public void Render(IRenderer renderer)
        {
            renderer.RenderGame(_engine);
            renderer.RenderMenu(_engine.Map.Width, _engine.Map.Height, Selection);
        }
    }
}
