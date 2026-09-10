// ====================================================================================================
// FILE: PlacementRenderer.cs
// PATH: Engine/Towers/PlacementRenderer.cs
// SUBSYSTEM: Tower Placement Rendering
//
// ROLE:
//     Deterministic visualization for tower placement using the modern RenderSystem + IDrawingContext
//     pipeline. Renders grid overlays, occupancy previews, range indicators, tower preview sprites,
//     and placement feedback icons.
//
// RESPONSIBILITIES:
//     - Visualize placement validity (valid/invalid occupancy preview).
//     - Render grid overlays aligned to NavigationGrid.
//     - Render tower preview sprites via RenderSystem.DrawSprite.
//     - Render range indicators and animated radial segments.
//     - Display cost indicators and placement feedback icons.
//     - Maintain deterministic color, geometry, and naming conventions.
//
// NON-RESPONSIBILITIES:
//     - Gameplay logic or placement rules.
//     - Resource loading (SpriteCache).
//     - GPU device or swap-chain management.
// ====================================================================================================

using System;
using DocumentFormat.OpenXml.Bibliography;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Render.Sprites;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Placement
{
    public sealed class PlacementRenderer
    {
        private readonly RenderSystem _renderSystem;

        private NavigationGrid _grid;
        private TowerData _towerData;

        private System.Numerics.Vector2 _worldPos;
        private Vector2Int _gridPos;
        private bool _canPlace;

        private bool _initialized;

        private float _pulseTime;
        private float _pulseSpeed = 2f;
        private float _pulseAmount = 0.2f;

        private float _rangeRotation;
        private float _rangeRotationSpeed = 0.5f;

        private Sprite _towerSprite;
        private Sprite _rangeSprite;

        private bool _showRange = true;
        private bool _showGrid = true;
        private bool _showOccupancy = true;

        private Color _validColor = Color.FromArgb(120, 0, 255, 0);
        private Color _invalidColor = Color.FromArgb(120, 255, 0, 0);
        private Color _rangeColor = Color.FromArgb(60, 255, 255, 0);
        private Color _gridColor = Color.FromArgb(40, 255, 255, 255);

        private float _previewAlpha = 0.7f;
        private float _rangeAlpha = 0.3f;
        private float _gridAlpha = 0.2f;

        public PlacementRenderer(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem ?? throw new ArgumentNullException(nameof(renderSystem));
        }

        public PlacementRenderer(RenderSystem renderSystem, NavigationGrid grid)
            : this(renderSystem)
        {
            _grid = grid;
        }

        public PlacementRenderer()
        {
            // Parameterless ctor intentionally left empty.
        }

        // ------------------------------------------------------------------------------------------------
        // INITIALIZATION
        // ------------------------------------------------------------------------------------------------

        public void Initialize()
        {
            _towerSprite = SpriteCache.GetSprite("tower_placeholder");
            _rangeSprite = SpriteCache.GetSprite("range_circle");
            _initialized = true;

            DLogger.Log(LogSubsystems.Towers, LogLevel.Info, "PlacementRenderer",
                "PlacementRenderer initialized");
        }

        public void Initialize(TowerData towerData)
        {
            if (towerData == null)
            {
                DLogger.Log(LogSubsystems.Towers, LogLevel.Warning, "PlacementRenderer",
                    "Initialize called with null TowerData");
                return;
            }

            _towerData = towerData;

            if (!string.IsNullOrEmpty(towerData.PreviewSprite))
                _towerSprite = SpriteCache.GetSprite(towerData.PreviewSprite);

            DLogger.Log(LogSubsystems.Towers, LogLevel.Info, "PlacementRenderer",
                $"Initialized for tower type {towerData.Type}");
        }

        // ------------------------------------------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------------------------------------------

        public void UpdatePosition(System.Numerics.Vector3 worldPosition, Vector3Int gridPosition, bool canPlace)
        {
            _worldPos = new System.Numerics.Vector2(worldPosition.X, worldPosition.Y);
            _gridPos = new Vector2Int(gridPosition.X, gridPosition.Y);
            _canPlace = canPlace;
        }

        public void Update(float deltaTime)
        {
            if (!_initialized) return;

            _pulseTime += deltaTime * _pulseSpeed;
            _rangeRotation += deltaTime * _rangeRotationSpeed;
        }

        // ------------------------------------------------------------------------------------------------
        // RENDER ENTRY POINT
        // ------------------------------------------------------------------------------------------------

        public void Render()
        {
            if (!_initialized || _towerData == null)
                return;

            if (_showGrid) RenderGridOverlay();
            if (_showOccupancy) RenderOccupancyPreview();
            if (_showRange) RenderRangeIndicator();

            RenderTowerPreview();
            RenderPlacementFeedback();
        }

        // ------------------------------------------------------------------------------------------------
        // GRID OVERLAY
        // ------------------------------------------------------------------------------------------------

        private void RenderGridOverlay()
        {
            if (_grid == null) return;

            float cellSize = _grid.CellSize;

            for (int x = 0; x <= _towerData.GridSize.X; x++)
            {
                float sx = _worldPos.X + x * cellSize;
                float sy = _worldPos.Y;

                _renderSystem.DrawLine(
                    new System.Numerics.Vector2(sx, sy),
                    new System.Numerics.Vector2(sx, sy + _towerData.GridSize.Y * cellSize),
                    _gridColor,
                    1f);
            }

            for (int y = 0; y <= _towerData.GridSize.Y; y++)
            {
                float sx = _worldPos.X;
                float sy = _worldPos.Y + y * cellSize;

                _renderSystem.DrawLine(
                    new System.Numerics.Vector2(sx, sy),
                    new System.Numerics.Vector2(sx + _towerData.GridSize.X * cellSize, sy),
                    _gridColor,
                    1f);
            }
        }

        // ------------------------------------------------------------------------------------------------
        // OCCUPANCY PREVIEW
        // ------------------------------------------------------------------------------------------------

        private void RenderOccupancyPreview()
        {
            if (_grid == null) return;

            float cellSize = _grid.CellSize;

            var baseColor = _canPlace ? _validColor : _invalidColor;
            var alpha = (byte)(baseColor.A * _previewAlpha);
            var finalColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

            for (int x = 0; x < _towerData.GridSize.X; x++)
            {
                for (int y = 0; y < _towerData.GridSize.Y; y++)
                {
                    float cx = _worldPos.X + x * cellSize + cellSize / 2f;
                    float cy = _worldPos.Y + y * cellSize + cellSize / 2f;

                    float pulse = 1f + MathF.Sin(_pulseTime) * _pulseAmount;
                    float size = cellSize * 0.8f * pulse;

                    var rect = new Core.Rectangle(
                        (int)(cx - size / 2f),
                        (int)(cy - size / 2f),
                        (int)size,
                        (int)size);

                    _renderSystem.FillRectangle(rect, (Color)finalColor);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------
        // RANGE INDICATOR
        // ------------------------------------------------------------------------------------------------

        private void RenderRangeIndicator()
        {
            if (_towerData.Range <= 0) return;

            var rangeColor = Color.FromArgb(
                (byte)(_rangeColor.A * _rangeAlpha),
                _rangeColor.R,
                _rangeColor.G,
                _rangeColor.B);

            float pulse = 1f + System.MathF.Sin(_pulseTime * 0.5f) * _pulseAmount * 0.5f;
            float currentRange = _towerData.Range * pulse;

            _renderSystem.DrawCircle(_worldPos, currentRange, rangeColor, 2f);

            RenderRangeSegments(currentRange, (Color)rangeColor);
        }

        private void RenderRangeSegments(float range, Color color)
        {
            const int segmentCount = 8;
            const float segmentLength = 0.3f;

            for (int i = 0; i < segmentCount; i++)
            {
                float startAngle = (i * (2f * System.MathF.PI / segmentCount)) + _rangeRotation;
                float endAngle = startAngle + segmentLength * (2f * System.MathF.PI / segmentCount);

                var start = new System.Numerics.Vector2(
                    _worldPos.X + System.MathF.Cos(startAngle) * range,
                    _worldPos.Y + System.MathF.Sin(startAngle) * range);

                var end = new System.Numerics.Vector2(
                    _worldPos.X + System.MathF.Cos(endAngle) * range,
                    _worldPos.Y + System.MathF.Sin(endAngle) * range);

                _renderSystem.DrawLine(start, end, color, 3f);
            }
        }

        // ------------------------------------------------------------------------------------------------
        // TOWER PREVIEW
        // ------------------------------------------------------------------------------------------------

        private void RenderTowerPreview()
        {
            if (_towerSprite == null) return;

            float cellSize = _grid?.CellSize ?? 1f;

            float towerX = _worldPos.X + _towerData.GridSize.X * cellSize / 2f;
            float towerY = _worldPos.Y + _towerData.GridSize.Y * cellSize / 2f;

            float hoverOffset = MathF.Sin(_pulseTime * 2f) * 0.05f;

            var pos = new System.Numerics.Vector2(towerX, towerY + hoverOffset);

            var baseColor = _canPlace ? _validColor : _invalidColor;
            var alpha = (byte)(baseColor.A * _previewAlpha);
            var finalColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

            _renderSystem.DrawSprite(
                _towerSprite.Texture,
                pos,
                _towerData.Size,
                finalColor);
        }

        // ------------------------------------------------------------------------------------------------
        // PLACEMENT FEEDBACK
        // ------------------------------------------------------------------------------------------------

        private void RenderPlacementFeedback()
        {
            var pos = new System.Numerics.Vector2(
                _worldPos.X + _towerData.GridSize.X * 0.5f,
                _worldPos.Y - 0.5f);

            if (_canPlace)
                RenderValidPlacementIcon(pos);
            else
                RenderInvalidPlacementIcon(pos);
        }

        private void RenderValidPlacementIcon(System.Numerics.Vector2 pos)
        {
            var color = Color.FromArgb((int)(200 * _previewAlpha), 0, 255, 0);
            _renderSystem.DrawText("✔", pos, 16f, color);
        }

        private void RenderInvalidPlacementIcon(System.Numerics.Vector2 pos)
        {
            var color = Color.FromArgb((int)(200 * _previewAlpha), 255, 0, 0);
            _renderSystem.DrawText("✖", pos, 16f, color);
        }

        // ------------------------------------------------------------------------------------------------
        // CONFIGURATION
        // ------------------------------------------------------------------------------------------------

        public void SetRenderingOptions(bool showRange, bool showGrid, bool showOccupancy)
        {
            _showRange = showRange;
            _showGrid = showGrid;
            _showOccupancy = showOccupancy;
        }

        public void SetColors(Color valid, Color invalid, Color range)
        {
            _validColor = valid;
            _invalidColor = invalid;
            _rangeColor = range;
        }

        public void SetTransparency(float previewAlpha, float rangeAlpha, float gridAlpha)
        {
            _previewAlpha = System.Math.Clamp(previewAlpha, 0f, 1f);
            _rangeAlpha = System.Math.Clamp(rangeAlpha, 0f, 1f);
            _gridAlpha = System.Math.Clamp(gridAlpha, 0f, 1f);
        }

        public void SetSpeeds(float pulseSpeed, float rotationSpeed)
        {
            _pulseSpeed = pulseSpeed;
            _rangeRotationSpeed = rotationSpeed;
        }
    }

    internal readonly struct Vector2Int
    {
        public readonly int X;
        public readonly int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
