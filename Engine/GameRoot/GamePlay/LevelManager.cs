// ====================================================================================================
//  FILE: LevelManager.cs
//  PATH: ./Engine/Gameplay/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the LevelManager module.
//
//  RESPONSIBILITIES:
//      - Provide LoadLevel() behavior for the Core subsystem.
//      - Provide UnloadLevel() behavior for the Core subsystem.
//      - Provide NextWave() behavior for the Core subsystem.
//      - Provide IsLevelComplete() behavior for the Core subsystem.
//      - Provide CanStartNextLevel() behavior for the Core subsystem.
//      - Provide GetNextLevelId() behavior for the Core subsystem.
//      - Provide GetLevelData() behavior for the Core subsystem.
//      - Provide GetAllLevels() behavior for the Core subsystem.
//      - Provide AddLevel() behavior for the Core subsystem.
//      - Provide RemoveLevel() behavior for the Core subsystem.
//      - Provide ResetProgress() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay
{
    ///<summary>
    ///Level manager for SAS Zombie Assault TD.
    ///Manages level loading, progression, and state.
    ///</summary>
    public class LevelManager
    {
        private readonly Dictionary<string, LevelData> _levels = new Dictionary<string, LevelData>();
        private string _currentLevelId = string.Empty;
        private LevelData _currentLevel = null;
        private int _currentWave = 0;

        public string CurrentLevelId => _currentLevelId;
        public LevelData CurrentLevel => _currentLevel;
        public int CurrentWave => _currentWave;
        public bool IsLevelLoaded => _currentLevel != null;

        public event Action<string> OnLevelLoaded;
        public event Action<string> OnLevelUnloaded;
        public event Action<int> OnWaveChanged;

        public LevelManager() => InitializeDefaultLevels();

        private void InitializeDefaultLevels()
        {
            //Create some default levels
            _levels["level_1"] = new LevelData
            {
                Id = "level_1",
                Name = "First Wave",
                Description = "The first zombie wave approaches",
                MaxWaves = 5,
                StartingLives = 20,
                StartingMoney = 500,
                Difficulty = 1.0f
            };

            _levels["level_2"] = new LevelData
            {
                Id = "level_2",
                Name = "Second Wave",
                Description = "Stronger zombies incoming",
                MaxWaves = 8,
                StartingLives = 20,
                StartingMoney = 750,
                Difficulty = 1.5f
            };

            _levels["level_3"] = new LevelData
            {
                Id = "level_3",
                Name = "Third Wave",
                Description = "The horde grows stronger",
                MaxWaves = 12,
                StartingLives = 25,
                StartingMoney = 1000,
                Difficulty = 2.0f
            };
        }

        public bool LoadLevel(string levelId)
        {
            if (!_levels.ContainsKey(levelId))
            {
                return false;
            }

            //Unload current level if any
            if (!string.IsNullOrEmpty(_currentLevelId))
            {
                UnloadLevel();
            }

            _currentLevelId = levelId;
            _currentLevel = _levels[levelId];
            _currentWave = 0;

            OnLevelLoaded?.Invoke(levelId);
            return true;
        }

        public void UnloadLevel()
        {
            if (!string.IsNullOrEmpty(_currentLevelId))
            {
                string previousLevelId = _currentLevelId;
                _currentLevelId = string.Empty;
                _currentLevel = null;
                _currentWave = 0;

                OnLevelUnloaded?.Invoke(previousLevelId);
            }
        }

        public bool NextWave()
        {
            if (_currentLevel == null)
                return false;

            if (_currentWave >= _currentLevel.MaxWaves - 1)
                return false;

            _currentWave++;
            OnWaveChanged?.Invoke(_currentWave);
            return true;
        }

        public bool IsLevelComplete()
        {
            return _currentLevel != null && _currentWave >= _currentLevel.MaxWaves - 1;
        }

        public bool CanStartNextLevel()
        {
            return IsLevelComplete() && GetNextLevelId() != null;
        }

        public string GetNextLevelId()
        {
            if (string.IsNullOrEmpty(_currentLevelId))
                return null;

            var levelIds = new List<string>(_levels.Keys);
            int currentIndex = levelIds.IndexOf(_currentLevelId);

            if (currentIndex >= 0 && currentIndex < levelIds.Count - 1)
            {
                return levelIds[currentIndex + 1];
            }

            return null;
        }

        public LevelData GetLevelData(string levelId)
        {
            return _levels.TryGetValue(levelId, out LevelData data) ? data : null;
        }

        public IEnumerable<LevelData> GetAllLevels()
        {
            return _levels.Values;
        }

        public void AddLevel(LevelData levelData)
        {
            if (levelData != null && !string.IsNullOrEmpty(levelData.Id))
            {
                _levels[levelData.Id] = levelData;
            }
        }

        public void RemoveLevel(string levelId)
        {
            if (_currentLevelId == levelId)
            {
                UnloadLevel();
            }
            _levels.Remove(levelId);
        }

        public void ResetProgress()
        {
            UnloadLevel();
        }

        public class LevelData
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int MaxWaves { get; set; } = 10;
            public int StartingLives { get; set; } = 20;
            public int StartingMoney { get; set; } = 500;
            public float Difficulty { get; set; } = 1.0f;
            public Vector3 SpawnPoint { get; set; } = new Vector3(0, 0, 0);
            public Rectangle Bounds { get; set; } = new Rectangle(
                (int)(float)0,
                (int)(float)0,
                (int)(float)1920,
                (int)(float)1080);
            public string BackgroundTexture { get; set; } = "default_background";
            public string MusicTrack { get; set; } = "level_music";
        }
    }
}

