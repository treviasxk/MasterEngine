using System.Drawing;
using Avalonia.Threading;
using MasterEngine.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class OpenGL : GraphicComponent {
    public GL? GL {get;set;}
    public OpenGL(){
        var options = WindowOptions.Default;
        options.FramesPerSecond = Application.FrameRate;
        options.WindowBorder = WindowBorder.Hidden;
        options.Position = new Vector2D<int>(0,0);
        Window = Silk.NET.Windowing.Window.Create(options);
        Window.Load += Load;
        Window.Update += Update;
        Window.Render += Render;
        Window.Closing += Closing;
        new Thread(()=>Dispatcher.UIThread.Invoke(Window.Run)){IsBackground = true}.Start();
    }

    private void Render(double deltaTime){
        OnRender?.Invoke(deltaTime);
        GL?.Clear(ClearBufferMask.ColorBufferBit);
    }

    private void Load(){
       if(!IsClosing){
            Init();
            GL = GL.GetApi(Window);
            GL.ClearColor(Color.CornflowerBlue);
            OnLoad?.Invoke();
            Console.WriteLine("Graphic API OpenGL initialized!");
       }
    }

    private void Update(double deltaTime){
        if(!IsClosing){
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private void Closing(){
        GL?.Dispose();
    }
}