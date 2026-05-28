// ============================================================================
// File:    ModernAudioSubsystem.cs
// Project: SASZombieAssaultTD
// Path:    Engine/Audio/ModernAudioSubsystem.cs
// Author:  BDC
// Purpose: Modern audio subsystem with 3D positioning, mixing, and resource
//          integration. Replaces legacy AudioSystem with a deterministic,
//          resource-pipeline-driven implementation.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Audio
{
    /// <summary>
    /// Modern audio subsystem with 3D positioning, mixing, and resource management.
    /// Replaces legacy AudioSystem with a complete modern implementation.
    /// </summary>
    public sealed class ModernAudioSubsystem : IDisposable
    {
        // --------------------------------------------------------------------
        // Fields
        // --------------------------------------------------------------------

        private readonly Dictionary<string, AudioSample> _samples = new();
        private readonly List<AudioSource> _activeSources = new();
        private readonly AudioMixer _mixer = new();
        private readonly ModernResourcePipeline _resourcePipeline;

        private bool _disposed;
        private bool _initialized;

        // Audio settings
        private float _masterVolume = 1.0f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 1.0f;
        private int _maxConcurrentSounds = 32;

        // --------------------------------------------------------------------
        // Construction
        // --------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the modern audio subsystem.
        /// </summary>
        /// <param name="resourcePipeline">
        /// The resource pipeline used to load audio assets.
        /// </param>
        public ModernAudioSubsystem(ModernResourcePipeline resourcePipeline)
        {
            _resourcePipeline = resourcePipeline ?? throw new ArgumentNullException(nameof(resourcePipeline));
        }

        // --------------------------------------------------------------------
        // Initialization
        // --------------------------------------------------------------------

        /// <summary>
        /// Initializes the audio subsystem and underlying mixer.
        /// </summary>
        public void Initialize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernAudioSubsystem));

            if (_initialized)
                return;

            try
            {
                _mixer.Initialize();
                _initialized = true;

                Engine.Diagnostics.DebugLogger.LogInfo("ModernAudioSubsystem initialized successfully.");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Failed to initialize audio: {ex.Message}");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // Public API - Sound Effects
        // --------------------------------------------------------------------

        /// <summary>
        /// Plays a sound effect with optional 3D positioning.
        /// </summary>
        /// <param name="soundName">The logical name of the sound to play.</param>
        /// <param name="position">
        /// Optional 3D position. If non-zero, 3D audio processing is applied.
        /// </param>
        /// <param name="volume">
        /// Per-sound volume multiplier in the range [0, 1].
        /// </param>
        public void PlaySound(string soundName, Vector3 position = default, float volume = 1.0f)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                // Resolve or load the audio sample.
                if (!_samples.TryGetValue(soundName, out var sample))
                {
                    sample = LoadAudioSample(soundName);
                    if (sample == null)
                        return;
                }

                // Enforce maximum concurrent sounds.
                if (_activeSources.Count >= _maxConcurrentSounds)
                {
                    var oldestSource = _activeSources.OrderBy(s => s.StartTime).First();
                    oldestSource.Stop();
                    _activeSources.Remove(oldestSource);
                }

                // Create and configure audio source.
                var source = new AudioSource
                {
                    Sample = sample,
                    OriginalVolume = volume,
                    Volume = volume * _sfxVolume * _masterVolume,
                    Position = position,
                    Is3D = position != Vector3.Zero,
                    IsLooping = false,
                    IsMusic = false,
                    StartTime = DateTime.UtcNow,
                    IsPlaying = false
                };

                // Play the sound.
                _mixer.PlaySource(source);
                _activeSources.Add(source);
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Failed to play sound '{soundName}': {ex.Message}");
            }
        }

        // --------------------------------------------------------------------
        // Public API - Music
        // --------------------------------------------------------------------

        /// <summary>
        /// Plays background music with optional looping.
        /// </summary>
        /// <param name="musicName">The logical name of the music track.</param>
        /// <param name="loop">Whether the music should loop.</param>
        public void PlayMusic(string musicName, bool loop = true)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                // Stop any currently playing music.
                StopMusic();

                // Load music sample.
                var sample = LoadAudioSample(musicName);
                if (sample == null)
                    return;

                // Create music source.
                var source = new AudioSource
                {
                    Sample = sample,
                    OriginalVolume = 1.0f,
                    Volume = _musicVolume * _masterVolume,
                    IsLooping = loop,
                    IsMusic = true,
                    Is3D = false,
                    Position = Vector3.Zero,
                    StartTime = DateTime.UtcNow,
                    IsPlaying = false
                };

                // Play music.
                _mixer.PlaySource(source);
                _activeSources.Add(source);

                Engine.Diagnostics.DebugLogger.LogInfo($"Playing music: {musicName}");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Failed to play music '{musicName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Stops all currently playing music sources.
        /// </summary>
        public void StopMusic()
        {
            if (!_initialized || _disposed)
                return;

            var musicSources = _activeSources.Where(s => s.IsMusic).ToList();
            foreach (var source in musicSources)
            {
                source.Stop();
                _activeSources.Remove(source);
            }
        }

        /// <summary>
        /// Stops all currently playing sounds (music and SFX).
        /// </summary>
        public void StopAllSounds()
        {
            if (!_initialized || _disposed)
                return;

            foreach (var source in _activeSources.ToList())
            {
                source.Stop();
            }

            _activeSources.Clear();
        }

        // --------------------------------------------------------------------
        // Public API - Volume Control
        // --------------------------------------------------------------------

        /// <summary>
        /// Sets the master volume level.
        /// </summary>
        /// <param name="volume">Volume in the range [0, 1].</param>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateAllVolumes();
        }

        /// <summary>
        /// Sets the music volume level.
        /// </summary>
        /// <param name="volume">Volume in the range [0, 1].</param>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateMusicVolumes();
        }

        /// <summary>
        /// Sets the sound effects volume level.
        /// </summary>
        /// <param name="volume">Volume in the range [0, 1].</param>
        public void SetSfxVolume(float volume)
        {
            _sfxVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateSfxVolumes();
        }

        // --------------------------------------------------------------------
        // Update Loop
        // --------------------------------------------------------------------

        /// <summary>
        /// Updates the audio subsystem. Call once per frame.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                // Update mixer state.
                _mixer.Update(deltaTime);

                // Remove finished sources.
                var finishedSources = _activeSources.Where(s => !s.IsPlaying).ToList();
                foreach (var source in finishedSources)
                {
                    _activeSources.Remove(source);
                }

                // Update 3D audio parameters.
                Update3DAudio();
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Audio update failed: {ex.Message}");
            }
        }

        // --------------------------------------------------------------------
        // Diagnostics
        // --------------------------------------------------------------------

        /// <summary>
        /// Returns current audio subsystem statistics.
        /// </summary>
        public AudioStats GetStats()
        {
            return new AudioStats
            {
                ActiveSources = _activeSources.Count,
                MaxConcurrentSounds = _maxConcurrentSounds,
                LoadedSamples = _samples.Count,
                MasterVolume = _masterVolume,
                MusicVolume = _musicVolume,
                SfxVolume = _sfxVolume
            };
        }

        // --------------------------------------------------------------------
        // Internal Helpers
        // --------------------------------------------------------------------

        private AudioSample LoadAudioSample(string audioName)
        {
            try
            {
                // Cache lookup.
                if (_samples.TryGetValue(audioName, out var cached))
                    return cached;

                // Load from resource pipeline (synchronously via Result).
                var sample = _resourcePipeline.LoadResourceAsync<AudioSample>(audioName).Result;
                if (sample != null)
                {
                    _samples[audioName] = sample;
                    Engine.Diagnostics.DebugLogger.LogDebug($"Loaded audio sample: {audioName}");
                }

                return sample;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Failed to load audio sample '{audioName}': {ex.Message}");
                return null;
            }
        }

        private void UpdateAllVolumes()
        {
            foreach (var source in _activeSources)
            {
                if (source.IsMusic)
                {
                    source.Volume = _musicVolume * _masterVolume;
                }
                else
                {
                    source.Volume = source.OriginalVolume * _sfxVolume * _masterVolume;
                }
            }
        }

        private void UpdateMusicVolumes()
        {
            foreach (var source in _activeSources.Where(s => s.IsMusic))
            {
                source.Volume = _musicVolume * _masterVolume;
            }
        }

        private void UpdateSfxVolumes()
        {
            foreach (var source in _activeSources.Where(s => !s.IsMusic))
            {
                source.Volume = source.OriginalVolume * _sfxVolume * _masterVolume;
            }
        }

        private void Update3DAudio()
        {
            // Integrate with camera/player position system as needed.
            foreach (var source in _activeSources.Where(s => s.Is3D))
            {
                _mixer.Update3DSource(source);
            }
        }

        // --------------------------------------------------------------------
        // Disposal
        // --------------------------------------------------------------------

        /// <summary>
        /// Disposes the audio subsystem and releases all resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            StopAllSounds();
            _mixer.Dispose();
            _samples.Clear();
            _activeSources.Clear();

            Engine.Diagnostics.DebugLogger.LogInfo("ModernAudioSubsystem disposed.");
        }
    }

    /// <summary>
    /// Audio subsystem statistics snapshot.
    /// </summary>
    public sealed class AudioStats
    {
        public int ActiveSources { get; set; }
        public int MaxConcurrentSounds { get; set; }
        public int LoadedSamples { get; set; }
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
    }

    // NOTE:
    // AudioSample, AudioSource, and AudioMixer are defined in:
    //   Engine/Audio/AudioSupportingClasses.cs
    // This file intentionally does not declare those types to avoid
    // duplicate type conflicts and ambiguity.
}
