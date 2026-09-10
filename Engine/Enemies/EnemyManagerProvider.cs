// =====================================================================================================
//  FILE: EnemyManagerProvider.cs
//  PATH: Engine/Enemies/EnemyManagerProvider.cs
//  SUBSYSTEM: Enemies
//
//  ROLE:
//      Executes deterministic enemy spawning for each wave, applying difficulty scaling,
//      spawn patterns, and wave modifiers. Acts as the runtime spawning engine for
//      WaveDirectorCore, converting WaveScript definitions into live enemy instances.
//
//  RESPONSIBILITIES:
//      - Execute spawn groups asynchronously and deterministically.
//      - Apply difficulty progression multipliers to enemy attributes and counts.
//      - Resolve spawn positions using NavigationGrid and spawn pattern calculators.
//      - Invoke enemy creation through EnemyFactory and register with active managers.
//      - Provide safe, predictable spawning behavior regardless of game state pauses.
//
//  NON-RESPONSIBILITIES:
//      - Managing wave lifecycle sequencing (WaveDirectorFlow handles sequencing).
//      - Loading or generating wave scripts (WaveDirectorInitialization handles loading).
//      - Difficulty configuration (DifficultyConfig handles configuration).
//      - Enemy AI, navigation, or combat behavior (handled by respective subsystems).
//
//  ARCHITECTURAL NOTES:
//      - All spawning operations must remain deterministic and testable.
//      - Uses NavigationGrid for spatial resolution and SpawnPtrn modules for pattern logic.
//      - DifficultyProgression and DifficultyScaler provide scaling values consumed here.
//      - WaveDirectorContext provides pause state, callbacks, and runtime bindings.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Enemies
{
    //===============================================================================================
    // ENEMY MANAGER PROVIDER
    //===============================================================================================
    public class EnemyManagerProvider
    {
        public static object GetActiveManager()
        {
            try
            {
                var candidateTypeNames = new[] { "EnemyManager", "EnemyService", "EnemyManagerService" };
                var instancePropertyNames = new[] { "Instance", "Current", "Active", "Singleton", "Default" };

                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try { types = asm.GetTypes(); }
                    catch { continue; }

                    foreach (var t in types)
                    {
                        bool isCandidate = false;

                        foreach (var name in candidateTypeNames)
                        {
                            if (string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase))
                            {
                                isCandidate = true;
                                break;
                            }
                        }

                        if (!isCandidate)
                        {
                            foreach (var iface in t.GetInterfaces())
                            {
                                if (string.Equals(iface.Name, "IEnemyManager", StringComparison.OrdinalIgnoreCase))
                                {
                                    isCandidate = true;
                                    break;
                                }
                            }
                        }

                        if (!isCandidate)
                            continue;

                        foreach (var propName in instancePropertyNames)
                        {
                            var prop = t.GetProperty(propName,
                                System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.Static);

                            if (prop?.GetMethod != null)
                            {
                                try
                                {
                                    var val = prop.GetValue(null);
                                    if (val != null)
                                        return val;
                                }
                                catch { }
                            }
                        }

                        foreach (var fieldName in instancePropertyNames)
                        {
                            var fld = t.GetField(fieldName,
                                System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.Static);

                            if (fld != null)
                            {
                                try
                                {
                                    var val = fld.GetValue(null);
                                    if (val != null)
                                        return val;
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
