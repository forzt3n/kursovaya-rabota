using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Klad.Core.Entities;

namespace Klad.Infrastructure.Resources
{
    public class ResourceManager : IDisposable
    {
        private readonly Dictionary<TextureId, int> _textures = new();
        private readonly string _spriteDir;

        public ResourceManager()
        {
            _spriteDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sprites");
            Initialize();
        }

        private void Initialize()
        {
            Color bgColor = Color.FromArgb(0x07, 0xad, 0x2b);

            _textures[TextureId.Empty] = LoadPlaceholder(bgColor);
            _textures[TextureId.SolidWall] = LoadTexture(Path.Combine(_spriteDir, "Wall.png"), bgColor);
            _textures[TextureId.BreakableWall] = LoadTexture(Path.Combine(_spriteDir, "Breackable_wall.png"), bgColor);
            _textures[TextureId.RecoverableWall] = LoadTexture(Path.Combine(_spriteDir, "ReWall.png"), bgColor);
            _textures[TextureId.TemporaryWallPlaceholder] = LoadTexture(Path.Combine(_spriteDir, "Wall.png"), bgColor);
            _textures[TextureId.Treasure] = LoadTexture(Path.Combine(_spriteDir, "Treasure.png"), bgColor);
            _textures[TextureId.Boost] = LoadTexture(Path.Combine(_spriteDir, "Boost.png"), bgColor);
            _textures[TextureId.Debuff] = LoadTexture(Path.Combine(_spriteDir, "Debuff.png"), bgColor);
            _textures[TextureId.Player1] = LoadTexture(Path.Combine(_spriteDir, "FPlayer.png"), bgColor);
            _textures[TextureId.Player2] = LoadTexture(Path.Combine(_spriteDir, "SPlayer.png"), bgColor);

            _textures[TextureId.MenuPlay] = LoadTextTexture("PLAY", Color.White, Color.Transparent);
            _textures[TextureId.MenuQuit] = LoadTextTexture("QUIT", Color.White, Color.Transparent);
        }

        public int GetTexture(TextureId id)
        {
            return _textures.TryGetValue(id, out int textureId) ? textureId : _textures[TextureId.Empty];
        }

        public int GetTextTexture(string text, Color color)
        {
            return LoadTextTexture(text, color, Color.Transparent);
        }

        private int LoadPlaceholder(Color color)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(color);
                    g.DrawRectangle(Pens.Black, 0, 0, 31, 31);
                }
                UploadBitmap(bmp);
            }
            SetTextureParameters(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            return id;
        }

        private int LoadTexture(string path, Color fallbackColor)
        {
            if (!File.Exists(path)) return LoadPlaceholder(fallbackColor);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            using (Bitmap bmp = new Bitmap(path))
            {
                UploadBitmap(bmp);
            }
            SetTextureParameters(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            return id;
        }

        private int LoadTextTexture(string text, Color textColor, Color backColor)
        {
            if (string.IsNullOrWhiteSpace(text)) text = " ";
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            {
                using (Bitmap tempBmp = new Bitmap(1, 1))
                using (var gMeasure = System.Drawing.Graphics.FromImage(tempBmp))
                {
                    var size = gMeasure.MeasureString(text, font);
                    int width = (int)Math.Ceiling(size.Width);
                    int height = (int)Math.Ceiling(size.Height);

                    using (Bitmap bmp = new Bitmap(width, height))
                    {
                        using (var g = System.Drawing.Graphics.FromImage(bmp))
                        {
                            g.Clear(backColor);
                            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                            g.DrawString(text, font, new SolidBrush(textColor), 0, 0);
                        }

                        int id = GL.GenTexture();
                        GL.BindTexture(TextureTarget.Texture2D, id);
                        UploadBitmap(bmp);
                        SetTextureParameters(TextureMinFilter.Linear, TextureMagFilter.Linear);
                        return id;
                    }
                }
            }
        }

        private void UploadBitmap(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
        }

        private void SetTextureParameters(TextureMinFilter min, TextureMagFilter mag)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)mag);
        }

        public void Dispose()
        {
            foreach (var id in _textures.Values)
            {
                GL.DeleteTexture(id);
            }
            _textures.Clear();
        }
    }
}
