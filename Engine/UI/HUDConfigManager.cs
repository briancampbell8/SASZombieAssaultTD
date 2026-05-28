/*
Program Name: SASZombieAssaultTD
File Path: Engine/UI/HUDConfigManager.cs
Purpose: Configuration manager for loading, saving, and managing HUD configuration data from JSON.
Features:
  - Loads HUD configuration from JSON file with automatic default config creation
  - Saves HUD configuration to JSON file with formatted output
  - Provides configuration data structures: PanelConfig, ButtonConfig, TextConfig, BackgroundConfig, CrosshairConfig
  - Includes GetPanel, GetButton, GetTextElement, GetBackground getters
  - Includes SetPanel and SetCrosshair setters with automatic save
  - Provides LoadPanel method to load configuration into HUDPanel_Finalizer
  - Uses Engine.Diagnostics.DebugLogger.Trace for deterministic diagnostics on all branches
  - No silent failures, no fallback logic except explicit default config creation
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Diagnostics;
using EngineDiagnostics = SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public static class HUDConfigManager
    {
        private static readonly string ConfigPath = "Assets/UI/hud_config.json";

        private static HUDConfiguration _config;
        private static bool _isLoaded = false;

        // =====================================================================
        // CONFIGURATION DATA STRUCTURES
        // =====================================================================

        public class HUDConfiguration
        {
            public string version { get; set; } = "1.0";

            public Dictionary<string, PanelConfig> panels { get; set; } = new();
            public Dictionary<string, ButtonConfig> buttons { get; set; } = new();
            public Dictionary<string, TextConfig> text_elements { get; set; } = new();
            public Dictionary<string, BackgroundConfig> backgrounds { get; set; } = new();
        }

        public class PanelConfig
        {
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int layer { get; set; }
            public CrosshairConfig crosshair { get; set; }
        }

        public class CrosshairConfig
        {
            public int x { get; set; }
            public int y { get; set; }
            public float centerX { get; set; }
            public float centerY { get; set; }
            public int armLength { get; set; }
            public int thickness { get; set; }
            public System.Drawing.Color color { get; set; }
            public bool useFlash { get; set; }
            public System.Drawing.Color flashColor { get; set; }
        }

        public class ButtonConfig
        {
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string text { get; set; }
        }

        public class TextConfig
        {
            public int x { get; set; }
            public int y { get; set; }
            public string text { get; set; }
            public System.Drawing.Color color { get; set; }
        }

        public class BackgroundConfig
        {
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public System.Drawing.Color color { get; set; }
        }

        // =====================================================================
        // LOAD / SAVE
        // =====================================================================

        public static void Load()
        {
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Load.Start", ConfigPath);

            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    _config = JsonSerializer.Deserialize<HUDConfiguration>(json);

                    Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Load.Complete",
                        $"Version={_config?.version}");
                }
                else
                {
                    Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Load.Missing", "Creating default config");
                    _config = CreateDefaultConfig();
                    Save();
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Load.Error", ex.Message);
                _config = CreateDefaultConfig();
                _isLoaded = true;
            }
        }

        public static void Save()
        {
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Save.Start", ConfigPath);

            try
            {
                if (_config == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Save.Abort", "Config is null");
                    return;
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_config, options);

                File.WriteAllText(ConfigPath, json);

                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Save.Complete", "OK");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.Save.Error", ex.Message);
            }
        }

        private static HUDConfiguration CreateDefaultConfig()
        {
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.CreateDefault.Start", "Creating default HUD config");

            var finalizer = new HUDPanel_Finalizer();

            var cfg = new HUDConfiguration
            {
                panels = new Dictionary<string, PanelConfig>
                {
                    ["cash_panel"] = finalizer.CreatePanelConfig()
                }
            };

            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.CreateDefault.Complete", "OK");
            return cfg;
        }

        // =====================================================================
        // GETTERS
        // =====================================================================

        public static PanelConfig GetPanel(string id)
        {
            EnsureLoaded();
            return _config.panels?.GetValueOrDefault(id);
        }

        public static ButtonConfig GetButton(string id)
        {
            EnsureLoaded();
            return _config.buttons?.GetValueOrDefault(id);
        }

        public static TextConfig GetTextElement(string id)
        {
            EnsureLoaded();
            return _config.text_elements?.GetValueOrDefault(id);
        }

        public static BackgroundConfig GetBackground(string id)
        {
            EnsureLoaded();
            return _config.backgrounds?.GetValueOrDefault(id);
        }

        // =====================================================================
        // SETTERS
        // =====================================================================

        public static void SetPanel(string id, int x, int y, int width, int height, int layer)
        {
            EnsureLoaded();

            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetPanel.Start", id);

            if (!_config.panels.ContainsKey(id))
            {
                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetPanel.Create", id);
                _config.panels[id] = new HUDPanel_Finalizer().CreatePanelConfig();
            }

            var p = _config.panels[id];
            p.x = x;
            p.y = y;
            p.width = width;
            p.height = height;
            p.layer = layer;

            Save();
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetPanel.Complete", id);
        }

        public static void SetCrosshair(
            string panelId,
            int centerX,
            int centerY,
            int armLength,
            int thickness,
            System.Drawing.Color color,
            bool useFlashColor,
            System.Drawing.Color flashColor)
        {
            EnsureLoaded();

            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetCrosshair.Start", panelId);

            if (!_config.panels.ContainsKey(panelId))
            {
                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetCrosshair.CreatePanel", panelId);
                _config.panels[panelId] = new HUDPanel_Finalizer().CreatePanelConfig();
            }

            _config.panels[panelId].crosshair =
                new HUDPanel_Finalizer().CreateCrosshairConfig(
                    centerX, centerY, armLength, thickness, color, useFlashColor, flashColor);

            Save();
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.SetCrosshair.Complete", panelId);
        }

        // =====================================================================
        // LOAD PANEL INTO FINALIZER
        // =====================================================================

        internal static void LoadPanel(string id, HUDPanel_Finalizer finalizer)
        {
            EnsureLoaded();

            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.LoadPanel.Start", id);

            if (!_config.panels.ContainsKey(id))
            {
                Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.LoadPanel.Missing", id);
                return;
            }

            var panel = _config.panels[id];

            // Geometry
            finalizer.X = panel.x;
            finalizer.Y = panel.y;
            finalizer.Width = panel.width;
            finalizer.Height = panel.height;

            // Crosshair
            if (panel.crosshair != null)
            {
                finalizer.CrosshairCenterX = panel.crosshair.x;
                finalizer.CrosshairCenterY = panel.crosshair.y;
                finalizer.CrosshairArmLength = panel.crosshair.armLength;
                finalizer.CrosshairThickness = panel.crosshair.thickness;

                finalizer.CrosshairColor = panel.crosshair.color;
                finalizer.UseFlashColor = panel.crosshair.useFlash;
                finalizer.FlashColor = panel.crosshair.flashColor;
            }

            // Proper diagnostic emission
            Engine.Diagnostics.DebugLogger.Trace("HUDConfigManager.LoadPanel.Complete", id);

        }

        // =====================================================================
        // HELPERS
        // =====================================================================

        private static void EnsureLoaded()
        {
            if (!_isLoaded || _config == null)
                Load();
        }

        public static HUDConfiguration GetFullConfig()
        {
            EnsureLoaded();
            return _config;
        }
    }

    internal class HUDPanel_Finalizer
    {
        internal int X;
        internal int Y;
        internal int Width;
        internal int Height;
        internal int CrosshairCenterX;
        internal int CrosshairCenterY;
        internal int CrosshairArmLength;
        internal int CrosshairThickness;
        internal System.Drawing.Color CrosshairColor;
        internal bool UseFlashColor;
        internal System.Drawing.Color FlashColor;

        public HUDPanel_Finalizer()
        {
        }

        internal HUDConfigManager.CrosshairConfig CreateCrosshairConfig(int centerX, int centerY, int armLength, int thickness, System.Drawing.Color color, bool useFlashColor, System.Drawing.Color flashColor)
        {
            return NI.Hit<HUDConfigManager.CrosshairConfig>();
        }

        internal HUDConfigManager.PanelConfig CreatePanelConfig()
        {
            return NI.Hit<HUDConfigManager.PanelConfig>();
        }
    }
}
