// ====================================================================================================
//  FILE: RenderContextD3D11Adapter.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: RenderContextD3D11Adapter.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

/*
File:    RenderContextD3D11Adapter.cs
Folder:  Engine/Rendering/
Purpose: Minimal D3D11-backed implementation of IRenderContext with NI-instrumented stubs.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;

//
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Stub implementation of IRenderTarget for RenderContextD3D11Adapter.
    ///BGFX is quarantined, so this provides a minimal non-BGFX implementation.
    ///</summary>
    internal class StubRenderTarget : IRenderTarget
    {
        public int Width => 1920;
        public int Height => 1080;
        public PixelFormat Format => PixelFormat.R8G8B8A8;

        public string Name => NI.Hit<string>();

        public void Dispose()
        {
            NI.Hit();
        }

        public byte[] GetTextureData() => new byte[Width * Height * 4];
    }

    ///<summary>
    ///Adapter to make RenderContextD3D11 compatible with IRenderContext interface.
    ///Provides minimal, NI-instrumented implementations for all required members.
    ///</summary>
    public class RenderContextD3D11Adapter : IRenderContext
    {
        private readonly D3D11.RenderContextD3D11 _inner;
        private Vector3 _viewportSize;

        public RenderContextD3D11Adapter(D3D11.RenderContextD3D11 inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _viewportSize = new Vector3(1920f, 1080f, 0f);
        }

        //--------------------------------------------------------------------
        // IRenderContext implementation
        //--------------------------------------------------------------------

        public void Clear(Color color)
        {
            _inner.SetClearColor(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f);
        }

        public void Clear(float r, float g, float b, float a)
        {
            _inner.SetClearColor(r, g, b, a);
        }

        public void DrawLine(int x, Vector3 start, Vector3 end, Color color, float thickness = 1.0f)
        {
            NI.Hit();
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            NI.Hit();
        }

        public void DrawRectangle(Rectangle rect, Color color, float thickness = 1.0f)
        {
            NI.Hit();
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            NI.Hit();
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            NI.Hit();
        }

        public void FillRectangle(Rectangle rect, Color color)
        {
            NI.Hit();
        }

        public void DrawCircle(Vector3 center, float radius, Color color, float thickness = 1.0f)
        {
            NI.Hit();
        }

        public void DrawCircle(float x, float y, float radius, Color color)
        {
            NI.Hit();
        }

        public void FillCircle(Vector3 center, float radius, Color color)
        {
            NI.Hit();
        }

        public void DrawText(string text, Vector3 position, Color color, float size = 12.0f)
        {
            NI.Hit();
        }

        public void DrawText(string text, float x, float y, float size, Color color)
        {
            NI.Hit();
        }

        public void DrawText(string text, int x, int y, int size, Color color)
        {
            NI.Hit();
        }

        public Vector3 MeasureText(string text, float size = 12.0f)
        {
            return NI.Hit<Vector3>();
        }

        public void ClearScreen()
        {
            //Default to black clear
            Clear(Color.Black);
        }

        public void Present()
        {
            //If inner has an explicit Present, call it; otherwise NI.
            NI.Hit();
        }

        public Vector3 ViewportSize
        {
            get => _viewportSize;
            set => _viewportSize = value;
        }

        public void BeginBatch()
        {
            NI.Hit();
        }

        public void EndBatch()
        {
            NI.Hit();
        }

        public void Initialize()
        {
            //Inner context is assumed to be initialized by its owner.
            //No-op here.
        }

        public void Shutdown()
        {
            _inner.Dispose();
        }

        public void DrawTexture(object texture, Vector3 position, Color color)
        {
            NI.Hit();
        }

        public void DrawSprite(object texture, float x, float y, float width, float height, Color color)
        {
            NI.Hit();
        }

        public void DrawSprite(object texture, float x, float y, Color color)
        {
            NI.Hit();
        }

        public void DrawRectangle(Func<float> x, Func<float> y, int width, int height, Color color)
        {
            NI.Hit();
        }

        public void DrawLine(int x1, int y1, int x2, int y2, uint pathColor)
        {
            NI.Hit();
        }

        public void DrawCircle(int x, int y, int radius, uint pathColor)
        {
            NI.Hit();
        }

        public void DrawText(string text, int x, int y)
        {
            NI.Hit();
        }

        public void DrawText(string displayText, int v1, int v2, System.Drawing.Color sysText)
        {
            throw new NotImplementedException();
        }

        public void DrawFilledRectangle(int x, int y, int width, int height, System.Drawing.Color sysFill)
        {
            throw new NotImplementedException();
        }
    }
}
