using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MasterEngine.Graphic;


namespace MasterEngine.Runtime;
public partial class App : Avalonia.Application{
    public override void Initialize(){
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted(){
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new RuntimeWindow(GraphicAPI.OpenGL);

        base.OnFrameworkInitializationCompleted();
    }
}