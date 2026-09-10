// =====================================================================================================
//  FILE: EnemyManagerExtensions.cs
//  PATH: Game/Enemies/EnemyManagerExtensions.cs
//  MODULE: Enemies
//
//  ROLE:
//      Provide deterministic, type‑safe extension methods for the Enemies subsystem.
//
//  RESPONSIBILITIES:
//      - Provide GetNearestEnemy() behavior for the Enemies subsystem.
//      - Provide GetNearestEnemyOfType() behavior for the Enemies subsystem.
//      - Provide GetEnemiesInsideRadius() behavior for the Enemies subsystem.
//      - Provide AnyEnemyInsideRadius() behavior for the Enemies subsystem.
//      - Provide HasEnemyOfType() behavior for the Enemies subsystem.
//      - Provide GetActiveEnemiesOfType() behavior for the Enemies subsystem.
//      - Provide GetTypeBreakdown() behavior for the Enemies subsystem.
//      - Provide GetEnemyPositionSummary() behavior for the Enemies subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Performing spawning or lifecycle management.
//      - Mutating EnemyManager state or overriding deterministic behavior.
//      - Cross‑subsystem coupling or reflection‑based access.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
//      Reflection removed; deterministic public accessors used exclusively.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Game.Enemies
{
    public static class EnemyManagerExtensions
    {
        // ----------------------------------------------------------------------------------------------
        //  INTERNAL ACCESS (DETERMINISTIC, NO REFLECTION)
        // ----------------------------------------------------------------------------------------------
        private static IEnumerable<Enemy> EnumerateActiveEnemies(this GetEnemyManager manager)
        {
            // Modern EnemyManager exposes deterministic enumeration
            return manager.ActiveEnemies;
        }

        // ----------------------------------------------------------------------------------------------
        //  NEAREST ENEMY HELPERS
        // ----------------------------------------------------------------------------------------------
        public static Enemy GetNearestEnemy(this GetEnemyManager manager, Vector3 position)
        {
            if (manager == null)
                return null;

            Enemy nearest = null;
            float bestDist = float.MaxValue;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                float dist = Vector3.Distance(enemy.Position, position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        public static Enemy GetNearestEnemyOfType(this GetEnemyManager manager, EnemyType type, Vector3 position)
        {
            if (manager == null)
                return null;

            Enemy nearest = null;
            float bestDist = float.MaxValue;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (enemy.Type != type.ToString())
                    continue;

                float dist = Vector3.Distance(enemy.Position, position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        // ----------------------------------------------------------------------------------------------
        //  RADIUS / RANGE HELPERS
        // ----------------------------------------------------------------------------------------------
        public static IEnumerable<Enemy> GetEnemiesInsideRadius(this GetEnemyManager manager, Vector3 center, float radius)
        {
            List<Enemy> results = new List<Enemy>();

            if (manager == null)
                return results;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (Vector3.Distance(enemy.Position, center) <= radius)
                    results.Add(enemy);
            }

            return results;
        }
        // public static void DoSomething(this EnemyManager manager)
        public static bool AnyEnemyInsideRadius(this GetEnemyManager manager, Vector3 center, float radius)
        {
            if (manager == null)
                return false;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (Vector3.Distance(enemy.Position, center) <= radius)
                    return true;
            }

            return false;
        }

        // ----------------------------------------------------------------------------------------------
        //  TYPE / CATEGORY HELPERS
        // ----------------------------------------------------------------------------------------------
        public static bool HasEnemyOfType(this GetEnemyManager manager, EnemyType type)
        {
            if (manager == null)
                return false;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (enemy.Type == type.ToString())
                    return true;
            }

            return false;
        }

        public static IEnumerable<Enemy> GetActiveEnemiesOfType(this GetEnemyManager manager, EnemyType type)
        {
            List<Enemy> results = new List<Enemy>();

            if (manager == null)
                return results;

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (enemy.Type == type.ToString())
                    results.Add(enemy);
            }

            return results;
        }

        // ----------------------------------------------------------------------------------------------
        //  DIAGNOSTIC HELPERS
        // ----------------------------------------------------------------------------------------------
        public static string GetTypeBreakdown(this GetEnemyManager manager)
        {
            if (manager == null)
                return "No manager.";

            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                if (!counts.ContainsKey(enemy.Type))
                    counts[enemy.Type] = 0;

                counts[enemy.Type]++;
            }

            List<string> parts = new List<string>();
            foreach (var kvp in counts)
                parts.Add($"{kvp.Key}: {kvp.Value}");

            return string.Join(", ", parts);
        }

        public static string GetEnemyPositionSummary(this GetEnemyManager manager)
        {
            if (manager == null)
                return "No manager.";

            List<string> lines = new List<string>();

            foreach (Enemy enemy in manager.EnumerateActiveEnemies())
            {
                int id = (int)(enemy.ECSEntityCore == null ? 0 : enemy.ECSEntityCore.Id);

                lines.Add(
                    $"ID={id}, Type={enemy.Type}, Pos=({enemy.Position.X:F1},{enemy.Position.Y:F1})"
                );
            }

            return string.Join("\n", lines);
        }
    }

    public class GetEnemyManager
    {
        public IEnumerable<Enemy> ActiveEnemies { get; internal set; }
    }
}
