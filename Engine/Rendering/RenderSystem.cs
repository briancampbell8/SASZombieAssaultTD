using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Simple Matrix structure for rendering.
    /// </summary>
    public struct Matrix
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;
        
        public static Matrix Identity => new Matrix(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1);
        
        public static Matrix CreateTranslation(float x, float y, float z)
        {
            return new Matrix(1,0,0,0, 0,1,0,0, 0,0,1,0, x,y,z,1);
        }
        
        public static Matrix CreateTranslation(Vector3 translation)
        {
            return CreateTranslation(translation.X, translation.Y, translation.Z);
        }
        
        public Matrix(float m11, float m12, float m13, float m14,
                     float m21, float m22, float m23, float m24,
                     float m31, float m32, float m33, float m34,
                     float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }
    }

    /// <summary>
    /// Simple render system to fix compilation errors.
    /// </summary>
    public static class RenderSystem
    {
        private static object TheType;
        private static object TheMember;

        public static void Initialize() { }
        public static void Shutdown() { }
        public static void BeginFrame() { }
        public static void EndFrame() { }
        public static void DrawLine(Vector3 start, Vector3 end, Color color) { }
        public static void DrawCircle(Vector3 center, float radius, Color color) { }
        public static void DrawRectangle(Vector3 position, float width, float height, Color color) { }
        public static void DrawText(string text, Vector3 position, Color color) { }
        public static void SetCamera(Matrix viewMatrix, Matrix projectionMatrix) { }
        public static void Flush() { }
        public static void DrawCheckmark(Vector3 position, Color color, float size) { }
        public static void DrawX(Vector3 position, Color color, float size) { }
        public static void DrawString(string text, Vector3 position, Color color, float size = 1.0f) { DrawText(text, position, color); }

        internal static void DrawRectangle(float v1, float v2, float x, float v3, System.Drawing.Color baseColor)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal static void DrawSprite(Sprite towerSprite, Vector3 towerPos, Vector3 vector3, System.Drawing.Color finalColor)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal static void DrawLine(Vector3 startPoint, Vector3 endPoint, System.Drawing.Color color, float v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal static void DrawCircle(float x, float y, float currentRange, System.Drawing.Color rangeColor, float v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
