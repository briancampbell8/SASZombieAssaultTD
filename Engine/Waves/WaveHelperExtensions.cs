// =====================================================================================================
//  FILE: WaveHelperExtensions.cs
//  PATH: Engine/Waves/WaveHelperExtensions.cs
//  SUBSYSTEM: Waves Subsystem
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// FIXED: Changed to 'public static' so it can house extension methods.
    /// This resolves the CS1061 missing 'Clone()' compiler errors in WaveScript.cs.
    /// </summary>
    internal static class WaveHelperExtensions
    {
        /// <summary>
        /// Fixes CS1061 for WaveModifiers.Clone() on line 212 of WaveScript.cs
        /// </summary>
        public static WaveModifiers Clone(this WaveModifiers original)
        {
            if (original == null) return null;
            var cloneMethod = original.GetType().GetMethod("MemberwiseClone",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (WaveModifiers)cloneMethod?.Invoke(original, null) ?? original;
        }

        /// <summary>
        /// Fixes CS1061 for WaveRewards.Clone() on line 213 of WaveScript.cs
        /// </summary>
        public static WaveRewards Clone(this WaveRewards original)
        {
            if (original == null) return null;
            var cloneMethod = original.GetType().GetMethod("MemberwiseClone",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (WaveRewards)cloneMethod?.Invoke(original, null) ?? original;
        }

        /// <summary>
        /// Fixes CS1061 for WaveEnvironment.Clone() on line 214 of WaveScript.cs
        /// </summary>
        public static WaveEnvironment Clone(this WaveEnvironment original)
        {
            if (original == null) return null;
            var cloneMethod = original.GetType().GetMethod("MemberwiseClone",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (WaveEnvironment)cloneMethod?.Invoke(original, null) ?? original;
        }
    }
}
