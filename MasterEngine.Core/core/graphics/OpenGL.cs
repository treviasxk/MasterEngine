using System;
using System.Drawing;
using System.Threading;
using Avalonia.Threading;
using MasterEngine.Graphic;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class OpenGL : GraphicComponent {
    public GL? GL {get;set;}
    public OpenGL(Platform platform){
        Platform = platform;
        var options = WindowOptions.Default;
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
    }

    private void Load(){
       if(!IsClosing){
            SetHandle();
            OnLoad?.Invoke();
            GL = GL.GetApi(Window);
            GL.ClearColor(Color.CornflowerBlue);
            Console.WriteLine("Graphic API OpenGL initialized!");
       }
    }

    private void Update(double deltaTime){
        if(!IsClosing){
            OnUpdate?.Invoke(deltaTime);
            GL?.Clear(ClearBufferMask.ColorBufferBit);
        }
    }

    private void Closing(){
        GL?.Dispose();
    }
}