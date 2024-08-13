using System.Runtime.InteropServices;
using Avalonia.Threading;
using MasterEngine.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class Vulkan : GraphicComponent {
    public Vk? VK {get;set;}
    private Instance instance;
    public Vulkan(){
        var options = WindowOptions.DefaultVulkan;
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

    private void Render(double deltaTime){
        OnRender?.Invoke(deltaTime);
    }

    private void Load(){
       if(!IsClosing){
            Init();
            InitializeGraphic();
            OnLoad?.Invoke();
            Console.WriteLine("Graphic API Vulkan initialized!");
       }
    }

    private unsafe void InitializeGraphic(){
        if(Window?.VkSurface is null)
            throw new Exception("Windowing platform doesn't support Vulkan.");

        VK = Vk.GetApi();

        ApplicationInfo appInfo = new(){
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(Application.ProductName),
            ApplicationVersion = new Version32(1, 0, 0),
            PEngineName = (byte*)Marshal.StringToHGlobalAnsi(Application.EngineName),
            EngineVersion = new Version32(1, 0, 0),
            ApiVersion = Vk.Version11
        };

        InstanceCreateInfo createInfo = new(){
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo
        };

        var glfwExtensions = Window.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);

        createInfo.EnabledExtensionCount = glfwExtensionCount;
        createInfo.PpEnabledExtensionNames = glfwExtensions;
        createInfo.EnabledLayerCount = 0;

        if(VK?.CreateInstance(ref createInfo, null, out instance) != Result.Success){
            throw new Exception("failed to create instance!");
        }

        Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
        Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
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