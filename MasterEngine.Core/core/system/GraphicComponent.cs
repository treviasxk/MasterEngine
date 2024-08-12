using System;
using System.Runtime.InteropServices;
using Silk.NET.Windowing;

namespace MasterEngine.Graphic;
public class GraphicComponent : IDisposable {
    [DllImport("user32.dll")]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    public nint Handle {get;set;}
    public Platform Platform {get;set;}
    public IWindow? Window {get;set;}
    public Action? OnLoad {get;set;}
    public Action<double>? OnUpdate {get;set;}
    public Action<double>? OnRender {get;set;}
    internal bool IsClosing {get;set;}

    public void SetHandle(){
        if(Window != null && Window.Native != null && Window.Native.DXHandle != null){
            switch(Platform){
                case Platform.Windows:  // set DXHandle
                    Handle = Window.Native.DXHandle.Value;
                break;
            }
        }
    }

    public void Dispose(){
        IsClosing = true;
        Window?.Close();
    }
}