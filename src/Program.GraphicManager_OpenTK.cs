using System;
using System.IO;
using System.Numerics;

/*using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;*/

namespace surveillance_system
{
    public partial class Program
    {
        /*public class GraphicManager_OpneTK:GameWindow
        {
            float[] vertices =
            {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
            };

            Shader 
            int VertexBufferObject;

            public GraphicManager_OpneTK(int width, int height, string title):base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = (width, height),
                Title = title
            })
            { }

            protected override void OnUpdateFrame(FrameEventArgs args)
            {
                base.OnUpdateFrame(args);

                KeyboardState input = KeyboardState;

                if (input.IsKeyDown(Keys.Escape))
                {
                    Close();
                }
            }

            protected override void OnLoad()
            {
                base.OnLoad();

                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

                VertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
            }

            protected override void OnRenderFrame(FrameEventArgs args)
            {
                base.OnRenderFrame(args);

                GL.Clear(ClearBufferMask.ColorBufferBit);

                SwapBuffers();
            }

            protected override void OnResize(ResizeEventArgs e)
            {
                base.OnResize(e);

                GL.Viewport(0, 0, e.Width, e.Height);
            }
        }

        public class Shader
        {
            int Handle;

            public Shader(string vertexPath, string fragmentPath)
            {
                string VertexShaderSource = File.ReadAllText(vertexPath);
                string FragmentShaderSource = File.ReadAllText(fragmentPath);

                int VertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexShader, VertexShaderSource);

                int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentShader, FragmentShaderSource);

                GL.CompileShader(VertexShader);

                GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
                if(success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(VertexShader);
                    Console.WriteLine(infoLog);
                }

                GL.CompileShader(FragmentShader);

                GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
                if(success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(FragmentShader);
                    Console.WriteLine(infoLog);
                }

                Handle = GL.CreateProgram();

                GL.AttachShader(Handle, VertexShader);
                GL.AttachShader(Handle, FragmentShader);

                GL.LinkProgram(Handle);

                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
                if(success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(Handle);
                    Console.WriteLine(infoLog);
                }

                GL.DetachShader(Handle, VertexShader);
                GL.DetachShader(Handle, FragmentShader);
                GL.DeleteShader(FragmentShader);
                GL.DeleteShader(VertexShader);
            }

            public void Use()
            {
                GL.UseProgram(Handle);
            }

            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if(!disposedValue)
                {
                    GL.DeleteProgram(Handle);

                    disposedValue = true;
                }
            }

            ~Shader()
            {
                GL.DeleteProgram(Handle);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }*/
    }
}
