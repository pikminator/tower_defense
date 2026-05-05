using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Timer = System.Windows.Forms.Timer;

namespace tower_defense
{
    public partial class GameForm : Form
    {
        private readonly GameController _game;
        private readonly GameRenderer _renderer;
        private readonly InputHandler _input;
        private readonly Timer _gameTimer;

        private int _cellSize = GameConstants.CellSize;
        private int _offsetX;
        private int _offsetY;
        private int _topHudHeight = 60;
        private Rectangle _bottomPanel;
        private Point _mousePos;
        private Point _mouseDownPos = Point.Empty;

        // GDI-объекты: создаются и освобождаются формой
        private Font _mainFont = null!;
        private Font _boldFont = null!;
        private Font _smallFont = null!;
        private Font _titleFont = null!;
        private Font _headerFont = null!;
        private Font _subheaderFont = null!;
        private Font _btnFont = null!;
        private Font _hugeFont = null!;

        private SolidBrush _accentBrush = null!;
        private SolidBrush _secondaryBrush = null!;
        private SolidBrush _dangerBrush = null!;
        private SolidBrush _successBrush = null!;
        private SolidBrush _warningBrush = null!;
        private SolidBrush _panelBrush = null!;
        private SolidBrush _pathBrush = null!;
        private SolidBrush _hudBrush = null!;
        private SolidBrush _shadowBrush = null!;
        private SolidBrush _overlayBrush = null!;
        private SolidBrush _whiteGhostBrush = null!;
        private SolidBrush _greenGhostBrush = null!;
        private SolidBrush _yellowGhostBrush = null!;
        private SolidBrush _redGhostBrush = null!;
        private SolidBrush _bossColorBrush = null!;
        private SolidBrush _enemyColorBrush = null!;
        private SolidBrush _projBrush = null!;
        private SolidBrush _blueBrush = null!;
        private SolidBrush _blackBrush = null!;
        private SolidBrush _tomatoBrush = null!;
        private SolidBrush _lightGreenBrush = null!;

        private Pen _gridPen = null!;
        private Pen _pathPen = null!;
        private Pen _redPen = null!;
        private Pen _whitePen = null!;
        private Pen _rangePen = null!;

        private StringFormat _centerAlign = null!;
        private StringFormat _nearFarAlign = null!;
        private StringFormat _centerFarAlign = null!;

        private PrivateFontCollection _fontCollection = null!; // Должен жить пока живы шрифты

        public GameForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ClientSize = new Size(1024, 768);
            BackColor = Color.FromArgb(24, 24, 27);

            _game = new GameController();
            _input = new InputHandler();
            _renderer = CreateRenderer();

            _bottomPanel = new Rectangle(0, ClientSize.Height - 100, ClientSize.Width, 100);

            _gameTimer = new Timer { Interval = 10 };
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            KeyPreview = true;
            KeyDown += OnKeyDown;
            Resize += OnResize;
            Disposed += OnDisposed;

            UpdateScaling();
        }

        private GameRenderer CreateRenderer()
        {
            InitGdiObjects();
            return new GameRenderer(
                SpriteAtlas.Sprites,
                _mainFont, _boldFont, _smallFont, _titleFont,
                _headerFont, _subheaderFont, _btnFont, _hugeFont,
                _accentBrush, _secondaryBrush, _dangerBrush, _successBrush,
                _warningBrush, _panelBrush, _pathBrush, _hudBrush,
                _shadowBrush, _overlayBrush, _whiteGhostBrush, _greenGhostBrush,
                _yellowGhostBrush, _redGhostBrush, _bossColorBrush, _enemyColorBrush,
                _projBrush, _blueBrush, _blackBrush, _tomatoBrush, _lightGreenBrush,
                _gridPen, _pathPen, _redPen, _whitePen, _rangePen,
                _centerAlign, _nearFarAlign, _centerFarAlign);
        }

