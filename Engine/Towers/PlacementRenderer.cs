using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;



namespace SASZombieAssaultTD.Engine.Towers

{

    /// <summary>

    /// Tower placement renderer for SAS Zombie Assault TD.

    /// Handles visual feedback for tower placement preview.

    /// </summary>

    public class PlacementRenderer

    {

        private TowerData _towerData;

        private Vector3 _currentWorldPosition;

        private Vector3Int _currentGridPosition;

        private bool _canPlace;

        private bool _isInitialized = false;



        // Navigation grid instance (must be provided)

        private NavigationGrid _grid;



        // Rendering properties

        private System.Drawing.Color _validColor = System.Drawing.Color.FromArgb(120, 0, 255, 0);  // Green with transparency

        private System.Drawing.Color _invalidColor = System.Drawing.Color.FromArgb(120, 255, 0, 0); // Red with transparency

        private System.Drawing.Color _rangeColor = System.Drawing.Color.FromArgb(60, 255, 255, 0);   // Yellow with transparency

        private System.Drawing.Color _gridColor = System.Drawing.Color.FromArgb(40, 255, 255, 255);  // White with transparency

        private float _previewAlpha = 0.7f;

        private float _rangeAlpha = 0.3f;

        private float _gridAlpha = 0.2f;



        // Animation properties

        private float _pulseTime = 0f;

        private float _pulseSpeed = 2f;

        private float _pulseAmount = 0.2f;

        private float _rangeRotation = 0f;

        private float _rangeRotationSpeed = 0.5f;



        // Rendering components

        private Sprite _towerSprite;

        private Sprite _rangeSprite;

        private bool _showRange = true;

        private bool _showGrid = true;

        private bool _showOccupancy = true;



        public PlacementRenderer()

        {

            Initialize();

        }



        /// <summary>

        /// Construct with explicit NavigationGrid dependency to avoid relying on a global singleton.

        /// </summary>

        /// <param name="grid">Navigation grid to use for rendering calculations.</param>

        public PlacementRenderer(NavigationGrid grid)

        {

            _grid = grid ?? throw new ArgumentNullException(nameof(grid));

            Initialize();

        }



        /// <summary>

        /// Set or change the navigation grid used by this renderer.

        /// </summary>

        public void SetNavigationGrid(NavigationGrid grid)

        {

            _grid = grid;

        }



        /// <summary>

        /// Initialize the placement renderer.

        /// </summary>

        private void Initialize()

        {

            // Load default sprites

            _towerSprite = (Sprite)SpriteCache.GetSprite("tower_placeholder");

            _rangeSprite = (Sprite)SpriteCache.GetSprite("range_circle");



            _isInitialized = true;

            Console.WriteLine("Placement Renderer initialized");

        }



        /// <summary>

        /// Initialize renderer with tower data.

        /// </summary>

        /// <param name="towerData">Tower data for rendering.</param>

        public void Initialize(TowerData towerData)

        {

            if (towerData == null)

            {

                Console.WriteLine("Tower data is null");

                return;

            }



            _towerData = towerData;



            // Load tower-specific sprite

            if (towerData.PreviewSprite != null)

            {

                _towerSprite = (Sprite)SpriteCache.GetSprite(towerData.PreviewSprite);

            }



            Console.WriteLine($"Placement renderer initialized for {towerData.Type}");

        }



        /// <summary>

        /// Update renderer position and validity.

        /// </summary>

        /// <param name="worldPosition">Current world position.</param>

        /// <param name="gridPosition">Current grid position.</param>

        /// <param name="canPlace">Whether placement is valid.</param>

        public void UpdatePosition(Vector3 worldPosition, Vector3Int gridPosition, bool canPlace)

        {

            _currentWorldPosition = worldPosition;

            _currentGridPosition = gridPosition;

            _canPlace = canPlace;

        }



        /// <summary>

        /// Update animation states.

        /// </summary>

        /// <param name="deltaTime">Time since last frame.</param>

        public void Update(float deltaTime)

        {

            if (!_isInitialized) return;



            // Update pulse animation

            _pulseTime += deltaTime * _pulseSpeed;



            // Update range rotation

            _rangeRotation += deltaTime * _rangeRotationSpeed;

        }



        /// <summary>

        /// Render the placement preview.

        /// </summary>

        public void Render()

        {

            if (!_isInitialized || _towerData == null) return;



            try

            {

                // Render grid overlay

                if (_showGrid)

                {

                    RenderGridOverlay();

                }



                // Render occupancy preview

                if (_showOccupancy)

                {

                    RenderOccupancyPreview();

                }



                // Render range indicator

                if (_showRange)

                {

                    RenderRangeIndicator();

                }



                // Render tower preview

                RenderTowerPreview();



                // Render placement feedback

                RenderPlacementFeedback();

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Error rendering placement preview: {ex.Message}");

            }

        }



