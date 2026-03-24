using System;

namespace SASZombieAssaultTD.Engine.Audio
{
    /// <summary>
    /// Audio configuration settings for the game engine.
    /// Phase 6: Final Pass - Add missing AudioSettings to fix CS1061 errors
    /// </summary>
    public class AudioSettings
    {
        private float _masterVolume = 1.0f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 0.9f;
        private bool _audioEnabled = true;
        private int _maxConcurrentSounds = 32;
        private float _dopplerFactor = 1.0f;
        private float _distanceModel = 1.0f;
        private bool _spatialAudioEnabled = true;
        private float _rolloffFactor = 1.0f;
        private int _sampleRate = 44100;
        private int _bufferSize = 512;

        /// <summary>
        /// Gets or sets the master audio volume (0.0 to 1.0).
        /// </summary>
        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = System.Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets the music volume (0.0 to 1.0).
        /// </summary>
        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = System.Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets the sound effects volume (0.0 to 1.0).
        /// </summary>
        public float SFXVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = System.Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets whether audio is enabled.
        /// Phase 6: Add missing AudioEnabled property for CS1061 fixes
        /// </summary>
        public bool AudioEnabled
        {
            get => _audioEnabled;
            set => _audioEnabled = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of concurrent sounds.
        /// </summary>
        public int MaxConcurrentSounds
        {
            get => _maxConcurrentSounds;
            set => _maxConcurrentSounds = System.Math.Max(1, value);
        }

        /// <summary>
        /// Gets or sets the Doppler effect factor.
        /// </summary>
        public float DopplerFactor
        {
            get => _dopplerFactor;
            set => _dopplerFactor = System.MathF.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets the distance model factor.
        /// </summary>
        public float DistanceModel
        {
            get => _distanceModel;
            set => _distanceModel = System.MathF.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets whether spatial audio is enabled.
        /// </summary>
        public bool SpatialAudioEnabled
        {
            get => _spatialAudioEnabled;
            set => _spatialAudioEnabled = value;
        }

        /// <summary>
        /// Gets or sets the rolloff factor for distance attenuation.
        /// </summary>
        public float RolloffFactor
        {
            get => _rolloffFactor;
            set => _rolloffFactor = System.MathF.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets the audio sample rate.
        /// </summary>
        public int SampleRate
        {
            get => _sampleRate;
            set => _sampleRate = value switch
            {
                22050 or 44100 or 48000 or 96000 => value,
                _ => 44100
            };
        }

        /// <summary>
        /// Gets or sets the audio buffer size.
        /// </summary>
        public int BufferSize
        {
            get => _bufferSize;
            set => _bufferSize = value switch
            {
                128 or 256 or 512 or 1024 or 2048 => value,
                _ => 512
            };
        }

        /// <summary>
        /// Gets the effective music volume with master volume applied.
        /// </summary>
        public float EffectiveMusicVolume => _audioEnabled ? _masterVolume * _musicVolume : 0f;

        /// <summary>
        /// Gets the effective SFX volume with master volume applied.
        /// </summary>
        public float EffectiveSFXVolume => _audioEnabled ? _masterVolume * _sfxVolume : 0f;

        /// <summary>
        /// Initializes a new AudioSettings instance with default values.
        /// </summary>
        public AudioSettings()
        {
            // Default values are set in field initializers
        }

        /// <summary>
        /// Creates a copy of these audio settings.
        /// </summary>
        /// <returns>A new AudioSettings instance with the same values</returns>
        public AudioSettings Clone()
        {
            return new AudioSettings
            {
                MasterVolume = this._masterVolume,
                MusicVolume = this._musicVolume,
                SFXVolume = this._sfxVolume,
                AudioEnabled = this._audioEnabled,
                MaxConcurrentSounds = this._maxConcurrentSounds,
                DopplerFactor = this._dopplerFactor,
                DistanceModel = this._distanceModel,
                SpatialAudioEnabled = this._spatialAudioEnabled,
                RolloffFactor = this._rolloffFactor,
                SampleRate = this._sampleRate,
                BufferSize = this._bufferSize
            };
        }

        /// <summary>
        /// Applies settings from another AudioSettings instance.
        /// </summary>
        /// <param name="other">Settings to copy from</param>
        public void ApplyFrom(AudioSettings other)
        {
            if (other == null) return;

            _masterVolume = other._masterVolume;
            _musicVolume = other._musicVolume;
            _sfxVolume = other._sfxVolume;
            _audioEnabled = other._audioEnabled;
            _maxConcurrentSounds = other._maxConcurrentSounds;
            _dopplerFactor = other._dopplerFactor;
            _distanceModel = other._distanceModel;
            _spatialAudioEnabled = other._spatialAudioEnabled;
            _rolloffFactor = other._rolloffFactor;
            _sampleRate = other._sampleRate;
            _bufferSize = other._bufferSize;
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        public void ResetToDefaults()
        {
            _masterVolume = 1.0f;
            _musicVolume = 0.8f;
            _sfxVolume = 0.9f;
            _audioEnabled = true;
            _maxConcurrentSounds = 32;
            _dopplerFactor = 1.0f;
            _distanceModel = 1.0f;
            _spatialAudioEnabled = true;
            _rolloffFactor = 1.0f;
            _sampleRate = 44100;
            _bufferSize = 512;
        }

        /// <summary>
        /// Validates the current settings.
        /// </summary>
        /// <returns>True if all settings are within valid ranges</returns>
        public bool Validate()
        {
            return _masterVolume >= 0f && _masterVolume <= 1f &&
                   _musicVolume >= 0f && _musicVolume <= 1f &&
                   _sfxVolume >= 0f && _sfxVolume <= 1f &&
                   _maxConcurrentSounds > 0 &&
                   _dopplerFactor >= 0f &&
                   _distanceModel >= 0f &&
                   _rolloffFactor >= 0f &&
                   (_sampleRate == 22050 || _sampleRate == 44100 || _sampleRate == 48000 || _sampleRate == 96000) &&
                   (_bufferSize == 128 || _bufferSize == 256 || _bufferSize == 512 || _bufferSize == 1024 || _bufferSize == 2048);
        }
    }
}
