//File: IPlatformRenderer.cs
//Interface for platform-specific rendering

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Interface for platform-specific rendering implementations
    ///</summary>
    public interface IPlatformRenderer
    {
        ///<summary>
        ///Initializes the renderer
        ///</summary>
        void Initialize();

        ///<summary>
        ///Renders a frame
        ///</summary>
        void RenderFrame();

        ///<summary>
        ///Sets the viewport size
        ///</summary>
        ///<param name="width">Viewport width</param>
        ///<param name="height">Viewport height</param>
        void SetViewport(int width, int height);

        ///<summary>
        ///Clears the render buffer
        ///</summary>
        void Clear();

        ///<summary>
        ///Gets the renderer capabilities
        ///</summary>
        ///<returns>Renderer capabilities</returns>
        RendererCapabilities GetCapabilities();
    }

    ///<summary>
    ///Describes renderer capabilities
    ///</summary>
    public struct RendererCapabilities
    {
        public bool supportsTransparency;
        public bool supportsBlending;
        public bool supportsDepthBuffer;
        public int maxTextureSize;
        public int maxRenderTargets;
    }
}