        /// <summary>

        /// Render grid overlay.

        /// </summary>

        private void RenderGridOverlay()

        {

            var grid = _grid;

            if (grid == null) return;



            var cellSize = grid.CellSize;

            var gridColor = System.Drawing.Color.FromArgb((byte)(_gridColor.A * _previewAlpha), _gridColor.R, _gridColor.G, _gridColor.B);



            // Render grid lines around placement area

            for (int x = 0; x <= _towerData.GridSize.X; x++)

            {

                var startX = _currentWorldPosition.X + (x * cellSize);

                var startY = _currentWorldPosition.Y;

                var endX = startX;

                var endY = _currentWorldPosition.Y + (_towerData.GridSize.Y * cellSize);



                RenderSystem.DrawLine(

                    new Vector3(startX, startY, 0),

                    new Vector3(endX, endY, 0),

                    Color.Red, 1f

                );

            }



            for (int y = 0; y <= _towerData.GridSize.Y; y++)

            {

                var startX = _currentWorldPosition.X;

                var startY = _currentWorldPosition.Y + (y * cellSize);

                var endX = _currentWorldPosition.X + (_towerData.GridSize.X * cellSize);

                var endY = startY;



                RenderSystem.DrawLine(

                    new Vector3(startX, startY, 0),

                    new Vector3(endX, endY, 0),

                    Color.Green, 1f

                );

            }

        }



        /// <summary>

        /// Render occupancy preview.

        /// </summary>

        private void RenderOccupancyPreview()

        {

            var grid = _grid;

            if (grid == null) return;



            var cellSize = grid.CellSize;

            var occupancyColor = _canPlace ? _validColor : _invalidColor;

            var alpha = (byte)(occupancyColor.A * _previewAlpha);

            var finalColor = System.Drawing.Color.FromArgb(alpha, occupancyColor.R, occupancyColor.G, occupancyColor.B);



            // Render occupied cells

            for (int x = 0; x < _towerData.GridSize.X; x++)

            {

                for (int y = 0; y < _towerData.GridSize.Y; y++)

                {

                    var cellX = _currentWorldPosition.X + (x * cellSize) + (cellSize / 2f);

                    var cellY = _currentWorldPosition.Y + (y * cellSize) + (cellSize / 2f);

                    var cellPos = new Vector3(cellX, cellY, 0);



                    // Add pulse effect

                    var pulse = 1f + (MathF.Sin(_pulseTime) * _pulseAmount);

                    var size = cellSize * 0.8f * pulse;



                    RenderSystem.DrawRectangle(

                        cellX - (size / 2f), cellY - (size / 2f),

                        size, size, Color.Blue

                    );

                }

            }

        }



        /// <summary>

        /// Render range indicator.

        /// </summary>

        private void RenderRangeIndicator()

        {

            if (_towerData.Range <= 0) return;



            var rangeColor = System.Drawing.Color.FromArgb((byte)(_rangeColor.A * _rangeAlpha), _rangeColor.R, _rangeColor.G, _rangeColor.B);



            // Calculate pulse for range indicator

            var rangePulse = 1f + (MathF.Sin(_pulseTime * 0.5f) * _pulseAmount * 0.5f);

            var currentRange = _towerData.Range * rangePulse;



            // Render range circle

            RenderSystem.DrawCircle(

                _currentWorldPosition.X, _currentWorldPosition.Y,

                currentRange, rangeColor, 2f

            );



            // Render range segments (for visual effect)

            RenderRangeSegments(currentRange, rangeColor);

        }



        /// <summary>

        /// Render animated range segments.

        /// </summary>

        private void RenderRangeSegments(float range, System.Drawing.Color color)

        {

            const int segmentCount = 8;

            const float segmentLength = 0.3f; // 30% of circumference



            for (int i = 0; i < segmentCount; i++)

            {

                var startAngle = (i * (2f * MathF.PI / segmentCount)) + _rangeRotation;

                var endAngle = startAngle + (segmentLength * (2f * MathF.PI / segmentCount));



                var startPoint = new Vector3(

                    _currentWorldPosition.X + MathF.Cos(startAngle) * range,

                    _currentWorldPosition.Y + MathF.Sin(startAngle) * range,

                    0

                );



                var endPoint = new Vector3(

                    _currentWorldPosition.X + MathF.Cos(endAngle) * range,

                    _currentWorldPosition.Y + MathF.Sin(endAngle) * range,

                    0

                );



                RenderSystem.DrawLine(startPoint, endPoint, color, 3f);

            }

        }



        /// <summary>

        /// Render tower preview.

