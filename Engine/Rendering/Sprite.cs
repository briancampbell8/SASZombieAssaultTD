using System;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Sprite class for 2D rendering with texture and transform properties.
    /// P20-05-02: Implements sprite with texture, position, and size.
    /// </summary>
    public class Sprite
    {
        private Texture2D _texture;
        private Vector3 _position;
        private Vector3 _size;
        private Rectangle? _sourceRect;
        private Color _color;
        private float _rotation;
        private Vector3 _origin;
        private Vector3 _scale;
        private float _layerDepth;
        private bool _isVisible;

        /// <summary>
        /// Gets or sets the texture for the sprite.
        /// </summary>
        public Texture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                UpdateSizeFromTexture();
            }
        }

        /// <summary>
        /// Gets or sets the position of the sprite.
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Gets or sets the size of the sprite.
        /// </summary>
        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// Gets or sets the source rectangle for texture sampling.
        /// </summary>
        public Rectangle? SourceRect
        {
            get => _sourceRect;
            set => _sourceRect = value;
        }

        /// <summary>
        /// Gets or sets the color tint of the sprite.
        /// </summary>
        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        /// <summary>
        /// Gets or sets the rotation of the sprite (in radians).
        /// </summary>
        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// Gets or sets the origin point of the sprite.
        /// </summary>
        public Vector3 Origin
        {
            get => _origin;
            set => _origin = value;
        }

        /// <summary>
        /// Gets or sets the scale of the sprite.
        /// </summary>
        public Vector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        /// <summary>
        /// Gets or sets the layer depth for rendering order (0.0 to 1.0).
        /// </summary>
        public float LayerDepth
        {
            get => _layerDepth;
            set => _layerDepth = System.Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets whether the sprite is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        /// <summary>
        /// Gets the bounding rectangle of the sprite.
        /// </summary>
        public Rectangle Bounds => Rectangle.FromPositionAndSize(_position.X, _position.Y, _size.X, _size.Y);

        /// <summary>
        /// Gets the center position of the sprite.
        /// </summary>
        public Vector3 Center => _position + _size / 2;

        /// <summary>
        /// Event fired when sprite properties change.
        /// </summary>
        public event Action<Sprite> OnSpriteChanged;

        /// <summary>
        /// Initializes a new sprite.
        /// </summary>
        /// <param name="texture">The texture for the sprite.</param>
        /// <param name="position">The initial position.</param>
        /// <param name="size">The initial size.</param>
        public Sprite(Texture2D? texture = null, Vector3? position = null, Vector3? size = null)
        {
            _texture = texture;
            _position = position ?? Vector3.Zero;
            _size = size ?? Vector3.Zero;
            _sourceRect = null;
            _color = Color.White;
            _rotation = 0f;
            _origin = Vector3.Zero;
            _scale = Vector3.One;
            _layerDepth = 0f;
            _isVisible = true;

            UpdateSizeFromTexture();
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Sprite: Created with texture '{_texture?.FilePath ?? "None"}' at {_position}");
        }

        /// <summary>
        /// Draws the sprite using a sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible || _texture == null || spriteBatch == null)
                return;

            // Use source rect if specified, otherwise use full texture
            var sourceRect = _sourceRect ?? Rectangle.FromPositionAndSize(0, 0, _texture.Width, _texture.Height);

            // Calculate effective size (texture size * scale)
            var effectiveSize = new Vector3(
            sourceRect.Width * _scale.X,
            sourceRect.Height * _scale.Y,
            0f
            );

            // Draw the sprite
            spriteBatch.Draw(
            _texture.Handle(),
            new System.Numerics.Vector3(_position.X, _position.Y, _position.Z),
            sourceRect,
            _color,
            _rotation,
            new System.Numerics.Vector3(_origin.X, _origin.Y, _origin.Z),
            new System.Numerics.Vector3(_scale.X, _scale.Y, _scale.Z),
            SpriteEffects.None,
            _layerDepth
            );
        }

        /// <summary>
        /// Sets the sprite to use the full texture.
        /// </summary>
        public void UseFullTexture()
        {
            if (_texture != null)
            {
                _sourceRect = null;
                UpdateSizeFromTexture();
                OnSpriteChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the sprite to use a specific region of the texture.
        /// P20-05-05: Add UV support for spritesheets.
        /// </summary>
        /// <param name="x">X coordinate in texture pixels.</param>
        /// <param name="y">Y coordinate in texture pixels.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public void SetTextureRegion(int x, int y, int width, int height)
        {
            _sourceRect = Rectangle.FromPositionAndSize(x, y, width, height);
            _size = new Vector3(width, height, 0f);
            OnSpriteChanged?.Invoke(this);
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Sprite: Set texture region ({x}, {y}, {width}, {height})");
        }

        /// <summary>
        /// Sets the sprite to use a specific UV region.
        /// </summary>
        /// <param name="u">U coordinate (0.0 to 1.0).</param>
        /// <param name="v">V coordinate (0.0 to 1.0).</param>
        /// <param name="width">Width in UV units.</param>
        /// <param name="height">Height in UV units.</param>
        public void SetUVRegion(float u, float v, float width, float height)
        {
            if (_texture == null)
                return;

            // Convert UV coordinates to pixel coordinates
            var pixelX = (int)(u * _texture.Width);
            var pixelY = (int)(v * _texture.Height);
            var pixelWidth = (int)(width * _texture.Width);
            var pixelHeight = (int)(height * _texture.Height);

            SetTextureRegion(pixelX, pixelY, pixelWidth, pixelHeight);
        }

        /// <summary>
        /// Sets the origin to the center of the sprite.
        /// </summary>
        public void SetOriginToCenter()
        {
            _origin = _size / 2;
            OnSpriteChanged?.Invoke(this);
        }

        /// <summary>
        /// Moves the sprite by the specified offset.
        /// </summary>
        /// <param name="offset">The offset to move by.</param>
        public void Move(Vector3 offset)
        {
            _position += offset;
            OnSpriteChanged?.Invoke(this);
        }

        /// <summary>
        /// Rotates the sprite by the specified angle.
        /// </summary>
        /// <param name="angle">The angle to rotate by (in radians).</param>
        public void Rotate(float angle)
        {
            _rotation += angle;
            OnSpriteChanged?.Invoke(this);
        }

        /// <summary>
        /// Scales the sprite by the specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        public void ScaleBy(Vector3 scale)
        {
            _scale *= scale;
            OnSpriteChanged?.Invoke(this);
        }

        /// <summary>
        /// Sets the sprite to a specific size while maintaining aspect ratio.
        /// </summary>
        /// <param name="targetSize">The target size.</param>
        public void SetSizeMaintainAspect(Vector3 targetSize)
        {
            if (_texture == null)
                return;

            var sourceRect = _sourceRect ?? Rectangle.FromPositionAndSize(0, 0, _texture.Width, _texture.Height);
            var aspectRatio = (float)sourceRect.Width / sourceRect.Height;
            var targetAspectRatio = targetSize.X / targetSize.Y;

            Vector3 newSize;
            if (aspectRatio > targetAspectRatio)
            {
                // Width is limiting factor
                newSize = new Vector3(targetSize.X, targetSize.X / aspectRatio, 0f);
            }
            else
            {
                // Height is limiting factor
                newSize = new Vector3(targetSize.Y * aspectRatio, targetSize.Y, 0f);
            }

            _size = newSize;
            OnSpriteChanged?.Invoke(this);
        }

        /// <summary>
        /// Checks if this sprite intersects with another sprite.
        /// </summary>
        /// <param name="other">The other sprite to check.</param>
        /// <returns>True if sprites intersect.</returns>
        public bool Intersects(Sprite other)
        {
            if (other == null || !_isVisible || !other._isVisible)
                return false;

            return Bounds.IntersectsWith(other.Bounds);
        }

        /// <summary>
        /// Checks if a point is within the sprite bounds.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if point is within bounds.</returns>
        public bool ContainsPoint(Vector3 point)
        {
            if (!_isVisible)
                return false;

            return Bounds.Contains(new Vector3(point.X, point.Y, 0));
        }

        /// <summary>
        /// Updates the sprite size based on the current texture.
        /// </summary>
        private void UpdateSizeFromTexture()
        {
            if (_texture != null && _size == Vector3.Zero)
            {
                var sourceRect = _sourceRect ?? Rectangle.FromPositionAndSize(0, 0, _texture.Width, _texture.Height);
                _size = new Vector3(sourceRect.Width, sourceRect.Height, 0f);
            }
        }

        /// <summary>
        /// Creates a copy of this sprite.
        /// </summary>
        /// <returns>A new sprite with the same properties.</returns>
        public Sprite Clone()
        {
            return new Sprite(_texture, _position, _size)
            {
                _sourceRect = _sourceRect,
                _color = _color,
                _rotation = _rotation,
                _origin = _origin,
                _scale = _scale,
                _layerDepth = _layerDepth,
                _isVisible = _isVisible
            };
        }

        /// <summary>
        /// Gets sprite information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"Sprite: Texture='{_texture?.FilePath ?? "None"}', Pos={_position}, " +
            $"Size={_size}, Visible={_isVisible}, Layer={_layerDepth:F2}";
        }
    }
}




