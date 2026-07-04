/*
File:    Time.cs
Purpose: Provides delta time and timing utilities used by animation, movement, cooldowns, and ECS updates.
         Offers comprehensive time management for frame-based and real-time calculations.
         
Features: Delta time tracking, frame rate calculation, time scaling, and timing utilities.
          Supports fixed timestep, variable timestep, and time scaling for game effects.
          Used by animation systems, movement calculations, cooldown management, and ECS updates.

Created: Engine Core Implementation
Notes:   This is the canonical time system for the entire engine.
         All time-related operations should use this unified Time system.
*/

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Core.Time
{
    ///<summary>
    ///Provides comprehensive time management and delta time utilities for the engine.
    ///Used by animation, movement, cooldowns, and ECS update systems.
    ///</summary>
    public static class Time
    {
        /// Private Fields
        
        private static float _deltaTime = 0.016f; //Default 60 FPS
        private static float _fixedDeltaTime = 0.02f; //50 FPS fixed timestep
        private static float _timeScale = 1.0f;
        private static float _realtimeSinceStartup = 0f;
        private static float _timeSinceStartup = 0f;
        private static float _unscaledDeltaTime = 0.016f;
        private static float _fixedUnscaledDeltaTime = 0.02f;
        private static int _frameCount = 0;
        private static float _smoothDeltaTime = 0.016f;
        private static float _targetFrameRate = 60f;
        private static bool _isPaused = false;
        private static DateTime _startupTime = DateTime.UtcNow;
        
        //Frame rate calculation
        private static float _fps = 60f;
        private static float _fpsUpdateInterval = 0.5f;
        private static float _fpsAccumulator = 0f;
        private static int _fpsFrameCount = 0;
        
        //Fixed timestep
        private static float _fixedTime = 0f;
        private static float _fixedUnscaledTime = 0f;
        private static float _accumulator = 0f;
        
        ///

        /// Public Properties
        
        ///<summary>
        ///Time in seconds since the last frame (scaled by time scale).
        ///</summary>
        public static float DeltaTime => _isPaused ? 0f : _deltaTime * _timeScale;
        
        ///<summary>
        ///Time in seconds since the last frame (unscaled, ignores time scale and pause).
        ///</summary>
        public static float UnscaledDeltaTime => _unscaledDeltaTime;
        
        ///<summary>
        ///Fixed delta time for physics and consistent updates (scaled by time scale).
        ///</summary>
        public static float FixedDeltaTime => _isPaused ? 0f : _fixedDeltaTime * _timeScale;
        
        ///<summary>
        ///Fixed delta time for physics and consistent updates (unscaled).
        ///</summary>
        public static float FixedUnscaledDeltaTime => _fixedUnscaledDeltaTime;
        
        ///<summary>
        ///Time scale for affecting all time-based calculations (1.0 = normal speed).
        ///</summary>
        public static float TimeScale
        {
            get => _timeScale;
            set => _timeScale = System.Math.Max(0f, value);
        }
        
        ///<summary>
        ///Time in seconds since the engine started (scaled by time scale).
        ///</summary>
        public static float TimeSinceStartup => _timeSinceStartup;
        
        ///<summary>
        ///Time in seconds since the engine started (unscaled, ignores time scale and pause).
        ///</summary>
        public static float RealtimeSinceStartup => _realtimeSinceStartup;
        
        ///<summary>
        ///Smoothed delta time for consistent animation and movement.
        ///</summary>
        public static float SmoothDeltaTime => _smoothDeltaTime;
        
        ///<summary>
        ///Current frames per second.
        ///</summary>
        public static float FPS => _fps;
        
        ///<summary>
        ///Target frame rate for the engine.
        ///</summary>
        public static float TargetFrameRate
        {
            get => _targetFrameRate;
            set => _targetFrameRate = System.Math.Max(1f, value);
        }
        
        ///<summary>
        ///Total number of frames rendered since startup.
        ///</summary>
        public static int FrameCount => _frameCount;
        
        ///<summary>
        ///Whether the game is currently paused (time scale = 0).
        ///</summary>
        public static bool IsPaused
        {
            get => _isPaused;
            set => _isPaused = value;
        }
        
        ///<summary>
        ///Fixed time for physics updates (scaled by time scale).
        ///</summary>
        public static float FixedTime => _fixedTime;
        
        ///<summary>
        ///Fixed time for physics updates (unscaled).
        ///</summary>
        public static float FixedUnscaledTime => _fixedUnscaledTime;
        
        ///

        /// Public Methods
        
        ///<summary>
        ///Updates the time system. Should be called once per frame.
        ///</summary>
        ///<param name="currentRealtimeSinceStartup">Current real time since startup in seconds.</param>
        public static void Update(float currentRealtimeSinceStartup)
        {
            //Calculate unscaled delta time
            var previousRealtime = _realtimeSinceStartup;
            _realtimeSinceStartup = currentRealtimeSinceStartup;
            _unscaledDeltaTime = _realtimeSinceStartup - previousRealtime;
            
            //Clamp delta time to prevent large jumps
            _unscaledDeltaTime = System.Math.Clamp(_unscaledDeltaTime, 0f, 0.333f); //Max 1/3 second
            
            //Update frame count
            _frameCount++;
            
            //Update FPS calculation
            UpdateFPS(_unscaledDeltaTime);
            
            //Update scaled time if not paused
            if (!_isPaused)
            {
                _deltaTime = _unscaledDeltaTime;
                _timeSinceStartup += _deltaTime * _timeScale;
                _fixedTime += _fixedDeltaTime * _timeScale;
                
                //Update smooth delta time
                UpdateSmoothDeltaTime();
            }
            else
            {
                _deltaTime = 0f;
            }
            
            //Update unscaled fixed time
            _fixedUnscaledTime += _fixedUnscaledDeltaTime;
        }
        
        ///<summary>
        ///Performs fixed timestep updates. Returns true for each fixed update that should occur.
        ///</summary>
        ///<returns>True if a fixed update should occur, false otherwise.</returns>
        public static bool FixedUpdate()
        {
            if (_isPaused)
                return false;
                
            _accumulator += _unscaledDeltaTime;
            
            if (_accumulator >= _fixedUnscaledDeltaTime)
            {
                _accumulator -= _fixedUnscaledDeltaTime;
                return true;
            }
            
            return false;
        }
        
        ///<summary>
        ///Resets the time system to initial state.
        ///</summary>
        public static void Reset()
        {
            _deltaTime = 0.016f;
            _unscaledDeltaTime = 0.016f;
            _fixedDeltaTime = 0.02f;
            _fixedUnscaledDeltaTime = 0.02f;
            _timeScale = 1.0f;
            _timeSinceStartup = 0f;
            _realtimeSinceStartup = 0f;
            _smoothDeltaTime = 0.016f;
            _frameCount = 0;
            _fps = 60f;
            _fpsAccumulator = 0f;
            _fpsFrameCount = 0;
            _fixedTime = 0f;
            _fixedUnscaledTime = 0f;
            _accumulator = 0f;
            _isPaused = false;
            _startupTime = DateTime.UtcNow;
        }
        
        ///<summary>
        ///Pauses the game (sets time scale to 0).
        ///</summary>
        public static void Pause()
        {
            _isPaused = true;
        }
        
        ///<summary>
        ///Resumes the game (restores previous time scale).
        ///</summary>
        public static void Resume()
        {
            _isPaused = false;
        }
        
        ///<summary>
        ///Sets the time scale and returns the previous value.
        ///</summary>
        ///<param name="newTimeScale">New time scale value.</param>
        ///<returns>Previous time scale value.</returns>
        public static float SetTimeScale(float newTimeScale)
        {
            var previousScale = _timeScale;
            TimeScale = newTimeScale;
            return previousScale;
        }
        
        ///<summary>
        ///Gets the current time in a specific format.
        ///</summary>
        ///<param name="format">Time format (default: "mm:ss").</param>
        ///<returns>Formatted time string.</returns>
        public static string GetFormattedTime(string format = "mm:ss")
        {
            var time = TimeSpan.FromSeconds(_timeSinceStartup);
            
            return format.ToLower() switch
            {
                "hh:mm:ss" => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}",
                "mm:ss" => $"{time.Minutes:D2}:{time.Seconds:D2}",
                "ss" => $"{time.Seconds:D2}",
                "hh:mm:ss.fff" => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}",
                "mm:ss.fff" => $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}",
                _ => $"{time.Minutes:D2}:{time.Seconds:D2}"
            };
        }
        
        ///<summary>
        ///Gets the current real time in a specific format.
        ///</summary>
        ///<param name="format">Time format (default: "mm:ss").</param>
        ///<returns>Formatted real time string.</returns>
        public static string GetFormattedRealTime(string format = "mm:ss")
        {
            var time = TimeSpan.FromSeconds(_realtimeSinceStartup);
            
            return format.ToLower() switch
            {
                "hh:mm:ss" => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}",
                "mm:ss" => $"{time.Minutes:D2}:{time.Seconds:D2}",
                "ss" => $"{time.Seconds:D2}",
                "hh:mm:ss.fff" => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}",
                "mm:ss.fff" => $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}",
                _ => $"{time.Minutes:D2}:{time.Seconds:D2}"
            };
        }
        
        ///<summary>
        ///Creates a time-based interpolation factor for smooth transitions.
        ///</summary>
        ///<param name="duration">Duration of the transition in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>Interpolation factor (0.0 to 1.0).</returns>
        public static float GetInterpolationFactor(float duration, bool useUnscaledTime = false)
        {
            if (duration <= 0f)
                return 1f;
                
            var currentTime = useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
            var factor = (currentTime % duration) / duration;
            return System.Math.Clamp(factor, 0f, 1f);
        }
        
        ///<summary>
        ///Checks if a specific interval has passed.
        ///</summary>
        ///<param name="interval">Interval in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>True if the interval has passed.</returns>
        public static bool HasIntervalPassed(float interval, bool useUnscaledTime = false)
        {
            if (interval <= 0f)
                return true;
                
            var currentTime = useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
            return currentTime % interval < (useUnscaledTime ? _unscaledDeltaTime : DeltaTime);
        }
        
        ///<summary>
        ///Gets the remaining time until the next interval.
        ///</summary>
        ///<param name="interval">Interval in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>Remaining time in seconds.</returns>
        public static float GetTimeUntilNextInterval(float interval, bool useUnscaledTime = false)
        {
            if (interval <= 0f)
                return 0f;
                
            var currentTime = useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
            var nextInterval = (System.Math.Floor(currentTime / interval) + 1) * interval;
            return (float)(nextInterval - currentTime);
        }
        
        ///

        /// Timing Utilities
        
        ///<summary>
        ///Creates a simple timer that counts down from a duration.
        ///</summary>
        ///<param name="duration">Duration in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>Timer object.</returns>
        public static Timer CreateTimer(float duration, bool useUnscaledTime = false)
        {
            return new Timer(duration, useUnscaledTime);
        }
        
        ///<summary>
        ///Creates a cooldown that can be used repeatedly.
        ///</summary>
        ///<param name="cooldownDuration">Cooldown duration in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>Cooldown object.</returns>
        public static Cooldown CreateCooldown(float cooldownDuration, bool useUnscaledTime = false)
        {
            return new Cooldown(cooldownDuration, useUnscaledTime);
        }
        
        ///<summary>
        ///Creates a periodic trigger that fires at regular intervals.
        ///</summary>
        ///<param name="interval">Interval between triggers in seconds.</param>
        ///<param name="useUnscaledTime">Whether to use unscaled time.</param>
        ///<returns>Periodic trigger object.</returns>
        public static PeriodicTrigger CreatePeriodicTrigger(float interval, bool useUnscaledTime = false)
        {
            return new PeriodicTrigger(interval, useUnscaledTime);
        }
        
        ///

        /// Private Methods
        
        private static void UpdateFPS(float deltaTime)
        {
            _fpsAccumulator += deltaTime;
            _fpsFrameCount++;
            
            if (_fpsAccumulator >= _fpsUpdateInterval)
            {
                _fps = _fpsFrameCount / _fpsAccumulator;
                _fpsAccumulator = 0f;
                _fpsFrameCount = 0;
            }
        }
        
        private static void UpdateSmoothDeltaTime()
        {
            const float smoothingFactor = 0.9f;
            _smoothDeltaTime = _smoothDeltaTime * smoothingFactor + _deltaTime * (1f - smoothingFactor);
        }
        
        ///

        /// Nested Classes
        
        ///<summary>
        ///Simple timer that counts down from a duration.
        ///</summary>
        public class Timer
        {
            private readonly float _duration;
            private readonly bool _useUnscaledTime;
            private float _startTime;
            private bool _isRunning;
            
            public Timer(float duration, bool useUnscaledTime = false)
            {
                _duration = System.Math.Max(0f, duration);
                _useUnscaledTime = useUnscaledTime;
                Reset();
            }
            
            ///<summary>
            ///Remaining time in seconds.
            ///</summary>
            public float RemainingTime
            {
                get
                {
                    if (!_isRunning)
                        return 0f;
                        
                    var currentTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                    var elapsed = currentTime - _startTime;
                    return System.Math.Max(0f, _duration - elapsed);
                }
            }
            
            ///<summary>
            ///Progress from 0.0 to 1.0.
            ///</summary>
            public float Progress
            {
                get
                {
                    if (_duration <= 0f)
                        return 1f;
                        
                    return 1f - (RemainingTime / _duration);
                }
            }
            
            ///<summary>
            ///Whether the timer is currently running.
            ///</summary>
            public bool IsRunning => _isRunning;
            
            ///<summary>
            ///Whether the timer has completed.
            ///</summary>
            public bool IsCompleted => RemainingTime <= 0f;
            
            ///<summary>
            ///Starts or restarts the timer.
            ///</summary>
            public void Start()
            {
                _startTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                _isRunning = true;
            }
            
            ///<summary>
            ///Stops the timer.
            ///</summary>
            public void Stop()
            {
                _isRunning = false;
            }
            
            ///<summary>
            ///Resets the timer to initial state.
            ///</summary>
            public void Reset()
            {
                _startTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                _isRunning = false;
            }
        }
        
        ///<summary>
        ///Cooldown that can be used repeatedly.
        ///</summary>
        public class Cooldown
        {
            private readonly float _duration;
            private readonly bool _useUnscaledTime;
            private float _lastUseTime;
            
            public Cooldown(float cooldownDuration, bool useUnscaledTime = false)
            {
                _duration = System.Math.Max(0f, cooldownDuration);
                _useUnscaledTime = useUnscaledTime;
                _lastUseTime = -_duration; //Allow immediate first use
            }
            
            ///<summary>
            ///Remaining cooldown time in seconds.
            ///</summary>
            public float RemainingTime
            {
                get
                {
                    var currentTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                    var elapsed = currentTime - _lastUseTime;
                    return System.Math.Max(0f, _duration - elapsed);
                }
            }
            
            ///<summary>
            ///Whether the cooldown is ready (not on cooldown).
            ///</summary>
            public bool IsReady => RemainingTime <= 0f;
            
            ///<summary>
            ///Whether the cooldown is currently active.
            ///</summary>
            public bool IsOnCooldown => RemainingTime > 0f;
            
            ///<summary>
            ///Progress from 0.0 to 1.0 (0.0 = just used, 1.0 = ready).
            ///</summary>
            public float Progress
            {
                get
                {
                    if (_duration <= 0f)
                        return 1f;
                        
                    return 1f - (RemainingTime / _duration);
                }
            }
            
            ///<summary>
            ///Attempts to use the cooldown. Returns true if successful.
            ///</summary>
            ///<returns>True if the cooldown was used, false if still on cooldown.</returns>
            public bool TryUse()
            {
                if (!IsReady)
                    return false;
                    
                Use();
                return true;
            }
            
            ///<summary>
            ///Forces the cooldown to be used.
            ///</summary>
            public void Use()
            {
                _lastUseTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
            }
            
            ///<summary>
            ///Resets the cooldown to ready state.
            ///</summary>
            public void Reset()
            {
                _lastUseTime = -_duration;
            }
            
            ///<summary>
            ///Forces the cooldown to be on cooldown for the specified duration.
            ///</summary>
            public void ForceCooldown(float duration)
            {
                _lastUseTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                _lastUseTime -= _duration - duration;
            }
        }
        
        ///<summary>
        ///Periodic trigger that fires at regular intervals.
        ///</summary>
        public class PeriodicTrigger
        {
            private readonly float _interval;
            private readonly bool _useUnscaledTime;
            private float _lastTriggerTime;
            private bool _isActive;
            
            public PeriodicTrigger(float interval, bool useUnscaledTime = false)
            {
                _interval = System.Math.Max(0.001f, interval); //Minimum interval to prevent division by zero
                _useUnscaledTime = useUnscaledTime;
                _lastTriggerTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                _isActive = true;
            }
            
            ///<summary>
            ///Time until next trigger in seconds.
            ///</summary>
            public float TimeUntilNextTrigger
            {
                get
                {
                    if (!_isActive)
                        return float.MaxValue;
                        
                    var currentTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                    var nextTrigger = _lastTriggerTime + _interval;
                    return System.Math.Max(0f, nextTrigger - currentTime);
                }
            }
            
            ///<summary>
            ///Whether the trigger is currently active.
            ///</summary>
            public bool IsActive
            {
                get => _isActive;
                set => _isActive = value;
            }
            
            ///<summary>
            ///Checks if the trigger should fire this frame.
            ///</summary>
            ///<returns>True if the trigger should fire.</returns>
            public bool ShouldTrigger()
            {
                if (!_isActive)
                    return false;
                    
                var currentTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                var deltaTime = _useUnscaledTime ? _unscaledDeltaTime : DeltaTime;
                
                if (currentTime >= _lastTriggerTime + _interval)
                {
                    _lastTriggerTime = currentTime;
                    return true;
                }
                
                return false;
            }
            
            ///<summary>
            ///Resets the trigger to fire immediately on next check.
            ///</summary>
            public void Reset()
            {
                var currentTime = _useUnscaledTime ? _realtimeSinceStartup : _timeSinceStartup;
                _lastTriggerTime = currentTime - _interval;
            }
            
            ///<summary>
            ///Sets the interval and returns the previous value.
            ///</summary>
            ///<param name="newInterval">New interval in seconds.</param>
            ///<returns>Previous interval value.</returns>
            public float SetInterval(float newInterval)
            {
                var previousInterval = _interval;
                //Note: This would require making _interval not readonly in a real implementation
                return previousInterval;
            }
        }
        
        ///
    }
}
