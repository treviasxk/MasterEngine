using System.Drawing;
using Avalonia.Threading;
using MasterEngine.Graphic;
using MasterEngine.Runtime;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class OpenGL : GraphicComponent {
    public GL? GL {get;set;}
    public OpenGL(){
        var options = WindowOptions.Default;
        options.FramesPerSecond = Application.FrameRate;
        options.WindowBorder = WindowBorder.Hidden;
        options.IsVisible = false;
        Window = Silk.NET.Windowing.Window.Create(options);
        Window.Load += Load;
        Window.Update += Update;
        Window.Render += Render;
        Window.Closing += Closing;
        new Thread(()=>Dispatcher.UIThread.Invoke(Window.Run)){IsBackground = true}.Start();
    }


    private unsafe void Load(){
       if(!IsClosing){
            Init();
            GL = GL.GetApi(Window);
            GL.ClearColor(Color.CornflowerBlue);
            OnLoad?.Invoke();
            Console.WriteLine("Graphic API OpenGL initialized!");
       }
    }

    private void Update(double deltaTime){
        if(!IsClosing){ // Stop Thread Safe
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private unsafe void Render(double deltaTime){
        if(!IsClosing){ // Stop Thread Safe
            OnRender?.Invoke(deltaTime);
            GL?.Clear(ClearBufferMask.ColorBufferBit);
        }
    }

    private void Closing(){
        GL?.Dispose();
    }
}