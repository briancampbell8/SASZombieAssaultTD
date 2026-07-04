// ====================================================================================================
//  FILE: HUDManager.cs
//  PATH: ./Engine/UI/
//  MODULE: HUD Manager
//
//  ROLE:
//      Central manager for HUD layout loading, rendering, and state management.
//
//  RESPONSIBILITIES:
//      - Load HUD layout JSON via HUDConfigManager.
//      - Manage HUD textures, text elements, and render order.
//      - Maintain hitboxes for cash/lives increment buttons.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================\n
// ====================================================================================================
//  FILE: HUDManager.cs
//  PATH: ./Engine/UI/
//  MODULE: HUD Manager
//
//  ROLE:
//      Central manager for HUD layout loading, rendering, and state management.
//
//  RESPONSIBILITIES:
//      - Load HUD layout JSON via HUDConfigManager.
//      - Manage HUD textures, text elements, and render order.
//      - Maintain hitboxes for cash/lives increment buttons.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================\n
// ====================================================================================================
//  FILE: HUDManager.cs
//  PATH: Engine/UI/
//  MODULE: HUD Manager
//
//  ROLE:
//      Central manager for HUD layout loading, rendering, and state management. Integrates the new
//      HUDPanel Finalizer pipeline (Manager → Control → ColorParser → Resolver → UIStateBuilder)
//      and coordinates all HUDPanel_* runtime elements.
//
//  RESPONSIBILITIES:
//      - Load HUD layout JSON via HUDConfigManager.
//      - Manage HUD textures, text elements, and render order.
//      - Maintain hitboxes for cash/lives increment buttons.
//      - Update HUD text values based on PlayerSystem state.
//      - Draw HUD elements using IDrawingContext.
//      - Integrate HUDPanelFinalizer_Manager for deterministic CASH panel configuration.
//      - Distribute resolved finalizer colors to HUDPanel_CashUpdate.
//
//  NON-RESPONSIBILITIES:
//      - Finalizer pipeline logic (handled by HUDPanelFinalizer_* modules).
//      - Rendering context creation.
//      - Player state management.
//      - Snapshot persistence.
//
//  NOTES:
//      This file has been fully modernized to remove legacy HUDPanel_Finalizer references and now
//      uses HUDPanelFinalizer_Manager exclusively. All type mismatches, accessibility issues, and
//      rendering context inconsistencies have been resolved.
//
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Player;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Snapshot;
using SASZombieAssaultTD.Engine.UI.HUDPanels;

namespace SASZombieAssaultTD.Engine.UI
{
    public class HUDManager
    {
        private Resources.Texture2D _hudTexture;
        private Resources.Texture2D _supportHUDTexture;

        private HUDLayout _layout;
        private readonly Dictionary<string, HUDTextureElement> _textureDict = new();
        private readonly Dictionary<string, HUDTextElement> _textDict = new();
        private readonly List<IHUDElement> _renderOrder = new();

        private readonly TextureManager _textureManager;

        private Rect cashPlusRect;
        private Rect livesPlusRect;

        public System.Drawing.Color ButtonFlashColor = System.Drawing.Color.Crimson;
        private float _buttonFlashTimer = 0f;
        protected const float BUTTON_FLASH_TIME = 0.10f;

        public static HUDManager Instance { get; private set; }

        // NEW FINALIZER PIPELINE
        private HUDPanelFinalizer_Manager _finalizer;
        private HUDPanel_CashUpdate _cashPanel;

        public HUDManager(TextureManager textureManager)
        {
            _textureManager = textureManager ?? throw new ArgumentNullException(nameof(textureManager));
            Instance = this;
        }

        public HUDPanelFinalizer_Manager GetCashPanelFinalizer()
        {
            return _finalizer;
        }

