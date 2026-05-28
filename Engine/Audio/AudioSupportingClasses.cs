/*
Program Name: SASZombieAssaultTD
File Path: Engine\Audio\AudioSupportingClasses.cs
Purpose: P90 Modern Audio Subsystem - Supporting classes for audio playback.
Features: AudioSample, AudioSource, AudioMixer implementations for complete audio system.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Audio
{
    /// <summary>
    /// Audio sample data loaded from audio files.
    /// P90-01: AudioSample implementation for audio asset management
    /// </summary>
    public class AudioSample
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public int BitsPerSample { get; set; }
        public float Duration { get; set; }
        public bool IsLoaded { get; set; }

        public AudioSample(string name)
        {
            Name = name;
            IsLoaded = false;
            Duration = 0f;
            Channels = 2;
            SampleRate = 44100;
            BitsPerSample = 16;
        }

        /// <summary>
        /// Creates a stub audio sample for testing.
        /// </summary>
        public static AudioSample CreateStub(string name, float duration = 2f)
        {
            return new AudioSample(name)
            {
                Duration = duration,
                IsLoaded = true,
                Data = new byte[0]
            };
        }
    }

    /// <summary>
    /// Audio source representing a playing sound instance.
    /// P90-02: AudioSource implementation for sound playback management
    /// </summary>
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
        public float CurrentTime { get; set; }
        public float Pitch { get; set; }
        public float Pan { get; set; }

        public AudioSource()
        {
            Volume = 1.0f;
            OriginalVolume = 1.0f;
            Pitch = 1.0f;
            Pan = 0.0f;
            CurrentTime = 0f;
            Position = Vector3.Zero;
        }

        public void Stop()
        {
            IsPlaying = false;
            CurrentTime = 0f;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// Gets whether the source has finished playing.
        /// </summary>
        public bool IsFinished => !IsLooping && Sample != null && CurrentTime >= Sample.Duration;
    }

    /// <summary>
    /// Audio mixer for managing audio output and routing.
    /// P90-03: AudioMixer implementation for audio mixing and routing
    /// </summary>
    public class AudioMixer : IDisposable
    {
        private readonly List<AudioSource> _sources = new();
        private bool _initialized = false;
        private bool _disposed = false;

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            System.Diagnostics.Debug.WriteLine("AudioMixer: Initialized");
        }

        public void PlaySource(AudioSource source)
        {
            if (!_initialized || _disposed) return;
            source.IsPlaying = true;
            source.StartTime = DateTime.UtcNow;
            _sources.Add(source);
        }

        public void Update(float deltaTime)
        {
            if (!_initialized || _disposed) return;

            // Update all sources
            foreach (var source in _sources)
            {
                if (source.IsPlaying && !source.IsLooping)
                {
                    source.CurrentTime += deltaTime * source.Pitch;
                    if (source.IsFinished)
                    {
                        source.IsPlaying = false;
                    }
                }
            }

            // Remove finished sources
            _sources.RemoveAll(s => !s.IsPlaying);
        }

        public void Update3DSource(AudioSource source)
        {
            if (source == null || !source.Is3D) return;
            // Calculate 3D audio parameters
            // This would integrate with the listener position system
            System.Diagnostics.Debug.WriteLine($"AudioMixer: Updated 3D source at {source.Position}");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _sources.Clear();
            System.Diagnostics.Debug.WriteLine("AudioMixer: Disposed");
        }

        /// <summary>
        /// Gets the number of active sources.
        /// </summary>
        public int ActiveSourceCount => _sources.Count;
    }
}
