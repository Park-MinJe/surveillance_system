using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Silk.NET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace surveillance_system
{
    public partial class Program
    {
        public class GraphicManager_TriangleTutorial
        {
            private IWindow window;
            private IInputContext input;
            private GL gl;
            private uint program;

            private readonly string VertexShaderSource = @"
            #version 330 core   //Using version GLSL version 3.3
            layout (location = 0) in vec3 vPos;
            layout (location = 1) in vec4 vCol;

            out vec4 outCol;

            void main()
            {
                outCol = vCol;
                gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
            }
            ";

            private readonly string FragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;

            in vec4 outCol;
            
            void main()
            {
                FragColor = outCol;
            }
            ";

            public void graphicTutorial()
            {
                WindowOptions options = WindowOptions.Default;
                options.Title = "Silk Tutorial";
                options.Size = new Vector2D<int>(1280, 720);
                window = Window.Create(options);

                window.Load += OnWindowOnLoad;
                window.Update += OnWindowOnUpdate;
                window.Render += OnWindowOnRender;

                window.Run();
            }

            private void OnWindowOnLoad()
            {
                input = window.CreateInput();
                gl = window.CreateOpenGL();

                foreach(IMouse mouse in input.Mice)
                {
                    mouse.Click += (IMouse cursor, MouseButton button, Vector2 pos) =>
                    {
                        Console.WriteLine("I Clicked!");
                    };
                }

                gl.ClearColor(2.0f, 0.0f, 0.0f, 1.0f);

                uint vshader = gl.CreateShader(ShaderType.VertexShader);
                //uint vshader = gl.CreateShader(GLEnum.VertexShader);
                uint fshader = gl.CreateShader(ShaderType.FragmentShader);

                gl.ShaderSource(vshader, VertexShaderSource);
                gl.ShaderSource(fshader, FragmentShaderSource);
                gl.CompileShader(vshader);
                gl.CompileShader(fshader);

                program = gl.CreateProgram();
                gl.AttachShader(program, vshader);
                gl.AttachShader(program, fshader);
                gl.LinkProgram(program);
                gl.DetachShader(program, vshader);
                gl.DetachShader(program, fshader);
                gl.DeleteShader(vshader);
                gl.DeleteShader(fshader);

            }

            private void OnWindowOnUpdate(double d)
            {
                //NO OPENGL
            }

            private unsafe void OnWindowOnRender(double d)
            {
                //YES OPENGL
                gl.Clear(ClearBufferMask.ColorBufferBit);

                uint vao = gl.GenVertexArray();
                gl.BindVertexArray(vao);

                uint vertices = gl.GenBuffer();
                uint colors = gl.GenBuffer();
                uint indices = gl.GenBuffer();

                float[] vertexArray = new float[]
                {
                    -0.5f, -0.5f, 0.0f,
                    +0.5f, -0.5f, 0.0f,
                    0.0f, +0.5f, 0.0f,
                };

                float[] colorArray = new float[]
                {
                    1.0f, 0.0f, 0.0f, 1.0f,
                    0.0f, 0.0f, 1.0f, 1.0f,
                    0.0f, 1.0f, 0.0f, 1.0f,
                };

                uint[] indexArray = new uint[] { 0, 1, 2 };

                gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
                gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
                gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 0, null);
                gl.EnableVertexAttribArray(0);

                gl.BindBuffer(GLEnum.ArrayBuffer, colors);
                gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
                gl.VertexAttribPointer(1, 4, GLEnum.Float, false, 0, null);
                gl.EnableVertexAttribArray(1);

                gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
                gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

                gl.BindBuffer(GLEnum.ArrayBuffer, 0);
                gl.UseProgram(program);
                gl.DrawElements(GLEnum.Triangles, 3, GLEnum.UnsignedInt, null);

                gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
                gl.BindVertexArray(vao);

                gl.DeleteBuffer(vertices);
                gl.DeleteBuffer(colors);
                gl.DeleteBuffer(indices);
                gl.DeleteBuffer(vao);
            }
        }
    }
}