        // ====================================================================================================
        // LOAD HUD
        // ====================================================================================================
        public void Load(string jsonPath)
        {
            // Load hitboxes from HUDConfigManager
            var cashPanelConfig = HUDConfigManager.GetPanel("cash_panel");
            if (cashPanelConfig != null)
            {
                var cx = cashPanelConfig.crosshair?.centerX ?? 122;
                var cy = cashPanelConfig.crosshair?.centerY ?? 553;
                cashPlusRect = new Rect(cx - 10, cy - 10, 20, 20);
            }
            else
            {
                cashPlusRect = new Rect(112, 543, 20, 20);
            }

            var livesPanelConfig = HUDConfigManager.GetPanel("lives_panel");
            if (livesPanelConfig != null)
            {
                livesPlusRect = new Rect(
                    livesPanelConfig.x + livesPanelConfig.width - 10,
                    livesPanelConfig.y + 5,
                    10, 10
                );
            }
            else
            {
                livesPlusRect = new Rect(230, 550, 10, 10);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                _layout = JsonSerializer.Deserialize<HUDLayout>(json, options) ?? new HUDLayout();
            }
            else
            {
                _layout = new HUDLayout();
            }

            _textureDict.Clear();
            _textDict.Clear();
            _renderOrder.Clear();

            // Load textures
            foreach (var tex in _layout.Images)
            {
                _textureDict[tex.Id] = tex;

                string path = tex.Path;
                if (tex.Id == "hud_surface")
                    path = "Assets/UI/HUD.png";
                else if (tex.Id == "support_hud")
                    path = "Assets/UI/SupportHUD.png";

                tex.Texture = _textureManager.Get(path);

                if (tex.Id == "hud_surface")
                    _hudTexture = (Resources.Texture2D)tex.Texture;
                else if (tex.Id == "support_hud")
                    _supportHUDTexture = (Resources.Texture2D)tex.Texture;
            }

            // Load text elements
            foreach (var txt in _layout.Text)
            {
                _textDict[txt.Id] = txt;

                if (txt.Id == "cash_text")
                    txt.Value = $"{((PlayerState)PlayerSystem.Instance?.State)?.Cash ?? 0}";
                else if (txt.Id == "lives_text")
                    txt.Value = $"{((PlayerState)PlayerSystem.Instance?.State)?.Lives ?? 0}";
                else if (txt.Id == "waves_text")
                    txt.Value = $"1/40";
            }

            // ====================================================================================================
            // CREATE FINALIZER PIPELINE
            // ====================================================================================================
            _cashPanel = new HUDPanel_CashUpdate();
            var control = new HUDPanelFinalizer_Control();
            var parser = new HUDPanelFinalizer_ColorParser();
            var resolver = new HUDPanelFinalizer_Resolver();
            var builder = new HUDPanelFinalizer_UIStateBuilder();

            _finalizer = new HUDPanelFinalizer_Manager(
                this,
                _cashPanel,
                control,
                parser,
                resolver,
                builder
            );

            _finalizer.Initialize();
            _renderOrder.Add(_cashPanel);

            // Build render order
            if (_layout.RenderOrder != null)
            {
                foreach (var id in _layout.RenderOrder)
                {
                    if (_textureDict.TryGetValue(id, out var tex))
                        _renderOrder.Add(tex);
                    else if (_textDict.TryGetValue(id, out var txt))
                        _renderOrder.Add(txt);
                }
            }

            foreach (var tex in _layout.Images.Where(i => !_renderOrder.Contains(i)).OrderBy(i => i.Layer))
                _renderOrder.Add(tex);

            foreach (var txt in _layout.Text.Where(t => !_renderOrder.Contains(t)).OrderBy(t => t.Layer))
                _renderOrder.Add(txt);

            if (!_renderOrder.Contains(_cashPanel))
            {
                _renderOrder.Insert(0, _cashPanel);
            }
            else
            {
                _renderOrder.Remove(_cashPanel);
                _renderOrder.Insert(0, _cashPanel);
            }

            DistributeFinalizerColors();
        }

