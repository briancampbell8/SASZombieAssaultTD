# Build-DiagnosticsSubsystem.ps1
param(
    [string]$RootPath
)

if (-not $RootPath -or -not (Test-Path $RootPath)) {
    $RootPath = Split-Path -Parent $PSCommandPath
    while ($RootPath -and -not (Test-Path (Join-Path $RootPath 'SASZombieAssaultTD.csproj'))) {
        $parent = Split-Path -Parent $RootPath
        if ($parent -eq $RootPath) { break }
        $RootPath = $parent
    }
}

if (-not (Test-Path (Join-Path $RootPath 'SASZombieAssaultTD.csproj'))) {
    Write-Error "Could not locate project root from $RootPath"
    exit 1
}

$diagDir = Join-Path $RootPath 'Engine\Systems\Diagnostics'
New-Item -ItemType Directory -Path $diagDir -Force | Out-Null

$frameStatsPath = Join-Path $diagDir 'FrameStats.cs'
$heartbeatPath = Join-Path $diagDir 'HeartbeatMonitor.cs'
$debugLoggerPath = Join-Path $diagDir 'DebugLogger.cs'

$frameStatsContent = @'
using System;

namespace Engine.Systems.Diagnostics
{
    public sealed class FrameStats
    {
        public double DeltaSeconds { get; private set; }
        public double FramesPerSecond { get; private set; }

        private double _accumulator;
        private int _frameCount;

        public void Update(TimeSpan deltaTime)
        {
            DeltaSeconds = deltaTime.TotalSeconds;
            _accumulator += DeltaSeconds;
            _frameCount++;

            if (_accumulator >= 1.0)
            {
                FramesPerSecond = _frameCount / _accumulator;
                _accumulator = 0.0;
                _frameCount = 0;
            }
        }
    }
}
'@

$heartbeatContent = @'
using System;

namespace Engine.Systems.Diagnostics
{
    public sealed class HeartbeatMonitor
    {
        public DateTime LastBeatUtc { get; private set; }
        public bool IsAlive => (DateTime.UtcNow - LastBeatUtc) < _timeout;

        private readonly TimeSpan _timeout;

        public HeartbeatMonitor(TimeSpan timeout)
        {
            _timeout = timeout;
            LastBeatUtc = DateTime.UtcNow;
        }

        public void Beat()
        {
            LastBeatUtc = DateTime.UtcNow;
        }
    }
}
'@

$debugLoggerContent = @'
using System;
using System.Collections.Generic;

namespace Engine.Systems.Diagnostics
{
    public sealed class DebugLogger
    {
        private readonly List<string> _buffer = new List<string>();

        public IReadOnlyList<string> Buffer => _buffer;

        public void Info(string message) => Log("INFO", message);
        public void Warn(string message) => Log("WARN", message);
        public void Error(string message) => Log("ERROR", message);

        private void Log(string level, string message)
        {
            var line = $"[{DateTime.UtcNow:O}] {level}: {message}";
            _buffer.Add(line);
            Console.WriteLine(line);
        }

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
'@

Set-Content -Path $frameStatsPath -Value $frameStatsContent -Encoding ASCII
Set-Content -Path $heartbeatPath -Value $heartbeatContent -Encoding ASCII
Set-Content -Path $debugLoggerPath -Value $debugLoggerContent -Encoding ASCII

Write-Host "Build-DiagnosticsSubsystem.ps1 — Wrote FrameStats.cs, HeartbeatMonitor.cs, DebugLogger.cs (ASCII, deterministic)."
