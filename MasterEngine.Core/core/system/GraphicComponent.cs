using MasterEngine.Runtime;
using Silk.NET.Windowing;

namespace MasterEngine.Graphic;
public class GraphicComponent : IDisposable {
    #pragma warning disable CS8618 // disable warning var null
    public nint Handle {get;set;}
    public IWindow Window {get;set;}
    public Action? OnLoad {get;set;}
    public Action<double> OnUpdate {get;set;}
    public Action<double> OnFixedUpdate {get;set;}
    public Action<double> OnRender {get;set;}
    internal bool IsClosing {get;set;}
    #pragma warning restore CS8618

    public void Init(){
        switch(Application.Platform){
            case Platform.Windows:  // set DXHandle
                Handle = Window.Native!.DXHandle!.Value;
            break;
            case Platform.Linux:  // set X11
                Handle = Window.Native!.X11!.Value.Display;
            break;
        }
    }

    public void Dispose(){
        IsClosing = true;
        Window.Close();
    }
}