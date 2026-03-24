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
namespace SASZombieAssaultTD.Engine.Diagnostics
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
        #region Public Properties

        /// <summary>
        /// Total number of frames rendered since startup.
        /// </summary>
        public long FrameCount { get; private set; }

        /// <summary>
        /// The most recently computed frames-per-second value.
        /// </summary>
        public float FramesPerSecond { get; private set; }

        #endregion

        #region Private Fields

        private float _accumulatedTime;
        private int _framesThisSecond;

        #endregion

        #region Public Methods

        /// <summary>
        /// Call once per frame with the elapsed time (in seconds) since the previous frame.
        /// </summary>
        /// <param name="deltaSeconds">The time elapsed since the last frame, in seconds.</param>
        public void OnFrame(float deltaSeconds)
        {
            if (deltaSeconds <= 0)
                return;

            FrameCount++;
            _framesThisSecond++;
            _accumulatedTime += deltaSeconds;

            if (_accumulatedTime >= 1.0f)
            {
                FramesPerSecond = _framesThisSecond / _accumulatedTime;
                ResetFrameAccumulator();
            }
        }

        /// <summary>
        /// Resets all frame statistics.
        /// </summary>
        public void Reset()
        {
            FrameCount = 0;
            FramesPerSecond = 0f;
            ResetFrameAccumulator();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the frame accumulator used for FPS calculation.
        /// </summary>
        private void ResetFrameAccumulator()
        {
            _accumulatedTime = 0f;
            _framesThisSecond = 0;
        }

        #endregion
    }
}


