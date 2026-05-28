/* ====================================================================================================
 *  FILE: LevelProgression.cs
 *  PATH: Engine/LevelUpControl/LevelProgression.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Public façade and orchestrator for the progression system.
 *
 *  RESPONSIBILITIES:
 *      - Expose the public API for milestones, achievements, and progression events.
 *      - Coordinate with PlayerLevel and other engine systems.
 *      - Maintain in-memory collections of milestones, achievements, and events.
 *      - Delegate serialization to ProgressionSerializer.
 *      - Delegate static content creation to ProgressionDefinitions.
 *      - Delegate progression rule evaluation to ProgressionLogic.
 *
 *  NON-RESPONSIBILITIES:
 *      - JSON serialization (handled by ProgressionSerializer).
 *      - Data modeling (handled by ProgressionModels).
 *      - DTO definitions (handled by ProgressionDTOs).
 *      - Static milestone/achievement/event definitions (handled by ProgressionDefinitions).
 *      - Business logic for progression (handled by ProgressionLogic).
 *
 *  DEPENDENCIES:
 *      - ProgressionModels.cs
 *      - ProgressionDTOs.cs
 *      - ProgressionSerializer.cs
 *      - ProgressionLogic.cs
 *      - ProgressionDefinitions.cs
 *      - PlayerLevel.cs
 *      - ModernPlayerStateSystem.cs
 *
 *  CALLED BY:
 *      - UI systems
 *      - Gameplay systems
 *      - Save/Load systems
 *      - Player progression systems
 *
 *  CALLS INTO:
 *      - ProgressionSerializer (save/load)
 *      - ProgressionLogic (achievement/milestone evaluation)
 *      - ProgressionDefinitions (initialization)
 *      - PlayerLevel (event subscriptions)
 *
 *  ARCHITECTURAL NOTES:
 *      - This class is intentionally thin; it is the façade and must remain stable.
 *      - All heavy logic is delegated to subsystem files to prevent monolithic drift.
 *      - This class must not contain serialization logic or static content definitions.
 *      - This class must remain safe for external callers and maintain backward compatibility.
 *
 * ==================================================================================================== */

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    /// <summary>
    /// Public façade for the progression system.
    /// </summary>
    public class LevelProgression
    {
        // INTERNAL STATE (exposed to serializer and logic through internal accessors)
        internal readonly List<ProgressionMilestone> _milestones;
        internal readonly Dictionary<string, ProgressionAchievement> _achievements;
        internal readonly List<ProgressionEvent> _events;

        private bool _isInitialized;
        private static LevelProgression _instance;

        // Optional external blob (kept for compatibility)
        private Dictionary<string, object> AchievementData;
        private object TheType;
        private object TheMember;

        // EVENTS EXPOSED TO OTHER SYSTEMS
        public event Action<ProgressionMilestone> OnMilestoneReached;
        public event Action<ProgressionAchievement> OnAchievementUnlocked;
        public event Action<ProgressionEvent> OnProgressionEvent;
        public event Action OnAllMilestonesCompleted;

        // PUBLIC PROPERTIES
        public bool IsInitialized => _isInitialized;
        public int TotalMilestones => _milestones.Count;
        public int TotalAchievements => _achievements.Count;
        public int CompletedMilestones => _milestones.Count(m => m.IsCompleted);
        public int UnlockedAchievements => _achievements.Count(static a => a.IsUnlocked());
        public IReadOnlyList<ProgressionMilestone> AllMilestones => _milestones;
        public IReadOnlyDictionary<string, ProgressionAchievement> AllAchievements => _achievements;

        // SINGLETON ACCESSOR
        public static LevelProgression Instance => _instance ??= new LevelProgression();

        // METHOD: Constructor
        // PURPOSE: Initialize internal collections and load static definitions.
        // CALLED BY: Singleton accessor
        // CALLS INTO: ProgressionDefinitions
        // NOTES: Heavy logic is delegated; constructor must remain lightweight.
        private LevelProgression()
        {
            _milestones = new List<ProgressionMilestone>();
            _achievements = new Dictionary<string, ProgressionAchievement>();
            _events = new List<ProgressionEvent>();

            ProgressionDefinitions.InitializeMilestones(_milestones);
            ProgressionDefinitions.InitializeAchievements(_achievements);
            ProgressionDefinitions.InitializeEvents(_events);
        }

        // METHOD: Initialize()
        // PURPOSE: Hook into PlayerLevel events and activate the progression system.
        // CALLED BY: Game startup systems
        // CALLS INTO: PlayerLevel event subscriptions
        // NOTES: Must be idempotent.
        public void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                if (PlayerLevel.Instance != null)
                {
                    PlayerLevel.Instance.OnLevelUp += OnPlayerLevelUp;
                    PlayerLevel.Instance.OnExperienceGained += OnExperienceGained;
                    PlayerLevel.Instance.OnMaxLevelReached += OnMaxLevelReached;
                }

                _isInitialized = true;
                OnAllMilestonesCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize Level Progression System: {ex.Message}");
                throw;
            }
        }

        // METHOD: AddMilestone()
        // PURPOSE: Add a custom milestone at runtime.
        // CALLED BY: Mod systems, dynamic content loaders
        // CALLS INTO: None
        public bool AddMilestone(ProgressionMilestone milestone)
        {
            if (milestone == null) return false;
            if (_milestones.Any(m => m.Id == milestone.Id)) return false;

            _milestones.Add(milestone);
            return true;
        }

        // METHOD: AddAchievement()
        // PURPOSE: Add a custom achievement at runtime.
        public bool AddAchievement(ProgressionAchievement achievement)
        {
            if (achievement == null) return false;
            if (_achievements.ContainsKey(achievement.Id)) return false;

            _achievements[achievement.Id] = achievement;
            return true;
        }

        // METHOD: AddProgressionEvent()
        // PURPOSE: Add a custom event at runtime.
        public bool AddProgressionEvent(ProgressionEvent progressionEvent)
        {
            if (progressionEvent == null) return false;
            _events.Add(progressionEvent);
            return true;
        }

        // METHOD: IsMilestoneCompleted()
        // PURPOSE: Query milestone completion state.
        public bool IsMilestoneCompleted(string milestoneId)
        {
            return _milestones.FirstOrDefault(m => m.Id == milestoneId)?.IsCompleted ?? false;
        }

        // METHOD: IsAchievementUnlocked()
        // PURPOSE: Query achievement unlock state.
        public bool IsAchievementUnlocked(string achievementId)
        {
            return _achievements.TryGetValue(achievementId, out var a) && a.IsUnlocked;
        }

        // METHOD: GetCompletedMilestones()
        // PURPOSE: Return all completed milestones.
        public IReadOnlyList<ProgressionMilestone> GetCompletedMilestones()
        {
            return _milestones.Where(m => m.IsCompleted).ToList();
        }

        // METHOD: GetAvailableAchievements()
        // PURPOSE: Return all unlocked achievements.
        public IReadOnlyList<ProgressionAchievement> GetAvailableAchievements()
        {
            return _achievements.Values.Where(a => a.IsUnlocked).ToList();
        }

        // METHOD: GetStatistics()
        // PURPOSE: Build a statistics snapshot for UI.
        // CALLS INTO: None
        public ProgressionStatistics GetStatistics()
        {
            return new ProgressionStatistics
            {
                TotalMilestones = _milestones.Count,
                CompletedMilestones = CompletedMilestones,
                TotalAchievements = _achievements.Count,
                UnlockedAchievements = UnlockedAchievements,
                CompletionPercentage = _milestones.Count > 0
                    ? (float)CompletedMilestones / _milestones.Count
                    : 0f,
                RecentMilestones = _milestones.Where(m => m.IsCompleted).Take(5).ToList(),
                RecentAchievements = _achievements.Values
                    .Where(a => a.IsUnlocked && a.UnlockedDate.HasValue)
                    .OrderByDescending(a => a.UnlockedDate)
                    .Take(10)
                    .ToList(),
                TotalEvents = _events.Count,
                AverageMilestonesPerSession = 2.5f
            };
        }

        // METHOD: ResetProgression()
        // PURPOSE: Reset all progression state.
        public void ResetProgression()
        {
            foreach (var m in _milestones)
            {
                m.IsCompleted = false;
                m.CompletionDate = null;
            }

            foreach (var a in _achievements.Values)
            {
                a.IsUnlocked = false;
                a.UnlockedDate = null;
                a.Progress = 0f;
            }

            _events.Clear();
        }

        // METHOD: SaveProgression()
        // PURPOSE: Save progression using serializer.
        public bool SaveProgression()
        {
            return SaveProgression(AchievementData);
        }

        // METHOD: SaveProgression(achievementData)
        // PURPOSE: Save progression with external achievement blob.
        // CALLS INTO: ProgressionSerializer
        public bool SaveProgression(Dictionary<string, object> achievementData)
        {
            try
            {
                var saveData = ProgressionSerializer.CreateSaveData(this, achievementData);

                var json = JsonSerializer.Serialize(saveData);
                var savePath = Path.Combine("Data", "Progression", "progression.json");
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                File.WriteAllText(savePath, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving progression: {ex.Message}");
                return false;
            }
        }

        // METHOD: LoadProgression()
        // PURPOSE: Load progression using serializer.
        // CALLS INTO: ProgressionSerializer
        public bool LoadProgression()
        {
            try
            {
                var savePath = Path.Combine("Data", "Progression", "progression.json");
                if (!File.Exists(savePath))
                    return false;

                var json = File.ReadAllText(savePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var saveData = JsonSerializer.Deserialize<ProgressionSaveData>(json, options);

                if (saveData == null)
                    return false;

                ProgressionSerializer.RestoreFromSaveData(this, saveData);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading progression: {ex.Message}");
                return false;
            }
        }

        // EVENT HANDLERS — delegate heavy logic to ProgressionLogic

        private void OnPlayerLevelUp(int newLevel)
        {
            ProgressionLogic.HandleLevelUp(this, newLevel);
        }

        private void OnExperienceGained(int level, int experience)
        {
            ProgressionLogic.HandleExperienceGained(this, level, experience);
        }

        private void OnMaxLevelReached()
        {
            ProgressionLogic.HandleMaxLevelReached(this);
        }

        internal void InvokeProgressionEvent(ProgressionEvent progressionEvent)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal void InvokeAchievementUnlocked(ProgressionAchievement achievement)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal void InvokeMilestoneReached(ProgressionMilestone milestone)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
