/*
File:    HealthBarRenderer.cs
Purpose: Renders health bars above entities with HealthComponent and TransformComponent.
Features: Configurable bar size, color, offset, health ratio clamping, full-health hiding.

P11-04-09-B: Renders health bars above entities with HealthComponent and TransformComponent.
Supports configurable bar size, color, and offset. Clamps health bar width to entity's
current/max health ratio. Optionally hides health bars for full-health entities.
Includes XML documentation and audit-friendly rendering logic.
*/

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Rendering;
using System;
using System.Collections.Generic;
using static System.Math;


using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    ///<summary>
    ///Renders health bars above entities that have HealthComponent and TransformComponent.
    ///P11-04-09-B: Supports configurable bar size, color, offset, health ratio clamping,
    ///and optional hiding for full-health entities.
    ///</summary>
    public class HealthBarRenderer
    {
        private readonly EntityManager _entityManager;
        private readonly bool _debugOutput = true;

        ///<summary>
        ///Gets or sets the width of health bars in pixels.
        ///</summary>
        public int BarWidth { get; set; } = 40;

        ///<summary>
        ///Gets or sets the height of health bars in pixels.
        ///</summary>
        public int BarHeight { get; set; } = 4;

        ///<summary>
        ///Gets or sets the vertical offset above entity position for health bar rendering.
        ///</summary>
        public float VerticalOffset { get; set; } = 10.0f;

        ///<summary>
        ///Gets or sets the color for healthy portions of health bars.
        ///</summary>
        public System.Drawing.Color HealthyColor { get; set; } = System.Drawing.Color.Green;

        ///<summary>
        ///Gets or sets the color for damaged portions of health bars.
        ///</summary>
        public System.Drawing.Color DamagedColor { get; set; } = System.Drawing.Color.Red;

        ///<summary>
        ///Gets or sets the color for health bar borders.
        ///</summary>
        public System.Drawing.Color BorderColor { get; set; } = System.Drawing.Color.Black;

        ///<summary>
        ///Gets or sets whether to hide health bars for entities at full health.
        ///</summary>
        public bool HideFullHealth { get; set; } = true;

        ///<summary>
        ///Gets or sets the health threshold (0-1) below which health bars are always shown.
        ///</summary>
        public float AlwaysShowThreshold { get; set; } = 0.95f;

        ///<summary>
        ///Initializes a new instance of the HealthBarRenderer class.
        ///</summary>
        ///<param name="entityManager">Entity manager for component access</param>
        public HealthBarRenderer(EntityManager entityManager)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            DebugLog("HealthBarRenderer: Initialized with EntityManager");
        }

        ///<summary>
        ///Renders health bars for all entities with HealthComponent and TransformComponent.
        ///P11-04-09-B: Renders health bars with configurable properties and health ratio clamping.
        ///</summary>
        ///<param name="context">Render context for drawing operations</param>
        public void RenderHealthBars(IRenderContext context)
        {
            if (context == null)
            {
                DebugLog("HealthBarRenderer: Render failed - Null render context");
                return;
            }

            try
            {
                var entitiesWithHealth = GetEntitiesWithHealthAndTransform();
                DebugLog($"HealthBarRenderer: Rendering {entitiesWithHealth.Count} health bars");

                foreach (var entity in entitiesWithHealth)
                {
                    var entityId = entity is Entity ent ? ent.Id : (uint)entity;
                    RenderEntityHealthBar(entityId, context);
                }
            }
            catch (Exception ex)
            {
                DebugLog($"HealthBarRenderer: Render failed - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets all entities that have both HealthComponent and TransformComponent.
        ///</summary>
        private IReadOnlyList<object> GetEntitiesWithHealthAndTransform()
        {
            var result = new List<object>();
            var entities = _entityManager.Entities;

            foreach (var entity in entities)
            {
                var entityId = entity is Entity ent ? ent.Id : (uint)entity;
                if (_entityManager.HasComponent<ECS.HealthComponent>(entityId) &&
                _entityManager.HasComponent<Engine.Components.TransformComponent>(entityId))
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        ///<summary>
        ///Renders a health bar for a single entity.
        ///P11-04-09-B: Clamps health bar width to current/max health ratio.
        ///</summary>
        private void RenderEntityHealthBar(uint entity, IRenderContext context)
        {
            try
            {
                var healthComponent = _entityManager.GetComponent<Engine.Components.HealthComponent>(entity);
                var transformComponent = _entityManager.GetComponent<Engine.Components.TransformComponent>(entity);
                if (healthComponent == null || transformComponent == null)
                    return;

                float healthRatio = (float)(healthComponent.CurrentHealth / (float)healthComponent.MaxHealth);
                float clampedRatio = Clamp(healthRatio, 0f, 1f);

                //Skip rendering if hiding full health and entity is healthy
                if (HideFullHealth && healthRatio >= AlwaysShowThreshold)
                    return;

                //Calculate health bar position
                var entityPosition = transformComponent.Position;
                var barX = entityPosition.X - (BarWidth / 2f);
                var barY = entityPosition.Y - VerticalOffset;

                //Render health bar background (damaged portion)
                var backgroundRect = new Rectangle(barX, barY, BarWidth, BarHeight);
                context.DrawRectangle(backgroundRect.X, backgroundRect.Y, backgroundRect.Width, backgroundRect.Height, new Color(DamagedColor.R, DamagedColor.G, DamagedColor.B, DamagedColor.A));

                //Render health bar foreground (healthy portion)
                if (clampedRatio > 0f)
                {
                    var healthWidth = BarWidth * clampedRatio;
                    var healthRect = new Rectangle(barX, barY, healthWidth, BarHeight);
                    context.DrawRectangle(healthRect.X, healthRect.Y, healthRect.Width, healthRect.Height, new Color(HealthyColor.R, HealthyColor.G, HealthyColor.B, HealthyColor.A));
                }

                //Render health bar border
                context.DrawRectangle(barX, barY, BarWidth, BarHeight, new Color(BorderColor.R, BorderColor.G, BorderColor.B, BorderColor.A));

                DebugLog($"HealthBarRenderer: Rendered health bar for entity - Health: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth} ({healthRatio:P0})");
            }
            catch (Exception ex)
            {
                DebugLog($"HealthBarRenderer: Failed to render entity health bar - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets statistics about the health bar renderer.
        ///</summary>
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

        private void DebugLog(string message)
        {
            if (_debugOutput)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    ///<summary>
    ///Statistics about the health bar renderer state.
    ///</summary>
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




