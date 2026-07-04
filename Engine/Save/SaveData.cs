using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Save
//
{
    ///<summary>
    ///Save data structure for player progress and settings.
    ///P20-10-01: Implements SaveData for player progress and settings.
    ///</summary>
    public class SaveData
    {
        private string _playerName;
        private int _highScore;
        private int _currentLevel;
        private int _totalKills;
        private int _totalWavesCompleted;
        private float _totalPlayTime;
        private DateTime _lastSaveTime;
        private GameSettings _settings;
        private List<LevelProgress> _levelProgress;
        private Dictionary<string, object> _customData;

        ///<summary>
        ///Gets or sets the player name.
        ///</summary>
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value ?? "Player";
        }

        ///<summary>
        ///Gets or sets the high score.
        ///</summary>
        public int HighScore
        {
            get => _highScore;
            set => _highScore = System.Math.Max(0, value);
        }

        ///<summary>
        ///Gets or sets the current level.
        ///</summary>
        public int CurrentLevel
        {
            get => _currentLevel;
            set => _currentLevel = System.Math.Max(1, value);
        }

        ///<summary>
        ///Gets or sets the total kills.
        ///</summary>
        public int TotalKills
        {
            get => _totalKills;
            set => _totalKills = System.Math.Max(0, value);
        }

        ///<summary>
        ///Gets or sets the total waves completed.
        ///</summary>
        public int TotalWavesCompleted
        {
            get => _totalWavesCompleted;
            set => _totalWavesCompleted = System.Math.Max(0, value);
        }

        ///<summary>
        ///Gets or sets the total play time in seconds.
        ///</summary>
        public float TotalPlayTime
        {
            get => _totalPlayTime;
            set => _totalPlayTime = System.Math.Max(0f, value);
        }

        ///<summary>
        ///Gets or sets the last save time.
        ///</summary>
        public DateTime LastSaveTime
        {
            get => _lastSaveTime;
            set => _lastSaveTime = value;
        }

        ///<summary>
        ///Gets or sets the game settings.
        ///</summary>
        public GameSettings Settings
        {
            get => _settings ?? (_settings = new GameSettings());
            set => _settings = value ?? new GameSettings();
        }

        ///<summary>
        ///Gets the list of level progress.
        ///</summary>
        public List<LevelProgress> LevelProgress => _levelProgress ?? (_levelProgress = new List<LevelProgress>());

        ///<summary>
        ///Gets the custom data dictionary.
        ///</summary>
        public Dictionary<string, object> CustomData => _customData ?? (_customData = new Dictionary<string, object>());

        ///<summary>
        ///Gets the save data version.
        ///</summary>
        public int Version { get; private set; }

        ///<summary>
        ///Event fired when save data is modified.
        ///</summary>
        public event Action<SaveData> OnDataModified;

        ///<summary>
        ///Initializes a new save data instance.
        ///</summary>
        public SaveData()
        {
            _playerName = "Player";
            _highScore = 0;
            _currentLevel = 1;
            _totalKills = 0;
            _totalWavesCompleted = 0;
            _totalPlayTime = 0f;
            _lastSaveTime = DateTime.UtcNow;
            _settings = new GameSettings();
            _levelProgress = new List<LevelProgress>();
            _customData = new Dictionary<string, object>();
            Version = 1;

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", "SaveData: Created new save data instance");
        }

        ///<summary>
        ///Updates the high score if the new score is higher.
        ///</summary>
        ///<param name="score">The new score to check.</param>
        ///<returns>True if high score was updated.</returns>
        public bool UpdateHighScore(int score)
        {
            if (score > _highScore)
            {
                _highScore = score;
                MarkAsModified();
                DLogger.Log(LogSubsystems.Save,LogLevel.Info, $"SaveData: New high score: {score}");
                return true;
            }
            return false;
        }

        ///<summary>
        ///Adds play time to the total.
        ///</summary>
        ///<param name="playTime">Play time to add in seconds.</param>
        public void AddPlayTime(float playTime)
        {
            _totalPlayTime += System.Math.Max(0f, playTime);
            MarkAsModified();
        }

        ///<summary>
        ///Adds kills to the total.
        ///</summary>
        ///<param name="kills">Number of kills to add.</param>
        public void AddKills(int kills)
        {
            _totalKills += System.Math.Max(0, kills);
            MarkAsModified();
        }

        ///<summary>
        ///Adds waves completed to the total.
        ///</summary>
        ///<param name="waves">Number of waves to add.</param>
        public void AddWavesCompleted(int waves)
        {
            _totalWavesCompleted += System.Math.Max(0, waves);
            MarkAsModified();
        }

        ///<summary>
        ///Gets progress for a specific level.
        ///</summary>
        ///<param name="levelId">The level ID.</param>
        ///<returns>The level progress, or null if not found.</returns>
        public LevelProgress GetLevelProgress(string levelId)
        {
            return _levelProgress?.Find(p => p.LevelId == levelId);
        }

        ///<summary>
        ///Updates progress for a specific level.
        ///</summary>
        ///<param name="levelId">The level ID.</param>
        ///<param name="completed">Whether the level was completed.</param>
        ///<param name="score">The score achieved.</param>
        ///<param name="stars">The number of stars earned.</param>
        public void UpdateLevelProgress(string levelId, bool completed, int score, int stars)
        {
            var progress = GetLevelProgress(levelId);
            if (progress == null)
            {
                progress = new LevelProgress(levelId);
                _levelProgress.Add(progress);
            }

            progress.UpdateProgress(completed, score, stars);
            MarkAsModified();
        }

        ///<summary>
        ///Sets custom data.
        ///</summary>
        ///<param name="key">The data key.</param>
        ///<param name="value">The data value.</param>
        public void SetCustomData(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            _customData[key] = value;
            MarkAsModified();
        }

        ///<summary>
        ///Gets custom data.
        ///</summary>
        ///<typeparam name="T">The data type.</typeparam>
        ///<param name="key">The data key.</param>
        ///<returns>The data value, or default if not found.</returns>
        public T GetCustomData<T>(string key)
        {
            if (string.IsNullOrEmpty(key) || !_customData.TryGetValue(key, out var value))
                return default;

            if (value is T typedValue)
                return typedValue;

            return default;
        }

        ///<summary>
        ///Resets all progress to default values.
        ///</summary>
        public void ResetProgress()
        {
            _highScore = 0;
            _currentLevel = 1;
            _totalKills = 0;
            _totalWavesCompleted = 0;
            _totalPlayTime = 0f;
            _levelProgress.Clear();
            _customData.Clear();
            MarkAsModified();

            DLogger.Log(LogSubsystems.Save,LogLevel.Info, "SaveData: Reset all progress to defaults");
        }

        ///<summary>
        ///Validates the save data.
        ///</summary>
        ///<returns>List of validation issues.</returns>
        public List<string> Validate()
        {
            var issues = new List<string>();

            if (string.IsNullOrEmpty(_playerName))
                issues.Add("Player name is empty");

            if (_highScore < 0)
                issues.Add("High score is negative");

            if (_currentLevel < 1)
                issues.Add("Current level is invalid");

            if (_totalKills < 0)
                issues.Add("Total kills is negative");

            if (_totalWavesCompleted < 0)
                issues.Add("Total waves completed is negative");

            if (_totalPlayTime < 0)
                issues.Add("Total play time is negative");

            //Validate settings
            var settingsIssues = _settings.Validate();
            issues.AddRange(settingsIssues);

            return issues;
        }

        ///<summary>
        ///Creates a copy of this save data.
        ///</summary>
        ///<returns>A new SaveData instance with the same values.</returns>
        public SaveData Clone()
        {
            var clone = new SaveData
            {
                _playerName = _playerName,
                _highScore = _highScore,
                _currentLevel = _currentLevel,
                _totalKills = _totalKills,
                _totalWavesCompleted = _totalWavesCompleted,
                _totalPlayTime = _totalPlayTime,
                _lastSaveTime = _lastSaveTime,
                _settings = _settings.Clone(),
                Version = Version
            };

            //Clone level progress
            foreach (var progress in _levelProgress)
            {
                clone._levelProgress.Add(progress.Clone());
            }

            //Clone custom data
            foreach (var kvp in _customData)
            {
                clone._customData[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        ///<summary>
        ///Marks the save data as modified.
        ///</summary>
        private void MarkAsModified()
        {
            _lastSaveTime = DateTime.UtcNow;
            OnDataModified?.Invoke(this);
        }

        ///<summary>
        ///Gets save data information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"SaveData: Player={_playerName}, HighScore={_highScore}, " +
            $"Level={_currentLevel}, Kills={_totalKills}, " +
            $"Waves={_totalWavesCompleted}, PlayTime={_totalPlayTime:F1}s, " +
            $"Version={Version}";
        }
    }

    ///<summary>
    ///Game settings structure.
    ///P20-10-03: Add settings persistence.
    ///</summary>
    public class GameSettings
    {
        private float _masterVolume;
        private float _musicVolume;
        private float _sfxVolume;
        private int _screenWidth;
        private int _screenHeight;
        private bool _fullscreen;
        private bool _vsync;
        private int _qualityLevel;
        private bool _showFPS;
        private bool _autoPause;

        ///<summary>
        ///Gets or sets the master volume (0.0 to 1.0).
        ///</summary>
        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = System.Math.Clamp(value, 0f, 1f);
        }

        ///<summary>
        ///Gets or sets the music volume (0.0 to 1.0).
        ///</summary>
        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = System.Math.Clamp(value, 0f, 1f);
        }

        ///<summary>
        ///Gets or sets the sound effects volume (0.0 to 1.0).
        ///</summary>
        public float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = System.Math.Clamp(value, 0f, 1f);
        }

        ///<summary>
        ///Gets or sets the screen width.
        ///</summary>
        public int ScreenWidth
        {
            get => _screenWidth;
            set => _screenWidth = System.Math.Max(640, value);
        }

        ///<summary>
        ///Gets or sets the screen height.
        ///</summary>
        public int ScreenHeight
        {
            get => _screenHeight;
            set => _screenHeight = System.Math.Max(480, value);
        }

        ///<summary>
        ///Gets or sets whether fullscreen is enabled.
        ///</summary>
        public bool Fullscreen
        {
            get => _fullscreen;
            set => _fullscreen = value;
        }

        ///<summary>
        ///Gets or sets whether VSync is enabled.
        ///</summary>
        public bool VSync
        {
            get => _vsync;
            set => _vsync = value;
        }

        ///<summary>
        ///Gets or sets the graphics quality level (0-3).
        ///</summary>
        public int QualityLevel
        {
            get => _qualityLevel;
            set => _qualityLevel = System.Math.Clamp(value, 0, 3);
        }

        ///<summary>
        ///Gets or sets whether to show FPS counter.
        ///</summary>
        public bool ShowFPS
        {
            get => _showFPS;
            set => _showFPS = value;
        }

        ///<summary>
        ///Gets or sets whether to auto-pause on focus loss.
        ///</summary>
        public bool AutoPause
        {
            get => _autoPause;
            set => _autoPause = value;
        }

        ///<summary>
        ///Initializes a new game settings instance.
        ///</summary>
        public GameSettings()
        {
            _masterVolume = 1f;
            _musicVolume = 0.8f;
            _sfxVolume = 0.8f;
            _screenWidth = 800;
            _screenHeight = 600;
            _fullscreen = false;
            _vsync = true;
            _qualityLevel = 2;
            _showFPS = false;
            _autoPause = true;

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", "GameSettings: Created with default values");
        }

        ///<summary>
        ///Resets settings to default values.
        ///</summary>
        public void ResetToDefaults()
        {
            _masterVolume = 1f;
            _musicVolume = 0.8f;
            _sfxVolume = 0.8f;
            _screenWidth = 800;
            _screenHeight = 600;
            _fullscreen = false;
            _vsync = true;
            _qualityLevel = 2;
            _showFPS = false;
            _autoPause = true;

            DLogger.Log(LogSubsystems.Save,LogLevel.Info, "GameSettings: Reset to default values");
        }

        ///<summary>
        ///Validates the settings.
        ///</summary>
        ///<returns>List of validation issues.</returns>
        public List<string> Validate()
        {
            var issues = new List<string>();

            if (_masterVolume < 0f || _masterVolume > 1f)
                issues.Add("Master volume is out of range");

            if (_musicVolume < 0f || _musicVolume > 1f)
                issues.Add("Music volume is out of range");

            if (_sfxVolume < 0f || _sfxVolume > 1f)
                issues.Add("SFX volume is out of range");

            if (_screenWidth < 640)
                issues.Add("Screen width is too small");

            if (_screenHeight < 480)
                issues.Add("Screen height is too small");

            if (_qualityLevel < 0 || _qualityLevel > 3)
                issues.Add("Quality level is out of range");

            return issues;
        }

        ///<summary>
        ///Creates a copy of these settings.
        ///</summary>
        ///<returns>A new GameSettings instance with the same values.</returns>
        public GameSettings Clone()
        {
            return new GameSettings
            {
                _masterVolume = _masterVolume,
                _musicVolume = _musicVolume,
                _sfxVolume = _sfxVolume,
                _screenWidth = _screenWidth,
                _screenHeight = _screenHeight,
                _fullscreen = _fullscreen,
                _vsync = _vsync,
                _qualityLevel = _qualityLevel,
                _showFPS = _showFPS,
                _autoPause = _autoPause
            };
        }

        ///<summary>
        ///Gets settings information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"GameSettings: Resolution={_screenWidth}x{_screenHeight}, " +
            $"Fullscreen={_fullscreen}, VSync={_vsync}, Quality={_qualityLevel}, " +
            $"Volumes=({_masterVolume:F2}, {_musicVolume:F2}, {_sfxVolume:F2})";
        }
    }

    ///<summary>
    ///Level progress information.
    ///</summary>
    public class LevelProgress
    {
        public string LevelId { get; set; }
        public bool Completed { get; set; }
        public int HighScore { get; set; }
        public int Stars { get; set; }
        public int Attempts { get; set; }
        public DateTime LastPlayed { get; set; }
        public float BestTime { get; set; }

        public LevelProgress(string levelId)
        {
            LevelId = levelId;
            Completed = false;
            HighScore = 0;
            Stars = 0;
            Attempts = 0;
            LastPlayed = DateTime.UtcNow;
            BestTime = float.MaxValue;
        }

        public void UpdateProgress(bool completed, int score, int stars)
        {
            Completed = completed;
            HighScore = System.Math.Max(HighScore, score);
            Stars = System.Math.Max(Stars, stars);
            Attempts++;
            LastPlayed = DateTime.UtcNow;
        }

        public LevelProgress Clone()
        {
            return new LevelProgress(LevelId)
            {
                Completed = Completed,
                HighScore = HighScore,
                Stars = Stars,
                Attempts = Attempts,
                LastPlayed = LastPlayed,
                BestTime = BestTime
            };
        }

        public override string ToString()
        {
            return $"LevelProgress: {LevelId}, Completed={Completed}, " +
            $"Score={HighScore}, Stars={Stars}, Attempts={Attempts}";
        }
    }
}







