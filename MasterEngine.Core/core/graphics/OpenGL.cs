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

    private static uint Vbo;
    private static uint Ebo;
    private static uint Vao;
    private static uint Shader;

    //Vertex shaders are run on each vertex.
    private static readonly string VertexShaderSource = @"
    #version 330 core //Using version GLSL version 3.3
    layout (location = 0) in vec4 vPos;
    
    void main()
    {
        gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
    }
    ";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private static readonly string FragmentShaderSource = @"
    #version 330 core
    out vec4 FragColor;

    void main()
    {
        FragColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);
    }
    ";


    //Vertex data, uploaded to the VBO.
    private static readonly float[] Vertices ={
        //X    Y      Z
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.5f
    };

    //Index data, uploaded to the EBO.
    private static readonly uint[] Indices ={
        0, 1, 3,
        1, 2, 3
    };

    public OpenGL(){
        var options = WindowOptions.Default;
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



    private unsafe void Load(){
       if(!IsClosing){
            Init();
            InitializeGraphic();

            OnLoad?.Invoke();
            Console.WriteLine("Graphic API OpenGL initialized!");
       }
    }

    private unsafe void InitializeGraphic(){
        GL = GL.GetApi(Window);
        GL.ClearColor(Color.CornflowerBlue);

        //Creating a vertex array.
        Vao = GL.GenVertexArray();
        GL.BindVertexArray(Vao);

        //Initializing a vertex buffer that holds the vertex data.
        Vbo = GL.GenBuffer(); //Creating the buffer.
        GL.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
        fixed (void* v = &Vertices[0]){
            GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Initializing a element buffer that holds the index data.
        Ebo = GL.GenBuffer(); //Creating the buffer.
        GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
        fixed (void* i = &Indices[0]){
            GL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Creating a vertex shader.
        uint vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, VertexShaderSource);
        GL.CompileShader(vertexShader);

        //Checking the shader for compilation errors.
        string infoLog = GL.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrWhiteSpace(infoLog)){
            Console.WriteLine($"Error compiling vertex shader {infoLog}");
        }

        //Creating a fragment shader.
        uint fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, FragmentShaderSource);
        GL.CompileShader(fragmentShader);

        //Checking the shader for compilation errors.
        infoLog = GL.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling fragment shader {infoLog}");
        }

        //Combining the shaders under one shader program.
        Shader = GL.CreateProgram();
        GL.AttachShader(Shader, vertexShader);
        GL.AttachShader(Shader, fragmentShader);
        GL.LinkProgram(Shader);

        //Checking the linking for errors.
        GL.GetProgram(Shader, GLEnum.LinkStatus, out var status);
        if(status == 0){
            Console.WriteLine($"Error linking shader {GL.GetProgramInfoLog(Shader)}");
        }

        //Delete the no longer useful individual shaders;
        GL.DetachShader(Shader, vertexShader);
        GL.DetachShader(Shader, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        //Tell opengl how to give the data to the shaders.
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        GL.EnableVertexAttribArray(0);
    }

    private void Update(double deltaTime){
        if(!IsClosing){ // Stop Thread Safe
            OnUpdate?.Invoke(deltaTime);
        }
    }

    private unsafe void Render(double deltaTime){
        if(!IsClosing){ // Stop Thread Safe
            OnRender?.Invoke(deltaTime);
            GL?.Clear(ClearBufferMask.ColorBufferBit);

            //Bind the geometry and shader.
            GL?.BindVertexArray(Vao);
            GL?.UseProgram(Shader);

            //Draw the geometry.
            GL?.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }

    private void FramebufferResize(Vector2D<int> newSize){
        GL?.Viewport(newSize);
    }

    private void Closing(){
        //Remember to delete the buffers.
        GL?.DeleteBuffer(Vbo);
        GL?.DeleteBuffer(Ebo);
        GL?.DeleteVertexArray(Vao);
        GL?.DeleteProgram(Shader);
        GL?.Dispose();
    }
}