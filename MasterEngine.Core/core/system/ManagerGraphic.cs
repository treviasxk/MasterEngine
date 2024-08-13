using Avalonia.Controls;
using MasterEngine.Core.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Input;

namespace MasterEngine.Graphic{
    public enum GraphicAPI {Auto, Direct3D11, Direct3D12, Vulkan, OpenGL}
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
            GraphicComponent!.Window.Size = new Silk.NET.Maths.Vector2D<int>((int)Viewport.Bounds.Size.Width, (int)Viewport.Bounds.Size.Height);
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
                case GraphicAPI.Direct3D11:
                    GraphicComponent = new Direct3D11();
                break;
                case GraphicAPI.Direct3D12:
                    GraphicComponent = new Direct3D12();
                break;
            }

            GraphicComponent!.OnLoad += OnLoad;
            GraphicComponent.OnUpdate += OnUpdate;
            GraphicComponent.OnFixedUpdate += OnFixedUpdate;
        }

        private void OnFixedUpdate(double deltaTime){
            
        }

        private void OnUpdate(double deltaTime){
            GraphicComponent!.Window.FramesPerSecond = Application.FrameRate;    // Apply FPS application
        }

        private void OnLoad(){
                // Create Inputs
            IInputContext input = GraphicComponent!.Window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;

            // Create viewport
            Viewport.Content = new ViewNativeControl(GraphicComponent.Handle);
            UpdateSize();
        }

        private void KeyDown(IKeyboard keyboard, Key key, int keyCode){
            if(key == Key.Escape){
                //Dispose();
            }
        }


        private void OnSizeChanded(object? sender, SizeChangedEventArgs e) => UpdateSize();

        public void Dispose(){
            GraphicComponent?.Dispose();
        }
    }
}