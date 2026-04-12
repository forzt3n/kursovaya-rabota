using OpenTK.Graphics.OpenGL;
using klad.Logic;
using klad.Models;

namespace klad.Graphics
{
    public class Renderer
    {
        private Dictionary<int, int> _textures;

        public Renderer(Dictionary<int, int> textures)
        {
            _textures = textures;
        }

        public void Render(GameEngine game)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();

            for (int x = 0; x < game.Map.Width; x++)
            {
                for (int y = 0; y < game.Map.Height; y++)
                {
                    if (game.Map.IsBlinking(x, y)) continue; 

                    var element = game.Map[x, y];
                    DrawQuad(x, y, _textures[element.TextureId]);
                }
            }

           
            foreach (var prize in game.Prizes)
            {
                DrawQuad(prize.X, prize.Y, _textures[prize.TextureId]);
            }

            
            DrawQuad(game.Player1.X, game.Player1.Y, _textures[game.Player1.TextureId]);
            DrawQuad(game.Player2.X, game.Player2.Y, _textures[game.Player2.TextureId]);
        }

        private void DrawQuad(float x, float y, int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(x, y);
            GL.TexCoord2(1, 0); GL.Vertex2(x + 1, y);
            GL.TexCoord2(1, 1); GL.Vertex2(x + 1, y + 1);
            GL.TexCoord2(0, 1); GL.Vertex2(x, y + 1);
            GL.End();
        }

        public void Resize(int width, int height, int mazeWidth, int mazeHeight)
        {
            GL.Viewport(0, 0, width, height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, mazeWidth, mazeHeight, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}
