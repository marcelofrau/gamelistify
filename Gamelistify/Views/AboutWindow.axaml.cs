using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Gamelistify.Helpers;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class AboutWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly RotateTransform _logoRotate = new();
    private readonly ScaleTransform _logoScale = new();
    private readonly RotateTransform _ringRotate = new();
    private readonly TranslateTransform _versionBob = new();
    private readonly TranslateTransform _marqueeTranslate = new();
    private TimeSpan _elapsed;

    public AboutWindow()
    {
        InitializeComponent();
        DataContext ??= new AboutViewModel();

        var logoTransform = new TransformGroup();
        logoTransform.Children.Add(_logoRotate);
        logoTransform.Children.Add(_logoScale);
        LogoImage.RenderTransform = logoTransform;
        LogoImage.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        RingEllipse.RenderTransform = _ringRotate;
        RingEllipse.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
        VersionText.RenderTransform = _versionBob;
        MarqueePanel.RenderTransform = _marqueeTranslate;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnTick;

        Opened += (_, _) =>
        {
            Logger.Information("AboutWindow opened");
            _timer.Start();
        };
        Closed += (_, _) =>
        {
            Logger.Information("AboutWindow closed");
            _timer.Stop();
        };
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _elapsed += TimeSpan.FromMilliseconds(33);
        double t = _elapsed.TotalSeconds;

        _logoRotate.Angle = 5.0 * Math.Sin(t * 0.9);
        double breathe = 1.0 + 0.05 * Math.Sin(t * 1.7);
        _logoScale.ScaleX = breathe;
        _logoScale.ScaleY = breathe;

        _ringRotate.Angle = (_ringRotate.Angle + 1.2) % 360;
        LogoGlow.Opacity = 0.30 + 0.12 * (0.5 + 0.5 * Math.Sin(t * 1.7));

        _versionBob.Y = -4.0 * Math.Sin(t * 3.2);

        double width = MarqueeText1.DesiredSize.Width;
        if (width <= 0)
        {
            width = 800;
        }

        double x = _marqueeTranslate.X - 1.6;
        if (x <= -width)
        {
            x += width;
        }

        _marqueeTranslate.X = x;

        SetTwinkle(Twinkle1, t, 1.1, 0);
        SetTwinkle(Twinkle2, t, 1.4, 1.2);
        SetTwinkle(Twinkle3, t, 0.9, 2.1);
        SetTwinkle(Twinkle4, t, 1.2, 3.0);
        SetTwinkle(Twinkle5, t, 1.6, 4.2);
        SetTwinkle(Twinkle6, t, 1.0, 5.3);
    }

    private static void SetTwinkle(Avalonia.Controls.Control star, double t, double freq, double phase)
    {
        star.Opacity = 0.10 + 0.55 * (0.5 + 0.5 * Math.Sin(t * freq + phase));
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("AboutWindow close button clicked");
        Close();
    }

    private static void OpenUrl(string url)
    {
        Logger.Information("Opening external URL {Url}", url);
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void OnRepositoryClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl(ProjectInfo.RepositoryUrl);
    }

    private void OnIssuesClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl(ProjectInfo.IssueTrackerUrl);
    }
}
