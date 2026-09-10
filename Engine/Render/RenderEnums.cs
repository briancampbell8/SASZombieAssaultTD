// =====================================================================================================
//  FILE: RenderEnums.cs
//  PATH: Engine/Animation/AnimationEnums.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Defines All enumerations used by the Render Pipeline
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
namespace SASZombieAssaultTD.Engine.Render
{
    public class RenderEnums
    {
        //-----------------------------------------
        // Render Target Usage
        //-----------------------------------------
        public enum RenderTargetUsage : byte
        {
            Color,
            Depth,
            DepthStencil,
            PostProcess,
            UIOverlay
        }

        /// <summary>
        /// Enumerates backend GPU command types.
        /// </summary>
        //-----------------------------------------
        // Enum: Backend Command Type
        public enum BackendCommandType : byte
        {
            SetRenderTarget,
            Clear,
            DrawQuad,
            DrawText,
            SetScissor
        }

        /// <summary>
        /// Supported image formats for deterministic decode.
        /// </summary>
        //--------------------------------------------------
        // Enum: Image Format
        //--------------------------------------------------
        public enum ImageFormat : byte
        {
            Png = 0,
            Jpg = 1,
            Tga = 2,
            Gif = 3,
            Bmp = 4
        }

        /// <summary>
        /// Result classification for merge operations.
        /// </summary>
        /// 
        //--------------------------------------------------
        // Enum: Merge Result
        //--------------------------------------------------
        public enum MergeResult : byte
        {
            Merge = 0,
            Skip = 1
        }
        //--------------------------------------------------
        // Enum: Color Write Mask Flags
        //--------------------------------------------------
        public enum ColorWriteMaskFlags
        {

            All = 0,
            Red = 1,
        }

        // XNA/MonoGame compatibility enums
        //--------------------------------------------------
        // Enum: Sprite Sort Mode
        //--------------------------------------------------
        public enum SpriteSortMode
        {
            Deferred,
            Immediate,
            Texture,
            BackToFront,
            FrontToBack
        }
        //--------------------------------------------------
        // Enum: Sprite Effects
        //--------------------------------------------------
        public enum SpriteEffects
        {
            None,
            FlipHorizontally,
            FlipVertically
        }
        //--------------------------------------------------
        // Enum: Renderer Backend
        //--------------------------------------------------
        public enum RendererBackend
        {
            D3D11,
            BGFX
        }

        //--------------------------------------------------
        // Enum: Render Command Type
        //--------------------------------------------------
        public enum RenderCommandType
        {
            SetRenderTarget,
            Clear,
            DrawQuad,
            DrawText,
            SetScissor
        }

        //--------------------------------------------------
        // Enum: Transparency Support
        //--------------------------------------------------
        public enum TransparencySupport : byte
        {
            None,
            AlphaBlend,
            PremultipliedAlpha,
            Additive
        }

        //--------------------------------------------------
        // Enum: Blending Support
        //--------------------------------------------------
        public enum BlendingSupport : byte
        {
            None,
            Basic,
            Advanced
        }

        //--------------------------------------------------
        // Enum: Depth Support
        //--------------------------------------------------

        public enum DepthSupport : byte
        {
            None,
            Depth16,
            Depth24,
            Depth32F
        }

        //---------------------------------------------------
        // Enum: Texture 2D Flags
        //---------------------------------------------------
        public enum Texture2DFlags : byte
        {
            None = 0x00,
            RenderTarget = 0x01,
            DepthStencil = 0x02
        }

        public enum RenderFeatures
        {
            None = 0,
            AllowViewport = 1 << 0,
            AllowClear = 1 << 1,
            AllowTexture = 1 << 2,
            AllowMesh = 1 << 3,
            AllowPresent = 1 << 4
        }
        public enum RenderMode
        {
            UI,
            World,
            HUD,
            Finalizer,
            Debug
        }
        public enum RenderPhase
        {
            BeginFrame,
            Draw,
            EndFrame
        }
        public enum RenderQualityPreset
        {
            Low,
            Medium,
            High,
            Ultra
        }

    }
}
