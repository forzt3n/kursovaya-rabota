using Klad.Core.Entities;
using Klad.Core.Map;

namespace Klad.Core.States
{
    public interface IGameState
    {
        void Update(float dt);

        void HandleInput();

        void Render(IRenderer renderer);
    }
}
