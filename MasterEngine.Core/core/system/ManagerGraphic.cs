using Avalonia.Controls;
using MasterEngine.Core.Graphic;
using MasterEngine.Runtime;

namespace MasterEngine.Graphic{
    public enum GraphicAPI {DirectX11, DirectX12, Vulkan, OpenGL}
    public enum Platform {Windows, Linux, Mac, Android, Unknown}
    public class ManagerGraphic : IDisposable {
        public GraphicAPI API {get;}
        public GraphicComponent? GraphicComponent {get; set;}
        public ContentControl Viewport {get;}

        public ManagerGraphic(ContentControl contentControl, GraphicAPI graphicAPI){
            Console.WriteLine("Checking graphics...");
            Viewport = contentControl;
            API = graphicAPI;
            Viewport.SizeChanged += OnSizeChanded;
            InitializeGraphic(API);
        }

        void UpdateSize(){
            if(GraphicComponent != null && GraphicComponent.Window != null){
                GraphicComponent.Window.Position = new Silk.NET.Maths.Vector2D<int>(0,0);
                GraphicComponent.Window.Size = new Silk.NET.Maths.Vector2D<int>((int)Viewport.Width, (int)Viewport.Height);
            }
        }

        void InitializeGraphic(GraphicAPI api){
            Console.WriteLine("Initializing {0}...", api);
            switch(api){
                case GraphicAPI.OpenGL:
                    GraphicComponent = new OpenGL();
                break;
                case GraphicAPI.Vulkan:
                    GraphicComponent = new Vulkan();
                break;
                case GraphicAPI.DirectX11:
                    GraphicComponent = new DirectX11();
                break;
                case GraphicAPI.DirectX12:
                    GraphicComponent = new DirectX12();
                break;
            }
            if(GraphicComponent != null){
                GraphicComponent.OnLoad += OnLoad;
                GraphicComponent.OnUpdate += OnUpdate;
                GraphicComponent.OnFixedUpdate += OnFixedUpdate;
            }
        }

        private void OnFixedUpdate(double deltaTime){
            
        }

        private void OnUpdate(double deltaTime){
            if(GraphicComponent != null && GraphicComponent.Window != null)
                GraphicComponent.Window.FramesPerSecond = Application.FrameRate;    // Apply FPS application
        }

        private void OnLoad(){
            if(GraphicComponent != null)
                Viewport.Content = new Viewport(GraphicComponent.Handle);
        }
        private void OnSizeChanded(object? sender, SizeChangedEventArgs e) => UpdateSize();

        public void Dispose(){
            GraphicComponent?.Dispose();
        }
    }
}