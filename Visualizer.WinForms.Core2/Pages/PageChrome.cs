using ResoEngine.Visualizer.Controls;
using ResoEngine.Visualizer.Core;
using SkiaSharp;

namespace ResoEngine.Visualizer.Pages;

internal static class PageChrome
{
    public static void PositionTopRightPanel(SkiaCanvas? canvasHost, Panel? panel, int margin = 18)
    {
        if (canvasHost == null || panel == null)
        {
            return;
        }

        panel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        panel.Location = new Point(
            Math.Max(12, canvasHost.ClientSize.Width - panel.Width - margin),
            margin);
        panel.BringToFront();
    }

    public static void DrawWrappedText(SKCanvas canvas, string text, float x, ref float y, float width, SKPaint paint)
    {
        foreach (var line in WrapText(text, width, paint))
        {
            canvas.DrawText(line, x, y, paint);
            y += paint.TextSize + 5f;
        }
    }

    public static IReadOnlyList<string> WrapText(string text, float width, SKPaint paint)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = string.Empty;

        foreach (var word in words)
        {
            var candidate = string.IsNullOrEmpty(current) ? word : $"{current} {word}";
            if (paint.MeasureText(candidate) <= width)
            {
                current = candidate;
            }
            else
            {
                if (!string.IsNullOrEmpty(current))
                {
                    lines.Add(current);
                }

                current = word;
            }
        }

        if (!string.IsNullOrEmpty(current))
        {
            lines.Add(current);
        }

        return lines;
    }

    public static void DrawRuler(
        SKCanvas canvas,
        CoordinateSystem coords,
        decimal minValue,
        decimal maxValue,
        float axisY,
        float top,
        float bottom,
        SKPaint rulerLinePaint,
        SKPaint zeroAxisPaint,
        SKPaint topTickPaint,
        SKPaint bottomTickPaint,
        SKPaint topTickTextPaint,
        SKPaint bottomTickTextPaint,
        int labelEvery = 2)
    {
        float innerLeft = coords.MathToPixel((float)minValue, 0f).X;
        float innerRight = coords.MathToPixel((float)maxValue, 0f).X;
        float zeroX = coords.MathToPixel(0f, 0f).X;

        canvas.DrawLine(zeroX, top, zeroX, bottom, zeroAxisPaint);
        canvas.DrawLine(innerLeft, axisY, innerRight, axisY, rulerLinePaint);

        int start = (int)decimal.Floor(minValue);
        int end = (int)decimal.Ceiling(maxValue);
        for (int tick = start; tick <= end; tick++)
        {
            float x = coords.MathToPixel(tick, 0f).X;
            canvas.DrawLine(x, axisY - 7f, x, axisY, topTickPaint);
            canvas.DrawLine(x, axisY, x, axisY + 7f, bottomTickPaint);

            if (labelEvery > 0 && tick % labelEvery == 0)
            {
                canvas.DrawText(FormatRealTick(tick), x, axisY - 12f, topTickTextPaint);
                canvas.DrawText(FormatImaginaryTick(tick), x, axisY + 20f, bottomTickTextPaint);
            }
        }
    }

    public static string FormatRealTick(int value) =>
        value > 0 ? $"+{value}" : value.ToString();

    public static string FormatImaginaryTick(int value)
    {
        int display = -value;
        string sign = display > 0 ? "+" : string.Empty;
        return $"{sign}{display}i";
    }
}
