// ====================================================================================================
//  FILE: RocketSpriteSmokeTest.cs
//  PATH: AssetLoaderTest\RocketSpriteSmokeTest.cs
//  PROGRAM: RocketSpriteSmokeTest.cs
//  MODULE: Diagnostics & Engine Pipeline (RocketSpriteSmokeTest)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for SASZombieAssaultTD.Tests.AssetLoaderTest.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide public interface and handling execution for Initialize().
//      - Provide public interface and handling execution for Render().
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Tests.AssetLoaderTest
{
    public static class RocketSpriteSmokeTest
    {
        private static bool _initialized;
        private static int _rocketWidth;
        private static int _rocketHeight;
        private static byte[]? _rocketPixels; //BGRA32

        ///<summary>
        ///Loads the rocket sprite from disk into a CPU-side BGRA32 buffer.
        ///Call once at startup.
        ///</summary>
        public static void Initialize(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath must be a valid path.", nameof(filePath));

            using var bmp = new Bitmap(filePath);

            _rocketWidth = bmp.Width;
            _rocketHeight = bmp.Height;
            _rocketPixels = new byte[_rocketWidth * _rocketHeight * 4];

            int index = 0;
            for (int y = 0; y < _rocketHeight; y++)
            {
                for (int x = 0; x < _rocketWidth; x++)
                {
                    System.Drawing.Color c = bmp.GetPixel(x, y);

                    //Framebuffer uses BGRA32
                    _rocketPixels[index + 0] = c.B;
                    _rocketPixels[index + 1] = c.G;
                    _rocketPixels[index + 2] = c.R;
                    _rocketPixels[index + 3] = c.A;

                    index += 4;
                }
            }

            _initialized = true;
        }

        ///<summary>
        ///Draws the rocket sprite into the framebuffer at the center of the screen.
        ///Call from GameRoot.Render after clearing the framebuffer.
        ///</summary>
        public static void Render(Framebuffer framebuffer)
        {
            if (!_initialized || _rocketPixels == null)
                return;

            int fbWidth = framebuffer.Width;
            int fbHeight = framebuffer.Height;
            byte[] fbPixels = framebuffer.Pixels;

            int startX = (fbWidth - _rocketWidth) / 2;
            int startY = (fbHeight - _rocketHeight) / 2;

            if (startX < 0 || startY < 0)
                return; //sprite larger than framebuffer

            for (int y = 0; y < _rocketHeight; y++)
            {
                int fbY = startY + y;
                if (fbY < 0 || fbY >= fbHeight)
                    continue;

                int srcRowStart = y * _rocketWidth * 4;
                int dstRowStart = (fbY * fbWidth + startX) * 4;

                for (int x = 0; x < _rocketWidth; x++)
                {
                    int fbX = startX + x;
                    if (fbX < 0 || fbX >= fbWidth)
                        continue;

                    int srcIndex = srcRowStart + x * 4;
                    int dstIndex = dstRowStart + x * 4;

                    fbPixels[dstIndex + 0] = _rocketPixels[srcIndex + 0]; //B
                    fbPixels[dstIndex + 1] = _rocketPixels[srcIndex + 1]; //G
                    fbPixels[dstIndex + 2] = _rocketPixels[srcIndex + 2]; //R
                    fbPixels[dstIndex + 3] = _rocketPixels[srcIndex + 3]; //A
                }
            }
        }
    }
}
