using System.Drawing;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using MasterEngine.Graphic;
using MasterEngine.Runtime;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace MasterEngine.Core.Graphic;
public class Direct3D11 : GraphicComponent {
    public DXGI? DXGI {get;set;}
    D3D11 D3D11 = null!;
    D3DCompiler compiler = null!;
    ComPtr<IDXGIAdapter> adapter = default;
    ComPtr<IDXGIFactory2> factory = default;
    ComPtr<IDXGISwapChain1> swapchain = default;
    ComPtr<ID3D11Device> device = default;
    ComPtr<ID3D11DeviceContext> deviceContext = default;
    ComPtr<ID3D11Buffer> vertexBuffer = default;
    ComPtr<ID3D11Buffer> indexBuffer = default;
    ComPtr<ID3D11VertexShader> vertexShader = default;
    ComPtr<ID3D11PixelShader> pixelShader = default;
    ComPtr<ID3D11InputLayout> inputLayout = default;
    float[] vertices = {
        //  X      Y      Z
        0.5f,  0.5f,  0.0f,
        0.5f, -0.5f,  0.0f,
        -0.5f, -0.5f,  0.0f,
        -0.5f,  0.5f,  0.5f,
    };

    uint[] indices ={
        0, 1, 3,
        1, 2, 3,
        };
    const string shaderSource = @"
    struct vs_in {
        float3 position_local : POS;
    };

    struct vs_out {
        float4 position_clip : SV_POSITION;
    };

    vs_out vs_main(vs_in input) {
        vs_out output = (vs_out)0;
        output.position_clip = float4(input.position_local, 1.0);
        return output;
    }

    float4 ps_main(vs_out input) : SV_TARGET {
        return float4( 1.0, 0.5, 0.2, 1.0 );
    }
    ";

    public Direct3D11(){
        var options = WindowOptions.DefaultVulkan;
        options.FramesPerSecond = Application.FrameRate;
        options.WindowBorder = WindowBorder.Hidden;
        options.IsVisible = false;
        Window = Silk.NET.Windowing.Window.Create(options);
        Window.Load += Load;
        Window.Update += Update;
        Window.Render += Render;
        Window.FramebufferResize += FramebufferResize;
        Window.Closing += Closing;
        new Thread(()=>Dispatcher.UIThread.Invoke(Window.Run)){IsBackground = true}.Start();
    }

    private void Load(){
       if(!IsClosing){
            Init();
            InitializeGraphic();
            OnLoad?.Invoke();
            Console.WriteLine("Graphic API DirectX11 initialized!");
       }
    }

