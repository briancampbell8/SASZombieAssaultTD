// =======================================================================================================
// Program: DDCDrawShapes
// Subsystem: Engine.Render.D3D11.DeviceCore
// Component: D3D11 Shapes Program (Lines / Rectangles / Circles)
// Author: Copilot (per BDC architectural guidance)
// Purpose: Provide a dedicated D3D11 program for 2D debug / UI shapes using an orthographic
// projection, with strict, deterministic behavior and full Comment Header + change log.
// -------------------------------------------------------------------------------------------------------
// Dependencies: SharpDX
// SharpDX.Direct3D
// SharpDX.Direct3D11
// SharpDX.D3DCompiler
// SharpDX.DXGI
// System.Math
// SASZombieAssaultTD.Engine.UI.Rendering.ColorRGBA
// -------------------------------------------------------------------------------------------------------
// Change Log:
// 2026-08-03 Copilot Initial creation of DDCDrawShapes program with full header,
// orthographic projection, and line/rectangle/circle support.
// 2026-08-03 Copilot Corrected API usage to SharpDX types, removed Engine.Math references,
// and fully qualified GPU objects to avoid type conflicts.
// =======================================================================================================
using System.Numerics;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Core;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
// Resolve type ambiguity between SharpDX.DXGI and SharpDX.Direct3D11
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDCDrawShapes
    {
        // --------------------------------------------------------------------------------------------------
        // Fields
        // --------------------------------------------------------------------------------------------------
        private readonly Device _device;
        private readonly DeviceContext _context;
        private VertexShader _vertexShader;
        private PixelShader _pixelShader;
        private InputLayout _inputLayout;
        private SharpDX.Direct3D11.Buffer _constantBuffer;
        private SharpDX.Direct3D11.Buffer _vertexBuffer;
        private int _vertexCapacity;
        private float _viewportWidth;
        private float _viewportHeight;
        private object OrthoOffCenterLH;

        // --------------------------------------------------------------------------------------------------
        // GPU Data Structures
        // --------------------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        private struct ShapesConstants
        {
            public Matrix4x4 Projection;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ShapesVertex
        {
            public Vector2 Position;
            public Vector4 Color;
        }

        // --------------------------------------------------------------------------------------------------
        // Construction
        // --------------------------------------------------------------------------------------------------
        public DDCDrawShapes(Device device, DeviceContext context)
        {
            _device = device;
            _context = context;
            _viewportWidth = 1.0f;
            _viewportHeight = 1.0f;
            InitializeProgram();
        }

        // --------------------------------------------------------------------------------------------------
        // Program Initialization
        // --------------------------------------------------------------------------------------------------
        private void InitializeProgram()
        {
            var vsBytecode = ShaderBytecode.FromFile("Shaders/Shapes2D_VS.cso");
            var psBytecode = ShaderBytecode.FromFile("Shaders/Shapes2D_PS.cso");

            _vertexShader = new VertexShader(_device, vsBytecode);
            _pixelShader = new PixelShader(_device, psBytecode);

            _inputLayout = new InputLayout(
                _device,
                vsBytecode,
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 8, 0),
                }
            );

            _constantBuffer = new SharpDX.Direct3D11.Buffer(
                _device,
                Marshal.SizeOf<ShapesConstants>(),
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0
            );

            _vertexCapacity = 0;
            _vertexBuffer = null;
        }

        // --------------------------------------------------------------------------------------------------
        // Viewport / Projection
        // --------------------------------------------------------------------------------------------------
        public void SetViewport(float width, float height)
        {
            _viewportWidth = width;
            _viewportHeight = height;
        }

        private void BindProgram()
        {
            var projection = GetProjection();

            var box = _context.MapSubresource(
                _constantBuffer,
                0,
                MapMode.WriteDiscard,
                MapFlags.None
            );

            unsafe
            {
                var ptr = (ShapesConstants*)box.DataPointer;
                ptr->Projection = projection;
            }

            _context.UnmapSubresource(_constantBuffer, 0);
            _context.InputAssembler.InputLayout = _inputLayout;
            _context.VertexShader.Set(_vertexShader);
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _context.PixelShader.Set(_pixelShader);
        }

        private Matrix4x4 GetProjection()
        {
            return Matrix4x4.CreateOrthographicOffCenter(
                0.0f,
                _viewportWidth,
                _viewportHeight,
                0.0f,
                0.0f,
                1.0f);
        }

        // --------------------------------------------------------------------------------------------------
        // Vertex Buffer Management
        // --------------------------------------------------------------------------------------------------
        private void EnsureVertexBuffer(int requiredVertexCount)
        {
            if (_vertexBuffer != null && requiredVertexCount <= _vertexCapacity)
                return;

            if (_vertexBuffer != null)
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }

            _vertexCapacity = System.Math.Max(requiredVertexCount, 2048);
            int stride = Marshal.SizeOf<ShapesVertex>();

            var desc = new BufferDescription
            {
                SizeInBytes = stride * _vertexCapacity,
                BindFlags = BindFlags.VertexBuffer,
                Usage = ResourceUsage.Dynamic,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            _vertexBuffer = new SharpDX.Direct3D11.Buffer(_device, desc);
        }

        // --------------------------------------------------------------------------------------------------
        // Utility Conversions
        // --------------------------------------------------------------------------------------------------
        private static Vector4 ToVec4(ColorRGBA c)
        {
            return new Vector4(c.R, c.G, c.B, c.A);
        }

        private static ColorRGBA FromHex(uint hex)
        {
            byte r = (byte)((hex >> 16) & 0xFF);
            byte g = (byte)((hex >> 8) & 0xFF);
            byte b = (byte)(hex & 0xFF);
            byte a = (byte)((hex >> 24) & 0xFF);

            // Fixed CS1503: Converted float results to expected parameter types if needed, 
            // but the root issue was missing explicit namespace/struct tracking.
            return new ColorRGBA(
                (byte)(int)(r / 255f),
                (byte)(int)(g / 255f),
                (byte)(int)(b / 255f),
                (byte)(int)(a / 255f));
        }

        // --------------------------------------------------------------------------------------------------
        // Public Shape API – Lines
        // --------------------------------------------------------------------------------------------------
        public void DrawLine(Vector2 start, Vector2 end, ColorRGBA color)
        {
            BindProgram();
            EnsureVertexBuffer(2);

            var box = _context.MapSubresource(
                _vertexBuffer,
                0,
                MapMode.WriteDiscard,
                MapFlags.None
            );

            unsafe
            {
                var ptr = (ShapesVertex*)box.DataPointer;
                var c = ToVec4(color);
                ptr[0].Position = start;
                ptr[0].Color = c;
                ptr[1].Position = end;
                ptr[1].Color = c;
            }

            _context.UnmapSubresource(_vertexBuffer, 0);
            int stride = Marshal.SizeOf<ShapesVertex>();
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            _context.Draw(2, 0);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, uint pathColor)
        {
            var start = new Vector2(x1, y1);
            var end = new Vector2(x2, y2);
            var color = FromHex(pathColor);
            DrawLine(start, end, color);
        }
        // --------------------------------------------------------------------------------------------------
        // Public Shape API – Rectangles
        // --------------------------------------------------------------------------------------------------
        public void DrawRectangle(int x, int y, int width, int height, uint pathColor)
        {
            var color = FromHex(pathColor);
            var tl = new Vector2(x, y);
            var tr = new Vector2(x + width, y);
            var bl = new Vector2(x, y + height);
            var br = new Vector2(x + width, y + height);
            DrawLine(tl, tr, color);
            DrawLine(tr, br, color);
            DrawLine(br, bl, color);
            DrawLine(bl, tl, color);
        }
        // --------------------------------------------------------------------------------------------------
        // Public Shape API – Circles
        // --------------------------------------------------------------------------------------------------
        public void DrawCircle(int cx, int cy, int radius, uint pathColor)
        {
            const int segments = 64;
            var color = FromHex(pathColor);
            double step = 2.0 * System.Math.PI / segments;
            for (int i = 0; i < segments; i++)
            {
                double a0 = i * step;
                double a1 = (i + 1) * step;
                var p0 = new Vector2(
                    cx + radius * (float)System.Math.Cos(a0),
                    cy + radius * (float)System.Math.Sin(a0));
                var p1 = new Vector2(
                    cx + radius * (float)System.Math.Cos(a1),
                    cy + radius * (float)System.Math.Sin(a1));
                DrawLine(p0, p1, color);
            }
        }
    }
}

