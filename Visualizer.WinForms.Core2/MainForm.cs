using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Pages;
using SkiaSharp;

namespace ResoEngine.Visualizer;

public class MainForm : Form
{
    private readonly SkiaCanvas _canvas;
    private readonly PageNavBar _navBar;
    private readonly PageManager _pageManager;
    private readonly DragController _dragController;

    private bool _originDragging;
    private SKPoint _originDragStart;
    private float _originStartX;
    private float _originStartY;

    public MainForm()
    {
        Text = "ResoEngine Visualizer (Core2)";
        Size = new Size(1320, 1120);
        MinimumSize = new Size(900, 760);
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

        var hitTest = new HitTestEngine();
        _dragController = new DragController(_canvas.Coords, hitTest);

        _canvas.OnPointerDown += OnPointerDown;
        _canvas.OnPointerMove += OnPointerMove;
        _canvas.OnPointerUp += OnPointerUp;

        _dragController.Changed += () => _canvas.InvalidateCanvas();

        _pageManager = new PageManager(_canvas, _navBar, hitTest);
        _pageManager.AddPage(new OrthogonalAxesPage());
        _pageManager.AddPage(new BooleanOpsPage());
        _pageManager.AddPage(new OrthogonalBooleanGalleryPage(0));
        _pageManager.AddPage(new OrthogonalBooleanGalleryPage(8));
        _pageManager.AddPage(new ParallelBooleanGalleryPage(0));
        _pageManager.AddPage(new ParallelBooleanGalleryPage(8));
    }

    private void OnPointerDown(SKPoint pt)
    {
        var page = _pageManager.CurrentPage;

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

        _dragController.EndDrag();
        _canvas.Cursor = Cursors.Default;
    }
}
