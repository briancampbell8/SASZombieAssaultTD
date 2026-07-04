/*
File:    RenderPipelineConfig.cs
Author:  BDC
Created: 2026-02-17

Purpose:
Configuration settings for the rendering pipeline.

Notes:
Centralizes all rendering-related configuration options.
Supports runtime modification and persistence.

*/
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Centralized configuration for rendering pipeline settings.
    ///</summary>
    public sealed class RenderPipelineConfig
    {
        //Frame and Display Settings
        public int TargetFrameRate { get; set; } = 60;
        public bool VSyncEnabled { get; set; } = true;
        public bool Fullscreen { get; set; } = false;
        public int ResolutionWidth { get; set; } = 1920;
        public int ResolutionHeight { get; set; } = 1080;

        //Quality Settings
        public int TextureFilterQuality { get; set; } = 2; //0=Point, 1=Linear, 2=Anisotropic
        public int AntiAliasingLevel { get; set; } = 0; //0=Off, 2=2x, 4=4x, 8=8x
        public bool EnableShadows { get; set; } = false;
        public int ShadowQuality { get; set; } = 1; //0=Low, 1=Medium, 2=High

        //Performance Settings
        public int MaxDrawCallsPerFrame { get; set; } = 10000;
        public int MaxTextureSize { get; set; } = 4096;
        public bool EnableBatching { get; set; } = true;
        public int BatchSize { get; set; } = 1000;

        //Debug Settings
        public bool ShowDebugOverlay { get; set; } = false;
        public bool ShowRenderBounds { get; set; } = false;
        public bool ShowFPS { get; set; } = true;
        public bool EnablePerformanceLogging { get; set; } = false;

        //Color Settings
        public Color ClearColor { get; set; } = Color.Black;
        public Color DebugTextColor { get; set; } = Color.White;
        public Color DebugOverlayColor { get; set; } = Color.FromArgb(128, 0, 0, 0);

        //Caching Settings
        public int TextureCacheSizeMB { get; set; } = 256;
        public int FontCacheSize { get; set; } = 100;
        public bool EnableTextureCompression { get; set; } = true;

        //Validation Settings
        public bool ValidateTexturesOnLoad { get; set; } = true;
        public bool ValidateShadersOnLoad { get; set; } = true;
        public bool LogRenderErrors { get; set; } = true;

        ///<summary>
        ///Applies a preset configuration.
        ///</summary>
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

        ///<summary>
        ///Validates all configuration settings.
        ///</summary>
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

        private void ApplyLowQualityPreset()
        {
            TextureFilterQuality = 0; //Point
            AntiAliasingLevel = 0; //Off
            EnableShadows = false;
            MaxTextureSize = 1024;
            EnableBatching = true;
            BatchSize = 500;
        }

        private void ApplyMediumQualityPreset()
        {
            TextureFilterQuality = 1; //Linear
            AntiAliasingLevel = 2; //2x
            EnableShadows = false;
            MaxTextureSize = 2048;
            EnableBatching = true;
            BatchSize = 1000;
        }

        private void ApplyHighQualityPreset()
        {
            TextureFilterQuality = 2; //Anisotropic
            AntiAliasingLevel = 4; //4x
            EnableShadows = true;
            ShadowQuality = 1; //Medium
            MaxTextureSize = 4096;
            EnableBatching = true;
            BatchSize = 1000;
        }

        private void ApplyUltraQualityPreset()
        {
            TextureFilterQuality = 2; //Anisotropic
            AntiAliasingLevel = 8; //8x
            EnableShadows = true;
            ShadowQuality = 2; //High
            MaxTextureSize = 8192;
            EnableBatching = false; //Disable for maximum quality
            BatchSize = 500;
        }
    }

    ///<summary>
    ///Predefined quality presets for rendering.
    ///</summary>
    public enum RenderQualityPreset
    {
        Low,
        Medium,
        High,
        Ultra
    }
}




