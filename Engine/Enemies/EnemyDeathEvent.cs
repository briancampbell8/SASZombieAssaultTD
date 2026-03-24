using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Event data for enemy death.
    /// Triggered when an enemy is killed, containing comprehensive death information.
    /// Phase 6: Final Pass - Add missing EnemyDeathEvent to fix CS1061 errors
    /// </summary>
    public class EnemyDeathEvent
    {
        public int EnemyId { get; set; }
        // Numeric standardization: All continuous values use double for precision
        public ZombieType EnemyType { get; set; }
        public Vector3 DeathPosition { get; set; }
        public DateTime DeathTime { get; set; }
        public double DamageDealt { get; set; }
        public double TimeAlive { get; set; }
        public bool WasHeadshot { get; set; }
        public bool WasMeleeKill { get; set; }
        public bool WasExplosiveKill { get; set; }
        public int KillerId { get; set; }
        public string KillerName { get; set; }
        public int TowerId { get; set; }
        public string TowerType { get; set; }
        public int ScoreValue { get; set; }
        public double CashValue { get; set; }
        public List<string> DamageSources { get; set; } = new();
        public Dictionary<string, double> DamageBreakdown { get; set; } = new();
        
        // Missing properties
        public object Enemy { get; set; }
        public string SourceOfDamage { get; set; }
        public float DistanceFromPlayer { get; set; }
        public bool WasBoss { get; set; }
        public int WaveNumber { get; set; }
        public bool WasCriticalKill { get; set; }
        public float ComboMultiplier { get; set; }
        public List<string> AchievementsTriggered { get; set; } = new();
        public Dictionary<string, object> KillStatistics { get; set; } = new();

        /// <summary>
        /// Parameterless constructor for cloning and serializers.
        /// </summary>
        public EnemyDeathEvent()
        {
            DamageSources = new List<string>();
            DamageBreakdown = new Dictionary<string, double>();
            AchievementsTriggered = new List<string>();
            KillStatistics = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new EnemyDeathEvent instance.
        /// </summary>
        public EnemyDeathEvent(uint id, string type, Vector3 position)
        {
            DeathTime = DateTime.Now;
            DamageSources = new List<string>();
            DamageBreakdown = new Dictionary<string, double>();
            AchievementsTriggered = new List<string>();
            KillStatistics = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new EnemyDeathEvent with basic parameters.
        /// </summary>
        /// <param name="enemyId">ID of the enemy that died</param>
        /// <param name="enemyType">Type of the enemy</param>
        /// <param name="deathPosition">Position where the enemy died</param>
        public EnemyDeathEvent(int enemyId, ZombieType enemyType, Vector3 deathPosition)
        {
            EnemyId = enemyId;
            EnemyType = enemyType;
            DeathPosition = deathPosition;
            DeathTime = DateTime.Now;
            DamageSources = new List<string>();
            DamageBreakdown = new Dictionary<string, double>();
            AchievementsTriggered = new List<string>();
            KillStatistics = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the total score value including multipliers.
        /// </summary>
        public int TotalScoreValue => (int)(ScoreValue * ComboMultiplier * (WasCriticalKill ? 1.5f : 1.0f));

        /// <summary>
        /// Gets the total cash value including multipliers.
        /// </summary>
        public double TotalCashValue => CashValue * ComboMultiplier * (WasCriticalKill ? 1.5 : 1.0);

        /// <summary>
        /// Gets the damage per second rate.
        /// </summary>
        public double DamagePerSecond => TimeAlive > 0 ? DamageDealt / TimeAlive : 0.0;

        /// <summary>
        /// Gets the efficiency rating based on kill method.
        /// </summary>
        public float EfficiencyRating
        {
            get
            {
                float rating = 1.0f;
                
                if (WasHeadshot) rating += 0.3f;
                if (WasCriticalKill) rating += 0.2f;
                if (WasExplosiveKill) rating += 0.1f;
                if (ComboMultiplier > 1.0f) rating += 0.1f;
                if (DistanceFromPlayer > 50f) rating += 0.1f;
                
                return System.Math.Clamp(rating, 0f, 2f);
            }
        }

        /// <summary>
        /// Adds a damage source to the death event.
        /// </summary>
        /// <param name="source">Name of the damage source</param>
        /// <param name="damage">Amount of damage dealt</param>
        public void AddDamageSource(string source, double damage)
        {
            if (!string.IsNullOrEmpty(source))
            {
                DamageSources.Add(source);
                
                if (DamageBreakdown.ContainsKey(source))
                {
                    DamageBreakdown[source] += damage;
                }
                else
                {
                    DamageBreakdown[source] = damage;
                }
                
                DamageDealt += (int)damage;
            }
        }

        /// <summary>
        /// Adds a statistic to the kill data.
        /// </summary>
        /// <param name="key">Statistic key</param>
        /// <param name="value">Statistic value</param>
        public void AddStatistic(string key, object value)
        {
            KillStatistics[key] = value;
        }

        /// <summary>
        /// Gets a statistic from the kill data.
        /// </summary>
        /// <typeparam name="T">Type of the statistic</typeparam>
        /// <param name="key">Statistic key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Statistic value or default</returns>
        public T GetStatistic<T>(string key, T defaultValue = default)
        {
            if (KillStatistics.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Adds an achievement to the triggered achievements list.
        /// </summary>
        /// <param name="achievement">Achievement name</param>
        public void AddAchievement(string achievement)
        {
            if (!string.IsNullOrEmpty(achievement) && !AchievementsTriggered.Contains(achievement))
            {
                AchievementsTriggered.Add(achievement);
            }
        }

        /// <summary>
        /// Gets the primary damage source.
        /// </summary>
        /// <returns>The damage source that dealt the most damage</returns>
        public string GetPrimaryDamageSource()
        {
            if (DamageBreakdown.Count == 0) return "Unknown";

            double maxDamage = 0;
            var primarySource = "Unknown";
            
            foreach (var kvp in DamageBreakdown)
            {
                if (kvp.Value > maxDamage)
                {
                    maxDamage = kvp.Value;
                    primarySource = kvp.Key;
                }
            }
            
            return primarySource;
        }

        /// <summary>
        /// Creates a summary string of the enemy death event.
        /// </summary>
        /// <returns>Formatted summary string</returns>
        public override string ToString()
        {
            return $"{EnemyType} killed by {KillerName} - " +
                   $"Score: {TotalScoreValue} - " +
                   $"Efficiency: {EfficiencyRating:F2}x - " +
                   $"Time: {TimeAlive:F1}s";
        }

        /// <summary>
        /// Creates a detailed report of the enemy death event.
        /// </summary>
        /// <returns>Detailed report string</returns>
        public string GetDetailedReport()
        {
            var report = new List<string>
            {
                $"=== Enemy Death Report ===",
                $"Enemy Type: {EnemyType}",
                $"Enemy ID: {EnemyId}",
                $"Death Position: {DeathPosition}",
                $"Death Time: {DeathTime:HH:mm:ss}",
                $"Time Alive: {TimeAlive:F2} seconds",
                "",
                "Kill Information:",
                $"  Killer: {KillerName}",
                $"  Tower Type: {TowerType}",
                $"  Was Headshot: {WasHeadshot}",
                $"  Was Critical: {WasCriticalKill}",
                $"  Was Boss: {WasBoss}",
                $"  Combo Multiplier: {ComboMultiplier:F1}x",
                "",
                "Damage Analysis:",
                $"  Total Damage: {DamageDealt}",
                $"  Damage/Second: {DamagePerSecond:F1}",
                $"  Primary Source: {GetPrimaryDamageSource()}",
                $"  Distance from Player: {DistanceFromPlayer:F1}",
                "",
                "Rewards:",
                $"  Base Score: {ScoreValue}",
                $"  Total Score: {TotalScoreValue}",
                $"  Base Cash: ${CashValue:F2}",
                $"  Total Cash: ${TotalCashValue:F2}",
                $"  Efficiency Rating: {EfficiencyRating:F2}x",
                "",
                "Damage Sources:"
            };

            foreach (var source in DamageSources)
            {
                var damage = DamageBreakdown.ContainsKey(source) ? DamageBreakdown[source] : 0f;
                report.Add($"  {source}: {damage:F1}");
            }

            if (AchievementsTriggered.Count > 0)
            {
                report.Add("");
                report.Add("Achievements Triggered:");
                foreach (var achievement in AchievementsTriggered)
                {
                    report.Add($"  - {achievement}");
                }
            }

            if (KillStatistics.Count > 0)
            {
                report.Add("");
                report.Add("Additional Statistics:");
                foreach (var stat in KillStatistics)
                {
                    report.Add($"  {stat.Key}: {stat.Value}");
                }
            }

            return string.Join(Environment.NewLine, report);
        }

        /// <summary>
        /// Clones this EnemyDeathEvent.
        /// </summary>
        /// <returns>A new EnemyDeathEvent with the same data</returns>
        public EnemyDeathEvent Clone()
        {
            return new EnemyDeathEvent
            {
                EnemyId = this.EnemyId,
                EnemyType = this.EnemyType,
                DeathPosition = this.DeathPosition,
                DeathTime = this.DeathTime,
                DamageDealt = this.DamageDealt,
                TimeAlive = this.TimeAlive,
                WasHeadshot = this.WasHeadshot,
                WasMeleeKill = this.WasMeleeKill,
                WasExplosiveKill = this.WasExplosiveKill,
                KillerId = this.KillerId,
                KillerName = this.KillerName,
                TowerId = this.TowerId,
                TowerType = this.TowerType,
                ScoreValue = this.ScoreValue,
                CashValue = this.CashValue,
                DamageSources = new List<string>(this.DamageSources),
                DamageBreakdown = new Dictionary<string, double>(this.DamageBreakdown),
                DistanceFromPlayer = this.DistanceFromPlayer,
                WasBoss = this.WasBoss,
                WaveNumber = this.WaveNumber,
                WasCriticalKill = this.WasCriticalKill,
                ComboMultiplier = this.ComboMultiplier,
                AchievementsTriggered = new List<string>(this.AchievementsTriggered),
                KillStatistics = new Dictionary<string, object>(this.KillStatistics)
            };
        }
    }
}
