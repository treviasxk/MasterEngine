using Avalonia.Threading;
using MasterEngine.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D12;
using Silk.NET.DXGI;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class Direct3D12 : GraphicComponent {
    public DXGI? DXGI {get;set;}
    public D3D12? D3D12 {get;set;}
    private ComPtr<ID3D12Debug> DebugController;
    public D3DCompiler? Compiler {get;set;}
    private ComPtr<IDXGIAdapter1> HardwareAdapters;
    private ComPtr<IDXGIFactory> Factory;
    ComPtr<ID3D12Device> Device = default;
    private ComPtr<ID3D12CommandQueue> CommandQueue;
    public Direct3D12(){
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

    private unsafe void Load(){
       if(!IsClosing){
            SetParent();
            OnLoad?.Invoke();
            
            DXGI = DXGI.GetApi(Window);
            D3D12 = D3D12.GetApi();
            Compiler = D3DCompiler.GetApi();


            uint dxgiFactoryFlags = 0;

#if DEBUG
            SilkMarshal.ThrowHResult(
                D3D12.GetDebugInterface(out DebugController)
            );
            DebugController.EnableDebugLayer();
            dxgiFactoryFlags |= 0x01;
#endif

            SilkMarshal.ThrowHResult(
                DXGI.CreateDXGIFactory2(dxgiFactoryFlags, out Factory)
            );

            Console.WriteLine("Graphic API DirectX11 initialized!");
       }
    }

    private unsafe ID3D12Device* GetDevice(){
        ComPtr<IDXGIAdapter1> adapter = default;
        ID3D12Device* device = default;

        for(uint i = 0; Factory.EnumAdapters(i, ref adapter) != 0x887A0002; i++){
            AdapterDesc1 desc = default;
            adapter.GetDesc1(ref desc);

            if((desc.Flags & (uint)AdapterFlag.Software) != 0){
                continue;
            }

            if(SupportsRequiredDirect3DVersion(adapter)) break;
        }

        var device_iid = ID3D12Device.Guid;
        IDXGIAdapter1* hardwareAdapters = adapter.Detach();
        HardwareAdapters = hardwareAdapters;
        SilkMarshal.ThrowHResult(
            D3D12!.CreateDevice((IUnknown*)hardwareAdapters, D3DFeatureLevel.Level110, &device_iid, (void**)&device)
        );

        return device;
    }

    private unsafe void CreateCommandQueue(){
        CommandQueueDesc commandQueueDesc = new CommandQueueDesc();
        commandQueueDesc.Flags = CommandQueueFlags.None;
        commandQueueDesc.Type = CommandListType.Direct;
        void* commandQueue = null;
        var iid = ID3D12CommandQueue.Guid;
        SilkMarshal.ThrowHResult(
            Device.CreateCommandQueue(&commandQueueDesc, ref iid, &commandQueue)
        );
        CommandQueue = (ID3D12CommandQueue*)commandQueue;
    }

    private void CreateSwapChain(){
        SwapChainDesc1 swapChainDesc = new(){
            Width = (uint)Window.FramebufferSize.X,
            Height = (uint)Window.FramebufferSize.Y,
            Format = Format.FormatR8G8B8A8Unorm,
            SampleDesc = new SampleDesc(1, 0),
            BufferUsage = DXGI.UsageRenderTargetOutput,
            BufferCount = 2,
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            //Flags = (uint)SwapChainFlag.AllowModeSwitch,
            AlphaMode = AlphaMode.Ignore
        };

        SwapChainFullscreenDesc swapChainFullscreenDesc = new()
        {
            RefreshRate = new Rational(0, 1),
            ScanlineOrdering = ModeScanlineOrder.Unspecified,
            Scaling = ModeScaling.Unspecified,
            Windowed = true
        };


    }

    private unsafe bool SupportsRequiredDirect3DVersion(IDXGIAdapter1* adapter1){
        var iid = ID3D12Device.Guid;
        return HResult.IndicatesSuccess(D3D12!.CreateDevice((IUnknown*)adapter1, D3DFeatureLevel.Level110, &iid, null));
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