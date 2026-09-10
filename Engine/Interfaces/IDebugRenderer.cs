//===================================================================================
//  File:    IDebugRenderer.cs
//  Path:    Engine/Interfaces/IDebugRenderer.cs
//  Purpose:   P11-16-05 - Core interface for debug rendering visualization.
//  Defines the contract for rendering debug shapes, text, and visual indicators.
//
//  Role:      Essential debug rendering interface for visualization and debugging.
//           - Provides methods for rendering debug shapes and visual indicators
//           - Handles text rendering for debug labels and information display
//           - Supports color and styling options for debug visualization
//           - Integrates with render context for debug overlay rendering
//           - Provides efficient debug rendering with minimal performance impact
//
//  Features:   Debug shape rendering with lines, points, spheres, and boxes.
//           Text rendering for debug labels and information display.
//           Color and styling options for comprehensive debug visualization.
//           Efficient rendering with batch operations for performance.
//           Integration with main render context for overlay rendering.
//           Support for both 2D and 3D debug visualization.
//
//  Notes:      This interface is implemented by concrete debug renderers.
//           Debug rendering is designed to have minimal performance impact.
//           All debug rendering operations are optimized for real-time use.
//           Interface supports both immediate and batched rendering modes.
//           Debug renderer integrates seamlessly with main rendering pipeline.
//====================================================================================


