using Klad.Core.Entities;

namespace Klad.Core
{
    public interface IRenderer
    {
        void RenderGame(GameEngine engine);
        void RenderMenu(int width, int height, int selection);
        void RenderGameOver(int width, int height, string winner);
        void Resize(int w, int h, int mw, int mh);
        void DrawRect(Rectangle rect, TextureId textureId);
        void RenderHUD(IPlayer p1, IPlayer p2, int mw);
    }
}
