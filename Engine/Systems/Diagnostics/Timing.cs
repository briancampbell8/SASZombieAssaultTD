using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems.Diagnostics
{
    public sealed class FrameTimer
    {
        private readonly Stopwatch _sw = new Stopwatch();
        private double _lastFrameTime;

        public double DeltaTime { get; private set; }

        public void Start()
        {
            _sw.Restart();
            _lastFrameTime = 0.0;
            DeltaTime = 0.0;
        }

        public void Tick()
        {
            double now = _sw.Elapsed.TotalSeconds;
            DeltaTime = now - _lastFrameTime;
            _lastFrameTime = now;
        }
    }

    public sealed class FpsCounter
    {
        private double _accum;
        private int _frames;
        private double _fps;

        public double Fps => _fps;

        public void Update(double deltaTime)
        {
            _accum += deltaTime;
            _frames++;

            if (_accum >= 1.0)
            {
                _fps = _frames / _accum;
                _accum = 0.0;
                _frames = 0;
            }
        }
    }
}
