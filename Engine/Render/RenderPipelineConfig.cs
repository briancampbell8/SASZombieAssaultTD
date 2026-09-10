// ====================================================================================================
//  FILE: RenderPipelineConfig.cs
//  PATH: ./Engine/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Centralized configuration object for the engine’s rendering pipeline.
//      Provides all runtime‑modifiable rendering, quality, performance, caching,
//      debugging, and adapter settings used by the Render subsystem.
//
//  RESPONSIBILITIES:
//      - Provide ApplyPreset() behavior for the Rendering subsystem.
//      - Provide ValidateSettings() behavior for the Rendering subsystem.
//      - Serve as the authoritative configuration surface for PerformanceSettingsValidator.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      This file replaces the duplicate UI/Rendering version.
//      Verified and merged for full compatibility with PerformanceSettingsValidator.
// ====================================================================================================

using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    /// <summary>
    /// Centralized configuration for rendering pipeline settings.
    /// </summary>
    public sealed class RenderPipelineConfig
    {
        // ---------------------------------------------------------------------------------------------
        //  FRAME & DISPLAY SETTINGS
        // ---------------------------------------------------------------------------------------------
        public int TargetFrameRate { get; set; } = 60;
        public bool VSyncEnabled { get; set; } = true;
        public bool Fullscreen { get; set; } = false;
        public int ResolutionWidth { get; set; } = 1920;
        public int ResolutionHeight { get; set; } = 1080;

        // ---------------------------------------------------------------------------------------------
        //  QUALITY SETTINGS
        // ---------------------------------------------------------------------------------------------
        public int TextureFilterQuality { get; set; } = 2; //0=Point, 1=Linear, 2=Anisotropic
        public int AntiAliasingLevel { get; set; } = 0;     //0=Off, 2=2x, 4=4x, 8=8x
        public bool EnableShadows { get; set; } = false;
        public int ShadowQuality { get; set; } = 1;         //0=Low, 1=Medium, 2=High

        // ---------------------------------------------------------------------------------------------
        //  PERFORMANCE SETTINGS
        // ---------------------------------------------------------------------------------------------
        public int MaxDrawCallsPerFrame { get; set; } = 10000;
        public int MaxTextureSize { get; set; } = 4096;
        public bool EnableBatching { get; set; } = true;
        public int BatchSize { get; set; } = 1000;

        // ⭐ REQUIRED BY PerformanceSettingsValidator
        public string ShaderModel { get; set; } = "SM5.0";      // Supported: SM4.0, SM5.0, SM6.0
        public int RenderThreadCount { get; set; } = 4;         // Typical: 2–8
        public string GPUAdapterName { get; set; } = "DefaultAdapter";

        // ---------------------------------------------------------------------------------------------
        //  DEBUG SETTINGS
        // ---------------------------------------------------------------------------------------------
        public bool ShowDebugOverlay { get; set; } = false;
        public bool ShowRenderBounds { get; set; } = false;
        public bool ShowFPS { get; set; } = true;
        public bool EnablePerformanceLogging { get; set; } = false;

        // ---------------------------------------------------------------------------------------------
        //  COLOR SETTINGS
        // ---------------------------------------------------------------------------------------------
        public Color ClearColor { get; set; } = Color.Black;
        public Color DebugTextColor { get; set; } = Color.White;
        public Color DebugOverlayColor { get; set; } = Color.FromArgb(128, 0, 0, 0);

        // ---------------------------------------------------------------------------------------------
        //  CACHING SETTINGS
        // ---------------------------------------------------------------------------------------------
        public int TextureCacheSizeMB { get; set; } = 256;
        public int FontCacheSize { get; set; } = 100;
        public bool EnableTextureCompression { get; set; } = true;

        // ---------------------------------------------------------------------------------------------
        //  VALIDATION FLAGS
        // ---------------------------------------------------------------------------------------------
        public bool ValidateTexturesOnLoad { get; set; } = true;
        public bool ValidateShadersOnLoad { get; set; } = true;
        public bool LogRenderErrors { get; set; } = true;

        // ---------------------------------------------------------------------------------------------
        //  PRESET APPLICATION
        // ---------------------------------------------------------------------------------------------
        public void ApplyPreset(RenderQualityPreset preset)
        {
            switch (preset)
            {
                case RenderQualityPreset.Low:
                    ApplyLowQualityPreset();
                    break;
                case RenderQualityPreset.Medium:
                    ApplyMediumQualityPreset();
                    break;
                case RenderQualityPreset.High:
                    ApplyHighQualityPreset();
                    break;
                case RenderQualityPreset.Ultra:
                    ApplyUltraQualityPreset();
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------
        //  BASIC VALIDATION
        // ---------------------------------------------------------------------------------------------
        public bool ValidateSettings(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (TargetFrameRate <= 0 || TargetFrameRate > 240)
            {
                errorMessage = "TargetFrameRate must be between 1 and 240";
                return false;
            }

            if (ResolutionWidth <= 0 || ResolutionHeight <= 0)
            {
                errorMessage = "Resolution dimensions must be positive";
                return false;
            }

            if (MaxTextureSize <= 0 || (MaxTextureSize & (MaxTextureSize - 1)) != 0)
            {
                errorMessage = "MaxTextureSize must be a positive power of 2";
                return false;
            }

            if (TextureFilterQuality < 0 || TextureFilterQuality > 2)
            {
                errorMessage = "TextureFilterQuality must be between 0 and 2";
                return false;
            }

            return true;
        }

        // ---------------------------------------------------------------------------------------------
        //  PRESET IMPLEMENTATIONS
        // ---------------------------------------------------------------------------------------------
        private void ApplyLowQualityPreset()
        {
            TextureFilterQuality = 0;
            AntiAliasingLevel = 0;
            EnableShadows = false;
            MaxTextureSize = 1024;
            EnableBatching = true;
            BatchSize = 500;
        }

        private void ApplyMediumQualityPreset()
        {
            TextureFilterQuality = 1;
            AntiAliasingLevel = 2;
            EnableShadows = false;
            MaxTextureSize = 2048;
            EnableBatching = true;
            BatchSize = 1000;
        }

        private void ApplyHighQualityPreset()
        {
            TextureFilterQuality = 2;
            AntiAliasingLevel = 4;
            EnableShadows = true;
            ShadowQuality = 1;
            MaxTextureSize = 4096;
            EnableBatching = true;
            BatchSize = 1000;
        }

        private void ApplyUltraQualityPreset()
        {
            TextureFilterQuality = 2;
            AntiAliasingLevel = 8;
            EnableShadows = true;
            ShadowQuality = 2;
            MaxTextureSize = 8192;
            EnableBatching = false;
            BatchSize = 500;
        }
    }
}
