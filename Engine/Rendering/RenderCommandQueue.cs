using System;
using System.Collections.Generic;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Render command queue for batching rendering operations.
    /// P20-04-06: Optional batching system placeholder for performance optimization.
    /// </summary>
    public class RenderCommandQueue
    {
        private readonly Queue<IRenderCommand> _commandQueue;
        private int _maxQueueSize;
        private bool _batchingEnabled;

        /// <summary>
        /// Gets the number of commands in the queue.
        /// </summary>
        public int CommandCount => _commandQueue.Count;

        /// <summary>
        /// Gets whether batching is enabled.
        /// </summary>
        public bool BatchingEnabled => _batchingEnabled;

        /// <summary>
        /// Gets the maximum queue size.
        /// </summary>
        public int MaxQueueSize => _maxQueueSize;

        /// <summary>
        /// Initializes a new render command queue.
        /// </summary>
        /// <param name="maxQueueSize">Maximum number of commands to store.</param>
        /// <param name="enableBatching">Whether to enable command batching.</param>
        public RenderCommandQueue(int maxQueueSize = 1000, bool enableBatching = true)
        {
            _commandQueue = new Queue<IRenderCommand>();
            _maxQueueSize = System.Math.Max(1, maxQueueSize);
            _batchingEnabled = enableBatching;

            ModernLoggingSystem.Log("INFO", $"RenderCommandQueue: Initialized with max size {_maxQueueSize}, batching={_batchingEnabled}");
        }

        /// <summary>
        /// Adds a render command to the queue.
        /// </summary>
        /// <param name="command">The render command to add.</param>
        /// <returns>True if command was added, false if queue is full.</returns>
        public bool EnqueueCommand(IRenderCommand command)
        {
            if (command == null)
            {
                ModernLoggingSystem.Log("WARNING", "RenderCommandQueue: Cannot enqueue null command");
                return false;
            }

            if (_commandQueue.Count >= _maxQueueSize)
            {
                ModernLoggingSystem.Log("WARNING", "RenderCommandQueue: Queue is full, dropping command");
                return false;
            }

            _commandQueue.Enqueue(command);
            ModernLoggingSystem.Log("TRACE", $"RenderCommandQueue: Enqueued {command.GetType().Name}");

            return true;
        }

        /// <summary>
        /// Gets the next render command from the queue.
        /// </summary>
        /// <returns>The next render command, or null if queue is empty.</returns>
        public IRenderCommand DequeueCommand()
        {
            if (_commandQueue.Count == 0)
                return null;

            var command = _commandQueue.Dequeue();
            ModernLoggingSystem.Log("TRACE", $"RenderCommandQueue: Dequeued {command.GetType().Name}");

            return command;
        }

        /// <summary>
        /// Clears all commands from the queue.
        /// </summary>
        public void ClearQueue()
        {
            var count = _commandQueue.Count;
            _commandQueue.Clear();
            ModernLoggingSystem.Log("DEBUG", $"RenderCommandQueue: Cleared {count} commands");
        }

        /// <summary>
        /// Processes all commands in the queue.
        /// </summary>
        /// <param name="renderer">The renderer to execute commands on.</param>
        /// <returns>Number of commands processed.</returns>
        public int ProcessQueue(Renderer renderer)
        {
            if (renderer == null)
            {
                ModernLoggingSystem.Log("WARNING", "RenderCommandQueue: Cannot process with null renderer");
                return 0;
            }

            var processedCount = 0;

            while (_commandQueue.Count > 0)
            {
                var command = DequeueCommand();
                if (command != null)
                {
                    try
                    {
                        command.Execute(renderer);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        ModernLoggingSystem.Log("ERROR", $"RenderCommandQueue: Failed to execute command - {ex.Message}");
                    }
                }
            }

            ModernLoggingSystem.Log("DEBUG", $"RenderCommandQueue: Processed {processedCount} commands");
            return processedCount;
        }

        /// <summary>
        /// Gets queue statistics.
        /// </summary>
        /// <returns>Queue statistics as a string.</returns>
        public override string ToString()
        {
            return $"RenderCommandQueue: Commands={_commandQueue.Count}/{_maxQueueSize}, " +
            $"Batching={_batchingEnabled}";
        }
    }

    /// <summary>
    /// Interface for render commands.
    /// </summary>
    public interface IRenderCommand
    {
        /// <summary>
        /// Executes the render command.
        /// </summary>
        /// <param name="renderer">The renderer to execute on.</param>
        void Execute(Renderer renderer);
    }

    /// <summary>
    /// Clear command for rendering.
    /// </summary>
    public class ClearCommand : IRenderCommand
    {
        public Color ClearColor { get; }

        public ClearCommand(Color clearColor)
        {
            ClearColor = clearColor;
        }

        public void Execute(Renderer renderer)
        {
            renderer.Clear(ClearColor);
        }
    }

    /// <summary>
    /// Draw rectangle command for rendering.
    /// </summary>
    public class DrawRectangleCommand : IRenderCommand
    {
        public Vector3 Position { get; }
        public Vector3 Size { get; }
        public Color Color { get; }

        public DrawRectangleCommand(Vector3 position, Vector3 size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
        }

        public void Execute(Renderer renderer)
        {
            // This would use the renderer to draw a rectangle
            ModernLoggingSystem.Log("TRACE", $"DrawRectangle: Drawing rect at {Position} size {Size}");
        }
    }

    /// <summary>
    /// Draw texture command for rendering.
    /// </summary>
    public class DrawTextureCommand : IRenderCommand
    {
        public IntPtr Texture { get; }
        public Vector3 Position { get; }
        public Vector3 Size { get; }
        public Rectangle? SourceRect { get; }

        public DrawTextureCommand(IntPtr texture, Vector3 position, Vector3 size, Rectangle? sourceRect = null)
        {
            Texture = texture;
            Position = position;
            Size = size;
            SourceRect = sourceRect;
        }

        public void Execute(Renderer renderer)
        {
            // This would use the renderer to draw a texture
            ModernLoggingSystem.Log("TRACE", $"DrawTexture: Drawing texture at {Position} size {Size}");
        }
    }

    /// <summary>
    /// Set viewport command for rendering.
    /// </summary>
    public class SetViewportCommand : IRenderCommand
    {
        public int Width { get; }
        public int Height { get; }

        public SetViewportCommand(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Execute(Renderer renderer)
        {
            renderer.SetViewport(Width, Height);
        }
    }
}




