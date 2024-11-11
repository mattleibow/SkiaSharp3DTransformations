using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Runtime.CompilerServices;

namespace ThreeDee;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private IDispatcherTimer? _timer;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            _timer?.Stop();
            _timer = null;
        }
        else
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0);
            _timer.IsRepeating = true;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
#if WINDOWS
        if (skiaView.Handler?.PlatformView is not Microsoft.UI.Xaml.FrameworkElement fe || fe.DispatcherQueue is null)
            return;
#endif

        skiaView.InvalidateSurface();
    }

    SKImage? checkers;
    SKImage Checkers
    {
        get
        {
            if (checkers is not null)
                return checkers;

            using var paint = new SKPaint();
            using var surface = SKSurface.Create(new SKImageInfo(100, 100));
            var canvas = surface.Canvas;
            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    paint.Color =( x + y) % 2 == 0
                        ? SKColors.Silver
                        : SKColors.Gray;
                    canvas.DrawRect(SKRect.Create(x * 10, y * 10, 10, 10), paint);
                }
            }

            checkers = surface.Snapshot();

            return checkers;
        }
    }

    float angle;

    private void OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        angle = (float)e.NewValue;
    }

    SKPaint yPaint = new()
    {
        Color = SKColors.Lime
    };

    private void OnPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;

        canvas.Clear(SKColors.White);

        canvas.Translate(e.Info.Width / 2, e.Info.Height / 2);
        canvas.Scale(3);

        canvas.Concat(SKMatrix44.CreateRotationDegrees(1, 0, 0, angle));
        canvas.Concat(SKMatrix44.CreateRotationDegrees(0, 0, 1, 20));

        
        canvas.Save();
        canvas.Translate(-50,-50);
        canvas.DrawImage(Checkers, 0, 0);
        canvas.Restore();


        canvas.DrawCircle(0, 0, 2, new() { Color = SKColors.Fuchsia });

        var length = 40;

        yPaint.Color = SKColors.Lime;
        canvas.Save();
        canvas.DrawLine(0, 0, 0, length, yPaint);
        canvas.DrawText("Y", -4, length+10, new SKFont(), yPaint);
        canvas.Restore();

        yPaint.Color = SKColors.Red;
        canvas.Save();
        canvas.Concat(SKMatrix44.CreateRotationDegrees(0, 0, 1, -90));
        canvas.DrawLine(0, 0, 0, length, yPaint);
        canvas.DrawText("X", -4, length + 10, new SKFont(), yPaint);
        canvas.Restore();

        yPaint.Color = SKColors.Blue;
        canvas.Save();
        canvas.Concat(SKMatrix44.CreateRotationDegrees(1, 0, 0, -90));
        canvas.DrawLine(0, 0, 0, -length, yPaint);
        canvas.DrawText("Z", -4, -length - 10, new SKFont(), yPaint);
        canvas.Restore();

    }
}

