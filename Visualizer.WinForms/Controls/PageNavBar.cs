namespace ResoEngine.Visualizer.Controls;

/// <summary>
/// Navigation bar with prev/next buttons and clickable dot indicators.
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

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = false,
            Anchor = AnchorStyles.None,
        };

        _prevButton = CreateNavButton("\u2190");
        _prevButton.Click += (_, _) => PrevClicked?.Invoke();

        _dotsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            Margin = new Padding(6, 10, 6, 0),
        };

        _nextButton = CreateNavButton("\u2192");
        _nextButton.Click += (_, _) => NextClicked?.Invoke();

        layout.Controls.Add(_prevButton);
        layout.Controls.Add(_dotsPanel);
        layout.Controls.Add(_nextButton);
        Controls.Add(layout);

        // Center the layout
        layout.Resize += (_, _) => CenterLayout(layout);
        Resize += (_, _) => CenterLayout(layout);
    }

    private void CenterLayout(FlowLayoutPanel layout)
    {
        layout.Left = (Width - layout.PreferredSize.Width) / 2;
        layout.Top = (Height - layout.PreferredSize.Height) / 2;
        layout.Width = layout.PreferredSize.Width;
        layout.Height = layout.PreferredSize.Height;
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
                Width = 10,
                Height = 10,
                Margin = new Padding(4, 5, 4, 0),
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

    private static Button CreateNavButton(string text)
    {
        return new Button
        {
            Text = text,
            Width = 36,
            Height = 36,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12f),
            Cursor = Cursors.Hand,
            BackColor = Color.FromArgb(230, 230, 230),
            Margin = new Padding(4, 2, 4, 0),
        };
    }
}
