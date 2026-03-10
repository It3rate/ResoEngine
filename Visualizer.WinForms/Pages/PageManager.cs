using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using ResoEngine.Visualizer.Input;

namespace ResoEngine.Visualizer.Pages;

/// <summary>
/// Manages page collection, navigation, and lifecycle.
/// </summary>
public class PageManager
{
    private readonly SkiaCanvas _canvas;
    private readonly PageNavBar _navBar;
    private readonly HitTestEngine _hitTest;
    private readonly List<IVisualizerPage> _pages = [];
    private int _currentIndex = -1;

    public IVisualizerPage? CurrentPage =>
        _currentIndex >= 0 && _currentIndex < _pages.Count ? _pages[_currentIndex] : null;

    public PageManager(SkiaCanvas canvas, PageNavBar navBar, HitTestEngine hitTest)
    {
        _canvas = canvas;
        _navBar = navBar;
        _hitTest = hitTest;

        _navBar.PrevClicked += () => GoTo(_currentIndex - 1);
        _navBar.NextClicked += () => GoTo(_currentIndex + 1);
        _navBar.DotClicked += GoTo;

        _canvas.OnRender += OnRender;
    }

    public void AddPage(IVisualizerPage page)
    {
        _pages.Add(page);
        UpdateNavBar();

        // Auto-navigate to first page
        if (_pages.Count == 1)
            GoTo(0);
    }

    public void GoTo(int index)
    {
        if (index < 0 || index >= _pages.Count) return;
        if (index == _currentIndex) return;

        // Destroy current page
        if (_currentIndex >= 0 && _currentIndex < _pages.Count)
        {
            _pages[_currentIndex].Destroy();
            _hitTest.Clear();
        }

        _currentIndex = index;

        // Init new page
        _pages[_currentIndex].Init(_canvas.Coords, _hitTest);
        UpdateNavBar();
        _canvas.InvalidateCanvas();
    }

    private void OnRender(SkiaSharp.SKCanvas canvas)
    {
        CurrentPage?.Render(canvas);
    }

    private void UpdateNavBar()
    {
        _navBar.UpdateDots(_pages.Count, _currentIndex);
        _navBar.UpdateButtons(_currentIndex > 0, _currentIndex < _pages.Count - 1);
    }
}
