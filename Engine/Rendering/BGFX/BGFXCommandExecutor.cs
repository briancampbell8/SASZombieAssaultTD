//
// * File:    BGFXCommandExecutor.cs
// * Path:    Engine/Rendering/BGFX/BGFXCommandExecutor.cs
// * Purpose: Backend command execution - translates high-level commands to GPU operations.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    // =======================================================================
    // DIAGNOSTIC STUBS (FORCE COMPILE)
    // =======================================================================

    internal enum BackendOpType
    {
        SetRenderTarget,
        SetScissor,
        Clear,
        DrawQuad,
        DrawText,
        SetBlendState,
        SetDepthState,
        SetStencilState,
        SetViewport,
        SetTransform,
        SetMaterial,
        SetTexture,
        DrawMesh
    }

    internal class BackendCommand
    {
        public BackendOpType Type;
        public Vector4? ClearColor;
    }

    internal static class BGFXNative
    {
        internal enum ClearFlags : ushort
        {
            Color = 1
        }

        internal static void bgfx_set_view_clear(
            ushort viewId,
            ushort flags,
            uint rgba,
            float depth,
            byte stencil)
        {
            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] bgfx_set_view_clear stub invoked");
        }

        internal static void bgfx_submit(
            ushort viewId,
            IntPtr program,
            int depth,
            bool preserveState)
        {
            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] bgfx_submit stub invoked");
        }
    }

    // internal struct VertexBufferHandle { } Already defined in BGFXResourceCache.cs
    // internal struct IndexBufferHandle { } Already defined in BGFXResourceCache.cs
    // internal struct TextureHandle { } Already defined in BGFXResourceCache.cs
    // internal struct ProgramHandle { } Already defined in BGFXResourceCache.cs

    internal class BGFXState
    {
        public int Width = 1920;
        public int Height = 1080;
    }

    internal class BGFXCommandMap
    { }

    // =======================================================================
    // BGFX COMMAND EXECUTOR
    // =======================================================================

    internal sealed class BGFXCommandExecutor
    {
        /// <summary>
        ///   private readonly BGFXDeviceCore _core;
        /// </summary>
        private readonly BGFXShaderManager _shaders;

        private readonly BGFXResourceCache _resources;

        private BGFXCommandMap _map;
        private BGFXActivationGuard _guard;
        private BGFXState _state;

        private VertexBufferHandle _vertexBuffer;
        private IndexBufferHandle _indexBuffer;
        private TextureHandle _texture;
        private ProgramHandle _shaderProgram;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct QuadVertex
        {
            public Vector3 Position;
            public uint Color;
        }

        private static readonly QuadVertex[] QuadVertices = new[]
        {
            new QuadVertex { Position = new Vector3(-0.5f, -0.5f, 0.0f), Color = 0xFFFFFFFF },
            new QuadVertex { Position = new Vector3( 0.5f, -0.5f, 0.0f), Color = 0xFFFFFFFF },
            new QuadVertex { Position = new Vector3(-0.5f,  0.5f, 0.0f), Color = 0xFFFFFFFF },
            new QuadVertex { Position = new Vector3( 0.5f,  0.5f, 0.0f), Color = 0xFFFFFFFF }
        };

        private static readonly ushort[] QuadIndices = new ushort[]
        {
            0, 1, 2,
            2, 1, 3
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct Vertex
        {
            public Vector3 Position;
            public Vector4 Color;
            public Vector2 UV;
        }

        public BGFXCommandExecutor(
            ///   BGFXDeviceCore core, not used yet but will be needed for actual GPU operations
            BGFXShaderManager shaders,
            BGFXResourceCache resources,
            BGFXActivationGuard guard,
            BGFXState state)
        {
            ///   _core = core; not used yet but will be needed for actual GPU operations
            _shaders = shaders;
            _resources = resources;
            _guard = guard;
            _state = state;

            _map = new BGFXCommandMap();
        }

        public void Execute(IReadOnlyList<BackendCommand> commands)
        {
            if (commands == null || commands.Count == 0) return;

            for (int i = 0; i < commands.Count; i++)
            {
                var op = commands[i];

                switch (op.Type)
                {
                    case BackendOpType.SetRenderTarget: ApplySetRenderTarget(op); break;
                    case BackendOpType.SetScissor: ApplySetScissor(op); break;
                    case BackendOpType.Clear: ApplyClear(op); break;
                    case BackendOpType.DrawQuad: ApplyDrawQuad(op); break;
                    case BackendOpType.DrawText: ApplyDrawText(op); break;
                    case BackendOpType.SetBlendState: ApplySetBlendState(op); break;
                    case BackendOpType.SetDepthState: ApplySetDepthState(op); break;
                    case BackendOpType.SetStencilState: ApplySetStencilState(op); break;
                    case BackendOpType.SetViewport: ApplySetViewport(op); break;
                    case BackendOpType.SetTransform: ApplySetTransform(op); break;
                    case BackendOpType.SetMaterial: ApplySetMaterial(op); break;
                    case BackendOpType.SetTexture: ApplySetTexture(op); break;
                    case BackendOpType.DrawMesh: ApplyDrawMesh(op); break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"[BGFX] Unsupported command type: {op.Type}");
                        break;
                }
            }
        }

        private bool Ready()
        {
            return _guard != null && _guard.IsReady && !_guard.IsInitializing && !_guard.IsShuttingDown;
        }

        private void ApplySetRenderTarget(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetRenderTarget");
        }

        private void ApplySetScissor(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetScissor");
        }

        private void ApplyClear(BackendCommand op)
        {
            if (!Ready()) return;

            uint clearColor = 0xFF303030;

            if (op.ClearColor.HasValue)
            {
                var c = op.ClearColor.Value;
                clearColor =
                    (uint)(((byte)(c.W * 255) << 24) |
                           ((byte)(c.X * 255) << 16) |
                           ((byte)(c.Y * 255) << 8) |
                           (byte)(c.Z * 255));
            }

            BGFXNative.bgfx_set_view_clear(0, (ushort)BGFXNative.ClearFlags.Color, clearColor, 1.0f, 0);
        }

        private void ApplyDrawQuad(BackendCommand op)
        {
            if (!Ready()) return;

            float w = _state.Width * 0.5f;
            float h = _state.Height * 0.5f;

            Matrix4x4 world = Matrix4x4.CreateTranslation(w, h, 0);
            Matrix4x4 view = Matrix4x4.Identity;
            Matrix4x4 proj = Matrix4x4.CreateOrthographicOffCenter(0, _state.Width, _state.Height, 0, -1, 1);

            Matrix4x4 mvp = world * view * proj;

            BGFXNative.bgfx_submit(0, IntPtr.Zero, 0, false);
        }

        private void ApplyDrawText(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplyDrawText");
        }

        private void ApplySetBlendState(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetBlendState");
        }

        private void ApplySetDepthState(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetDepthState");
        }

        private void ApplySetStencilState(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetStencilState");
        }

        private void ApplySetViewport(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetViewport");
        }

        private void ApplySetTransform(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetTransform");
        }

        private void ApplySetMaterial(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetMaterial");
        }

        private void ApplySetTexture(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplySetTexture");
        }

        private void ApplyDrawMesh(BackendCommand op)
        {
            if (!Ready()) return;

            System.Diagnostics.Debug.WriteLine("[DIAGNOSTIC] ApplyDrawMesh");
        }
    }
}
