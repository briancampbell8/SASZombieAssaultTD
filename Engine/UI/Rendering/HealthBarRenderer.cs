// ====================================================================================================
//  FILE: HealthBarRenderer.cs
//  PATH: ./Engine/UI/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide RenderHealthBars() behavior for the Rendering subsystem.
//      - Provide GetStatistics() behavior for the Rendering subsystem.
//      - Provide ToString() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    HealthBarRenderer.cs
Purpose: Renders health bars above entities with HealthComponent and TransformComponent.
Features: Configurable bar size, color, offset, health ratio clamping, full-health hiding.

P11-04-09-B: Renders health bars above entities with HealthComponent and TransformComponent.
Supports configurable bar size, color, and offset. Clamps health bar width to ECSEntityCore's
current/max health ratio. Optionally hides health bars for full-health entities.
Includes XML documentation and audit-friendly rendering logic.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static System.Math;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Renders health bars above entities that have HealthComponent and TransformComponent. P11-04-09-B: Supports
    /// configurable bar size, color, offset, health ratio clamping, and optional hiding for full-health entities.
    /// </summary>
    public class HealthBarRenderer
    {
        private readonly ECSEntityCore _ECSEntityCore;
        private readonly ECSComponents _components;
        private readonly bool _debugOutput = true;
        private object healthRect;

        /// <summary>
        /// Gets or sets the width of health bars in pixels.
        /// </summary>
        public int BarWidth { get; set; } = 40;

        /// <summary>
        /// Gets or sets the height of health bars in pixels.
        /// </summary>
        public int BarHeight { get; set; } = 4;

        /// <summary>
        /// Gets or sets the vertical offset above ECSEntityCore position for health bar rendering.
        /// </summary>
        public float VerticalOffset { get; set; } = 10.0f;

        /// <summary>
        /// Gets or sets the color for healthy portions of health bars.
        /// </summary>
        public System.Drawing.Color HealthyColor { get; set; } = System.Drawing.Color.Green;

        /// <summary>
        /// Gets or sets the color for damaged portions of health bars.
        /// </summary>
        public System.Drawing.Color DamagedColor { get; set; } = System.Drawing.Color.Red;

        /// <summary>
        /// Gets or sets the color for health bar borders.
        /// </summary>
        public System.Drawing.Color BorderColor { get; set; } = System.Drawing.Color.Black;

        /// <summary>
        /// Gets or sets whether to hide health bars for entities at full health.
        /// </summary>
        public bool HideFullHealth { get; set; } = true;

        /// <summary>
        /// Gets or sets the health threshold (0-1) below which health bars are always shown.
        /// </summary>
        public float AlwaysShowThreshold { get; set; } = 0.95f;

        /// <summary>
        /// Initializes a new instance of the HealthBarRenderer class.
        /// </summary>
        /// <param name="ECSEntityCore">Entity manager for component access</param>
        public HealthBarRenderer(ECSEntityCore ECSEntityCore)
        {
            _ECSEntityCore = ECSEntityCore ?? throw new ArgumentNullException(nameof(ECSEntityCore));
            DLogger.Log(LogSubsystems.ResourcesPipeline, "HealthBarRenderer: Initialized with ECSEntityCore");
        }

        /// <summary>
        /// Renders health bars for all entities with HealthComponent and TransformComponent. P11-04-09-B: Renders
        /// health bars with configurable properties and health ratio clamping.
        /// </summary>
        /// <param name="context">Render context for drawing operations</param>
        public void RenderHealthBars(D3D11Adapter_Core context)
        {
            if (context == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "HealthBarRenderer: Render failed - Null render context");
                return;
            }

            try
            {
                var entitiesWithHealth = GetEntitiesWithHealthAndTransform();
                DLogger.Log($"HealthBarRenderer: Rendering {entitiesWithHealth.Count} health bars");

                foreach (var ECSEntityCore in entitiesWithHealth)
                {
                    var ECSEntityCoreId = ECSEntityCore is ECSEntityCore ent ? ent.Id : (uint)ECSEntityCore;
                    RenderEntityHealthBar(ECSEntityCoreId, context);
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"HealthBarRenderer: Render failed - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all entities that have both HealthComponent and TransformComponent.
        /// </summary>
        private IReadOnlyList<object> GetEntitiesWithHealthAndTransform()
        {
            var result = new List<object>();
            var entities = _ECSEntityCore.Entities;

            foreach (var ECSEntityCore in entities)
            {
                var ECSEntityCoreId = ECSEntityCore is ECSEntityCore ent ? ent.Id : (uint)ECSEntityCore;
                if (_components.HasComponent<HealthComponent>(ECSEntityCoreId) &&
                _components.HasComponent<Engine.Components.TransformComponent>(ECSEntityCoreId))
                {
                    result.Add(ECSEntityCore);
                }
            }

            return result;
        }

        /// <summary>
        /// Renders a health bar for a single ECSEntityCore. P11-04-09-B: Clamps health bar width to current/max health
        /// ratio.
        /// </summary>
        private void RenderEntityHealthBar(uint ECSEntityCore, D3D11Adapter_Core context)
        {
            try
            {
                var healthComponent = _components.GetComponent<Engine.Components.HealthComponent>(ECSEntityCore);
                var transformComponent = _components.GetComponent<Engine.Components.TransformComponent>(ECSEntityCore);
                if (healthComponent == null || transformComponent == null)
                    return;

                float healthRatio = (float)(healthComponent.CurrentHealth / (float)healthComponent.MaxHealth);
                float clampedRatio = Clamp(healthRatio, 0f, 1f);

                //Skip rendering if hiding full health and ECSEntityCore is healthy
                if (HideFullHealth && healthRatio >= AlwaysShowThreshold)
                    return;

                //Calculate health bar position
                var ECSEntityCorePosition = transformComponent.Position;
                var barX = ECSEntityCorePosition.X - (BarWidth / 2f);
                var barY = ECSEntityCorePosition.Y - VerticalOffset;

                //Render health bar background (damaged portion)
                var backgroundRect = new Rectangle((float)barX,
                    (float)barY, (float)BarWidth, (float)BarHeight);
                context.DrawRectangle(
                    backgroundRect.X,
                    backgroundRect.Y,
                    backgroundRect.Width,
                    backgroundRect.Height,
                    new Color(
                        DamagedColor.R,
                        DamagedColor.G,
                        DamagedColor.B,
                        DamagedColor.A));

                //Render health bar foreground (healthy portion)
                if (clampedRatio > 0f)
                {
                    var healthWidth = BarWidth * clampedRatio;
                    var healthRect = new Rectangle(barX, barY, healthWidth, BarHeight);
                    context.DrawRectangle(healthRect.X, healthRect.Y, healthRect.Width, healthRect.Height, new Color(HealthyColor.R, HealthyColor.G, HealthyColor.B, HealthyColor.A));
                }

                //Render health bar border
                context.DrawRectangle(
                    (int)barX,
                    (int)barY,
                    (int)BarWidth,
                    (int)BarHeight,
                    new Color(
                        BorderColor.R,
                        BorderColor.G,
                        BorderColor.B,
                        BorderColor.A));

                DLogger.Log(
                    $"HealthBarRenderer: Rendered health bar for ECSEntityCore - " +
                    $"Health: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth} ({healthRatio:P0})");
            }
            catch (Exception ex)
            {
                DLogger.Log($"HealthBarRenderer: Failed to render ECSEntityCore health bar - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets statistics about the health bar renderer.
        /// </summary>
        public HealthBarRendererStatistics GetStatistics()
        {
            var entitiesWithHealth = GetEntitiesWithHealthAndTransform();
            return new HealthBarRendererStatistics
            {
                BarWidth = BarWidth,
                BarHeight = BarHeight,
                VerticalOffset = VerticalOffset,
                HideFullHealth = HideFullHealth,
                EntitiesWithHealth = entitiesWithHealth.Count
            };
        }

        private void Log(string message)
        {
            if (_debugOutput)
            {
                DLogger.Log($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    /// <summary>
    /// Statistics about the health bar renderer state.
    /// </summary>
    public class HealthBarRendererStatistics
    {
        public int BarWidth { get; set; }
        public int BarHeight { get; set; }
        public float VerticalOffset { get; set; }
        public bool HideFullHealth { get; set; }
        public int EntitiesWithHealth { get; set; }

        public override string ToString()
        {
            return $"Health Bar Renderer Statistics - Bar: {BarWidth}x{BarHeight}, Offset: {VerticalOffset}, " +
            $"Hide Full Health: {HideFullHealth}, Entities: {EntitiesWithHealth}";
        }
    }
}