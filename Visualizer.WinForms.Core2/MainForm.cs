using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Pages;
using SkiaSharp;
using System.Drawing.Drawing2D;

namespace ResoEngine.Visualizer;

public class MainForm : Form
{
    private readonly SkiaCanvas _canvas;
    private readonly PageNavBar _navBar;
    private readonly PageManager _pageManager;
    private readonly DragController _dragController;
    private readonly Panel _pageNamePanel;
    private readonly Label _pageNameLabel;
    private readonly Button _copyPageNameButton;
    private readonly Bitmap _copyIcon;

    private bool _originDragging;
    private SKPoint _originDragStart;
    private float _originStartX;
    private float _originStartY;

    public MainForm()
    {
        Text = "ResoEngine Visualizer (Core2)";
        Size = new Size(1420, 1220);
        MinimumSize = new Size(980, 820);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(240, 240, 240);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        _canvas = new SkiaCanvas { Dock = DockStyle.Fill };
        _navBar = new PageNavBar { Dock = DockStyle.Fill };
        _copyIcon = CreateCopyIcon();
        _pageNamePanel = new Panel
        {
            BackColor = Color.FromArgb(248, 248, 248),
            Size = new Size(286, 30),
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
        };
        _pageNameLabel = new Label
        {
            AutoSize = false,
            Location = new Point(10, 5),
            Size = new Size(238, 18),
            Font = new Font(VisualStyle.UiFontFamily, 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(132, 132, 132),
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _copyPageNameButton = new Button
        {
            Width = 24,
            Height = 24,
            Location = new Point(254, 3),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(248, 248, 248),
            Image = _copyIcon,
            TabStop = false,
            Cursor = Cursors.Hand,
        };
        _copyPageNameButton.FlatAppearance.BorderSize = 0;
        _copyPageNameButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(232, 232, 232);
        _copyPageNameButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        _copyPageNameButton.Click += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(_pageNameLabel.Text))
            {
                Clipboard.SetText(_pageNameLabel.Text);
            }
        };
        _pageNamePanel.Controls.Add(_pageNameLabel);
        _pageNamePanel.Controls.Add(_copyPageNameButton);

        layout.Controls.Add(_canvas, 0, 0);
        layout.Controls.Add(_navBar, 0, 1);
        Controls.Add(layout);
        _canvas.Controls.Add(_pageNamePanel);
        PositionPageNamePanel();
        _canvas.Resize += (_, _) => PositionPageNamePanel();

        var hitTest = new HitTestEngine();
        _dragController = new DragController(_canvas.Coords, hitTest);

        _canvas.OnPointerDown += OnPointerDown;
        _canvas.OnPointerMove += OnPointerMove;
        _canvas.OnPointerUp += OnPointerUp;

        _dragController.Changed += () => _canvas.InvalidateCanvas();

        _pageManager = new PageManager(_canvas, _navBar, hitTest);
        _pageManager.CurrentPageChanged += page => UpdatePageName(page);
        _pageManager.AddPage(new PinningAxisPage());
        _pageManager.AddPage(new FriezePatternEditorPage());
        _pageManager.AddPage(new FriezeGalleryPage());
        _pageManager.AddPage(new SquareWaveDynamicsPage());
        _pageManager.AddPage(new OrthogonalAxesPage());
        _pageManager.AddPage(new BooleanOpsPage());
        _pageManager.AddPage(new OrthogonalBooleanGalleryPage());
        _pageManager.AddPage(new ParallelBooleanGalleryPage());
        _pageManager.AddPage(new FractionalPowerPage());
        _pageManager.AddPage(new BoundaryRepetitionPage());
        _pageManager.AddPage(new UnitsQuantityPage());
        _pageManager.AddPage(new ContainmentTensionPage());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _copyIcon.Dispose();
        }

        base.Dispose(disposing);
    }

    private void UpdatePageName(IVisualizerPage? page)
    {
        _pageNameLabel.Text = page?.GetType().Name ?? string.Empty;
    }

    private void PositionPageNamePanel()
    {
        _pageNamePanel.Location = new Point(12, Math.Max(12, _canvas.ClientSize.Height - _pageNamePanel.Height - 12));
        _pageNamePanel.BringToFront();
    }

    private static Bitmap CreateCopyIcon()
    {
        var bitmap = new Bitmap(16, 16);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var shadowPen = new Pen(Color.FromArgb(160, 160, 160), 1.3f);
        using var frontPen = new Pen(Color.FromArgb(110, 110, 110), 1.3f);

        graphics.DrawRectangle(shadowPen, 5, 2, 7, 9);
        graphics.DrawRectangle(frontPen, 3, 5, 7, 9);
        return bitmap;
    }

    private void OnPointerDown(SKPoint pt)
    {
        var page = _pageManager.CurrentPage;

        if (page != null && page.OnPointerDown(pt))
        {
            _canvas.InvalidateCanvas();
            return;
        }

        if (page != null && page.IsOriginHit(pt))
        {
            _originDragging = true;
            _originDragStart = pt;
            _originStartX = _canvas.Coords.OriginX;
            _originStartY = _canvas.Coords.OriginY;
            return;
        }

        if (_dragController.BeginDrag(pt))
        {
            _canvas.InvalidateCanvas();
        }
    }

    private void OnPointerMove(SKPoint pt)
    {
        if (_originDragging)
        {
            float dx = pt.X - _originDragStart.X;
            float dy = pt.Y - _originDragStart.Y;
            _canvas.Coords.OriginX = _originStartX + dx;
            _canvas.Coords.OriginY = _originStartY + dy;
            _canvas.InvalidateCanvas();
            return;
        }

        if (_dragController.IsDragging)
        {
            if (_dragController.UpdateDrag(pt))
            {
                _canvas.InvalidateCanvas();
            }
        }
        else
        {
            var page = _pageManager.CurrentPage;
            page?.OnPointerMove(pt);
            if (page != null && page.IsOriginHit(pt))
            {
                _canvas.Cursor = Cursors.SizeAll;
            }
            else
            {
                _canvas.Cursor = _dragController.GetCursor(pt);
            }
        }
    }

    private void OnPointerUp(SKPoint pt)
    {
        if (_originDragging)
        {
            _originDragging = false;
            _canvas.Cursor = Cursors.Default;
            return;
        }

        _pageManager.CurrentPage?.OnPointerUp(pt);
        _dragController.EndDrag();
        _canvas.Cursor = Cursors.Default;
    }
}
