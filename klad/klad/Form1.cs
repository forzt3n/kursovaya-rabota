using OpenTK;
using OpenTK.Graphics.OpenGL;
using klad.Logic;
using klad.Graphics;
using System.Drawing;

namespace klad
{
    public partial class Form1 : Form
    {
        private GLControl _glControl = null!;
        private GameEngine _game = null!;
        private Renderer _renderer = null!;
        private Dictionary<Keys, bool> _keys = new Dictionary<Keys, bool>();
        private System.Windows.Forms.Timer _timer = null!;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Text = "Кладоискатель - 2 Players";
            this.ClientSize = new Size(800, 600);

            _glControl = new GLControl();
            _glControl.Dock = DockStyle.Fill;
            _glControl.Load += GlControl_Load;
            _glControl.Paint += GlControl_Paint;
            _glControl.Resize += GlControl_Resize;
            _glControl.KeyDown += (s, e) => _keys[e.KeyCode] = true;
            _glControl.KeyUp += (s, e) => {
                _keys[e.KeyCode] = false;
                HandleActionKeys(e.KeyCode);
            };
            this.Controls.Add(_glControl);

            _game = new GameEngine();
            
            // Путь к BMP карте
            string mapsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Maps");
            Directory.CreateDirectory(mapsDir);
            string mapPath = Path.Combine(mapsDir, "level1.bmp");
            
            // Если карты нет, создаем тестовую по ТЗ
            if (!File.Exists(mapPath))
            {
                CreateDefaultBmp(mapPath);
            }

            _game.InitializeFromBmp(mapPath);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 16; // ~60 FPS
            _timer.Tick += GameLoop;
            _timer.Start();
        }

        private void CreateDefaultBmp(string path)
        {
            using (Bitmap bmp = new Bitmap(21, 21))
            {
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Lime); // Салатовый фон (проходимая)
                    
                    // Зеленые стены по периметру
                    Pen greenPen = new Pen(Color.Green);
                    g.DrawRectangle(greenPen, 0, 0, 20, 20);

                    // Коричневые (разрушаемые)
                    bmp.SetPixel(5, 5, Color.Brown);
                    bmp.SetPixel(6, 5, Color.Brown);
                    
                    // Красные (проходные)
                    bmp.SetPixel(10, 0, Color.Red);
                    bmp.SetPixel(0, 10, Color.Red);
                    
                    // Желтые (сокровища/призы)
                    bmp.SetPixel(10, 10, Color.Yellow);
                    
                    // Стартовые позиции (спец цвета для MapLoader)
                    bmp.SetPixel(1, 1, Color.Magenta);
                    bmp.SetPixel(19, 19, Color.Cyan);
                }
                bmp.Save(path);
            }
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.Texture2D);
            var textures = TextureLoader.InitializeTextures();
            _renderer = new Renderer(textures);
            _renderer.Resize(_glControl.Width, _glControl.Height, _game.Map.Width, _game.Map.Height);
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            if (_renderer != null && _game != null)
                _renderer.Resize(_glControl.Width, _glControl.Height, _game.Map.Width, _game.Map.Height);
        }

        private void GlControl_Paint(object? sender, PaintEventArgs e)
        {
            if (_renderer != null && _game != null)
            {
                _renderer.Render(_game);
                _glControl.SwapBuffers();
            }
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            UpdateInput();
            _game.Update(0.016f); // 16ms
            
            if (_game.IsGameOver())
            {
                _timer.Stop();
                MessageBox.Show($"Game Over! Winner: {_game.GetWinner()}\nScores: P1: {_game.Player1.Score}, P2: {_game.Player2.Score}");
                _game.InitializeFromBmp(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Maps", "level1.bmp"));
                _timer.Start();
            }

            _glControl.Invalidate();
        }

        private void UpdateInput()
        {
            // Player 1 (WASD)
            if (_keys.ContainsKey(Keys.W) && _keys[Keys.W]) _game.MovePlayer(1, 0, -1);
            if (_keys.ContainsKey(Keys.S) && _keys[Keys.S]) _game.MovePlayer(1, 0, 1);
            if (_keys.ContainsKey(Keys.A) && _keys[Keys.A]) _game.MovePlayer(1, -1, 0);
            if (_keys.ContainsKey(Keys.D) && _keys[Keys.D]) _game.MovePlayer(1, 1, 0);

            // Player 2 (Arrows)
            if (_keys.ContainsKey(Keys.Up) && _keys[Keys.Up]) _game.MovePlayer(2, 0, -1);
            if (_keys.ContainsKey(Keys.Down) && _keys[Keys.Down]) _game.MovePlayer(2, 0, 1);
            if (_keys.ContainsKey(Keys.Left) && _keys[Keys.Left]) _game.MovePlayer(2, -1, 0);
            if (_keys.ContainsKey(Keys.Right) && _keys[Keys.Right]) _game.MovePlayer(2, 1, 0);
        }

        private void HandleActionKeys(Keys key)
        {
            // Player 1 actions
            if (key == Keys.Q) _game.PlaceTempPassage(1);
            if (key == Keys.E) _game.PlaceTempWall(1);
            if (key == Keys.F) _game.DestroyWall(1);

            // Player 2 actions
            if (key == Keys.NumPad1 || key == Keys.D1) _game.PlaceTempPassage(2);
            if (key == Keys.NumPad2 || key == Keys.D2) _game.PlaceTempWall(2);
            if (key == Keys.NumPad3 || key == Keys.D3) _game.DestroyWall(2);
        }
    }
}
