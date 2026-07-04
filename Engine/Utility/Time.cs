/*
File:    Time.cs
Purpose: DeltaTime smoothing; fixed timestep placeholder.
*/


using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Utility
{
    public static class Time
    {
        private const int SmoothingSamples = 10;
        private static readonly float[] _deltaSamples = new float[SmoothingSamples];
        private static int _sampleIndex;

        public static float DeltaTime { get; set; }
        public static float TotalTime { get; set; }
        public static float SmoothedDeltaTime { get; private set; }

        public static float FixedDeltaTime { get; set; } = 1f / 60f;

        public static void Advance(float deltaTime)
        {
            DeltaTime = deltaTime;
            TotalTime += deltaTime;
            _deltaSamples[_sampleIndex % SmoothingSamples] = deltaTime;
            _sampleIndex++;
            float sum = 0;
            int count = System.Math.Min(_sampleIndex, SmoothingSamples);
            for (int i = 0; i < count; i++)
                sum += _deltaSamples[i];
            SmoothedDeltaTime = count > 0 ? sum / count : deltaTime;
        }
    }
}




