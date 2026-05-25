using Klad.Core.Entities;
using Klad.Core.Map;
using System.Numerics;

namespace Klad.Core.States
{
    public class GameplayState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly IInputService _input;

        public GameplayState(GameEngine engine, IInputService input)
        {
            _engine = engine;
            _input = input;
        }
        public void Update(float dt)
        {
            _engine.Map.Update(dt, _engine.Player1, _engine.Player2);
            _engine.Player1.CheckCollisions(_engine.Prizes);
            _engine.Player2.CheckCollisions(_engine.Prizes);

            if (_engine.IsGameOver())
            {
                _engine.SetState(new GameOverState(_engine, _input));
            }
        }
        public void HandleInput()
        {
            Vector2 p1Move = Vector2.Zero;
            if (_input.IsKeyPressed(GameKey.Up)) p1Move.Y -= 1;
            if (_input.IsKeyPressed(GameKey.Down)) p1Move.Y += 1;
            if (_input.IsKeyPressed(GameKey.Left)) p1Move.X -= 1;
            if (_input.IsKeyPressed(GameKey.Right)) p1Move.X += 1;
            if (p1Move != Vector2.Zero) _engine.Player1.Move(_engine.Map, Vector2.Normalize(p1Move));

            if (_input.IsKeyDown(GameKey.Action1)) _engine.Player1.ActionBreakWall(_engine.Map);
            if (_input.IsKeyDown(GameKey.Action2)) _engine.Player1.ActionPlaceWall(_engine.Map);

            Vector2 p2Move = Vector2.Zero;
            if (_input.IsKeyPressed(GameKey.P2_Up)) p2Move.Y -= 1;
            if (_input.IsKeyPressed(GameKey.P2_Down)) p2Move.Y += 1;
            if (_input.IsKeyPressed(GameKey.P2_Left)) p2Move.X -= 1;
            if (_input.IsKeyPressed(GameKey.P2_Right)) p2Move.X += 1;
            if (p2Move != Vector2.Zero) _engine.Player2.Move(_engine.Map, Vector2.Normalize(p2Move));

            if (_input.IsKeyDown(GameKey.P2_Action1)) _engine.Player2.ActionBreakWall(_engine.Map);
            if (_input.IsKeyDown(GameKey.P2_Action2)) _engine.Player2.ActionPlaceWall(_engine.Map);

            if (_input.IsKeyDown(GameKey.Escape))
            {
                _engine.SetState(new MenuState(_engine, _input));
            }
        }
        public void Render(IRenderer renderer)
        {

            renderer.RenderGame(_engine);


            renderer.RenderHUD(_engine.Player1, _engine.Player2, _engine.Map.Width);
        }
    }
}
