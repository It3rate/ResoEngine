namespace ResoEngine.Visualizer.Controls;

/// <summary>
/// Navigation bar with prev/next buttons and clickable dot indicators.
/// Uses standard Windows buttons for reliable rendering.
/// </summary>
public class PageNavBar : UserControl
{
    private readonly Button _prevButton;
    private readonly Button _nextButton;
    private readonly FlowLayoutPanel _dotsPanel;
    private readonly List<Panel> _dots = [];
    private int _activeIndex;

    public event Action? PrevClicked;
    public event Action? NextClicked;
    public event Action<int>? DotClicked;

    public PageNavBar()
    {
        Height = 50;

        _prevButton = new Button
        {
            Text = "\u25C0",
            Width = 44,
            Height = 32,
            FlatStyle = FlatStyle.System,
            Font = new Font("Segoe UI", 10f),
            Cursor = Cursors.Hand,
            Margin = new Padding(4, 4, 4, 0),
        };
        _prevButton.Click += (_, _) => PrevClicked?.Invoke();

        _dotsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            Margin = new Padding(8, 8, 8, 0),
        };

        _nextButton = new Button
        {
            Text = "\u25B6",
            Width = 44,
            Height = 32,
            FlatStyle = FlatStyle.System,
            Font = new Font("Segoe UI", 10f),
            Cursor = Cursors.Hand,
            Margin = new Padding(4, 4, 4, 0),
        };
        _nextButton.Click += (_, _) => NextClicked?.Invoke();

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
        };
        layout.Controls.Add(_prevButton);
        layout.Controls.Add(_dotsPanel);
        layout.Controls.Add(_nextButton);
        Controls.Add(layout);

        // Center the layout within the bar
        layout.Resize += (_, _) => CenterLayout(layout);
        Resize += (_, _) => CenterLayout(layout);
    }

    private void CenterLayout(FlowLayoutPanel layout)
    {
        int pw = layout.PreferredSize.Width;
        int ph = layout.PreferredSize.Height;
        layout.SetBounds(
            (Width - pw) / 2,
            (Height - ph) / 2,
            pw, ph);
    }

    public void UpdateDots(int pageCount, int activeIndex)
    {
        _activeIndex = activeIndex;
        _dotsPanel.Controls.Clear();
        _dots.Clear();

        for (int i = 0; i < pageCount; i++)
        {
            int pageIdx = i;
            var dot = new Panel
            {
                Width = 12,
                Height = 12,
                Margin = new Padding(4, 4, 4, 0),
                Cursor = Cursors.Hand,
            };
            dot.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var color = pageIdx == _activeIndex
                    ? Color.FromArgb(80, 80, 80)
                    : Color.FromArgb(200, 200, 200);
                using var brush = new SolidBrush(color);
                e.Graphics.FillEllipse(brush, 0, 0, dot.Width - 1, dot.Height - 1);
            };
            dot.Click += (_, _) => DotClicked?.Invoke(pageIdx);
            _dotsPanel.Controls.Add(dot);
            _dots.Add(dot);
        }
    }

    public void UpdateButtons(bool canPrev, bool canNext)
    {
        _prevButton.Enabled = canPrev;
        _nextButton.Enabled = canNext;
    }
}