        /// </summary>

        private void RenderTowerPreview()

        {

            if (_towerSprite == null) return;



            var previewColor = _canPlace ? _validColor : _invalidColor;

            var alpha = (byte)(previewColor.A * _previewAlpha);

            var finalColor = System.Drawing.Color.FromArgb(alpha, previewColor.R, previewColor.G, previewColor.B);



            // Calculate tower position (center of grid cells)

            var grid = _grid;

            var cellSize = grid?.CellSize ?? 1f;

            var towerX = _currentWorldPosition.X + (_towerData.GridSize.X * cellSize / 2f);

            var towerY = _currentWorldPosition.Y + (_towerData.GridSize.Y * cellSize / 2f);

            var towerPos = new Vector3(towerX, towerY, 0);



            // Add subtle hover effect

            var hover = MathF.Sin(_pulseTime * 2f) * 0.05f;

            towerPos = new Vector3(towerPos.X, towerPos.Y + hover, towerPos.Z);




            // Render tower sprite - ensure size is a Vector3

            RenderSystem.DrawSprite(_towerSprite, towerPos, _towerData.Size.ToVector3(), finalColor);



            // Render tower base

            RenderTowerBase(towerPos, finalColor);

        }



        /// <summary>

        /// Render tower base/foundation.

        /// </summary>

        private void RenderTowerBase(Vector3 position, System.Drawing.Color color)

        {

            var baseSize = _towerData.Size.ToVector3() * 0.8f;

            var baseColor = System.Drawing.Color.FromArgb((byte)(color.A * 0.5f), color.R, color.G, color.B);



            // Render foundation

            RenderSystem.DrawRectangle(

                position.X - (baseSize.X / 2f),

                position.Y - (baseSize.Y / 4f),

                baseSize.X, baseSize.Y / 2f, baseColor

            );



            // Render foundation corners

            var cornerSize = new Vector3(baseSize.X * 0.1f, baseSize.Y * 0.1f, 0f);

            var cornerOffsetX = baseSize.X / 2f - cornerSize.X;

            var cornerOffsetY = baseSize.Y / 2f - cornerSize.Y;



            // Top-left corner

            RenderSystem.DrawRectangle(

                position.X - cornerOffsetX,

                position.Y - cornerOffsetY,

                cornerSize.X, cornerSize.Y, color

            );



            // Top-right corner

            RenderSystem.DrawRectangle(

                position.X + cornerOffsetX - cornerSize.X,

                position.Y - cornerOffsetY,

                cornerSize.X, cornerSize.Y, color

            );



            // Bottom-left corner

            RenderSystem.DrawRectangle(

                position.X - cornerOffsetX,

                position.Y + cornerOffsetY - cornerSize.Y,

                cornerSize.X, cornerSize.Y, color

            );



            // Bottom-right corner

            RenderSystem.DrawRectangle(

                position.X + cornerOffsetX - cornerSize.X,

                position.Y + cornerOffsetY - cornerSize.Y,

                cornerSize.X, cornerSize.Y, color

            );

        }



        /// <summary>

        /// Render placement feedback (icons, text, etc.).

        /// </summary>

        private void RenderPlacementFeedback()

        {

            var feedbackPosition = new Vector3(

                _currentWorldPosition.X + (_towerData.GridSize.X * 0.5f),

                _currentWorldPosition.Y - 0.5f,

                0

            );



            if (_canPlace)

            {

                // Render checkmark for valid placement

                RenderValidPlacementIcon(feedbackPosition);

            }

            else

            {

                // Render X for invalid placement

                RenderInvalidPlacementIcon(feedbackPosition);

            }



            // Render cost indicator

            RenderCostIndicator(feedbackPosition);

        }



        /// <summary>

        /// Render valid placement icon.

        /// </summary>

        private void RenderValidPlacementIcon(Vector3 position)

        {

            var iconColor = System.Drawing.Color.FromArgb((byte)(200 * _previewAlpha), 0, 255, 0);

            var iconSize = 0.3f;



            // Render checkmark

            var checkmarkColor = System.Drawing.Color.FromArgb((byte)(iconColor.A * 0.8f), iconColor.R, iconColor.G, iconColor.B);

            RenderSystem.DrawCheckmark(position, new Color(checkmarkColor.R, checkmarkColor.G, checkmarkColor.B, checkmarkColor.A), iconSize);

        }



        /// <summary>

        /// Render invalid placement icon.

        /// </summary>

        private void RenderInvalidPlacementIcon(Vector3 position)

