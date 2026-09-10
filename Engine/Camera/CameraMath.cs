// ====================================================================================================
//  FILE: CameraMath.cs
//  PATH: ./Engine/Camera/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the CameraMath module.
//
//  RESPONSIBILITIES:
//      - Provide ScreenToWorld() behavior for the Core subsystem.
//      - Provide ScreenToWorld() behavior for the Core subsystem.
//      - Provide WorldToScreen() behavior for the Core subsystem.
//      - Provide WorldToScreen() behavior for the Core subsystem.
//      - Provide IsVisible() behavior for the Core subsystem.
//      - Provide GetVisibleBounds() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;


namespace SASZombieAssaultTD.Engine.Camera
{
    /// <summary>
    /// Camera mathematical utilities for SAS Zombie Assault TD. Provides screen-to-world and world-to-screen coordinate
    /// conversions.
    /// </summary>
    public static class CameraMath
    {
        /// <summary>
        /// Converts screen coordinates to world coordinates.
        /// </summary>
        /// <param name="screenX">Screen X coordinate.</param>
        /// <param name="screenY">Screen Y coordinate.</param>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <returns>World position as Vector3.</returns>
        public static Vector3 ScreenToWorld(float screenX, float screenY, Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight)
        {
            //Convert screen coordinates to normalized device coordinates (-1 to 1)
            float ndcX = (screenX / screenWidth) * 2f - 1f;
            float ndcY = 1f - (screenY / screenHeight) * 2f; //Flip Y axis

            //Convert to world coordinates
            float worldX = cameraPosition.X + (ndcX * screenWidth / (2f * cameraZoom));
            float worldY = cameraPosition.Y + (ndcY * screenHeight / (2f * cameraZoom));

            return new Vector3(worldX, worldY, cameraPosition.Z);
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates.
        /// </summary>
        /// <param name="screenPosition">Screen position as Vector3.</param>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <returns>World position as Vector3.</returns>
        public static Vector3 ScreenToWorld(Vector3 screenPosition, Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight)
        {
            return ScreenToWorld(screenPosition.X, screenPosition.Y, cameraPosition, cameraZoom, screenWidth, screenHeight);
        }

        /// <summary>
        /// Converts world coordinates to screen coordinates.
        /// </summary>
        /// <param name="worldX">World X coordinate.</param>
        /// <param name="worldY">World Y coordinate.</param>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <returns>Screen position as Vector3.</returns>
        public static Vector3 WorldToScreen(float worldX, float worldY, Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight)
        {
            //Calculate relative position from camera
            float relativeX = (worldX - cameraPosition.X) * cameraZoom;
            float relativeY = (worldY - cameraPosition.Y) * cameraZoom;

            //Convert to screen coordinates
            float screenX = (relativeX / screenWidth + 0.5f) * screenWidth;
            float screenY = (0.5f - relativeY / screenHeight) * screenHeight; //Flip Y axis

            return new Vector3(screenX, screenY, 0f);
        }

        /// <summary>
        /// Converts world coordinates to screen coordinates.
        /// </summary>
        /// <param name="worldPosition">World position as Vector3.</param>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <returns>Screen position as Vector3.</returns>
        public static Vector3 WorldToScreen(Vector3 worldPosition, Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight)
        {
            return WorldToScreen(worldPosition.X, worldPosition.Y, cameraPosition, cameraZoom, screenWidth, screenHeight);
        }

        /// <summary>
        /// Checks if a world position is visible on screen.
        /// </summary>
        /// <param name="worldPosition">World position to check.</param>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <param name="margin">Optional margin around screen edges.</param>
        /// <returns>True if position is visible.</returns>
        public static bool IsVisible(Vector3 worldPosition, Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight, float margin = 0f)
        {
            Vector3 screenPos = WorldToScreen(worldPosition, cameraPosition, cameraZoom, screenWidth, screenHeight);
            return screenPos.X >= -margin && screenPos.X <= screenWidth + margin &&
                   screenPos.Y >= -margin && screenPos.Y <= screenHeight + margin;
        }

        /// <summary>
        /// Gets the visible world bounds.
        /// </summary>
        /// <param name="cameraPosition">Camera world position.</param>
        /// <param name="cameraZoom">Camera zoom level.</param>
        /// <param name="screenWidth">Screen width.</param>
        /// <param name="screenHeight">Screen height.</param>
        /// <returns>Rectangle representing visible world bounds.</returns>
        public static Rectangle GetVisibleBounds(Vector3 cameraPosition, float cameraZoom, int screenWidth, int screenHeight)
        {
            Vector3 topLeft = ScreenToWorld(0f, 0f, cameraPosition, cameraZoom, screenWidth, screenHeight);
            Vector3 bottomRight = ScreenToWorld(screenWidth, screenHeight, cameraPosition, cameraZoom, screenWidth, screenHeight);

            return new Rectangle(
                System.Math.Min(topLeft.X, bottomRight.X),
                System.Math.Min(topLeft.Y, bottomRight.Y),
                System.Math.Abs(bottomRight.X - topLeft.X),
                System.Math.Abs(bottomRight.Y - topLeft.Y)
            );
        }
    }
}
