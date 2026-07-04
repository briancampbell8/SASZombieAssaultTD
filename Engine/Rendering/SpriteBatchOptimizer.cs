using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering
//
{
    ///<summary>
    ///Optimized sprite batch with advanced batching and grouping.
    ///P30-03-01: Add sprite batching optimization.
    ///P30-03-03: Reduce draw calls via grouping.
    ///</summary>
    public class SpriteBatchOptimizer
    {
        private readonly Dictionary<IntPtr, List<OptimizedSpriteCommand>> _textureGroups;
        private readonly List<OptimizedSpriteCommand> _commands;
        private readonly Dictionary<IntPtr, TextureAtlas> _textureAtlases;
        private readonly FrustumCuller _frustumCuller;
        private bool _isDrawing;
        private SpriteSortMode _sortMode;
        private BlendState _blendState;
        private SamplerState _samplerState;
        private Matrix3x2 _transformMatrix;
        private Rectangle _viewport;
        private int _drawCalls;
        private int _culledSprites;
        private int _batchedSprites;

        ///<summary>
        ///Gets whether the sprite batch is currently drawing.
        ///</summary>
        public bool IsDrawing => _isDrawing;

        ///<summary>
        ///Gets the number of draw calls in the last frame.
        ///</summary>
        public int DrawCalls => _drawCalls;

        ///<summary>
        ///Gets the number of sprites culled in the last frame.
        ///</summary>
        public int CulledSprites => _culledSprites;

        ///<summary>
        ///Gets the number of sprites batched in the last frame.
        ///</summary>
        public int BatchedSprites => _batchedSprites;

        ///<summary>
        ///Gets the current viewport.
        ///</summary>
        public Rectangle Viewport => _viewport;

        ///<summary>
        ///Event fired when performance metrics are updated.
        ///</summary>
        public event Action<SpriteBatchOptimizer> OnPerformanceUpdated;

        ///<summary>
        ///Initializes a new sprite batch optimizer.
        ///</summary>
        public SpriteBatchOptimizer()
        {
            _textureGroups = new Dictionary<IntPtr, List<OptimizedSpriteCommand>>();
            _commands = new List<OptimizedSpriteCommand>();
            _textureAtlases = new Dictionary<IntPtr, TextureAtlas>();
            _frustumCuller = new FrustumCuller();
            _isDrawing = false;
            _sortMode = SpriteSortMode.Deferred;
            _blendState = BlendState.AlphaBlend;
            _samplerState = SamplerState.LinearClamp;
            _transformMatrix = Matrix3x2.Identity;
            _viewport = new Rectangle(0, 0, 800, 600);
            _drawCalls = 0;
            _culledSprites = 0;
            _batchedSprites = 0;

            DLogger.Log(LogSubsystems.Rendering, LogLevel.Info, "SpriteBatchOptimizer: Initialized");
        }

        ///<summary>
        ///Begins a sprite batch drawing session.
        ///</summary>
        ///<param name="sortMode">The sort mode for sprites.</param>
        ///<param name="blendState">The blend state.</param>
        ///<param name="samplerState">The sampler state.</param>
        ///<param name="transformMatrix">The transform matrix.</param>
        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState blendState = null,
        SamplerState samplerState = null,
        Matrix3x2? transformMatrix = null)
        {
            blendState ??= BlendState.AlphaBlend;
            samplerState ??= SamplerState.LinearClamp;
            if (_isDrawing)
            {
                DLogger.Log(LogSubsystems.Rendering, LogLevel.Warning, "SpriteBatchOptimizer: Begin called while already drawing");
                return;
            }

            _sortMode = sortMode;
            _blendState = blendState;
            _samplerState = samplerState;
            _transformMatrix = transformMatrix ?? Matrix3x2.Identity;
            _commands.Clear();
            _textureGroups.Clear();
            _drawCalls = 0;
            _culledSprites = 0;
            _batchedSprites = 0;
            _isDrawing = true;

            DLogger.Log(LogSubsystems.Rendering, LogLevel.Debug, $"SpriteBatchOptimizer: Began drawing (SortMode: {_sortMode})");
        }

        ///<summary>
        ///Ends the sprite batch drawing session and flushes optimized commands.
        ///</summary>
        public void End()
        {
            if (!_isDrawing)
            {
                DLogger.Log(LogSubsystems.Rendering, LogLevel.Warning, "SpriteBatchOptimizer: End called while not drawing");
                return;
            }

            try
            {
                //Optimize and flush commands
                OptimizeCommands();
                FlushOptimizedCommands();

                _isDrawing = false;
                DLogger.Log(LogSubsystems.Rendering, LogLevel.Debug, $"SpriteBatchOptimizer: Ended drawing (DrawCalls: {_drawCalls}, Batched: {_batchedSprites}, Culled: {_culledSprites})");

                OnPerformanceUpdated?.Invoke(this);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Rendering, LogLevel.Error, $"SpriteBatchOptimizer: Failed to end drawing - {ex.Message}");
                _isDrawing = false;
            }
        }

        ///<summary>
        ///Draws an optimized sprite.
        ///</summary>
        ///<param name="texture">The texture to draw.</param>
        ///<param name="position">The position to draw at.</param>
        ///<param name="sourceRect">Source rectangle (optional).</param>
        ///<param name="color">The color tint.</param>
        ///<param name="rotation">Rotation in radians.</param>
        ///<param name="origin">Origin point.</param>
        ///<param name="scale">Scale factor.</param>
        ///<param name="effects">Sprite effects.</param>
        ///<param name="layerDepth">Layer depth for sorting.</param>
        public void Draw(IntPtr texture, Vector3 position, Rectangle? sourceRect = null,
        Color? color = null, float rotation = 0f, Vector3? origin = null,
        Vector3? scale = null, SpriteEffects effects = SpriteEffects.None,
        float layerDepth = 0f)
        {
            if (!_isDrawing || texture == IntPtr.Zero)
                return;

            var command = new OptimizedSpriteCommand
            {
                Texture = texture,
                Position = position,
                SourceRect = sourceRect,
                Color = color ?? Color.White,
                Rotation = rotation,
                Origin = origin ?? Vector3.Zero,
                Scale = scale ?? Vector3.One,
                Effects = effects,
                LayerDepth = System.Math.Clamp(layerDepth, 0f, 1f)
            };

            //Apply transform
            var matrix4x4 = new System.Numerics.Matrix4x4(
                _transformMatrix.M11, _transformMatrix.M12, 0, 0,
                _transformMatrix.M21, _transformMatrix.M22, 0, 0,
                0, 0, 1, 0,
                _transformMatrix.M31, _transformMatrix.M32, 0, 1);
            command.Position = Vector3.Transform(command.Position, matrix4x4);

            //Frustum culling
            if (!_frustumCuller.IsVisible(command, _viewport))
            {
                _culledSprites++;
                return;
            }

            _commands.Add(command);
        }

        ///<summary>
        ///Sets the viewport for culling.
        ///</summary>
        ///<param name="viewport">The viewport rectangle.</param>
        public void SetViewport(Rectangle viewport)
        {
            _viewport = viewport;
            _frustumCuller.SetViewport(viewport);
        }

        ///<summary>
        ///Registers a texture atlas.
        ///P30-03-02: Add texture atlas support.
        ///</summary>
        ///<param name="texture">The texture atlas.</param>
        ///<param name="atlas">The atlas data.</param>
        public void RegisterTextureAtlas(IntPtr texture, TextureAtlas atlas)
        {
            _textureAtlases[texture] = atlas;
            DLogger.Log(LogSubsystems.Rendering, LogLevel.Debug, $"SpriteBatchOptimizer: Registered texture atlas");
        }

        ///<summary>
        ///Optimizes sprite commands.
        ///</summary>
        private void OptimizeCommands()
        {
            //Group by texture
            _textureGroups.Clear();
            foreach (var command in _commands)
            {
                if (!_textureGroups.ContainsKey(command.Texture))
                {
                    _textureGroups[command.Texture] = new List<OptimizedSpriteCommand>();
                }
                _textureGroups[command.Texture].Add(command);
            }

            //Sort within groups if needed
            if (_sortMode != SpriteSortMode.Deferred)
            {
                foreach (var group in _textureGroups.Values)
                {
                    SortCommands(group);
                }
            }

            //Apply texture atlas optimization
            OptimizeTextureAtlases();

            _batchedSprites = _commands.Count;
        }

        ///<summary>
        ///Sorts commands based on sort mode.
        ///</summary>
        ///<param name="commands">Commands to sort.</param>
        private void SortCommands(List<OptimizedSpriteCommand> commands)
        {
            switch (_sortMode)
            {
                case SpriteSortMode.Texture:
                    //Already grouped by texture
                    break;
                case SpriteSortMode.BackToFront:
                    commands.Sort((a, b) => a.LayerDepth.CompareTo(b.LayerDepth));
                    break;
                case SpriteSortMode.FrontToBack:
                    commands.Sort((a, b) => b.LayerDepth.CompareTo(a.LayerDepth));
                    break;
                case SpriteSortMode.Immediate:
                    //No sorting needed
                    break;
            }
        }

        ///<summary>
        ///Optimizes commands using texture atlases.
        ///</summary>
        private void OptimizeTextureAtlases()
        {
            foreach (var kvp in _textureGroups.ToList())
            {
                var texture = kvp.Key;
                var commands = kvp.Value;

                if (_textureAtlases.TryGetValue(texture, out var atlas))
                {
                    //Optimize commands using atlas
                    foreach (var command in commands)
                    {
                        if (command.SourceRect.HasValue)
                        {
                            //Convert source rect to atlas coordinates
                            var atlasRect = atlas.GetAtlasRect(command.SourceRect.Value);
                            if (atlasRect.HasValue)
                            {
                                command.SourceRect = atlasRect;
                            }
                        }
                    }
                }
            }
        }

        ///<summary>
        ///Flushes optimized commands to renderer.
        ///</summary>
        private void FlushOptimizedCommands()
        {
            _drawCalls = 0;

            foreach (var kvp in _textureGroups)
            {
                var texture = kvp.Key;
                var commands = kvp.Value;

                if (commands.Count == 0)
                    continue;

                //Set texture state
                //This would bind the texture to the rendering pipeline

                //Batch render commands
                RenderBatch(texture, commands);
                _drawCalls++;
            }
        }

        ///<summary>
        ///Renders a batch of sprites.
        ///</summary>
        ///<param name="texture">The texture to use.</param>
        ///<param name="commands">The commands to render.</param>
        private void RenderBatch(IntPtr texture, List<OptimizedSpriteCommand> commands)
        {
            //This would render all sprites in the batch with a single draw call
            //For now, we'll just log the operation
           DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", $"SpriteBatchOptimizer: Rendered batch with {commands.Count} sprites");
        }

        ///<summary>
        ///Gets performance statistics.
        ///</summary>
        ///<returns>Performance statistics.</returns>
        public SpriteBatchStatistics GetStatistics()
        {
            return new SpriteBatchStatistics
            {
                DrawCalls = _drawCalls,
                SpritesDrawn = _commands.Count,
                CulledSprites = _culledSprites,
                BatchedSprites = _batchedSprites,
                TextureGroups = _textureGroups.Count,
                SortMode = _sortMode,
                BlendState = _blendState,
                SamplerState = _samplerState
            };
        }

        ///<summary>
        ///Gets sprite batch optimizer information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"SpriteBatchOptimizer: Drawing={_isDrawing}, DrawCalls={_drawCalls}, " +
            $"Sprites={_commands.Count}, Culled={_culledSprites}, " +
            $"Batched={_batchedSprites}, Groups={_textureGroups.Count}";
        }
    }

    ///<summary>
    ///Optimized sprite command with additional data for batching.
    ///</summary>
    public class OptimizedSpriteCommand
    {
        public IntPtr Texture { get; set; }
        public Vector3 Position { get; set; }
        public Rectangle? SourceRect { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteEffects Effects { get; set; }
        public float LayerDepth { get; set; }
        public Rectangle Bounds { get; set; }
    }

    ///<summary>
    ///Frustum culler for off-screen sprite optimization.
    ///P30-03-04: Add frustum culling for off-screen sprites.
    ///</summary>
    public class FrustumCuller
    {
        private Rectangle _viewport;

        ///<summary>
        ///Sets the viewport for culling.
        ///</summary>
        ///<param name="viewport">The viewport rectangle.</param>
        public void SetViewport(Rectangle viewport)
        {
            _viewport = viewport;
        }

        ///<summary>
        ///Checks if a sprite command is visible.
        ///</summary>
        ///<param name="command">The sprite command.</param>
        ///<param name="viewport">The viewport rectangle.</param>
        ///<returns>True if the sprite is visible.</returns>
        public bool IsVisible(OptimizedSpriteCommand command, Rectangle viewport)
        {
            //Calculate sprite bounds
            var size = command.SourceRect.HasValue ?
            new Vector3(command.SourceRect.Value.Width, command.SourceRect.Value.Height, 0f) :
            new Vector3(64f, 64f, 0f); //Default size

            var scaledSize = size * command.Scale;
            var bounds = new Rectangle(
            (int)(command.Position.X - command.Origin.X),
            (int)(command.Position.Y - command.Origin.Y),
            (int)scaledSize.X,
            (int)scaledSize.Y
            );

            command.Bounds = bounds;

            //Check if bounds intersect with viewport
            return bounds.IntersectsWith(viewport);
        }
    }

    ///<summary>
    ///Texture atlas for texture optimization.
    ///</summary>
    public class TextureAtlas
    {
        private readonly Dictionary<Rectangle, Rectangle> _rectMappings;

        public TextureAtlas()
        {
            _rectMappings = new Dictionary<Rectangle, Rectangle>();
        }

        ///<summary>
        ///Gets the atlas rectangle for a source rectangle.
        ///</summary>
        ///<param name="sourceRect">The source rectangle.</param>
        ///<returns>The atlas rectangle, or null if not found.</returns>
        public Rectangle? GetAtlasRect(Rectangle sourceRect)
        {
            return _rectMappings.TryGetValue(sourceRect, out var atlasRect) ? atlasRect : (Rectangle?)null;
        }

        ///<summary>
        ///Adds a rectangle mapping.
        ///</summary>
        ///<param name="sourceRect">The source rectangle.</param>
        ///<param name="atlasRect">The atlas rectangle.</param>
        public void AddMapping(Rectangle sourceRect, Rectangle atlasRect)
        {
            _rectMappings[sourceRect] = atlasRect;
        }
    }

    ///<summary>
    ///Sprite batch statistics.
    ///</summary>
    public class SpriteBatchStatistics
    {
        public int DrawCalls { get; set; }
        public int SpritesDrawn { get; set; }
        public int CulledSprites { get; set; }
        public int BatchedSprites { get; set; }
        public int TextureGroups { get; set; }
        public SpriteSortMode SortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }

        public override string ToString()
        {
            return $"SpriteBatch: DrawCalls={DrawCalls}, Sprites={SpritesDrawn}, " +
            $"Culled={CulledSprites}, Batched={BatchedSprites}, Groups={TextureGroups}";
        }
    }
}




