using System;
using System.Threading;
using MasterEngine.Graphic;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class DirectX12 : GraphicComponent {
    public DXGI? DXGI {get;set;}
    public DirectX12(Platform platform){
        Platform = platform;
        var options = WindowOptions.DefaultVulkan;
        options.WindowBorder = WindowBorder.Hidden;
        options.Position = new Vector2D<int>(0,0);
        Window = Silk.NET.Windowing.Window.Create(options);
        Window.Load += Load;
        Window.Update += Update;
        Window.Render += Render;
        Window.Closing += Closing;
        new Thread(Window.Run){IsBackground = true}.Start();
    }

    private void Render(double deltaTime){
        OnRender?.Invoke(deltaTime);
    }

    private void Load(){
       if(!IsClosing){
            SetHandle();
            OnLoad?.Invoke();
            
            DXGI = DXGI.GetApi(Window);
            Console.WriteLine("Graphic API DirectX11 initialized!");
       }
    }

    private void Update(double deltaTime){
        if(!IsClosing){
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private void Closing(){
        DXGI?.Dispose();
    }
}