        private void InitGdiObjects()
        {
            // Press Start 2P: ретро-пиксельный шрифт
            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "PressStart2P.ttf"));
            var pf = _fontCollection.Families[0];

            _mainFont = new Font(pf, 9, FontStyle.Regular);
            _boldFont = new Font(pf, 9, FontStyle.Regular);
            _smallFont = new Font(pf, 8, FontStyle.Regular);
            _titleFont = new Font(pf, 28, FontStyle.Regular);
            _headerFont = new Font(pf, 16, FontStyle.Regular);
            _subheaderFont = new Font(pf, 12, FontStyle.Regular);
            _btnFont = new Font(pf, 10, FontStyle.Regular);
            _hugeFont = new Font(pf, 32, FontStyle.Regular);

            _accentBrush = new SolidBrush(Color.FromArgb(79, 70, 229));
            _secondaryBrush = new SolidBrush(Color.FromArgb(82, 82, 91));
            _dangerBrush = new SolidBrush(Color.FromArgb(239, 68, 68));
            _successBrush = new SolidBrush(Color.FromArgb(34, 197, 94));
            _warningBrush = new SolidBrush(Color.FromArgb(234, 179, 8));
            _panelBrush = new SolidBrush(Color.FromArgb(39, 39, 42));
            _pathBrush = new SolidBrush(Color.FromArgb(63, 63, 70));
            _hudBrush = new SolidBrush(Color.FromArgb(240, 39, 39, 42));
            _shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            _overlayBrush = new SolidBrush(Color.FromArgb(150, 9, 9, 11));
            _whiteGhostBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            _greenGhostBrush = new SolidBrush(Color.FromArgb(100, 34, 197, 94));
            _yellowGhostBrush = new SolidBrush(Color.FromArgb(100, 245, 158, 11));
            _redGhostBrush = new SolidBrush(Color.FromArgb(100, 239, 68, 68));
            _bossColorBrush = new SolidBrush(Color.FromArgb(220, 38, 38));
            _enemyColorBrush = new SolidBrush(Color.FromArgb(244, 63, 94));
            _projBrush = new SolidBrush(Color.FromArgb(251, 146, 60));
            _blueBrush = new SolidBrush(Color.FromArgb(56, 189, 248));
            _blackBrush = new SolidBrush(Color.Black);
            _tomatoBrush = new SolidBrush(Color.Tomato);
            _lightGreenBrush = new SolidBrush(Color.LightGreen);

            _gridPen = new Pen(Color.FromArgb(39, 39, 42), 1);
            _pathPen = new Pen(Color.FromArgb(63, 63, 70), 24) { LineJoin = LineJoin.Round };
            _redPen = new Pen(Color.Red, 2);
            _whitePen = new Pen(Color.White, 2);
            _rangePen = new Pen(Color.FromArgb(150, 56, 189, 248), 2) { DashStyle = DashStyle.Dash };

            _centerAlign = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            _nearFarAlign = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            _centerFarAlign = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };
        }

        private void UpdateScaling()
        {
            // Сначала грубая оценка cellSize для расчёта HUD и панели
            int tmpCell = Math.Min(ClientSize.Width / GameConstants.TargetCols,
                (ClientSize.Height - 60 - 100) / GameConstants.TargetRows);
            int iconSize = tmpCell - 4;
            int margin = tmpCell / 2;
            int statsW = 6 * (iconSize + 4 + tmpCell * 2) + iconSize * 5;
            int btnsW = tmpCell * 2 + tmpCell * 3 + tmpCell * 3 + tmpCell * 4 + tmpCell * 4 + 4 * margin;
            bool twoRowsHud = ClientSize.Width < margin * 2 + statsW + btnsW + margin * 2;
            int panelH = tmpCell * 3 + 15;
            _topHudHeight = twoRowsHud ? tmpCell * 2 + margin * 3 : tmpCell + margin * 2;
            _cellSize = Math.Min(ClientSize.Width / GameConstants.TargetCols,
                (ClientSize.Height - _topHudHeight - panelH) / GameConstants.TargetRows);
            _offsetX = Math.Max(0, (ClientSize.Width - GameConstants.TargetCols * _cellSize) / 2);
            _offsetY = _topHudHeight + Math.Max(0,
                (ClientSize.Height - _topHudHeight - panelH - GameConstants.TargetRows * _cellSize) / 2);
            _bottomPanel = new Rectangle(0, ClientSize.Height - panelH, ClientSize.Width, panelH);
            _game.CellSize = _cellSize;
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (!_input.InMainMenu && !_input.IsPaused)
            {
                _game.Update(0.033 * _input.TimeScale);
            }
            Invalidate();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _input.HandleKeyDown(e.KeyCode);
        }

        private void OnResize(object? sender, EventArgs e)
        {
            UpdateScaling();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(24, 24, 27));

            if (_input.InMainMenu)
            {
                if (_input.ShowingLevels)
                    _renderer.DrawLevelSelect(g, ClientSize);
                else if (_input.ShowingTechs)
                    _renderer.DrawTechTree(g, ClientSize);
                else if (_input.ShowingRecords)
                    _renderer.DrawRecordsScreen(g, ClientSize);
                else
                    _renderer.DrawMainMenu(g, ClientSize);
                return;
            }

            // Экран конца игры
            if (_game.BaseHP <= 0 || (_game.Wave > GameConstants.FinalWave && _game.Enemies.Count == 0))
            {
                _renderer.DrawGameOver(g, _game, ClientSize, out _);
                return;
            }

            // Игровое поле
            _renderer.DrawGameField(g, _game, _cellSize, _mousePos, _input.SelectedTower,
                _offsetX, _offsetY, _topHudHeight, _bottomPanel);

            // Интерфейс
            _renderer.DrawHUD(g, _game, ClientSize,
                _mouseDownPos.X, _mouseDownPos.Y, _input.IsPaused, _input.TimeScale, _topHudHeight, _cellSize);
            _renderer.DrawBottomPanel(g, ClientSize, _input.SelectedTower, _bottomPanel.Y, _cellSize);

            // Панель информации о башне
            if (_input.SelectedTowerObj != null)
            {
                _renderer.DrawTowerInfoPanel(g, _input.SelectedTowerObj, ClientSize,
                    _cellSize, _offsetX, _offsetY, _mouseDownPos.X, _mouseDownPos.Y, _topHudHeight);
            }

            // Оверлей паузы
            if (_input.IsPaused)
            {
                _renderer.DrawPauseOverlay(g, ClientSize);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDownPos = e.Location;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseDownPos = Point.Empty;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _mousePos = e.Location;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _input.HandleMouseClick(e.Location, ClientSize, _game,
                    _topHudHeight, _bottomPanel.Y, _offsetX, _offsetY, _cellSize);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _input.HandleRightClick();
            }
        }

        private void OnDisposed(object? sender, EventArgs e)
        {
            _mainFont?.Dispose();
            _boldFont?.Dispose();
            _smallFont?.Dispose();
            _titleFont?.Dispose();
            _headerFont?.Dispose();
            _subheaderFont?.Dispose();
            _btnFont?.Dispose();
            _hugeFont?.Dispose();

            _accentBrush?.Dispose();
            _secondaryBrush?.Dispose();
            _dangerBrush?.Dispose();
            _successBrush?.Dispose();
            _warningBrush?.Dispose();
            _panelBrush?.Dispose();
            _pathBrush?.Dispose();
            _hudBrush?.Dispose();
            _shadowBrush?.Dispose();
            _overlayBrush?.Dispose();
            _whiteGhostBrush?.Dispose();
            _greenGhostBrush?.Dispose();
            _yellowGhostBrush?.Dispose();
            _redGhostBrush?.Dispose();
            _bossColorBrush?.Dispose();
            _enemyColorBrush?.Dispose();
            _projBrush?.Dispose();
            _blueBrush?.Dispose();
            _blackBrush?.Dispose();
            _tomatoBrush?.Dispose();
            _lightGreenBrush?.Dispose();

            _gridPen?.Dispose();
            _pathPen?.Dispose();
            _redPen?.Dispose();
            _whitePen?.Dispose();
            _rangePen?.Dispose();

            _centerAlign?.Dispose();
            _nearFarAlign?.Dispose();
            _centerFarAlign?.Dispose();
            _fontCollection?.Dispose();
        }
    }
}
