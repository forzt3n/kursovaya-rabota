using OpenTK.Graphics.OpenGL;
using Klad.Core;
using Klad.Core.Map;
using Klad.Core.Entities;
using Klad.Infrastructure.Resources;
using System.Drawing;
using System.Numerics;

namespace Klad.Infrastructure.Graphics
{
    public class Renderer : IRenderer
    {
        private readonly ResourceManager _resourceManager;
        private int _winnerTextureId = -1;
        private string _lastWinnerName = "";

        public Renderer(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public void Render(GameEngine engine)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();

            engine.Render(this);
        }

        public void RenderGame(GameEngine game)
        {
            for (int x = 0; x < game.Map.Width; x++)
            {
                for (int y = 0; y < game.Map.Height; y++)
                {
                    if (!game.Map.IsVisible(x, y)) continue;
                    
                    DrawRect(new Klad.Core.Rectangle(x, y, 1, 1), TextureId.Empty);

                    IMazeElement cell = game.Map.Grid[x, y];
                    if (cell.TextureId != TextureId.Empty)
                    {
                        DrawRect(new Klad.Core.Rectangle(x, y, 1, 1), cell.TextureId);
                    }
                }
            }

            foreach (var p in game.Prizes)
            {
                DrawRect(new Klad.Core.Rectangle(p.X, p.Y, 1, 1), p.TextureId);
            }
            
            DrawRect(new Klad.Core.Rectangle(game.Player1.Position.X, game.Player1.Position.Y, 1, 1), game.Player1.TextureId);
            DrawRect(new Klad.Core.Rectangle(game.Player2.Position.X, game.Player2.Position.Y, 1, 1), game.Player2.TextureId);
        }

        public void RenderMenu(int mw, int mh, int selection)
        {
            DrawOverlay(mw, mh, 0.85f);

            DrawMenuOption(new Klad.Core.Rectangle(mw / 2f - 1.25f, mh / 2f - 2, 2.5f, 1), selection == 0, TextureId.MenuPlay);
            DrawMenuOption(new Klad.Core.Rectangle(mw / 2f - 1.25f, mh / 2f, 2.5f, 1), selection == 1, TextureId.MenuQuit);
        }

        public void RenderGameOver(int mw, int mh, string winner)
        {
            DrawOverlay(mw, mh, 0.85f);

            if (_lastWinnerName != winner)
            {
                if (_winnerTextureId != -1) GL.DeleteTexture(_winnerTextureId);
                _winnerTextureId = _resourceManager.GetTextTexture($"{winner.ToUpper()} WINS!", Color.Gold);
                _lastWinnerName = winner;
            }

            GL.BindTexture(TextureTarget.Texture2D, _winnerTextureId);
            GL.Begin(PrimitiveType.Quads);
            float nW = 10, nH = 2.5f;
            float x = mw / 2f - nW / 2;
            float y = mh / 2f - 1;
            GL.TexCoord2(0, 0); GL.Vertex2(x, y);
            GL.TexCoord2(1, 0); GL.Vertex2(x + nW, y);
            GL.TexCoord2(1, 1); GL.Vertex2(x + nW, y + nH);
            GL.TexCoord2(0, 1); GL.Vertex2(x, y + nH);
            GL.End();

            DrawMenuOption(new Klad.Core.Rectangle(mw / 2f - 1.5f, mh / 2f + 3, 3, 1.2f), true, TextureId.MenuPlay);
        }

        public void DrawRect(Klad.Core.Rectangle rect, TextureId textureId)
        {
            int tid = _resourceManager.GetTexture(textureId);
            GL.BindTexture(TextureTarget.Texture2D, tid);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(rect.X, rect.Y);
            GL.TexCoord2(1, 0); GL.Vertex2(rect.X + rect.Width, rect.Y);
            GL.TexCoord2(1, 1); GL.Vertex2(rect.X + rect.Width, rect.Y + rect.Height);
            GL.TexCoord2(0, 1); GL.Vertex2(rect.X, rect.Y + rect.Height);
            GL.End();
        }

        private void DrawOverlay(int mw, int mh, float alpha)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(0f, 0f, 0f, alpha);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(0, 0);
            GL.Vertex2(mw, 0);
            GL.Vertex2(mw, mh);
            GL.Vertex2(0, mh);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);
        }

        private void DrawMenuOption(Klad.Core.Rectangle rect, bool selected, TextureId tid)
        {
            GL.Disable(EnableCap.Texture2D);
            if (selected) GL.Color4(1f, 1f, 0f, 0.8f);
            else GL.Color4(1f, 1f, 1f, 0.4f);

            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(rect.X, rect.Y);
            GL.Vertex2(rect.X + rect.Width, rect.Y);
            GL.Vertex2(rect.X + rect.Width, rect.Y + rect.Height);
            GL.Vertex2(rect.X, rect.Y + rect.Height);
            GL.End();

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);
            DrawRect(rect, tid);
        }

        public void Resize(int w, int h, int mw, int mh)
        {
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, mw, mh, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void RenderHUD(IPlayer p1, IPlayer p2, int mw)
        {
            float barWidth = 6.0f; // Ширина плашки
            float barHeight = 0.8f; // Высота плашки
            float padding = 0.2f;   // Отступ от краев экрана

            // --- Плашка Игрока 1 (Слева) ---
            Klad.Core.Rectangle rect1 = new Klad.Core.Rectangle(padding, padding, barWidth, barHeight);
            DrawHUDBar(rect1, $"Игрок 1: Очки: {p1.Score}", Color.Cyan);

            // --- Плашка Игрока 2 (Справа) ---
            float x2 = mw - barWidth - padding;
            Klad.Core.Rectangle rect2 = new Klad.Core.Rectangle(x2, padding, barWidth, barHeight);
            DrawHUDBar(rect2, $"Игрок 2: Очки: {p2.Score}", Color.OrangeRed);
        }

        private void DrawHUDBar(Klad.Core.Rectangle rect, string text, Color textColor)
        {
            // 1. Рисуем темную полупрозрачную подложку
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(0f, 0f, 0f, 0.6f); // Черный с 60% прозрачностью
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(rect.X, rect.Y);
            GL.Vertex2(rect.X + rect.Width, rect.Y);
            GL.Vertex2(rect.X + rect.Width, rect.Y + rect.Height);
            GL.Vertex2(rect.X, rect.Y + rect.Height);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f); // Сбрасываем цвет в белый для текстур

            // 2. Рисуем текст
            // Чтобы текст не растягивался, используем чуть меньший размер внутри плашки
            int textId = _resourceManager.GetTextTexture(text, textColor);
            GL.BindTexture(TextureTarget.Texture2D, textId);
            GL.Begin(PrimitiveType.Quads);
            // Делаем небольшие отступы для текста внутри плашки
            float tx = rect.X + 0.2f;
            float ty = rect.Y + 0.1f;
            float tw = rect.Width - 0.4f;
            float th = rect.Height - 0.2f;

            GL.TexCoord2(0, 0); GL.Vertex2(tx, ty);
            GL.TexCoord2(1, 0); GL.Vertex2(tx + tw, ty);
            GL.TexCoord2(1, 1); GL.Vertex2(tx + tw, ty + th);
            GL.TexCoord2(0, 1); GL.Vertex2(tx, ty + th);
            GL.End();
        }
    }
}
