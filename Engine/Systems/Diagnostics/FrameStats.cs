/*
    File:    FrameStats.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Tracks frame timing and statistics for diagnostics.

    Notes:
        Example usage: GameScene uses FrameStats to track FPS and frame times.
        Call OnFrame() once per frame; use FramesPerSecond for diagnostics.
*/
namespace SASZombieAssaultTD.Engine.Systems.Diagnostics
{
    /// <summary>
    /// Tracks frame timing and statistics for diagnostics.
    /// Example usage: 
    ///   var stats = new FrameStats();
    ///   stats.OnFrame(deltaSeconds);
    ///   float fps = stats.FramesPerSecond;
    /// </summary>
    public sealed class FrameStats
    {
        /// <summary>
        /// Total number of frames rendered since startup.
        /// </summary>
        public long FrameCount { get; private set; }

        /// <summary>
        /// The most recently computed frames-per-second value.
        /// </summary>
        public float FramesPerSecond { get; private set; }

        private float _accumulatedTime;
        private int _framesThisSecond;

        /// <summary>
        /// Call once per frame with the elapsed time (in seconds) since the previous frame.
        /// </summary>
        public void OnFrame(float deltaSeconds)
        {
            FrameCount++;
            _framesThisSecond++;
            _accumulatedTime += deltaSeconds;

            if (_accumulatedTime >= 1.0f)
            {
                FramesPerSecond = _framesThisSecond / _accumulatedTime;
                _framesThisSecond = 0;
                _accumulatedTime = 0f;
            }
        }

        /// <summary>
        /// Resets all frame statistics.
        /// </summary>
        public void Reset()
        {
            FrameCount = 0;
            FramesPerSecond = 0f;
            _accumulatedTime = 0f;
            _framesThisSecond = 0;
        }
    }
}