//===============================================================================
// FILE: PlacementRenderer.cs
// FILE PATH: Engine\Towers\PlacementRenderer.cs
// PURPOSE: Handles tower placement and preview rendering.
// ROLE: Handles tower placement and preview rendering.
// FEATURES: Handles tower placement and preview rendering.
//NOTES: Handles tower placement and preview rendering.
//===============================================================================
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers
{
    public class PlacementRenderer
    {
        private TowerData _towerData;
        private Vector3 _currentWorldPosition;
        private Vector3Int _currentGridPosition;
        private bool _canPlace;
        private bool _isInitialized;

        private NavigationGrid _grid;

        private Color _validColor = Color.FromArgb(120, 0, 255, 0);
        private Color _invalidColor = Color.FromArgb(120, 255, 0, 0);
        private Color _rangeColor = Color.FromArgb(60, 255, 255, 0);
        private Color _gridColor = Color.FromArgb(40, 255, 255, 255);

        private float _previewAlpha = 0.7f;
        private float _rangeAlpha = 0.3f;
        private float _gridAlpha = 0.2f;

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
        private bool _showPreview = true;
        private RenderSystem _renderSystem;

        public PlacementRenderer(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public PlacementRenderer(RenderSystem renderSystem, NavigationGrid grid) : this(renderSystem)
        {
            _grid = grid;
        }

        private RenderSystem RS => _renderSystem;



        //private RenderSystem RS => GameRoot.RenderContext.renderSystem;

        public PlacementRenderer()
        {
            Initialize();
        }

        public PlacementRenderer(NavigationGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            Initialize();
        }

        public void SetNavigationGrid(NavigationGrid grid)
        {
            _grid = grid;
        }

        private void Initialize()
        {
            _towerSprite = (Sprite)SpriteCache.GetSprite("tower_placeholder");
            _rangeSprite = (Sprite)SpriteCache.GetSprite("range_circle");

            _isInitialized = true;

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

            if (towerData.PreviewSprite != null)
                _towerSprite = (Sprite)SpriteCache.GetSprite(towerData.PreviewSprite);

            DLogger.Log(LogSubsystems.Towers, LogLevel.Info, "PlacementRenderer",
                $"Initialized for tower type {towerData.Type}");
        }

        public void UpdatePosition(Vector3 worldPosition, Vector3Int gridPosition, bool canPlace)
        {
            _currentWorldPosition = worldPosition;
            _currentGridPosition = gridPosition;
            _canPlace = canPlace;
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            _pulseTime += deltaTime * _pulseSpeed;
            _rangeRotation += deltaTime * _rangeRotationSpeed;
        }

        public void Render()
        {
            if (!_isInitialized || _towerData == null) return;

            try
            {
                if (_showGrid)
                    RenderGridOverlay();

                if (_showOccupancy)
                    RenderOccupancyPreview();

                if (_showRange)
                    RenderRangeIndicator();

                RenderTowerPreview();
                RenderPlacementFeedback();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Towers, LogLevel.Error, "PlacementRenderer",
                    $"Render error: {ex.Message}");
            }
        }

        private void RenderGridOverlay()
        {
            if (_grid == null) return;

            float cell = _grid.CellSize;

            for (int x = 0; x <= _towerData.GridSize.X; x++)
            {
                float sx = _currentWorldPosition.X + (x * cell);
                float sy = _currentWorldPosition.Y;

                RS.DrawLine(
                    new Vector3(sx, sy, 0),
                    new Vector3(sx, sy + (_towerData.GridSize.Y * cell), 0),
                    Color.White,
                    1f);
            }

            for (int y = 0; y <= _towerData.GridSize.Y; y++)
            {
                float sx = _currentWorldPosition.X;
                float sy = _currentWorldPosition.Y + (y * cell);

                RS.DrawLine(
                    new Vector3(sx, sy, 0),
                    new Vector3(sx + (_towerData.GridSize.X * cell), sy, 0),
                    Color.White,
                    1f);
            }
        }

        private void RenderOccupancyPreview()
        {
            if (_grid == null) return;

            float cell = _grid.CellSize;
            var baseColor = _canPlace ? _validColor : _invalidColor;
            var alpha = (byte)(baseColor.A * _previewAlpha);
            var finalColor = Color.FromArgb(alpha, (byte)baseColor.R, (byte)baseColor.G, (byte)baseColor.B);

            for (int x = 0; x < _towerData.GridSize.X; x++)
            {
                for (int y = 0; y < _towerData.GridSize.Y; y++)
                {
                    float cx = _currentWorldPosition.X + (x * cell) + (cell / 2f);
                    float cy = _currentWorldPosition.Y + (y * cell) + (cell / 2f);

                    float pulse = 1f + (MathF.Sin(_pulseTime) * _pulseAmount);
                    float size = cell * 0.8f * pulse;

                    RS.DrawRectangle(
                        cx - size / 2f,
                        cy - size / 2f,
                        size,
                        size,
                        finalColor);
                }
            }
        }

        private void RenderRangeIndicator()
        {
            if (_towerData.Range <= 0) return;

            var rc = Color.FromArgb(
                (byte)(_rangeColor.A * _rangeAlpha),
                (byte)_rangeColor.R,
                (byte)_rangeColor.G,
                (byte)_rangeColor.B);

            float pulse = 1f + (MathF.Sin(_pulseTime * 0.5f) * _pulseAmount * 0.5f);
            float currentRange = _towerData.Range * pulse;

            RS.DrawCircle(
                _currentWorldPosition.X,
                _currentWorldPosition.Y,
                currentRange,
                rc,
                2f);

            RenderRangeSegments(currentRange, rc);
        }

        private void RenderRangeSegments(float range, Color color)
        {
            const int segments = 8;
            const float segLength = 0.3f;

            for (int i = 0; i < segments; i++)
            {
                float startAngle = (i * (2f * MathF.PI / segments)) + _rangeRotation;
                float endAngle = startAngle + (segLength * (2f * MathF.PI / segments));

                var p1 = new Vector3(
                    _currentWorldPosition.X + MathF.Cos(startAngle) * range,
                    _currentWorldPosition.Y + MathF.Sin(startAngle) * range,
                    0);

                var p2 = new Vector3(
                    _currentWorldPosition.X + MathF.Cos(endAngle) * range,
                    _currentWorldPosition.Y + MathF.Sin(endAngle) * range,
                    0);

                RS.DrawLine(p1, p2, color, 3f);
            }
        }

        private void RenderTowerPreview()
        {
            if (_towerSprite == null) return;

            var baseColor = _canPlace ? _validColor : _invalidColor;
            var alpha = (byte)(baseColor.A * _previewAlpha);
            var finalColor = Color.FromArgb(alpha,
                (byte)baseColor.R,
                (byte)baseColor.G,
                (byte)baseColor.B);

            float cell = _grid?.CellSize ?? 1f;

            float tx = _currentWorldPosition.X + (_towerData.GridSize.X * cell / 2f);
            float ty = _currentWorldPosition.Y + (_towerData.GridSize.Y * cell / 2f);

            float hover = MathF.Sin(_pulseTime * 2f) * 0.05f;

            var pos = new Vector3(tx, ty + hover, 0);

            RS.DrawSprite(_towerSprite, pos, _towerData.Size.ToVector3(), finalColor);
            RenderTowerBase(pos, finalColor);
        }

        private void RenderTowerBase(Vector3 pos, Color color)
        {
            var baseSize = _towerData.Size.ToVector3() * 0.8f;
            var baseColor = Color.FromArgb((byte)(color.A * 0.5f),
                (byte)color.R,
                (byte)color.G,
                (byte)color.B);

            RS.DrawRectangle(
                pos.X - baseSize.X / 2f,
                pos.Y - baseSize.Y / 4f,
                baseSize.X,
                baseSize.Y / 2f,
                baseColor);
        }

        private void RenderPlacementFeedback()
        {
            var pos = new Vector3(
                _currentWorldPosition.X + (_towerData.GridSize.X * 0.5f),
                _currentWorldPosition.Y - 0.5f,
                0);

            if (_canPlace)
                RenderValidPlacementIcon(pos);
            else
                RenderInvalidPlacementIcon(pos);

            RenderCostIndicator(pos);
        }

        private void RenderValidPlacementIcon(Vector3 pos)
        {
            var c = Color.FromArgb((int)(200 * _previewAlpha), 0, 255, 0);
            RS.DrawCheckmark(pos, 0.3f, c);
        }

        private void RenderInvalidPlacementIcon(Vector3 pos)
        {
            var c = Color.FromArgb((int)(200 * _previewAlpha), 255, 0, 0);
            RS.DrawX(pos, 0.3f, c);
        }

        private void RenderCostIndicator(Vector3 pos)
        {
            var c = Color.FromArgb((int)(180 * _previewAlpha), 255, 255, 0);

            string text = $"${_towerData.Cost}";
            var textPos = new Vector3(pos.X, pos.Y - 0.3f, 0);

            RS.DrawString(text, textPos, c, 1f);
        }

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

        public void SetAnimationSpeed(float pulseSpeed, float rotationSpeed)
        {
            _pulseSpeed = pulseSpeed;
            _rangeRotationSpeed = rotationSpeed;
        }
    }
}
