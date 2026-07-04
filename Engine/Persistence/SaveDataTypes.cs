using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Persistence
{
    ///<summary>
    ///Save data for game settings
    ///</summary>
    public class SettingsSaveData
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 0.8f;
        public float SFXVolume { get; set; } = 0.9f;
        public bool Fullscreen { get; set; } = false;
        public int ResolutionWidth { get; set; } = 1920;
        public int ResolutionHeight { get; set; } = 1080;
    }

    ///<summary>
    ///Save data for world state
    ///</summary>
    public class WorldSaveData
    {
        public string CurrentScene { get; set; } = string.Empty;
        public float GameTime { get; set; } = 0.0f;
        public Dictionary<string, object> WorldState { get; set; } = new();
        
        //Phase 1: Add missing properties to fix CS1061 errors
        public int CurrentLevel { get; set; }
        public int DefeatedEnemies { get; set; }
        public List<string> ActivatedSwitches { get; set; } = new();
        public float TimePlayedSeconds { get; set; }
    }

    ///<summary>
    ///Interface for event management
    ///</summary>
    public interface IEventManager
    {
        void Subscribe<T>(Action<T> handler) where T : class;
        void Unsubscribe<T>(Action<T> handler) where T : class;
        void Publish<T>(T eventData) where T : class;
    }
}