        // ====================================================================================================
        // UPDATE HUD
        // ====================================================================================================
        public void Update(float deltaTime)
        {
            try
            {
                if (_buttonFlashTimer > 0)
                {
                    _buttonFlashTimer -= deltaTime;
                    if (_buttonFlashTimer <= 0)
                        ResetButtonFlash();
                }

                if (InputSystem.IsMouseButtonJustPressed(0))
                {
                    var mouse = InputSystem.GetMousePosition();
                    var mp = new System.Numerics.Vector2(mouse.X, mouse.Y);

                    PlayerState state = ((PlayerState)PlayerSystem.Instance.State);

                    if (cashPlusRect.Contains(mp))
                    {
                        state.Cash += 1;
                        TriggerButtonFlash();
                        SnapshotIntegration.EnsureTodaySnapshot();
                    }
                    else if (livesPlusRect.Contains(mp))
                    {
                        state.Lives += 1;
                        TriggerButtonFlash();
                        SnapshotIntegration.EnsureTodaySnapshot();
                    }
                }

                PlayerState ps = ((PlayerState)PlayerSystem.Instance.State);

                UpdateText("cash_text", $"${ps.Cash}");
                UpdateText("lives_text", $"{ps.Lives}");
                UpdateText("waves_text", "1/40");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: HUDManager: Update failed: {ex.Message}");
                throw;
            }
        }

        // ====================================================================================================
        // DRAW HUD
        // ====================================================================================================
        public void Draw(IDrawingContext context, int crosshairCenterX = 0, int crosshairCenterY = 0)
        {
            try
            {
                if (_layout == null)
                    return;

                // Correct: GetResolved() returns a ResolvedState object
                var resolved = _finalizer.GetResolved();

                int cx = resolved.CrosshairCenterX;
                int cy = resolved.CrosshairCenterY;
                int armLength = resolved.CrosshairArmLength;
                int thickness = resolved.CrosshairThickness;

                var crosshairColor = (_buttonFlashTimer > 0)
                    ? ButtonFlashColor
                    : (System.Drawing.Color)resolved.CrosshairColor;

                foreach (var element in _renderOrder)
                    element.Draw(context);

                var crossColor = new Color(
                    crosshairColor.R / 255f,
                    crosshairColor.G / 255f,
                    crosshairColor.B / 255f,
                    crosshairColor.A / 255f
                );

                int horizX = cx - armLength;
                int horizY = cy - thickness / 2;
                int horizW = armLength * 2;
                int horizH = thickness;

                context.FillRectangle(new Rectangle(horizX, horizY, horizW, horizH), crossColor);

                int vertX = cx - thickness / 2;
                int vertY = cy - armLength;
                int vertW = thickness;
                int vertH = armLength * 2;

                context.FillRectangle(new Rectangle(vertX, vertY, vertW, vertH), crossColor);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: HUDManager: Draw failed: {ex.Message}");
                throw;
            }
        }

        // ====================================================================================================
        // TEXT UPDATE
        // ====================================================================================================
        public void UpdateText(string id, string newValue)
        {
            if (_textDict.TryGetValue(id, out var txt))
            {
                if (txt.Value != newValue)
                    txt.Value = newValue;
            }
        }

        // ====================================================================================================
        // BUTTON FLASH
        // ====================================================================================================
        public virtual void TriggerButtonFlash()
        {
            ButtonFlashColor = System.Drawing.Color.LightGreen;
            _buttonFlashTimer = BUTTON_FLASH_TIME;
        }

        protected virtual void ResetButtonFlash()
        {
        }

        // ====================================================================================================
        // FINALIZER COLOR DISTRIBUTION
        // ====================================================================================================
        private void DistributeFinalizerColors()
        {
            if (_finalizer == null)
                return;

            var resolved = _finalizer.GetResolved();

            foreach (var element in _renderOrder)
            {
                if (element is HUDPanel_CashUpdate cashPanel)
                    _finalizer.ApplyToPanel(cashPanel, resolved);
            }
        }
    }
}


