using Applied.Geometry.LetterFormation;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Input;
using ResoEngine.Visualizer.Pages;

namespace ResoEngine.Visualizer.Browser;

internal static class Program
{
    private static Task Main(string[] args) =>
        AppBuilder.Configure<App>().StartBrowserAppAsync("out");
}

internal sealed class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new PatternVisualizerView();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

internal sealed class PatternVisualizerView : Grid
{
    private readonly IReadOnlyList<IVisualizerPage> pages =
    [
        new LetterGoalPinsPage(),
        new LetterFormationDynamicsPage(),
        new ResolutionModesPage(),
        new BooleanOpsPage(),
        new OrthogonalBooleanGalleryPage(),
        new ParallelBooleanGalleryPage(),
    ];
    private readonly HitTestEngine hitTest = new();
    private readonly SkiaCanvas canvas;
    private readonly Button animationButton;
    private readonly TextBlock pageLabel;
    private readonly WrapPanel letterControls;
    private readonly DispatcherTimer timer;
    private int currentPageIndex = 1;

    private LetterFormationDynamicsPage LetterPage => (LetterFormationDynamicsPage)pages[1];

    private IVisualizerPage CurrentPage => pages[currentPageIndex];

    public PatternVisualizerView()
    {
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        RowDefinitions.Add(new RowDefinition(GridLength.Star));
        Background = new SolidColorBrush(Color.Parse("#F4F2EB"));

        canvas = new SkiaCanvas(CurrentPage);
        CurrentPage.Init(canvas.Coords, hitTest, canvas);

        var toolbar = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(14, 10),
        };

        toolbar.Children.Add(CreateLabel("PATTERNS", 13, FontWeight.Bold));
        var previousButton = CreateButton("Previous");
        previousButton.Click += (_, _) => GoTo(currentPageIndex - 1);
        toolbar.Children.Add(previousButton);

        pageLabel = CreateLabel(CurrentPage.Title, 15, FontWeight.SemiBold);
        toolbar.Children.Add(pageLabel);

        var nextButton = CreateButton("Next");
        nextButton.Click += (_, _) => GoTo(currentPageIndex + 1);
        toolbar.Children.Add(nextButton);

        letterControls = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
        };

        foreach (LetterFormationPresetKind preset in LetterPage.Presets)
        {
            var presetButton = CreateButton(LetterFormationPresetFactory.GetShortLabel(preset));
            presetButton.Click += (_, _) =>
            {
                LetterPage.SelectPreset(preset);
                canvas.InvalidateCanvas();
            };
            letterControls.Children.Add(presetButton);
        }

        animationButton = CreateButton("Pause");
        animationButton.Click += (_, _) =>
        {
            LetterPage.SetAnimating(!LetterPage.IsAnimating);
            animationButton.Content = LetterPage.IsAnimating ? "Pause" : "Run";
        };
        letterControls.Children.Add(animationButton);

        var stepButton = CreateButton("Step");
        stepButton.Click += (_, _) => LetterPage.Step();
        letterControls.Children.Add(stepButton);

        var resetButton = CreateButton("Reset");
        resetButton.Click += (_, _) =>
        {
            LetterPage.Reset();
            animationButton.Content = "Pause";
        };
        letterControls.Children.Add(resetButton);
        toolbar.Children.Add(letterControls);

        Children.Add(toolbar);
        Grid.SetRow(canvas, 1);
        Children.Add(canvas);

        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(90) };
        timer.Tick += (_, _) =>
        {
            if (CurrentPage == LetterPage && LetterPage.IsAnimating)
            {
                LetterPage.Step();
            }
        };
        timer.Start();
    }

    private void GoTo(int index)
    {
        if (index < 0 || index >= pages.Count || index == currentPageIndex)
        {
            return;
        }

        CurrentPage.Destroy();
        hitTest.Clear();
        currentPageIndex = index;
        canvas.SetPage(CurrentPage);
        CurrentPage.Init(canvas.Coords, hitTest, canvas);
        pageLabel.Text = CurrentPage.Title;
        letterControls.IsVisible = CurrentPage == LetterPage;
        canvas.InvalidateCanvas();
    }

    private static TextBlock CreateLabel(string text, double fontSize, FontWeight weight) =>
        new()
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = weight,
            Foreground = new SolidColorBrush(Color.Parse("#28323A")),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(6, 0, 10, 0),
        };

    private static Button CreateButton(string text) =>
        new()
        {
            Content = text,
            MinWidth = 48,
            MinHeight = 30,
            Padding = new Thickness(10, 4),
            Margin = new Thickness(3, 2),
            Background = new SolidColorBrush(Color.Parse("#FFFDF8")),
            BorderBrush = new SolidColorBrush(Color.Parse("#C9C5BA")),
            BorderThickness = new Thickness(1),
        };
}
