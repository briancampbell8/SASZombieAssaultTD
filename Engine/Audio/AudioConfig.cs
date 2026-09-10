// ====================================================================================================
//  FILE: AudioConfig.cs
//  PATH: Engine/Audio/
//  MODULE: Audio Subsystem (Configuration)
//
//  ROLE:
//      Centralized audio configuration holder and simple validation utilities.
//
//  RESPONSIBILITIES:
//      - Expose Master/Music/SFX/UI volume controls and audio-related toggles.
//      - Provide default configuration factory and validation helpers.
//
//  NON-RESPONSIBILITIES:
//      - Persisting configuration to disk (higher-level manager should handle persistence).
//      - Platform-specific audio device configuration.
//
//  ARCHITECTURAL NOTES:
//      - Keep this class small and POCO-like to simplify serialization and testing.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Audio configuration settings for the audio subsystem. P90-04: AudioConfig implementation for centralized audio
    /// settings
    /// </summary>
    public class AudioConfig
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 1.0f;
        public float UIVolume { get; set; } = 0.9f;
        public int MaxConcurrentSounds { get; set; } = 32;
        public bool AudioEnabled { get; set; } = true;
        public bool SpatialAudioEnabled { get; set; } = true;
        public int SampleRate { get; set; } = 44100;
        public int BufferSize { get; set; } = 512;

        /// <summary>
        /// Creates a default audio configuration.
        /// </summary>
        public static AudioConfig Default => new AudioConfig();

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        public bool Validate()
        {
            return MasterVolume >= 0f && MasterVolume <= 1f &&
                   MusicVolume >= 0f && MusicVolume <= 1f &&
                   SfxVolume >= 0f && SfxVolume <= 1f &&
                   UIVolume >= 0f && UIVolume <= 1f &&
                   MaxConcurrentSounds > 0 &&
                   SampleRate > 0 &&
                   BufferSize > 0;
        }
    }

    /// <summary>
    /// Audio registry mapping sound names to file paths. P90-05: AudioRegistry implementation for sound asset
    /// management
    /// </summary>
    public class AudioRegistry
    {
        private readonly Dictionary<string, string> _soundPaths = new();
        private readonly Dictionary<string, AudioCategory> _soundCategories = new();

        /// <summary>
        /// Registers a sound with its file path.
        /// </summary>
        public void RegisterSound(string soundName, string filePath, AudioCategory category = AudioCategory.SFX)
        {
            if (string.IsNullOrWhiteSpace(soundName) || string.IsNullOrWhiteSpace(filePath))
                return;

            _soundPaths[soundName] = filePath;
            _soundCategories[soundName] = category;

            DLogger.Log($"AudioRegistry: Registered sound '{soundName}' -> '{filePath}' (Category: {category})");
        }

        /// <summary>
        /// Gets the file path for a sound.
        /// </summary>
        public string GetSoundPath(string soundName)
        {
            return _soundPaths.TryGetValue(soundName, out var path) ? path : null;
        }

        /// <summary>
        /// Gets the category for a sound.
        /// </summary>
        public AudioCategory GetSoundCategory(string soundName)
        {
            return _soundCategories.TryGetValue(soundName, out var category) ? category : AudioCategory.SFX;
        }

        /// <summary>
        /// Checks if a sound is registered.
        /// </summary>
        public bool IsSoundRegistered(string soundName)
        {
            return _soundPaths.ContainsKey(soundName);
        }

        /// <summary>
        /// Gets all registered sound names.
        /// </summary>
        public IEnumerable<string> GetRegisteredSounds()
        {
            return _soundPaths.Keys;
        }

        /// <summary>
        /// Clears all registered sounds.
        /// </summary>
        public void Clear()
        {
            _soundPaths.Clear();
            _soundCategories.Clear();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "AudioRegistry: Cleared all sounds");
        }

        /// <summary>
        /// Loads default sound registrations.
        /// </summary>
        public void LoadDefaults()
        {
            //Success sounds
            RegisterSound("success_generic", "Audio/SFX/success_generic.wav", AudioCategory.SFX);
            RegisterSound("success_tower_purchase", "Audio/SFX/tower_purchase.wav", AudioCategory.SFX);
            RegisterSound("success_upgrade_purchase", "Audio/SFX/upgrade_purchase.wav", AudioCategory.SFX);
            RegisterSound("success_level_up", "Audio/SFX/level_up.wav", AudioCategory.SFX);
            RegisterSound("success_achievement", "Audio/SFX/achievement.wav", AudioCategory.SFX);
            RegisterSound("success_wave_complete", "Audio/SFX/wave_complete.wav", AudioCategory.SFX);
            RegisterSound("success_game_complete", "Audio/SFX/game_complete.wav", AudioCategory.SFX);
            RegisterSound("success_ability_unlock", "Audio/SFX/ability_unlock.wav", AudioCategory.SFX);

            //Error sounds
            RegisterSound("error_generic", "Audio/SFX/error_generic.wav", AudioCategory.SFX);
            RegisterSound("error_insufficient_funds", "Audio/SFX/insufficient_funds.wav", AudioCategory.SFX);
            RegisterSound("error_invalid_placement", "Audio/SFX/invalid_placement.wav", AudioCategory.SFX);
            RegisterSound("error_insufficient_resources", "Audio/SFX/insufficient_resources.wav", AudioCategory.SFX);
            RegisterSound("error_invalid_action", "Audio/SFX/invalid_action.wav", AudioCategory.SFX);
            RegisterSound("error_upgrade_unavailable", "Audio/SFX/upgrade_unavailable.wav", AudioCategory.SFX);
            RegisterSound("error_tower_limit", "Audio/SFX/tower_limit.wav", AudioCategory.SFX);
            RegisterSound("error_cooldown", "Audio/SFX/cooldown.wav", AudioCategory.SFX);

            //UI sounds
            RegisterSound("ui_click", "Audio/UI/click.wav", AudioCategory.UI);
            RegisterSound("ui_hover", "Audio/UI/hover.wav", AudioCategory.UI);
            RegisterSound("ui_open", "Audio/UI/open.wav", AudioCategory.UI);
            RegisterSound("ui_close", "Audio/UI/close.wav", AudioCategory.UI);
            RegisterSound("ui_cash_increase", "Audio/UI/cash_increase.wav", AudioCategory.UI);
            RegisterSound("ui_cash_decrease", "Audio/UI/cash_decrease.wav", AudioCategory.UI);

            //Gameplay sounds
            RegisterSound("wave_start", "Audio/Gameplay/wave_start.wav", AudioCategory.SFX);
            RegisterSound("enemy_death", "Audio/Gameplay/enemy_death.wav", AudioCategory.SFX);
            RegisterSound("tower_fire", "Audio/Gameplay/tower_fire.wav", AudioCategory.SFX);
            RegisterSound("tower_place", "Audio/Gameplay/tower_place.wav", AudioCategory.SFX);
            RegisterSound("tower_sell", "Audio/Gameplay/tower_sell.wav", AudioCategory.SFX);

            //Music
            RegisterSound("music_menu", "Audio/Music/menu.ogg", AudioCategory.Music);
            RegisterSound("music_gameplay", "Audio/Music/gameplay.ogg", AudioCategory.Music);
            RegisterSound("music_wave", "Audio/Music/wave.ogg", AudioCategory.Music);
            RegisterSound("music_victory", "Audio/Music/victory.ogg", AudioCategory.Music);
            RegisterSound("music_defeat", "Audio/Music/defeat.ogg", AudioCategory.Music);

            DLogger.Log(LogSubsystems.ResourcesPipeline, "AudioRegistry: Loaded default sound registrations");
        }
    }

    /// <summary>
    /// Audio category for volume grouping.
    /// </summary>
    public enum AudioCategory
    {
        Music,
        SFX,
        UI
    }
}
