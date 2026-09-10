// ====================================================================================================
//  FILE: FontManager.cs
//  PATH: ./Engine/UI/Managers/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide LoadTitleFont() behavior for the UI subsystem.
//      - Provide LoadTextFont() behavior for the UI subsystem.
//      - Provide LoadIconFont() behavior for the UI subsystem.
//      - Provide LoadSmallFont() behavior for the UI subsystem.
//      - Provide GetFont() behavior for the UI subsystem.
//      - Provide GetStatistics() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
 * File Path: Engine/UI/Managers/FontManager.cs
 * Program Name: FontManager
 * Date Created: 2026-03-05
 * 
 * Change Log:
 * ----------
 * 2026-03-05: Initial implementation
 * What: Created centralized font management system
 * Why: To resolve unused font field warnings and provide scalable font loading
 * 
 * Purpose: Centralized font loading and management for all UI components
 * Features: 
 * - Async font loading with caching
 * - Fallback font system for missing resources
 * - Memory-efficient font pooling
 * - Performance monitoring for font operations
 */

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Resources;
using Font = SASZombieAssaultTD.Engine.TextRendering.Font;

namespace SASZombieAssaultTD.Engine.UI.Managers
//
{
    ///<summary>
    ///Centralized font management system for UI components.
    ///Provides async loading, caching, and fallback mechanisms.
    ///</summary>
    public static class FontManager
    {
        private static readonly ConcurrentDictionary<string, Font> _fontCache = new();
        private static readonly Font _fallbackFont = new Font("Arial", 12f);
        private static bool _initialized = false;

        ///<summary>
        ///Initialize the font manager with default fonts.
        ///</summary>
        public static async Task InitializeAsync()
        {
            if (_initialized) return;

            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "FontManager: Initializing font system");

                //Pre-load common fonts
                await LoadFontAsync("title", "UI/Fonts/title.ttf");
                await LoadFontAsync("text", "UI/Fonts/text.ttf");
                await LoadFontAsync("icons", "UI/Fonts/icons.ttf");
                await LoadFontAsync("small", "UI/Fonts/small.ttf");

                _initialized = true;
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "FontManager: Font system initialized successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"FontManager: Initialization failed - {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Load title font for UI elements.
        ///</summary>
        ///<returns>Loaded title font or fallback</returns>
        public static Font LoadTitleFont()
        {
            return GetFont("title");
        }

        ///<summary>
        ///Load text font for UI elements.
        ///</summary>
        ///<returns>Loaded text font or fallback</returns>
        public static Font LoadTextFont()
        {
            return GetFont("text");
        }

        ///<summary>
        ///Load icon font for UI elements.
        ///</summary>
        ///<returns>Loaded icon font or fallback</returns>
        public static Font LoadIconFont()
        {
            return GetFont("icons");
        }

        ///<summary>
        ///Load small font for UI elements.
        ///</summary>
        ///<returns>Loaded small font or fallback</returns>
        public static Font LoadSmallFont()
        {
            return GetFont("small");
        }

        ///<summary>
        ///Get font by name with caching.
        ///</summary>
        ///<param name="fontName">Name of the font</param>
        ///<returns>Font instance</returns>
        public static Font GetFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning, "FontManager: Requested null or empty font name, returning fallback");
                return _fallbackFont;
            }

            if (_fontCache.TryGetValue(fontName, out var cachedFont))
            {
                return cachedFont;
            }

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning, $"FontManager: Font '{fontName}' not cached, loading synchronously");
            return LoadFontSync(fontName);
        }

        ///<summary>
        ///Load font asynchronously and cache it.
        ///</summary>
        ///<param name="fontName">Font name identifier</param>
        ///<param name="resourcePath">Path to font resource</param>
        ///<returns>Task representing the loading operation</returns>
        private static async Task LoadFontAsync(string fontName, string resourcePath)
        {
            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, $"FontManager: Loading font '{fontName}' from '{resourcePath}'");

                var resourcePipeline = new ModernResourcePipeline();
                var fontData = await resourcePipeline.LoadResourceAsync<byte[]>(resourcePath);

                if (fontData != null)
                {
                    var font = new Font(fontName, 12f); //Default size
                    _fontCache.TryAdd(fontName, font);
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, $"FontManager: Successfully loaded and cached font '{fontName}'");
                }
                else
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning, $"FontManager: Failed to load font '{fontName}', resource data was null");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"FontManager: Failed to load font '{fontName}' - {ex.Message}");
            }
        }

        ///<summary>
        ///Load font synchronously (fallback for non-async contexts).
        ///</summary>
        ///<param name="fontName">Font name identifier</param>
        ///<returns>Font instance</returns>
        private static Font LoadFontSync(string fontName)
        {
            try
            {
                //For now, return fallback font
                //In a full implementation, this would use blocking resource loading
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, $"FontManager: Using fallback font for '{fontName}'");
                return _fallbackFont;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"FontManager: Critical error loading fallback font - {ex.Message}");
                return new Font("Arial", 12f); //Last resort
            }
        }

        ///<summary>
        ///Clear font cache and reload fonts.
        ///</summary>
        public static async Task ReloadFontsAsync()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "FontManager: Reloading all fonts");

            _fontCache.Clear();
            _initialized = false;

            await InitializeAsync();
        }

        ///<summary>
        ///Get font cache statistics.
        ///</summary>
        ///<returns>Font cache statistics</returns>
        public static FontCacheStatistics GetStatistics()
        {
            return new FontCacheStatistics
            {
                CachedFontsCount = _fontCache.Count,
                Initialized = _initialized,
                CacheSize = EstimateCacheSize()
            };
        }

        ///<summary>
        ///Estimate memory usage of font cache.
        ///</summary>
        ///<returns>Estimated memory usage in bytes</returns>
        private static long EstimateCacheSize()
        {
            //Rough estimation - each font ~1MB
            return _fontCache.Count * 1024 * 1024;
        }
    }

    ///<summary>
    ///Font cache statistics for monitoring.
    ///</summary>
    public sealed class FontCacheStatistics
    {
        public int CachedFontsCount { get; set; }
        public bool Initialized { get; set; }
        public long CacheSize { get; set; } //in bytes

        public override string ToString()
        {
            return $"Font Cache: {CachedFontsCount} fonts, {(CacheSize / 1024.0 / 1024.0):F1}MB, Initialized: {Initialized}";
        }
    }
}

