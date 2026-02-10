# Automation/EventDispatcher-Build.ps1
# Structure-first, additive-only, UTF-8 (no BOM)

$ErrorActionPreference = 'Stop'

$root = Get-Location
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

# Paths
$csPath  = Join-Path $root 'Engine/Systems/EventDispatcher.cs'
$logPath = Join-Path $root 'PowerShellLog.md'

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Append-Line {
    param([string]$Path, [string]$Line)
    Add-Content -Path $Path -Value $Line -Encoding UTF8
}

# ---------------------------------------------------------
# 1. Create EventDispatcher.cs if missing
# ---------------------------------------------------------

$initialContent = @"
using System;
using System.Collections.Generic;

namespace Engine.Systems
{
    /// <summary>
    /// EventDispatcher
    /// Routes events between systems in a decoupled manner.
    /// Structure-first stub.
    /// </summary>
    public class EventDispatcher
    {
        private readonly Dictionary<string, Action<object>> _handlers =
            new Dictionary<string, Action<object>>();

        public void Register(string eventName, Action<object> handler)
        {
            if (!_handlers.ContainsKey(eventName))
                _handlers[eventName] = handler;
        }

        public void Dispatch(string eventName, object payload)
        {
            if (_handlers.TryGetValue(eventName, out var handler))
                handler(payload);
        }
    }
}
"@

$created = $false

if (-not (Test-Path $csPath)) {
    [System.IO.File]::WriteAllText($csPath, $initialContent, $utf8NoBom)
    $created = $true
}

# ---------------------------------------------------------
# 2. Log execution
# ---------------------------------------------------------

Append-Line $logPath "
Append-Line $logPath "[$timestamp] EventDispatcher-Build.ps1 executed."
if ($created) {
    Append-Line $logPath "- Created EventDispatcher.cs"
} else {
    Append-Line $logPath "- EventDispatcher.cs already existed (skipped)"
}
Append-Line $logPath "- Structure-first, additive-only."
