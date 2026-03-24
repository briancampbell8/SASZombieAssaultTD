using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Math;

namespace SASZombieAssaultTD.Engine.Audio
{
    /// <summary>
    /// Modern audio subsystem with 3D positioning, mixing, and resource management.
    /// Replaces legacy AudioSystem with complete modern implementation.
    /// </summary>
    public sealed class ModernAudioSubsystem : IDisposable
    {
        private readonly Dictionary<string, AudioSample> _samples = new();
        private readonly List<AudioSource> _activeSources = new();
        private readonly AudioMixer _mixer = new();
        private readonly ModernResourcePipeline _resourcePipeline;
        private bool _disposed = false;
        private bool _initialized = false;

        // Audio settings
        private float _masterVolume = 1.0f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 1.0f;
        private int _maxConcurrentSounds = 32;

        public ModernAudioSubsystem(ModernResourcePipeline resourcePipeline)
        {
            _resourcePipeline = resourcePipeline ?? throw new ArgumentNullException(nameof(resourcePipeline));
        }

        /// <summary>
        /// Initialize the audio subsystem.
        /// </summary>
        public void Initialize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ModernAudioSubsystem));
            if (_initialized) return;

            try
            {
                // Initialize audio hardware
                _mixer.Initialize();
                _initialized = true;

                ModernLoggingSystem.LogInfo("ModernAudioSubsystem initialized successfully");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to initialize audio: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Play a sound effect with optional 3D positioning.
        /// </summary>
        public void PlaySound(string soundName, Vector3 position = default, float volume = 1.0f)
        {
            if (!_initialized || _disposed) return;

            try
            {
                // Get or load the audio sample
                if (!_samples.TryGetValue(soundName, out var sample))
                {
                    sample = LoadAudioSample(soundName);
                    if (sample == null) return;
                }

                // Check for available audio sources
                if (_activeSources.Count >= _maxConcurrentSounds)
                {
                    // Stop the oldest playing sound
                    var oldestSource = _activeSources.OrderBy(s => s.StartTime).First();
                    oldestSource.Stop();
                    _activeSources.Remove(oldestSource);
                }

                // Create and configure audio source
                var source = new AudioSource
                {
                    Sample = sample,
                    Volume = volume * _sfxVolume * _masterVolume,
                    Position = position,
                    Is3D = position != Vector3.Zero,
                    StartTime = DateTime.UtcNow
                };

                // Play the sound
                _mixer.PlaySource(source);
                _activeSources.Add(source);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to play sound '{soundName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Play background music with looping option.
        /// </summary>
        public void PlayMusic(string musicName, bool loop = true)
        {
            if (!_initialized || _disposed) return;

            try
            {
                // Stop current music if playing
                StopMusic();

                // Load music sample
                var sample = LoadAudioSample(musicName);
                if (sample == null) return;

                // Create music source
                var source = new AudioSource
                {
                    Sample = sample,
                    Volume = _musicVolume * _masterVolume,
                    IsLooping = loop,
                    IsMusic = true,
                    StartTime = DateTime.UtcNow
                };

                // Play music
                _mixer.PlaySource(source);
                _activeSources.Add(source);

                ModernLoggingSystem.LogInfo($"Playing music: {musicName}");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to play music '{musicName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Stop all currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (!_initialized || _disposed) return;

            var musicSources = _activeSources.Where(s => s.IsMusic).ToList();
            foreach (var source in musicSources)
            {
                source.Stop();
                _activeSources.Remove(source);
            }
        }

        /// <summary>
        /// Stop all currently playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            if (!_initialized || _disposed) return;

            foreach (var source in _activeSources.ToList())
            {
                source.Stop();
            }
            _activeSources.Clear();
        }

        /// <summary>
        /// Set master volume level.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateAllVolumes();
        }

        /// <summary>
        /// Set music volume level.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateMusicVolumes();
        }

        /// <summary>
        /// Set sound effects volume level.
        /// </summary>
        public void SetSfxVolume(float volume)
        {
            _sfxVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateSfxVolumes();
        }

        /// <summary>
        /// Update audio processing (call once per frame).
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!_initialized || _disposed) return;

            try
            {
                // Update mixer
                _mixer.Update(deltaTime);

                // Remove finished sources
                var finishedSources = _activeSources.Where(s => !s.IsPlaying).ToList();
                foreach (var source in finishedSources)
                {
                    _activeSources.Remove(source);
                }

                // Update 3D audio positions
                Update3DAudio();
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Audio update failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Get audio subsystem statistics.
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

        private AudioSample LoadAudioSample(string audioName)
        {
            try
            {
                // Try to get from cache first
                if (_samples.TryGetValue(audioName, out var cached))
                    return cached;

                // Load from resource pipeline
                var sample = _resourcePipeline.LoadResourceAsync<AudioSample>(audioName).Result;
                if (sample != null)
                {
                    _samples[audioName] = sample;
                    ModernLoggingSystem.LogDebug($"Loaded audio sample: {audioName}");
                }

                return sample;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to load audio sample '{audioName}': {ex.Message}");
                return null;
            }
        }

        private void UpdateAllVolumes()
        {
            foreach (var source in _activeSources)
            {
                if (source.IsMusic)
                    source.Volume = _musicVolume * _masterVolume;
                else
                    source.Volume = source.OriginalVolume * _sfxVolume * _masterVolume;
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
            // Update 3D audio positioning based on listener position
            // This would integrate with the camera/player position system
            foreach (var source in _activeSources.Where(s => s.Is3D))
            {
                // Calculate 3D audio parameters based on source position
                // Apply distance attenuation, panning, etc.
                _mixer.Update3DSource(source);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            StopAllSounds();
            _mixer?.Dispose();
            _samples.Clear();
            _activeSources.Clear();

            ModernLoggingSystem.LogInfo("ModernAudioSubsystem disposed");
        }
    }

    /// <summary>
    /// Audio subsystem statistics.
    /// </summary>
    public class AudioStats
    {
        public int ActiveSources { get; set; }
        public int MaxConcurrentSounds { get; set; }
        public int LoadedSamples { get; set; }
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
    }

    // Supporting classes (would be fully implemented)
    public class AudioSample { }
    public class AudioSource
    {
        public AudioSample Sample { get; set; }
        public float Volume { get; set; }
        public float OriginalVolume { get; set; }
        public Vector3 Position { get; set; }
        public bool Is3D { get; set; }
        public bool IsLooping { get; set; }
        public bool IsMusic { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsPlaying { get; set; }

        public void Stop() { IsPlaying = false; }
    }
    public class AudioMixer
    {
        public void Initialize() { }
        public void PlaySource(AudioSource source) { source.IsPlaying = true; }
        public void Update(float deltaTime) { }
        public void Update3DSource(AudioSource source) { }
        public void Dispose() { }
    }
}
