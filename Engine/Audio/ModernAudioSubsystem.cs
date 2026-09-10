// ====================================================================================================
//  FILE: ModernAudioSubsystem.cs
//  PROJECT: SASZombieAssaultTD
//  PATH: Engine/Audio/ModernAudioSubsystem.cs
//  AUTHOR: BDC
//  SUBSYSTEM: Audio
//
//  PURPOSE:
//      Modern audio subsystem with 3D positioning, mixing, and resource integration.
//      Replaces legacy AudioSystem with a deterministic, resource-pipeline-driven implementation.
//
//  ROLE:
//      - 3D audio positioning and mixing
//      - Audio resource loading via ModernResourcePipeline
//      - Music and SFX playback management
//      - Volume and concurrency control
//      - Runtime audio diagnostics via AudioStats
//
//  NOTES:
//      - Integrates with ModernResourcePipeline for audio assets
//      - Uses AudioSample, AudioSource, and AudioMixer from AudioSupportingClasses.cs
//      - Designed as a complete replacement for legacy AudioSystem
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
{
    public sealed class ModernAudioSubsystem : IDisposable
    {
        // ---------------------------------------------------------------------------------------------
        // Fields
        // ---------------------------------------------------------------------------------------------
        private readonly Dictionary<string, AudioSample> _samples = new();
        private readonly List<AudioSource> _activeSources = new();
        private readonly AudioMixer _mixer = new();
        private readonly ModernResourcePipeline _resourcePipeline;

        private bool _disposed;
        private bool _initialized;

        private float _masterVolume = 1.0f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 1.0f;
        private int _maxConcurrentSounds = 32;

        // ---------------------------------------------------------------------------------------------
        // Construction
        // ---------------------------------------------------------------------------------------------
        public ModernAudioSubsystem(ModernResourcePipeline resourcePipeline) => _resourcePipeline = resourcePipeline ?? throw new ArgumentNullException(nameof(resourcePipeline));

        // ---------------------------------------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------------------------------------
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

                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Info,
                    "Audio",
                    "ModernAudioSubsystem initialized successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Error,
                    "Audio",
                    $"Failed to initialize audio: {ex.Message}");
                throw;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Public API — Sound Effects
        // ---------------------------------------------------------------------------------------------
        public void PlaySound(string soundName, Vector3 position = default, float volume = 1.0f)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                if (!_samples.TryGetValue(soundName, out var sample))
                {
                    sample = LoadAudioSample(soundName);
                    if (sample == null)
                        return;
                }

                if (_activeSources.Count >= _maxConcurrentSounds)
                {
                    var oldestSource = _activeSources.OrderBy(s => s.StartTime).First();
                    oldestSource.Stop();
                    _activeSources.Remove(oldestSource);
                }

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

                _mixer.PlaySource(source);
                _activeSources.Add(source);
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Error,
                    "Audio",
                    $"Failed to play sound '{soundName}': {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Public API — Music
        // ---------------------------------------------------------------------------------------------
        public void PlayMusic(string musicName, bool loop = true)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                StopMusic();

                var sample = LoadAudioSample(musicName);
                if (sample == null)
                    return;

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

                _mixer.PlaySource(source);
                _activeSources.Add(source);

                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Info,
                    "Audio",
                    $"Playing music: {musicName}");
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Error,
                    "Audio",
                    $"Failed to play music '{musicName}': {ex.Message}");
            }
        }

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

        // ---------------------------------------------------------------------------------------------
        // Public API — Volume Control
        // ---------------------------------------------------------------------------------------------
        public void SetMasterVolume(float volume)
        {
            _masterVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateAllVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateMusicVolumes();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = System.Math.Clamp(volume, 0.0f, 1.0f);
            UpdateSfxVolumes();
        }

        // ---------------------------------------------------------------------------------------------
        // Update Loop
        // ---------------------------------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (!_initialized || _disposed)
                return;

            try
            {
                _mixer.Update(deltaTime);

                var finishedSources = _activeSources.Where(s => !s.IsPlaying).ToList();
                foreach (var source in finishedSources)
                {
                    _activeSources.Remove(source);
                }

                Update3DAudio();
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Error,
                    "Audio",
                    $"Audio update failed: {ex.Message}");
            }
        }



        // ---------------------------------------------------------------------------------------------
        // Internal Helpers
        // ---------------------------------------------------------------------------------------------
        private AudioSample LoadAudioSample(string audioName)
        {
            try
            {
                if (_samples.TryGetValue(audioName, out var cached))
                    return cached;

                var sample = _resourcePipeline
                    .LoadResourceAsync<AudioSample>(audioName)
                    .Result;

                if (sample != null)
                {
                    _samples[audioName] = sample;

                    DLogger.Log(
                        LogSubsystems.Audio,
                        LogEnums.LogLevel.Info,
                        "Audio",
                        $"Loaded audio sample: {audioName}");
                }

                return sample;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Audio,
                    LogEnums.LogLevel.Error,
                    "Audio",
                    $"Failed to load audio sample '{audioName}': {ex.Message}");
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
            foreach (var source in _activeSources.Where(s => s.Is3D))
            {
                _mixer.Update3DSource(source);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Disposal
        // ---------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            StopAllSounds();
            _mixer.Dispose();
            _samples.Clear();
            _activeSources.Clear();

            DLogger.Log(
                LogSubsystems.Audio,
                LogEnums.LogLevel.Info,
                "Audio",
                "ModernAudioSubsystem disposed");
        }
    }
}
