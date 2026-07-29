using Avalonia.Controls;
using Gamelistify.Services;

namespace Gamelistify;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Debug("SplashWindow opened");
        Closed += (_, _) => Logger.Debug("SplashWindow closed");
    }
}
