// =====================================================================================================
//  FILE: MathTrigonometryExtensions.cs
//  PATH: Engine/VectorMath/MathExtensions/MathTrigonometryExtensions.cs
//  SUBSYSTEM: VectorMath MathExtensions Subsystem
//
//  ROLE:
//      Provides unified, static mathematical functions for tracking angular states, conversions,
//      and wrapped rotational boundaries. Serves as a fundamental math library for turret targeting,
//      projectile trajectories, orientation updates, and field-of-view evaluations across the engine.
//
//  RESPONSIBILITIES:
//      - Wrap raw radiant angles to bounded ranges like [-PI, PI] or [0, 2*PI].
//      - Convert numeric values between degree and radian representations smoothly.
//      - Calculate minimal angular differences to determine optimal rotation direction vectors.
//
//  NON-RESPONSIBILITIES:
//      - Storing ECSEntityCore rotation matrices or tracking spatial transform components directly.
//      - Managing complex skeletal animation rigs or pathfinding target networks.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the unified mathematical extensions module.
//      - All methods are pure, stateless, and safe to execute across concurrent thread pools.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.VectorMath.MathExtensions
{
    public static class MathTrigonometryExtensions
    {
        public static double WrapAngle(double angle)
        {
            return MathCommonExtensions.Wrap(angle, -System.Math.PI, System.Math.PI);
        }

        public static float WrapAngle(float angle)
        {
            return MathCommonExtensions.Wrap(angle, -(float)System.Math.PI, (float)System.Math.PI);
        }

        public static double WrapAnglePositive(double angle)
        {
            return MathCommonExtensions.Wrap(angle, 0.0, 2.0 * System.Math.PI);
        }

        public static float WrapAnglePositive(float angle)
        {
            return MathCommonExtensions.Wrap(angle, 0f, 2f * (float)System.Math.PI);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)System.Math.PI / 180f;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / System.Math.PI;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * 180f / (float)System.Math.PI;
        }

        public static double NormalizeAngle(double angle)
        {
            return WrapAngle(angle);
        }

        public static float NormalizeAngle(float angle)
        {
            return WrapAngle(angle);
        }

        public static double AngleDifference(double a, double b)
        {
            var diff = b - a;
            return WrapAngle(diff);
        }

        public static float AngleDifference(float a, float b)
        {
            var diff = b - a;
            return WrapAngle(diff);
        }
    }
}
