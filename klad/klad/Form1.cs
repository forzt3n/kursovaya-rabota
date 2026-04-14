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
        private string _mapPath = "";

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Text = "Кладоискатель - 2 Players (F/G vs Num2/Num1)";
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
            string mapsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Maps");
            Directory.CreateDirectory(mapsDir);
            _mapPath = Path.Combine(mapsDir, "level1.bmp");
            
            _game.InitializeNewRandomGame(21, 21, _mapPath);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 16;
            _timer.Tick += GameLoop;
            _timer.Start();
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
            _game.Update(0.016f);
            
            if (_game.IsGameOver())
            {
                _timer.Stop();
                MessageBox.Show($"Игра окончена! Победитель: {_game.GetWinner()}\nСчёт: P1: {_game.Player1.Score}, P2: {_game.Player2.Score}");
                _game.InitializeNewRandomGame(21, 21, _mapPath);
                _renderer.Resize(_glControl.Width, _glControl.Height, _game.Map.Width, _game.Map.Height);
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

            // Player 2 (Arrows - корректное управление)
            if (_keys.ContainsKey(Keys.Up) && _keys[Keys.Up]) _game.MovePlayer(2, 0, -1);
            if (_keys.ContainsKey(Keys.Down) && _keys[Keys.Down]) _game.MovePlayer(2, 0, 1);
            if (_keys.ContainsKey(Keys.Left) && _keys[Keys.Left]) _game.MovePlayer(2, -1, 0);
            if (_keys.ContainsKey(Keys.Right) && _keys[Keys.Right]) _game.MovePlayer(2, 1, 0);
        }

        private void HandleActionKeys(Keys key)
        {
            // Player 1 actions
            if (key == Keys.F) _game.ActionRemoveWall(1); // Убрать
            if (key == Keys.G) _game.ActionPlaceWall(1);  // Поставить

            // Player 2 actions
            if (key == Keys.NumPad2 || key == Keys.D2) _game.ActionRemoveWall(2); // Убрать
            if (key == Keys.NumPad1 || key == Keys.D1) _game.ActionPlaceWall(2);  // Поставить
        }
    }
}
