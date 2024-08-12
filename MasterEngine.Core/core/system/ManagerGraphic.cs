using System;
using Avalonia.Controls;
using Avalonia.Platform;
using MasterEngine.Core.Graphic;

namespace MasterEngine.Graphic{
    public enum GraphicAPI {DirectX11, DirectX12, Vulkan, OpenGL}
    public enum Platform {Windows, Linux, Mac, Android, Unknown}
    public class ManagerGraphic : NativeControlHost, IDisposable {
        public GraphicAPI API {get;}
        public GraphicComponent? BackEndGraphic {get; set;}
        public ContentControl ContentControl {get;}
        public Platform Platform {get;}
        public IPlatformHandle? View {get;set;}

        public ManagerGraphic(ContentControl contentControl, GraphicAPI graphicAPI){
            Console.WriteLine("Checking graphics...");
            ContentControl = contentControl;
            API = graphicAPI;
            Platform = CheckPlatform();
            InitializeGraphic(API);
            ContentControl.SizeChanged += OnSizeChanded;
            UpdateSize();
        }

        void UpdateSize(){
            if(BackEndGraphic != null && BackEndGraphic.Window != null){
                BackEndGraphic.Window.Position = new Silk.NET.Maths.Vector2D<int>(0,0);
                BackEndGraphic.Window.Size = new Silk.NET.Maths.Vector2D<int>((int)ContentControl.Width, (int)ContentControl.Height);
            }
        }

        void InitializeGraphic(GraphicAPI api){
            Console.WriteLine("Initializing {0}...", api);
            switch(api){
                case GraphicAPI.OpenGL:
                    BackEndGraphic = new OpenGL(Platform);
                break;
                case GraphicAPI.Vulkan:
                    BackEndGraphic = new Vulkan(Platform);
                break;
                case GraphicAPI.DirectX11:
                    BackEndGraphic = new DirectX11(Platform);
                break;
                case GraphicAPI.DirectX12:
                    BackEndGraphic = new DirectX12(Platform);
                break;
            }
            if(BackEndGraphic != null)
                BackEndGraphic.OnLoad += OnLoad;
        }

        private void OnLoad(){
            if(BackEndGraphic != null)
                ContentControl.Content = new Viewport(BackEndGraphic.Handle, Platform);
        }

        private Platform CheckPlatform(){
            if(OperatingSystem.IsWindows())
                return Platform.Windows;

            if(OperatingSystem.IsLinux())
                return Platform.Linux;

            if(OperatingSystem.IsMacOS())
                return Platform.Mac;

            if(OperatingSystem.IsAndroid())
                return Platform.Android;

            return Platform.Unknown;
        }


        private void OnSizeChanded(object? sender, SizeChangedEventArgs e) => UpdateSize();

        public void Dispose(){
            BackEndGraphic?.Dispose();
        }
    }
}