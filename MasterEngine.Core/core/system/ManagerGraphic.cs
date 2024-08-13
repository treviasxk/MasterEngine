using Avalonia.Controls;
using MasterEngine.Core.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Input;

namespace MasterEngine.Graphic{
    public enum GraphicAPI {DirectX11, DirectX12, Vulkan, OpenGL, Auto}
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
            if(GraphicComponent != null)
                GraphicComponent.Window.Size = new Silk.NET.Maths.Vector2D<int>((int)Viewport.Bounds.Size.Width, (int)Viewport.Bounds.Size.Height);
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
                    GraphicComponent = new Direct3D11();
                break;
                case GraphicAPI.DirectX12:
                    GraphicComponent = new Direct3D12();
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
            if(GraphicComponent != null)
                GraphicComponent.Window.FramesPerSecond = Application.FrameRate;    // Apply FPS application
        }

        private void OnLoad(){
            if(GraphicComponent != null){
                // Create Inputs
                IInputContext input = GraphicComponent.Window.CreateInput();
                for (int i = 0; i < input.Keyboards.Count; i++)
                    input.Keyboards[i].KeyDown += KeyDown;

                // Create viewport
                Viewport.Content = new ViewNativeControl(GraphicComponent.Handle);
                UpdateSize();
            }
        }

        private void KeyDown(IKeyboard keyboard, Key key, int keyCode){
            if(GraphicComponent != null && key == Key.Escape){
                //Dispose();
            }
        }


        private void OnSizeChanded(object? sender, SizeChangedEventArgs e) => UpdateSize();

        public void Dispose(){
            GraphicComponent?.Dispose();
        }
    }
}