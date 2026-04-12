using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace klad.Graphics
{
    public class TextureLoader
    {
        public static int LoadPlaceholder(Color color)
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

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        public static Dictionary<int, int> InitializeTextures()
        {
            var textures = new Dictionary<int, int>();
        
            textures[0] = LoadPlaceholder(Color.Lime);   
        
            textures[1] = LoadPlaceholder(Color.Green);  
           
            textures[2] = LoadPlaceholder(Color.Brown);  
           
            textures[3] = LoadPlaceholder(Color.Red);    
           
            textures[4] = LoadPlaceholder(Color.DarkGray); 
           
            textures[5] = LoadPlaceholder(Color.Yellow); 
            
            textures[6] = LoadPlaceholder(Color.Gold);
           
            textures[7] = LoadPlaceholder(Color.Purple);
          
            textures[8] = LoadPlaceholder(Color.Magenta);
           
            textures[9] = LoadPlaceholder(Color.Cyan);
            return textures;
        }
    }
}
