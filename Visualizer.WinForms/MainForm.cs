using SkiaSharp;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Pages;

namespace ResoEngine.Visualizer;

public class MainForm : Form
{
    private readonly SkiaCanvas _canvas;
    private readonly PageNavBar _navBar;
    private readonly PageManager _pageManager;
    private readonly DragController _dragController;

    // Origin drag state (handled separately from DragController)
    private bool _originDragging;
    private SKPoint _originDragStart;
    private float _originStartX;
    private float _originStartY;

    private void InitializeComponent()
    {
    }

    public MainForm()
    {
        Text = "ResoEngine Visualizer";
        Size = new Size(780, 720);
        MinimumSize = new Size(500, 500);
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

        layout.Controls.Add(_canvas, 0, 0);
        layout.Controls.Add(_navBar, 0, 1);
        Controls.Add(layout);

        // Wire up drag controller (unchanged per user request)
        var hitTest = new HitTestEngine();
        _dragController = new DragController(_canvas.Coords, hitTest);

        _canvas.OnPointerDown += OnPointerDown;
        _canvas.OnPointerMove += OnPointerMove;
        _canvas.OnPointerUp += OnPointerUp;

        _dragController.Changed += () => _canvas.InvalidateCanvas();

        // Page manager + first page
        _pageManager = new PageManager(_canvas, _navBar, hitTest);
        _pageManager.AddPage(new OrthogonalAxesPage());
    }

    private void OnPointerDown(SKPoint pt)
    {
        var page = _pageManager.CurrentPage;

        // Check origin hit first (priority over segment hits)
        // Dragging the origin pans the entire viewport without changing segment values
        if (page != null && page.IsOriginHit(pt))
        {
            _originDragging = true;
            _originDragStart = pt;
            _originStartX = _canvas.Coords.OriginX;
            _originStartY = _canvas.Coords.OriginY;
            return;
        }

        // Fall through to regular drag
        if (_dragController.BeginDrag(pt))
            _canvas.InvalidateCanvas();
    }

    private void OnPointerMove(SKPoint pt)
    {
        if (_originDragging)
        {
            // Pan the entire viewport by moving the coordinate system origin
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
                _canvas.InvalidateCanvas();
        }
        else
        {
            // Update cursor based on hover
            var page = _pageManager.CurrentPage;
            if (page != null && page.IsOriginHit(pt))
                _canvas.Cursor = Cursors.SizeAll;
            else
                _canvas.Cursor = _dragController.GetCursor(pt);
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

        _dragController.EndDrag();
        _canvas.Cursor = Cursors.Default;
    }

    private static float Snap(float val) =>
        MathF.Round(val / VisualStyle.SnapIncrement) * VisualStyle.SnapIncrement;
}
