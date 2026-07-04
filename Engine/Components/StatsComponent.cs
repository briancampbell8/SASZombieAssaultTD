using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Core ECS component for tracking entity statistics.
    ///</summary>
    public class StatsComponent
    {
        ///<summary>
        ///Player-specific statistics, such as kills, deaths, and streaks.
        ///</summary>
        public PlayerStatsData PlayerStats { get; set; } = new PlayerStatsData();

        ///<summary>
        ///Tracks the number of kills by enemy type.
        ///</summary>
        public Dictionary<string, int> KillByType { get; private set; } = new();

        ///<summary>
        ///Total number of kills made by the entity.
        ///</summary>
        public int TotalKills { get; private set; }

        ///<summary>
        ///Total number of deaths of the entity.
        ///</summary>
        public int TotalDeaths { get; private set; }

        ///<summary>
        ///The type of the last death the entity experienced.
        ///</summary>
        public DeathType LastDeathType { get; private set; }

        ///<summary>
        ///The timestamp of the last kill made by the entity.
        ///</summary>
        public DateTime LastKillTime { get; private set; }

        ///<summary>
        ///The timestamp of the last death the entity experienced.
        ///</summary>
        public DateTime LastDeathTime { get; private set; }

        ///<summary>
        ///Records a kill and updates relevant statistics.
        ///</summary>
        ///<param name="enemyType">The type of enemy killed.</param>
        public void RecordKill(string enemyType)
        {
            if (string.IsNullOrWhiteSpace(enemyType))
                throw new ArgumentException("Enemy type cannot be null or empty.", nameof(enemyType));

            TotalKills++;
            LastKillTime = DateTime.UtcNow;

            if (KillByType.ContainsKey(enemyType))
            {
                KillByType[enemyType]++;
            }
            else
            {
                KillByType[enemyType] = 1;
            }

            PlayerStats.AddKill();
        }

        ///<summary>
        ///Records a death and updates relevant statistics.
        ///</summary>
        ///<param name="deathType">The type of death the entity experienced.</param>
        public void RecordDeath(DeathType deathType)
        {
            TotalDeaths++;
            LastDeathType = deathType;
            LastDeathTime = DateTime.UtcNow;

            PlayerStats.AddDeath();
        }

        ///<summary>
        ///Resets all statistics to their default values.
        ///</summary>
        public void Reset()
        {
            PlayerStats.Reset();
            KillByType.Clear();
            TotalKills = 0;
            TotalDeaths = 0;
            LastDeathType = default;
            LastKillTime = DateTime.MinValue;
            LastDeathTime = DateTime.MinValue;
        }

        ///<summary>
        ///Provides a summary of the entity's statistics.
        ///</summary>
        ///<returns>A formatted string summarizing the statistics.</returns>
        public override string ToString()
        {
            return $"Stats(Total Kills: {TotalKills}, Total Deaths: {TotalDeaths}, " +
                   $"Last Kill: {LastKillTime}, Last Death: {LastDeathTime})";
        }
    }

    public enum DeathType
    {
        Natural,
        Combat,
        Environmental
    }
}
