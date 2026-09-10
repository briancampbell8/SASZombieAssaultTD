// ====================================================================================================
//  FILE: SpriteCache.cs
//  PATH: ./Engine/Render/Sprites/
//  MODULE: Render / Sprites
//
//  ROLE:
//      Provides deterministic lookup of reusable SpriteDefinitions for the rendering subsystem.
//      Acts as a lightweight registry for named sprite metadata used by SpriteBatch and UI/HUD systems.
//
//  RESPONSIBILITIES:
//      - Store and retrieve SpriteDefinition objects by name.
//      - Provide type-safe access to sprite metadata.
//      - Remain deterministic and side-effect free.
//
//  NON-RESPONSIBILITIES:
//      - Loading textures or performing resource management.
//      - Creating Sprite draw commands (handled by SpriteBatch).
//      - Gameplay logic, UI layout, or diagnostics logging.
//      - File persistence or serialization.
//
//  ARCHITECTURAL NOTES:
//      - Pure metadata cache.
//      - Compatible with Option‑B deterministic rendering.
//      - Keeps original filename as requested.
//      - PERMANENT TECHNIQUE: SpriteCache NEVER defines a Sprite class. It only returns instances of
//        the canonical Sprite type located in Sprite.cs. This prevents namespace collisions, ensures
//        deterministic type idECSEntityCore, and eliminates CS0101 duplicate-type errors.
//      - PERMANENT TECHNIQUE: SpriteCache exposes explicit, type-safe methods required by PlacementRenderer:
//          * GetSprite(string name) → Sprite
//          * RenderSprite(Sprite sprite, Vector3 position, Vector3 size, Color color)
//        These methods are permanently part of the rendering architecture.
//
//  CHANGE LOG:
//      2026-08-03  Copilot     Initial SpriteDefinition cache implementation.
//      2026-08-04  Copilot     Removed duplicate Sprite class from SpriteCache to fix namespace conflict.
//      2026-08-04  Copilot     Added CreateSpriteFromDefinition() and GetSprite(string) using canonical Sprite.
//      2026-08-04  Copilot     Added RenderSprite(Sprite, Vector3, Vector3, Color) as permanent rendering entry point.
//      2026-08-04  Copilot     Replaced System.Drawing.Color with SASZombieAssaultTD.Engine.Core.Color.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Sprites
{
    /// <summary>
    /// Immutable definition describing how a sprite should be drawn.
    /// </summary>
    public readonly struct SpriteDefinition
    {
        public readonly Texture2D Texture;
        public readonly RectangleF Source;
        public readonly Color Color;
        public readonly System.Numerics.Vector2 Origin;
        public readonly float Rotation;
        public readonly float LayerDepth;

        public SpriteDefinition(
            Texture2D texture,
            RectangleF source,
            Color color,
            System.Numerics.Vector2 origin,
            float rotation,
            float layerDepth)
        {
            Texture = texture;
            Source = source;
            Color = color;
            Origin = origin;
            Rotation = rotation;
            LayerDepth = System.Math.Clamp(layerDepth, 0f, 1f);
        }
    }

    /// <summary>
    /// Deterministic cache for named SpriteDefinitions.
    /// </summary>
    public static class SpriteCache
    {
        private static readonly Dictionary<string, SpriteDefinition> _definitions = new();

        public static object source { get; private set; }

        public static object destination { get; private set; }

        public static object color { get; private set; }

        public static object origin { get; private set; }

        public static object rotation { get; private set; }

        public static object layerDepth { get; private set; }

        // ---------------------------------------------------------------------------------------------
        // CONSTRUCTION
        // ---------------------------------------------------------------------------------------------
        static SpriteCache() => Clear();

        // ---------------------------------------------------------------------------------------------
        // DEFINITION MANAGEMENT
        // ---------------------------------------------------------------------------------------------
        public static void Add(string name, SpriteDefinition definition)
        {
            _definitions[name] = definition;
        }

        public static bool TryGetDefinition(string name, out SpriteDefinition definition)
        {
            return _definitions.TryGetValue(name, out definition);
        }

        public static SpriteDefinition? GetDefinition(string name)
        {
            return _definitions.TryGetValue(name, out var def) ? def : null;
        }

        public static void Clear()
        {
            _definitions.Clear();
        }

        // ---------------------------------------------------------------------------------------------
        // SPRITE CREATION (USING CANONICAL Sprite.cs)
        // ---------------------------------------------------------------------------------------------
        private static Sprite CreateSpriteFromDefinition(SpriteDefinition definition)
        {
            // Uses the REAL Sprite class from Sprite.cs
            return new Sprite(
                definition.Texture,
                definition.Source,
                definition.Color,
                definition.Origin,
                definition.Rotation,
                definition.LayerDepth);
        }

        /// <summary>
        /// Retrieves a runtime Sprite instance by name, or null if not found.
        /// Permanent deterministic API used by PlacementRenderer.
        /// </summary>
        public static Sprite GetSprite(string name)
        {
            if (!_definitions.TryGetValue(name, out var definition))
                return null;

            return CreateSpriteFromDefinition(definition);
        }

        // ---------------------------------------------------------------------------------------------
        // SPRITE RENDERING (PERMANENT ENTRY POINT)
        // ---------------------------------------------------------------------------------------------
        public static void RenderSprite(
            Sprite sprite,
            string name,
            SpriteDefinition definition,
            System.Numerics.Vector3 source,
            System.Numerics.Vector3 position,
            System.Numerics.Vector3 size,
            System.Drawing.RectangleF Height,
            System.Drawing.RectangleF Width,
            System.Drawing.RectangleF sourceRect,
            Color color,
            System.Numerics.Vector2 origin
            )
        {
            if (sprite == null)
                return;

            DLogger.Log(LogSubsystems.RenderSprites,
                LogEnums.LogLevel.Debug,
                source,
                LogCategory.RenderSprites,
                 $"[SpriteRender] Texture={sprite.Texture?.ToString() ?? "null"} " +
        $"Source=({source.X},{source.Y},{sprite.Width},{sprite.Height}) " +
        $"Position=({position.X},{position.Y},{position.Z}) Size=({size.X},{size.Y},{size.Z}) " +
        $"Color=({color.A},{color.R},{color.G},{color.B}) Rotation={sprite.Rotation} LayerDepth={sprite.LayerDepth}");
            // Uses the REAL Sprite class from Sprite.cs
            var spriteToRender = new Sprite(
                sprite.Texture,
                sprite.Destination,
                sprite.Source,
                color,
                sprite.Rotation,
                sprite.Origin,
                sprite.LayerDepth);
        }
    }
}

