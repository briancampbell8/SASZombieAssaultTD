using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Stores player statistics for tracking kills, deaths, and streaks.
    /// </summary>
    public class PlayerStatsData
    {
        /// <summary>
        /// Total number of kills by the player.
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Total number of deaths of the player.
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        /// Current kill streak of the player.
        /// </summary>
        public int CurrentKillStreak { get; set; }

        /// <summary>
        /// Longest kill streak achieved by the player.
        /// </summary>
        public int LongestKillStreak { get; set; }

        /// <summary>
        /// Timestamp of the last kill made by the player.
        /// </summary>
        public DateTime LastKillTime { get; set; }

        /// <summary>
        /// History of kills, storing the number of kills in each session or round.
        /// </summary>
        public List<int> KillHistory { get; set; } = new();

        /// <summary>
        /// Resets the player's statistics to their default values.
        /// </summary>
        public void Reset()
        {
            Kills = 0;
            Deaths = 0;
            CurrentKillStreak = 0;
            LongestKillStreak = 0;
            LastKillTime = DateTime.MinValue;
            KillHistory.Clear();
        }

        /// <summary>
        /// Updates the kill statistics when a new kill is made.
        /// </summary>
        public void AddKill()
        {
            Kills++;
            CurrentKillStreak++;
            LastKillTime = DateTime.UtcNow;

            if (CurrentKillStreak > LongestKillStreak)
            {
                LongestKillStreak = CurrentKillStreak;
            }
        }

        /// <summary>
        /// Updates the death statistics when the player dies.
        /// </summary>
        public void AddDeath()
        {
            Deaths++;
            KillHistory.Add(CurrentKillStreak);
            CurrentKillStreak = 0;
        }

        /// <summary>
        /// Provides a summary of the player's statistics.
        /// </summary>
        /// <returns>A formatted string summarizing the player's stats.</returns>
        public override string ToString()
        {
            return $"Kills: {Kills}, Deaths: {Deaths}, Current Streak: {CurrentKillStreak}, " +
                   $"Longest Streak: {LongestKillStreak}, Last Kill: {LastKillTime}, " +
                   $"Kill History: [{string.Join(", ", KillHistory)}]";
        }
    }
}
