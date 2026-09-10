// ====================================================================================================
//  FILE: PlayerProgressionController.cs
//  PATH: ./Engine/Player/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the PlayerProgressionController module.
//
//  RESPONSIBILITIES:
//      - Provide AddExperience() behavior for the Core subsystem.
//      - Provide Reset() behavior for the Core subsystem.
//      - Provide RestoreFromData() behavior for the Core subsystem.
//      - Provide GetTotalExperienceEarned() behavior for the Core subsystem.
//      - Provide GetData() behavior for the Core subsystem.
//      - Provide GetUnlockedTowers() behavior for the Core subsystem.
//      - Provide IsTowerUnlocked() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File: PlayerProgressionController.cs
//FilePath: Engine/Player/PlayerProgressionController.cs
//Purpose: Runtime driver for player progression. Performs all progression
//actions, updates state, triggers events, and coordinates unlock logic.
//Integration: 
//  - Uses ProgressionCore for XP math and unlock rules.
//  - Uses PlayerSystem for event dispatch.
//  - Uses PlayerProgressionData as the persistence container.
//  - Uses PlayerState for runtime player context.
//Data Flow:
//  - XP enters through AddExperience().
//  - Level changes propagate through LevelUp().
//  - Unlocks propagate through CheckTowerUnlocks().
//  - Save/load systems interact through GetData() and RestoreFromData().
//============================================================================

//
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Snapshot;

namespace SASZombieAssaultTD.Engine.Player
{
    ///<summary>
    ///Orchestrates all progression operations. Executes XP updates, level
    ///transitions, unlock checks, and event dispatch. No math is performed
    ///directly; all rules are delegated to ProgressionCore.
    ///</summary>
    public class PlayerProgressionController
    {
        private readonly PlayerProgressionData _data;      //Persistent progression values
        private readonly PlayerState _playerState;         //Runtime player context

        private int _experienceToNextLevel;                //Cached XP requirement
        private long _totalExperienceEarned;               //Cumulative XP across all levels

        private readonly object _lock = new object();      //Thread-safety gate

        //--------------------------------------------------------------------
        //Properties
        //--------------------------------------------------------------------

        public int CurrentLevel
        {
            get
            {
                lock (_lock)
                    return _data.CurrentLevel;
            }
        }

        public int CurrentExperience
        {
            get
            {
                lock (_lock)
                    return _data.CurrentExperience;
            }
        }

        public int ExperienceToNextLevel
        {
            get
            {
                lock (_lock)
                    return _experienceToNextLevel;
            }
        }

        public HashSet<string> UnlockedTowers
        {
            get
            {
                lock (_lock)
                    return new HashSet<string>(_data.UnlockedTowers);
            }
        }

        //--------------------------------------------------------------------
        //Constructors
        //--------------------------------------------------------------------

        public PlayerProgressionController(PlayerState playerState)
        {
            _playerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
            _data = new PlayerProgressionData();

            _experienceToNextLevel = ProgressionCore.CalculateExperienceForNextLevel(1);
            _totalExperienceEarned = 0;

            DLogger.Log(LogSubsystems.Player,
                $"PlayerProgressionController: Initialized - Level {_data.CurrentLevel}, XP {_data.CurrentExperience}/{_experienceToNextLevel}",
                "Info");
        }

        public PlayerProgressionController(PlayerState playerState, PlayerProgressionData data)
        {
            _playerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
            _data = data ?? throw new ArgumentNullException(nameof(data));

            _experienceToNextLevel = ProgressionCore.CalculateExperienceForNextLevel(_data.CurrentLevel);
            _totalExperienceEarned = 0;

            DLogger.Log(
                LogSubsystems.Player,
                $"PlayerProgressionController: Initialized with data - Level {_data.CurrentLevel}, XP {_data.CurrentExperience}/{_experienceToNextLevel}",
                "Info");
        }

        //--------------------------------------------------------------------
        //Experience Handling
        //--------------------------------------------------------------------

        public void AddExperience(int amount, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            if (!ProgressionCore.IsValidExperienceAddition(amount))
                throw new ArgumentOutOfRangeException(nameof(amount));

            lock (_lock)
            {
                try
                {
                    int initialLevel = _data.CurrentLevel;

                    _data.CurrentExperience += amount;     //XP increment
                    _totalExperienceEarned += amount;      //Cumulative XP

                    //Level-up loop (supports multi-level jumps)
                    while (_data.CurrentExperience >= _experienceToNextLevel &&
                           _data.CurrentLevel < ProgressionCore.MAX_LEVEL)
                    {
                        LevelUp();
                    }

                    //Max-level clamp
                    if (_data.CurrentLevel >= ProgressionCore.MAX_LEVEL)
                    {
                        _data.CurrentExperience = 0;
                        _experienceToNextLevel = int.MaxValue;
                    }

                    //Diagnostics
                    if (initialLevel < _data.CurrentLevel)
                    {
                        DLogger.Log(LogSubsystems.Player,
                            $"PlayerProgressionController: Level up {initialLevel} → {_data.CurrentLevel} (XP {amount}, Source {source})",
                            "Info");
                    }
                    else
                    {
                        DLogger.Log(LogSubsystems.Player,
                            $"PlayerProgressionController: XP added {amount} (Source {source}) → {_data.CurrentExperience}/{_experienceToNextLevel}",
                            "Debug");
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: AddExperience failed ({ex.Message})",
                        "Error");
                    throw;
                }
            }
        }