using System.Collections.Generic;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Defines the contract for debug rendering visualization. Implements P11-16-05: Debug rendering visualization for
    /// animation and system debugging.
    /// </summary>
    public interface IDebugRenderer
    {
        /// <summary>
        /// Gets whether debug rendering is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the current debug rendering layer.
        /// </summary>
        DebugRenderLayer CurrentLayer { get; set; }

        /// <summary>
        /// Enables or disables debug rendering.
        /// </summary>
        /// <param name="enabled">Whether to enable debug rendering.</param>
        void SetEnabled(bool enabled);

        /// <summary>
        /// Begins a debug rendering batch.
        /// </summary>
        void BeginBatch();

        /// <summary>
        /// Ends a debug rendering batch and submits all debug primitives.
        /// </summary>
        void EndBatch();

        /// <summary>
        /// Draws multiple lines in a single batch operation.
        /// </summary>
        /// <param name="lines">A collection of line data, each containing start, end, color, and thickness.</param>
        void DrawLines(IEnumerable<(System.Numerics.Vector3 start,
            System.Numerics.Vector3 end, System.Drawing.Color color, float thickness)> lines);

        /// <summary>
        /// Draws a line between two
        /// points with specified color and thickness. Used for skeleton bone connections and
        /// debug lines.
        /// </summary>
        /// <param name="start">The start position of the line.</param>
        /// <param name="end">The end position of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        void DrawLine(
            System.Numerics.Vector3 start,
            System.Numerics.Vector3 end,
            System.Drawing.Color color,
            float thickness = 1.0f);

        /// <summary>
        /// Draws multiple points in a single batch operation.
        /// </summary>
        /// <param name="points">A collection of point data, each containing position, color, and size.</param>
        void DrawPoints(IEnumerable<(System.Numerics.Vector3 position, System.Drawing.Color color, float size)> points);

        /// <summary>
        /// Draws a point at specified position with color and size. Used for bone joint visualization and debug points.
        /// </summary>
        /// <param name="position">The position to draw the point.</param>
        /// <param name="color">The color of the point.</param>
        /// <param name="size">The size of the point.</param>
        void DrawPoint(System.Numerics.Vector3 position, System.Drawing.Color color, float size = 1.0f);

        /// <summary>
        /// Draws a sphere at specified position with radius and color. Used for state indicators and collision
        /// visualization.
        /// </summary>
        /// <param name="center">The center position of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="color">The color of the sphere.</param>
        void DrawSphere(System.Numerics.Vector3 center, float radius, System.Drawing.Color color);

        /// <summary>
        /// Draws a box at specified position with size and color. Used for bounding box visualization and debug
        /// containers.
        /// </summary>
        /// <param name="center">The center position of the box.</param>
        /// <param name="size">The size of the box.</param>
        /// <param name="color">The color of the box.</param>
        void DrawBox(System.Numerics.Vector3 center, System.Numerics.Vector3 size, System.Drawing.Color color);

        /// <summary>
        /// Draws a circle at specified position with radius and color. Used for range indicators and circular
        /// visualization.
        /// </summary>
        /// <param name="center">The center position of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        void DrawCircle(System.Numerics.Vector3 center, float radius, System.Drawing.Color color);

        /// <summary>
        /// Draws text at specified position with color and font size. Used for debug labels and information display.
        /// </summary>
        /// <param name="position">The position to draw the text.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="fontSize">The font size for the text.</param>
        void DrawText(System.Numerics.Vector3 position, string text, System.Drawing.Color color, int fontSize);

        /// <summary>
        /// Draws text at specified screen position with color and font size. Used for UI overlay debug information.
        /// </summary>
        /// <param name="position">The screen position to draw the text.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="fontSize">The font size for the text.</param>
        void DrawScreenText(System.Numerics.Vector3 position, string text, System.Drawing.Color color, int fontSize);

        /// <summary>
        /// Draws an arrow from start to end position with specified color and thickness. Used for direction indicators
        /// and vector visualization.
        /// </summary>
        /// <param name="start">The start position of the arrow.</param>
        /// <param name="end">The end position of the arrow.</param>
        /// <param name="color">The color of the arrow.</param>
        /// <param name="thickness">The thickness of the arrow.</param>
        void DrawArrow(System.Numerics.Vector3 start, System.Numerics.Vector3 end, System.Drawing.Color color, float thickness = 1.0f);

        /// <summary>
        /// Draws a wireframe mesh at specified position with rotation and scale. Used for mesh visualization and
        /// collision shape debugging.
        /// </summary>
        /// <param name="position">The position to draw the mesh.</param>
        /// <param name="rotation">The rotation of the mesh.</param>
        /// <param name="scale">The scale of the mesh.</param>
        /// <param name="mesh">The mesh to draw.</param>
        /// <param name="color">The color of the mesh.</param>
        void DrawWireframeMesh(System.Numerics.Vector3 position, Quaternion rotation, System.Numerics.Vector3 scale, IMesh mesh, System.Drawing.Color color);

        /// <summary>
        /// Draws a frustum for camera visualization. Used for camera frustum debugging and culling visualization.
        /// </summary>
        /// <param name="camera">The camera whose frustum to draw.</param>
        /// <param name="color">The color of the frustum.</param>
        void DrawCameraFrustum(ICamera camera, System.Drawing.Color color);

        /// <summary>
        /// Draws a grid at specified position with size and spacing. Used for spatial reference and level editing
        /// visualization.
        /// </summary>
        /// <param name="position">The position of the grid center.</param>
        /// <param name="size">The size of the grid.</param>
        /// <param name="spacing">The spacing between grid lines.</param>
        /// <param name="color">The color of the grid.</param>
        void DrawGrid(System.Numerics.Vector3 position, System.Numerics.Vector3 size, float spacing, System.Drawing.Color color);

        /// <summary>
        /// Clears all debug rendering primitives.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets the debug rendering layer for subsequent operations.
        /// </summary>
        /// <param name="layer">The debug rendering layer to set.</param>
        void SetLayer(DebugRenderLayer layer);

        /// <summary>
        /// Gets the number of debug primitives currently queued for rendering.
        /// </summary>
        /// <returns>The number of debug primitives.</returns>
        int GetPrimitiveCount();

        /// <summary>
        /// Gets performance statistics for debug rendering.
        /// </summary>
        /// <returns>Debug rendering performance statistics.</returns>
        DebugRenderStats GetStats();
        void DrawSphere(VectorMath.Vector3 position, int x, System.Drawing.Color sphereColor);
        void DrawBox(VectorMath.Vector3 barPosition, VectorMath.Vector3 barSize, System.Drawing.Color gray);
        void DrawText(VectorMath.Vector3 textPosition, string stateName, System.Drawing.Color textColor, int v);
        void DrawPoint(VectorMath.Vector3 position, object highlight);
        void DrawText(VectorMath.Vector3 position, string v, object info);
    }

    public interface ICamera
    {
    }

    /// <summary>
    /// Defines the contract for mesh data used in debug rendering.
    /// </summary>
    public interface IMesh
    {
        /// <summary>
        /// Gets the vertices of the mesh.
        /// </summary>
        IReadOnlyList<System.Numerics.Vector3> Vertices { get; }

        /// <summary>
        /// Gets the indices of the mesh.
        /// </summary>
        IReadOnlyList<int> Indices { get; }

        /// <summary>
        /// Gets the number of vertices in the mesh.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Gets the number of triangles in the mesh.
        /// </summary>
        int TriangleCount { get; }
    }

    /// <summary>
    /// Debug rendering layer enumeration.
    /// </summary>
    public enum DebugRenderLayer
    {
        /// <summary>
        /// Default debug rendering layer.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Animation debug layer.
        /// </summary>
        Animation = 1,

        /// <summary>
        /// Physics debug layer.
        /// </summary>
        Physics = 2,

        /// <summary>
        /// Collision debug layer.
        /// </summary>
        Collision = 3,

        /// <summary>
        /// AI debug layer.
        /// </summary>
        AI = 4,

        /// <summary>
        /// UI debug layer.
        /// </summary>
        UI = 5,

        /// <summary>
        /// Performance debug layer.
        /// </summary>
        Performance = 6,

        /// <summary>
        /// Network debug layer.
        /// </summary>
        Network = 7,

        /// <summary>
        /// Audio debug layer.
        /// </summary>
        Audio = 8,

        /// <summary>
        /// Input debug layer.
        /// </summary>
        Input = 9
    }

    /// <summary>
    /// Debug rendering performance statistics.
    /// </summary>
    public class DebugRenderStats
    {
        /// <summary>
        /// Gets the number of lines rendered.
        /// </summary>
        public int LineCount { get; set; }

        /// <summary>
        /// Gets the number of points rendered.
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// Gets the number of spheres rendered.
        /// </summary>
        public int SphereCount { get; set; }

        /// <summary>
        /// Gets the number of boxes rendered.
        /// </summary>
        public int BoxCount { get; set; }

        /// <summary>
        /// Gets the number of circles rendered.
        /// </summary>
        public int CircleCount { get; set; }

        /// <summary>
        /// Gets the number of text elements rendered.
        /// </summary>
        public int TextCount { get; set; }

        /// <summary>
        /// Gets the number of arrows rendered.
        /// </summary>
        public int ArrowCount { get; set; }

        /// <summary>
        /// Gets the number of meshes rendered.
        /// </summary>
        public int MeshCount { get; set; }

        /// <summary>
        /// Gets the total number of primitives rendered.
        /// </summary>
        public int TotalPrimitives => LineCount + PointCount + SphereCount + BoxCount + CircleCount + TextCount + ArrowCount + MeshCount;

        /// <summary>
        /// Gets the time taken for debug rendering in milliseconds.
        /// </summary>
        public float RenderTime { get; set; }

        /// <summary>
        /// Gets the memory used by debug rendering in bytes.
        /// </summary>
        public long MemoryUsage { get; set; }
    }
}
