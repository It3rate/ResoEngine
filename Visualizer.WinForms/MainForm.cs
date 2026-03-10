using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Pages;

namespace ResoEngine.Visualizer;

public class MainForm : Form
{
    private readonly SkiaCanvas _canvas;
    private readonly PageNavBar _navBar;
    private readonly PageManager _pageManager;
    private readonly DragController _dragController;

    public MainForm()
    {
        Text = "ResoEngine Visualizer";
        Size = new Size(640, 620);
        MinimumSize = new Size(400, 400);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(240, 240, 240);

        // Layout: canvas fills top, nav bar at bottom (fixed 50px)
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

        // Wire up input → drag controller → repaint
        var hitTest = new HitTestEngine();
        _dragController = new DragController(_canvas.Coords, hitTest);

        _canvas.OnPointerDown += pt =>
        {
            if (_dragController.BeginDrag(pt))
                _canvas.InvalidateCanvas();
        };

        _canvas.OnPointerMove += pt =>
        {
            if (_dragController.IsDragging)
            {
                if (_dragController.UpdateDrag(pt))
                    _canvas.InvalidateCanvas();
            }
            else
            {
                _canvas.Cursor = _dragController.GetCursor(pt);
            }
        };

        _canvas.OnPointerUp += _ =>
        {
            _dragController.EndDrag();
            _canvas.Cursor = Cursors.Default;
        };

        _dragController.Changed += () => _canvas.InvalidateCanvas();

        // Page manager + first page
        _pageManager = new PageManager(_canvas, _navBar, hitTest);
        _pageManager.AddPage(new OrthogonalAxesPage());
    }
}