        //--------------------------------------------------------------------
        //Level-Up Processing
        //--------------------------------------------------------------------

        private void LevelUp()
        {
            try
            {
                int carryOver = _data.CurrentExperience - _experienceToNextLevel;

                _data.CurrentLevel = ProgressionCore.ClampLevel(_data.CurrentLevel + 1);
                _experienceToNextLevel = ProgressionCore.CalculateExperienceForNextLevel(_data.CurrentLevel);
                _data.CurrentExperience = ProgressionCore.ClampExperience(carryOver, _experienceToNextLevel);

                CheckTowerUnlocks(_data.CurrentLevel);

                var evt = new LevelUp
                {
                    NewLevel = _data.CurrentLevel,
                    Experience = _data.CurrentExperience
                };

                PlayerSystem.Instance.TriggerLevelUp(evt);

                DLogger.Log(LogSubsystems.Player,
                    $"PlayerProgressionController: LevelUp processed → Level {_data.CurrentLevel}, Carry {carryOver}, Next {_experienceToNextLevel}",
                    "Info");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Player,
                    $"PlayerProgressionController: LevelUp failed ({ex.Message})",
                    "Error");
                throw;
            }
        }

        //--------------------------------------------------------------------
        //Unlock Processing
        //--------------------------------------------------------------------

        private void CheckTowerUnlocks(int newLevel)
        {
            try
            {
                int unlocked = 0;

                foreach (var kvp in ProgressionCore.TowerUnlockLevels)
                {
                    string towerId = kvp.Key;
                    int requiredLevel = kvp.Value;

                    if (newLevel >= requiredLevel &&
                        !_data.UnlockedTowers.Contains(towerId))
                    {
                        _data.UnlockedTowers.Add(towerId);
                        unlocked++;

                        PlayerSystem.Instance.TriggerUnlock(towerId);

                        DLogger.Log(LogSubsystems.Player,
                            $"PlayerProgressionController: Tower unlocked {towerId} at level {newLevel}",
                            "Info");
                    }
                }

                if (unlocked > 0)
                {
                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: Unlock scan complete ({unlocked} new, {_data.UnlockedTowers.Count} total)",
                        "Info");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Player,
                    $"PlayerProgressionController: CheckTowerUnlocks failed ({ex.Message})",
                    "Error");
            }
        }

        //--------------------------------------------------------------------
        //Reset / Restore
        //--------------------------------------------------------------------

        public void Reset()
        {
            lock (_lock)
            {
                try
                {
                    _data.CurrentLevel = 1;
                    _data.CurrentExperience = 0;
                    _experienceToNextLevel = ProgressionCore.CalculateExperienceForNextLevel(1);
                    _totalExperienceEarned = 0;

                    _data.UnlockedTowers.Clear();
                    CheckTowerUnlocks(1);

                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: Reset → Level {_data.CurrentLevel}, XP {_data.CurrentExperience}/{_experienceToNextLevel}",
                        "Info");
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: Reset failed ({ex.Message})",
                        "Error");

                    _data.CurrentLevel = 1;
                    _data.CurrentExperience = 0;
                    _experienceToNextLevel = 100;
                    _data.UnlockedTowers.Clear();
                    _totalExperienceEarned = 0;
                }
            }
        }

        public void RestoreFromData(PlayerProgressionData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            lock (_lock)
            {
                try
                {
                    _data.CurrentLevel = ProgressionCore.ClampLevel(data.CurrentLevel);
                    _data.CurrentExperience = ProgressionCore.ClampExperience(
                        data.CurrentExperience,
                        _experienceToNextLevel);

                    _experienceToNextLevel = ProgressionCore.CalculateExperienceForNextLevel(_data.CurrentLevel);

                    _data.UnlockedTowers.Clear();

                    if (data.UnlockedTowers != null)
                    {
                        foreach (string id in data.UnlockedTowers)
                        {
                            if (!string.IsNullOrWhiteSpace(id))
                                _data.UnlockedTowers.Add(id);
                        }
                    }

                    CheckTowerUnlocks(_data.CurrentLevel);

                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: Restored → Level {_data.CurrentLevel}, XP {_data.CurrentExperience}/{_experienceToNextLevel}",
                        "Info");
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Player,
                        $"PlayerProgressionController: Restore failed ({ex.Message})",
                        "Error");

                    _data.CurrentLevel = 1;
                    _data.CurrentExperience = 0;
                    _experienceToNextLevel = 100;
                    _data.UnlockedTowers.Clear();
                    _totalExperienceEarned = 0;

                    CheckTowerUnlocks(1);
                }
            }
        }

        //--------------------------------------------------------------------
        //Data Accessors
        //--------------------------------------------------------------------

        public long GetTotalExperienceEarned()
        {
            lock (_lock)
                return _totalExperienceEarned;
        }

        public PlayerProgressionData GetData()
        {
            lock (_lock)
            {
                return new PlayerProgressionData
                {
                    CurrentLevel = _data.CurrentLevel,
                    CurrentExperience = _data.CurrentExperience,
                    UnlockedTowers = new HashSet<string>(_data.UnlockedTowers)
                };
            }
        }

        public List<string> GetUnlockedTowers()
        {
            lock (_lock)
                return new List<string>(_data.UnlockedTowers);
        }

        public bool IsTowerUnlocked(string towerId)
        {
            if (string.IsNullOrWhiteSpace(towerId))
                throw new ArgumentNullException(nameof(towerId));

            lock (_lock)
                return _data.UnlockedTowers.Contains(towerId);
        }
    }
}

