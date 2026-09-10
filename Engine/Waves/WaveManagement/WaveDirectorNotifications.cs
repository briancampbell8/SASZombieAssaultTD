// =====================================================================================================
//  FILE: WaveDirectorNotifications.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorNotifications.cs
//  SUBSYSTEM: Waves WaveManagement
//
//  ROLE:
//      Provides deterministic, non‑gameplay UI notifications for wave start, wave completion,
//      wave descriptions, and wave reward announcements.
//      Acts as the presentation layer for WaveDirectorCore, converting wave events into
//      user‑facing banner notifications and audio cues.
//
//  RESPONSIBILITIES:
//      - Display wave start and completion notifications.
//      - Generate human‑readable wave descriptions based on difficulty and progression.
//      - Announce wave completion bonuses.
//      - Integrate with ModernPlaySound and NotificationBanner deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Performing wave lifecycle control (WaveDirector_WaveFlow handles lifecycle).
//      - Spawning logic (WaveDirector_Spawning handles spawning).
//      - Script loading (WaveDirectorInitialization handles loading).
//      - Stats and progress calculations (WaveDirector_Stats handles stats).
//
//  ARCHITECTURAL NOTES:
//      - All UI and reward logic is internal and never exposed publicly.
//      - Must remain deterministic and avoid gameplay drift.
//      - NotificationBanner is a stubbed UI system until full UI integration is implemented.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.GameRoot.GamePlay;
using SASZombieAssaultTD.Engine.Waves.Difficulty;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public sealed class WaveDirectorNotifications
    {
        //===============================================================================================
        // WAVE START NOTIFICATION
        //===============================================================================================

        internal void ShowWaveNotification_Internal(WaveScript script)
        {
            if (script == null)
                return;

            try
            {
                var description = GetWaveDescription_Internal(script);

                ModernPlaySound.Play("wave_start");

                NotificationBanner.Show(
                    title: $"Wave {script.WaveNumber}",
                    message: description,
                    duration: 3f
                );
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to show wave notification: {ex.Message}");
            }
        }

        //===============================================================================================
        // WAVE COMPLETE NOTIFICATION
        //===============================================================================================

        internal void ShowWaveCompleteNotification_Internal(WaveScript script)
        {
            if (script == null)
                return;

            try
            {
                ModernPlaySound.Play("wave_complete");

                NotificationBanner.Show(
                    title: $"Wave {script.WaveNumber} Complete",
                    message: "Prepare for the next wave!",
                    duration: 3f
                );
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to show wave completion notification: {ex.Message}");
            }
        }

        //===============================================================================================
        // WAVE DESCRIPTION
        //===============================================================================================

        internal string GetWaveDescription_Internal(WaveScript script)
        {
            if (script == null)
                return "Unknown wave";

            try
            {
                // FIX: DifficultyManager is typed as object in WaveDirectorCore → cast required
                var difficulty = ((DifficultyManager)WaveDirectorCore.Instance.DifficultyManager)
                                 .GetCurrentDifficulty();

                var config = DifficultyConfig.Get(difficulty);

                // FIX: Use correct indexer syntax
                var progression = DifficultyProgression.GetMultiplier[script.WaveNumber];

                int totalEnemies = 0;

                foreach (var group in script.SpawnGroups)
                {
                    int scaledCount = DifficultyProgression.GetScaledEnemyCount(
                        waveNumber: script.WaveNumber,
                        enemyType: group.EnemyType,
                        difficulty: difficulty,
                        baseCount: group.Count,
                        config: config,
                        progression: progression);

                    totalEnemies += scaledCount;
                }

                return $"{totalEnemies} enemies approaching";
            }
            catch
            {
                return "Enemies approaching";
            }
        }

        //===============================================================================================
        // WAVE COMPLETION BONUS
        //===============================================================================================

        internal void AwardWaveCompletionBonus_Internal(int waveNumber)
        {
            try
            {
                int bonus = 25 + (waveNumber * 5);

                // Deterministic reward accumulation
                PlayerStats.Instance.AddedCash += bonus;

                NotificationBanner.Show(
                    title: "Wave Bonus",
                    message: $"+{bonus} Cash",
                    duration: 2.5f
                );

                ModernPlaySound.Play("reward");
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to award wave completion bonus: {ex.Message}");
            }
        }
    }

    internal class DifficultyManager
    {
        internal DifficultyMode GetCurrentDifficulty()
        {
            // Return the current difficulty mode. The surrounding class context shown does not
            // expose a backing field or property for the active difficulty, so return the
            // default enum value to avoid introducing assumptions about storage or members.
            return default;
        }
    }

    //===============================================================================================
    // NOTIFICATION BANNER IMPLEMENTATION
    //===============================================================================================

    internal static class NotificationBanner
    {
        internal static void Show(string title, string message, float duration)
        {
            NotImplementedGuard.Hit("UI_NOTIFICATION_SYSTEM");
            DLogger.Log($"[NotificationBanner] {title}: {message} (duration {duration}s)");
        }
    }
}