        {

            var iconColor = System.Drawing.Color.FromArgb((byte)(200 * _previewAlpha), 255, 0, 0);

            var iconSize = 0.3f;



            // Render X

            var xColor = System.Drawing.Color.FromArgb((byte)(iconColor.A * 0.8f), iconColor.R, iconColor.G, iconColor.B);

            RenderSystem.DrawX(position, new Color(xColor.R, xColor.G, xColor.B, xColor.A), iconSize);

        }



        /// <summary>

        /// Render cost indicator.

        /// </summary>

        private void RenderCostIndicator(Vector3 position)

        {

            // Economy system not available - always show as affordable

            var canAfford = true;

            var costColor = canAfford ? System.Drawing.Color.Yellow : System.Drawing.Color.Red;

            var costAlpha = (byte)(180 * _previewAlpha);

            var finalCostColor = System.Drawing.Color.FromArgb(costAlpha, costColor.R, costColor.G, costColor.B);



            // Render cost text

            var costText = $"${_towerData.Cost}";

            var font = FontCache.GetFont("small");

            var textPosition = new Vector3(position.X, position.Y - 0.3f, 0);



            RenderSystem.DrawString(costText, textPosition, new Color(finalCostColor.R, finalCostColor.G, finalCostColor.B, finalCostColor.A), 1.0f);

        }



        /// <summary>

        /// Set rendering options.

        /// </summary>

        public void SetRenderingOptions(bool showRange, bool showGrid, bool showOccupancy)

        {

            _showRange = showRange;

            _showGrid = showGrid;

            _showOccupancy = showOccupancy;

        }



        /// <summary>

        /// Set rendering colors.

        /// </summary>

        public void SetColors(System.Drawing.Color validColor, System.Drawing.Color invalidColor, System.Drawing.Color rangeColor)

        {

            _validColor = validColor;

            _invalidColor = invalidColor;

            _rangeColor = rangeColor;

        }



        /// <summary>

        /// Set transparency values.

        /// </summary>

        public void SetTransparency(float previewAlpha, float rangeAlpha, float gridAlpha)

        {

            _previewAlpha = System.Math.Clamp(previewAlpha, 0f, 1f);

            _rangeAlpha = System.Math.Clamp(rangeAlpha, 0f, 1f);

            _gridAlpha = System.Math.Clamp(gridAlpha, 0f, 1f);

        }



        /// <summary>

        /// Set animation speed.

        /// </summary>

        public void SetAnimationSpeed(float pulseSpeed, float rangeRotationSpeed)

        {

            _pulseSpeed = pulseSpeed;

            _rangeRotationSpeed = rangeRotationSpeed;

        }



        /// <summary>

        /// Cleanup renderer resources.

        /// </summary>

        public void Cleanup()

        {

            _towerSprite = null;

            _rangeSprite = null;

            _towerData = null;

            _isInitialized = false;

            _grid = null;

        }

    }



    /// <summary>

    /// Extension methods for RenderSystem.

    /// </summary>

    public static class RenderSystemExtensions

    {

        /// <summary>

        /// Draw a checkmark icon.

        /// </summary>

        public static void DrawCheckmark(Vector3 position, float size, System.Drawing.Color color)

        {

            var lineColor = System.Drawing.Color.FromArgb((byte)(color.A * 0.8f), color.R, color.G, color.B);


            var lineWidth = size * 0.15f;



            // Draw checkmark lines

            var start1 = new Vector3(position.X - size * 0.3f, position.Y - size * 0.1f, 0);

            var end1 = new Vector3(position.X - size * 0.1f, position.Y + size * 0.1f, 0);

            RenderSystem.DrawLine(start1, end1, lineColor, lineWidth);



            var start2 = new Vector3(position.X - size * 0.1f, position.Y + size * 0.1f, 0);

            var end2 = new Vector3(position.X + size * 0.3f, position.Y - size * 0.3f, 0);

            RenderSystem.DrawLine(start2, end2, lineColor, lineWidth);

        }



        /// <summary>

        /// Draw an X icon.

        /// </summary>

        public static void DrawX(Vector3 position, float size, System.Drawing.Color color)

        {

            var lineColor = System.Drawing.Color.FromArgb((byte)(color.A * 0.8f), color.R, color.G, color.B);


            var lineWidth = size * 0.15f;



            // Draw X lines

            var start1 = new Vector3(position.X - size * 0.3f, position.Y - size * 0.3f, 0);

            var end1 = new Vector3(position.X + size * 0.3f, position.Y + size * 0.3f, 0);

            RenderSystem.DrawLine(start1, end1, Color.Yellow, lineWidth);



            var start2 = new Vector3(position.X - size * 0.3f, position.Y + size * 0.3f, 0);

            var end2 = new Vector3(position.X + size * 0.3f, position.Y - size * 0.3f, 0);

            RenderSystem.DrawLine(start2, end2, Color.Yellow, lineWidth);

        }

    }

}

