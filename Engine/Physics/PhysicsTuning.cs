using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Physics
{
    ///<summary>
    ///Static class containing physics tuning parameters for gameplay systems
    ///P60-R-1-04-01: Centralized physics configuration to replace hardcoded constants
    ///</summary>
    public static class PhysicsTuning
    {
        ///<summary>
        ///Strength of gravity applied to physics objects
        ///</summary>
        public static float GravityStrength { get; private set; }

        ///<summary>
        ///Default friction coefficient for physics materials
        ///</summary>
        public static float DefaultFriction { get; private set; }

        ///<summary>
        ///Default bounciness/restitution coefficient for physics materials
        ///</summary>
        public static float DefaultBounciness { get; private set; }

        ///<summary>
        ///Maximum slope angle in degrees that characters can walk on
        ///</summary>
        public static float MaxSlopeAngleDegrees { get; private set; }

        ///<summary>
        ///Static constructor initializes physics tuning with sensible defaults
        ///</summary>
        static PhysicsTuning()
        {
            try
            {
                //Initialize with sensible defaults
                GravityStrength = 9.81f;
                DefaultFriction = 0.6f;
                DefaultBounciness = 0.0f;
                MaxSlopeAngleDegrees = 45.0f;

                //Log initialization summary
                LogTuningSummary();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PhysicsTuning: Error initializing - {ex.Message}");

                //Fallback to safe defaults
                GravityStrength = 9.81f;
                DefaultFriction = 0.6f;
                DefaultBounciness = 0.0f;
                MaxSlopeAngleDegrees = 45.0f;
            }
        }

        ///<summary>
        ///Logs the current physics tuning configuration
        ///</summary>
        private static void LogTuningSummary()
        {
            System.Diagnostics.Debug.WriteLine("PhysicsTuning: Initialized with configuration:");
            System.Diagnostics.Debug.WriteLine($"  Gravity Strength: {GravityStrength}");
            System.Diagnostics.Debug.WriteLine($"  Default Friction: {DefaultFriction}");
            System.Diagnostics.Debug.WriteLine($"  Default Bounciness: {DefaultBounciness}");
            System.Diagnostics.Debug.WriteLine($"  Max Slope Angle: {MaxSlopeAngleDegrees}°");
        }

        ///<summary>
        ///Updates physics tuning parameters (for runtime configuration)
        ///</summary>
        ///<param name="gravityStrength">New gravity strength</param>
        ///<param name="defaultFriction">New default friction</param>
        ///<param name="defaultBounciness">New default bounciness</param>
        ///<param name="maxSlopeAngleDegrees">New max slope angle</param>
        public static void UpdateTuning(float gravityStrength, float defaultFriction, float defaultBounciness, float maxSlopeAngleDegrees)
        {
            try
            {
                GravityStrength = System.Math.Max(0f, gravityStrength);
                DefaultFriction = System.Math.Clamp(defaultFriction, 0f, 1f);
                DefaultBounciness = System.Math.Clamp(defaultBounciness, 0f, 1f);
                MaxSlopeAngleDegrees = System.Math.Clamp(maxSlopeAngleDegrees, 0f, 90f);

                System.Diagnostics.Debug.WriteLine("PhysicsTuning: Updated configuration:");
                LogTuningSummary();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PhysicsTuning: Error updating tuning - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets the maximum slope angle in radians
        ///</summary>
        ///<returns>Max slope angle in radians</returns>
        public static float GetMaxSlopeAngleRadians()
        {
            return MaxSlopeAngleDegrees * (float)System.Math.PI / 180f;
        }

        ///<summary>
        ///Validates if a slope angle is walkable
        ///</summary>
        ///<param name="angleDegrees">Slope angle in degrees</param>
        ///<returns>True if the slope is walkable</returns>
        public static bool IsSlopeWalkable(float angleDegrees)
        {
            return System.Math.Abs(angleDegrees) <= MaxSlopeAngleDegrees;
        }
    }
}