    private unsafe void InitializeGraphic(){
        const bool forceDxvk = false;

        DXGI = DXGI.GetApi(Window, forceDxvk);
        D3D11 = D3D11.GetApi(Window, forceDxvk);
        compiler = D3DCompiler.GetApi();


        // Create our D3D11 logical device.
        SilkMarshal.ThrowHResult
        (
            D3D11.CreateDevice
            (
                adapter,
                D3DDriverType.Hardware,
                Software: default,
                (uint) CreateDeviceFlag.Debug,
                null,
                0,
                D3D11.SdkVersion,
                ref device,
                null,
                ref deviceContext
            )
        );

        //This is not supported under DXVK 
        //TODO: PR a stub into DXVK for this maybe?
        if(OperatingSystem.IsWindows()){
            // Log debug messages for this device (given that we've enabled the debug flag). Don't do this in release code!
            device.SetInfoQueueCallback(msg => Console.WriteLine(SilkMarshal.PtrToString((nint) msg.PDescription)));
        }

        // Create our swapchain.
        var swapChainDesc = new SwapChainDesc1{
            BufferCount = 2, // double buffered
            Format = Format.FormatB8G8R8A8Unorm,
            BufferUsage = DXGI.UsageRenderTargetOutput,
            SwapEffect = SwapEffect.FlipDiscard,
            SampleDesc = new SampleDesc(1, 0)
        };

        // Create our DXGI factory to allow us to create a swapchain. 
        factory = DXGI.CreateDXGIFactory<IDXGIFactory2>();

        // Create the swapchain.
        SilkMarshal.ThrowHResult(
            factory.CreateSwapChainForHwnd(
                device,
                Window.Native!.DXHandle!.Value,
                in swapChainDesc,
                null,
                ref Unsafe.NullRef<IDXGIOutput>(),
                ref swapchain
            )
        );

        // Create our vertex buffer.
        var bufferDesc = new BufferDesc{
            ByteWidth = (uint) (vertices.Length * sizeof(float)),
            Usage = Usage.Default,
            BindFlags = (uint) BindFlag.VertexBuffer
        };

        fixed (uint* indexData = indices){
            var subresourceData = new SubresourceData
            {
                PSysMem = indexData
            };

            SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, in subresourceData, ref indexBuffer));
        }

        var shaderBytes = System.Text.Encoding.ASCII.GetBytes(shaderSource);

        // Compile vertex shader.
        ComPtr<ID3D10Blob> vertexCode = default;
        ComPtr<ID3D10Blob> vertexErrors = default;
        HResult hr = compiler.Compile(
            in shaderBytes[0],
            (nuint) shaderBytes.Length,
            nameof(shaderSource),
            null,
            ref Unsafe.NullRef<ID3DInclude>(),
            "vs_main",
            "vs_5_0",
            0,
            0,
            ref vertexCode,
            ref vertexErrors
        );

        // Compile pixel shader.
        ComPtr<ID3D10Blob> pixelCode = default;
        ComPtr<ID3D10Blob> pixelErrors = default;
        hr = compiler.Compile(
            in shaderBytes[0],
            (nuint)shaderBytes.Length,
            nameof(shaderSource),
            null,
            ref Unsafe.NullRef<ID3DInclude>(),
            "ps_main",
            "ps_5_0",
            0,
            0,
            ref pixelCode,
            ref pixelErrors
        );

        // Check for compilation errors.
        if(hr.IsFailure){
            if(pixelErrors.Handle is not null){
                Console.WriteLine(SilkMarshal.PtrToString((nint) pixelErrors.GetBufferPointer()));
            }
            hr.Throw();
        }


        // Create vertex shader.
        SilkMarshal.ThrowHResult(
            device.CreateVertexShader(
                vertexCode.GetBufferPointer(),
                vertexCode.GetBufferSize(),
                ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                ref vertexShader
            )
        );
        
        // Create pixel shader.
        SilkMarshal.ThrowHResult(
            device.CreatePixelShader(
                pixelCode.GetBufferPointer(),
                pixelCode.GetBufferSize(),
                ref Unsafe.NullRef<ID3D11ClassLinkage>(),
                ref pixelShader
            )
        );

        // Describe the layout of the input data for the shader.
        fixed (byte* name = SilkMarshal.StringToMemory("POS"))
        {
            var inputElement = new InputElementDesc
            {
                SemanticName = name,
                SemanticIndex = 0,
                Format = Format.FormatB8G8R8A8Unorm,
                InputSlot = 0,
                AlignedByteOffset = 0,
                InputSlotClass = InputClassification.PerVertexData,
                InstanceDataStepRate = 0
            };

            SilkMarshal.ThrowHResult
            (
                device.CreateInputLayout
                (
                    in inputElement,
                    1,
                    vertexCode.GetBufferPointer(),
                    vertexCode.GetBufferSize(),
                    ref inputLayout
                )
            );
        }

        // Clean up any resources.
        vertexCode.Dispose();
        vertexErrors.Dispose();
        pixelCode.Dispose();
        pixelErrors.Dispose();
    }


    private void Update(double deltaTime){
        if(!IsClosing){
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private unsafe void Render(double deltaTime){
        OnRender?.Invoke(deltaTime);

        var vertexStride = 3U * sizeof(float);
        var vertexOffset = 0U;
        Color color = Color.CornflowerBlue;
        float[]? backgroundColour = new[]{color.R / 255f, color.G, color.B / 255f, color.A / 255f};

        // Obtain the framebuffer for the swapchain's backbuffer.
        using var framebuffer = swapchain.GetBuffer<ID3D11Texture2D>(0);
        
        // Create a view over the render target.
        ComPtr<ID3D11RenderTargetView> renderTargetView = default;
        SilkMarshal.ThrowHResult(device.CreateRenderTargetView(framebuffer, null, ref renderTargetView));
        
        // Clear the render target to be all black ahead of rendering.
        deviceContext.ClearRenderTargetView(renderTargetView, ref backgroundColour[0]);
        
        // Update the rasterizer state with the current viewport.
        var viewport = new Viewport(0, 0, Window.FramebufferSize.X, Window.FramebufferSize.Y, 0, 1);
        deviceContext.RSSetViewports(1, in viewport);
        
        // Tell the output merger about our render target view.
        deviceContext.OMSetRenderTargets(1, ref renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());
        
        // Update the input assembler to use our shader input layout, and associated vertex & index buffers.
        deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelist);
        deviceContext.IASetInputLayout(inputLayout);
        deviceContext.IASetVertexBuffers(0, 1, ref vertexBuffer, in vertexStride, in vertexOffset);
        deviceContext.IASetIndexBuffer(indexBuffer, Format.FormatR32Uint, 0);
        
        // Bind our shaders.
        deviceContext.VSSetShader(vertexShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
        deviceContext.PSSetShader(pixelShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
        
        // Draw the quad.
        deviceContext.DrawIndexed(6, 0, 0);
        
        // Present the drawn image.
        swapchain.Present(1, 0);
        
        // Clean up any resources created in this method.
        renderTargetView.Dispose();
    }

    unsafe void FramebufferResize(Vector2D<int> newSize){
        // If the window resizes, we need to be sure to update the swapchain's back buffers.
        SilkMarshal.ThrowHResult(
            swapchain.ResizeBuffers(0, (uint) newSize.X, (uint) newSize.Y, Format.FormatB8G8R8A8Unorm, 0)
        );
    }

    private void Closing(){
        // Clean up any resources.
        factory.Dispose();
        swapchain.Dispose();
        device.Dispose();
        deviceContext.Dispose();
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        vertexShader.Dispose();
        pixelShader.Dispose();
        inputLayout.Dispose();
        compiler.Dispose();
        D3D11.Dispose();
        DXGI?.Dispose();
    }
}