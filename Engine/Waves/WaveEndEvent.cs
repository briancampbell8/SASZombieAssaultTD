// ====================================================================================================
//  FILE: WaveEndEvent.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide AddStatistic() behavior for the WaveDirector subsystem.
//      - Provide AddAchievement() behavior for the WaveDirector subsystem.
//      - Provide ToString() behavior for the WaveDirector subsystem.
//      - Provide GetDetailedReport() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Event data for wave completion.
    ///Triggered when a wave ends, either through victory or defeat.
    ///Phase 6: Final Pass - Add missing WaveEndEvent to fix CS1061 errors
    ///</summary>
    public class WaveEndEvent
    {
        public int WaveNumber { get; set; }
        public bool Victory { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int EnemiesSpawned { get; set; }
        public int EnemiesKilled { get; set; }
        public int EnemiesEscaped { get; set; }
        public float ScoreEarned { get; set; }
        public float CashEarned { get; set; }
        public List<string> AchievementsUnlocked { get; set; } = new();
        public Dictionary<string, object> WaveStatistics { get; set; } = new();
        public bool WasPerfectWave { get; set; }
        public float PerformanceRating { get; set; }

        ///<summary>
        ///Initializes a new WaveEndEvent instance.
        ///</summary>
        public WaveEndEvent()
        {
            EndTime = DateTime.Now;
            WaveStatistics = new Dictionary<string, object>();
            AchievementsUnlocked = new List<string>();
        }

        ///<summary>
        ///Initializes a new WaveEndEvent with specified parameters.
        ///</summary>
        ///<param name="waveNumber">The wave number that ended</param>
        ///<param name="victory">Whether the wave was won</param>
        ///<param name="duration">Duration of the wave</param>
        public WaveEndEvent(int waveNumber, bool victory, TimeSpan duration)
        {
            WaveNumber = waveNumber;
            Victory = victory;
            Duration = duration;
            EndTime = DateTime.Now;
            WaveStatistics = new Dictionary<string, object>();
            AchievementsUnlocked = new List<string>();
        }

        ///<summary>
        ///Gets the kill rate as a percentage.
        ///</summary>
        public float KillRate => EnemiesSpawned > 0 ? (float)EnemiesKilled / EnemiesSpawned * 100f : 0f;

        ///<summary>
        ///Gets the escape rate as a percentage.
        ///</summary>
        public float EscapeRate => EnemiesSpawned > 0 ? (float)EnemiesEscaped / EnemiesSpawned * 100f : 0f;

        ///<summary>
        ///Gets the efficiency rating based on performance.
        ///</summary>
        public float EfficiencyRating
        {
            get
            {
                if (!Victory) return 0f;
                
                float killBonus = KillRate * 0.4f;
                float escapePenalty = EscapeRate * 0.3f;
                float timeBonus = Duration.TotalSeconds < 300f ? 0.3f : 0f;
                float perfectBonus = WasPerfectWave ? 0.2f : 0f;
                
                return System.Math.Clamp(killBonus - escapePenalty + timeBonus + perfectBonus, 0f, 1f);
            }
        }

        ///<summary>
        ///Adds a statistic to the wave data.
        ///</summary>
        ///<param name="key">Statistic key</param>
        ///<param name="value">Statistic value</param>
        public void AddStatistic(string key, object value)
        {
            WaveStatistics[key] = value;
        }

        ///<summary>
        ///Gets a statistic from the wave data.
        ///</summary>
        ///<typeparam name="T">Type of the statistic</typeparam>
        ///<param name="key">Statistic key</param>
        ///<param name="defaultValue">Default value if not found</param>
        ///<returns>Statistic value or default</returns>
        public T GetStatistic<T>(string key, T defaultValue = default)
        {
            if (WaveStatistics.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        ///<summary>
        ///Adds an achievement to the unlocked achievements list.
        ///</summary>
        ///<param name="achievement">Achievement name</param>
        public void AddAchievement(string achievement)
        {
            if (!string.IsNullOrEmpty(achievement) && !AchievementsUnlocked.Contains(achievement))
            {
                AchievementsUnlocked.Add(achievement);
            }
        }

        ///<summary>
        ///Creates a summary string of the wave end event.
        ///</summary>
        ///<returns>Formatted summary string</returns>
        public override string ToString()
        {
            return $"Wave {WaveNumber}: {(Victory ? "Victory" : "Defeat")} - " +
                   $"Kills: {EnemiesKilled}/{EnemiesSpawned} ({KillRate:F1}%) - " +
                   $"Score: {ScoreEarned:F0} - Rating: {PerformanceRating:F2}";
        }

        ///<summary>
        ///Creates a detailed report of the wave end event.
        ///</summary>
        ///<returns>Detailed report string</returns>
        public string GetDetailedReport()
        {
            var report = new List<string>
            {
                $"=== Wave {WaveNumber} Report ===",
                $"Result: {(Victory ? "VICTORY" : "DEFEAT")}",
                $"Duration: {Duration:mm//:ss}",
                $"Performance: {PerformanceRating:F2}/1.00",
                "",
                "Combat Statistics:",
                $"  Enemies Spawned: {EnemiesSpawned}",
                $"  Enemies Killed: {EnemiesKilled}",
                $"  Enemies Escaped: {EnemiesEscaped}",
                $"  Kill Rate: {KillRate:F1}%",
                $"  Escape Rate: {EscapeRate:F1}%",
                "",
                "Rewards:",
                $"  Score Earned: {ScoreEarned:F0}",
                $"  Cash Earned: ${CashEarned:F2}",
                "",
                "Achievements:"
            };

            if (AchievementsUnlocked.Count > 0)
            {
                foreach (var achievement in AchievementsUnlocked)
                {
                    report.Add($"  - {achievement}");
                }
            }
            else
            {
                report.Add("  None");
            }

            report.Add("");
            report.Add("Additional Statistics:");

            foreach (var stat in WaveStatistics)
            {
                report.Add($"  {stat.Key}: {stat.Value}");
            }

            return string.Join(Environment.NewLine, report);
        }

        ///<summary>
        ///Clones this WaveEndEvent.
        ///</summary>
        ///<returns>A new WaveEndEvent with the same data</returns>
        public WaveEndEvent Clone()
        {
            return new WaveEndEvent
            {
                WaveNumber = this.WaveNumber,
                Victory = this.Victory,
                EndTime = this.EndTime,
                Duration = this.Duration,
                EnemiesSpawned = this.EnemiesSpawned,
                EnemiesKilled = this.EnemiesKilled,
                EnemiesEscaped = this.EnemiesEscaped,
                ScoreEarned = this.ScoreEarned,
                CashEarned = this.CashEarned,
                AchievementsUnlocked = new List<string>(this.AchievementsUnlocked),
                WaveStatistics = new Dictionary<string, object>(this.WaveStatistics),
                WasPerfectWave = this.WasPerfectWave,
                PerformanceRating = this.PerformanceRating
            };
        }
    }
}

