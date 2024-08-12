using MasterEngine.Graphic;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class Vulkan : GraphicComponent {
    public Vk? VK {get;set;}
    public Vulkan(){
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
            Init();
            OnLoad?.Invoke();
            
            VK = Vk.GetApi();
            Console.WriteLine("Graphic API Vulkan initialized!");
       }
    }

    private void Update(double deltaTime){
        if(!IsClosing){
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private void Closing(){
        VK?.Dispose();
    }
}