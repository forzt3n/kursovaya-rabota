using OpenTK;
using OpenTK.Graphics.OpenGL;
using Klad.Core;
using Klad.Core.Map;
using Klad.Core.Entities;
using Klad.Infrastructure.Graphics;
using Klad.Infrastructure.Resources;
using Klad.Infrastructure.Input;
using System.Drawing;

namespace Klad.App.UI
{
    public partial class MainWindow : Form
    {
        private GLControl? _glControl;
        private GameEngine? _game;
        private Renderer? _renderer;
        private ResourceManager? _resourceManager;
        private InputService _inputService = new InputService();
        private System.Windows.Forms.Timer _timer = null!;
        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private float _lastTime = 0;
        private Label _scoreLabel = null!;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyPreview = true;
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Text = "Клад";
            this.ClientSize = new Size(800, 650);

            _glControl = new GLControl();
            _glControl.Dock = DockStyle.Fill;
            _glControl.Load += GlControl_Load;
            _glControl.Paint += GlControl_Paint;
            _glControl.Resize += GlControl_Resize;

            _glControl.PreviewKeyDown += (s, e) => e.IsInputKey = true;
            _glControl.KeyDown += (s, e) => _inputService.UpdateKey(e.KeyCode, true);
            _glControl.KeyUp += (s, e) => _inputService.UpdateKey(e.KeyCode, false);

            this.KeyDown += (s, e) => _inputService.UpdateKey(e.KeyCode, true);
            this.KeyUp += (s, e) => _inputService.UpdateKey(e.KeyCode, false);

            this.Controls.Add(_glControl);

            _game = new GameEngine(_inputService);
            _game.Initialize(21, 21);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 16;
            _timer.Tick += GameLoop;

            _stopwatch.Start();
            _timer.Start();
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(0x07, 0xad, 0x2b));
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _resourceManager = new ResourceManager();
            if (_glControl != null && _game != null)
            {
                _glControl.Focus();
                _renderer = new Renderer(_resourceManager);
                _renderer.Resize(_glControl.Width, _glControl.Height, _game.Map.Width, _game.Map.Height);
            }
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            if (_renderer != null && _game != null && _glControl != null)
                _renderer.Resize(_glControl.Width, _glControl.Height, _game.Map.Width, _game.Map.Height);
        }

        private void GlControl_Paint(object? sender, PaintEventArgs e)
        {
            if (_renderer != null && _game != null && _glControl != null)
            {
                _renderer.Render(_game);
                _glControl.SwapBuffers();
            }
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (_game == null) return;

            if (_game.QuitRequested) Application.Exit();

            float currentTime = (float)_stopwatch.Elapsed.TotalSeconds;
            float deltaTime = currentTime - _lastTime;
            _lastTime = currentTime;

            if (deltaTime > 0.1f) deltaTime = 0.1f;

            _game.Update(deltaTime);
            _inputService.Tick();

            if (_scoreLabel != null && _game.Player1 != null && _game.Player2 != null)
            {
                _scoreLabel.Text = $"P1 Score: {_game.Player1.Score} | P2 Score: {_game.Player2.Score}\nControls - P1: WASD+F/G | P2: Arrows+1/2";
            }

            _glControl?.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _resourceManager?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
