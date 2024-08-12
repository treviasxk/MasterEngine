using MasterEngine.Runtime;
using Silk.NET.Windowing;

namespace MasterEngine.Graphic;
public class GraphicComponent : IDisposable {
    public nint Handle {get;set;}
    public IWindow? Window {get;set;}
    public Action? OnLoad {get;set;}
    public Action<double>? OnUpdate {get;set;}
    public Action<double>? OnFixedUpdate {get;set;}
    public Action<double>? OnRender {get;set;}
    internal bool IsClosing {get;set;}

    public void Init(){
        if(Window != null && Window.Native != null){
            switch(Application.Platform){
                case Platform.Windows:  // set DXHandle
                    if(Window.Native.DXHandle != null)
                        Handle = Window.Native.DXHandle.Value;
                break;
                case Platform.Linux:  // set X11
                    if(Window.Native.X11 != null)
                        Handle = Window.Native.X11.Value.Display;
                break;
            }
        }
    }

    public void Dispose(){
        IsClosing = true;
        Window?.Close();
    }
}