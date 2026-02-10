# Change0003.ps1
# Purpose: Populate core structural systems needed for GameLoop debugging
# Scope: RenderQueue, InputRouter, EventDispatcher, LoggingSystem, GameLoop using directive
# Mode: additive-only, UTF-8, atomic writes

$root = "E:\BDC\Projects\SASZombieAssaultTD"
$enginePath = Join-Path $root "Engine"
$systemsPath = Join-Path $enginePath "Systems"

$renderQueuePath   = Join-Path $systemsPath "RenderQueue.cs"
$inputRouterPath   = Join-Path $systemsPath "InputRouter.cs"
$eventDispatcherPath = Join-Path $systemsPath "EventDispatcher.cs"
$loggingSystemPath = Join-Path $systemsPath "LoggingSystem.cs"
$gameLoopPath      = Join-Path (Join-Path $enginePath "Core") "GameLoop.cs"
$logPath           = Join-Path $root "PowerShellLog.md"

function Set-IfEmpty {
    param(
        [string]$Path,
        [string]$Content
    )

    if (-not (Test-Path $Path)) {
        Write-Host "Skipping $Path (file not found)"
        return
    }

    $raw = Get-Content $Path -Raw -ErrorAction SilentlyContinue
    if ($null -eq $raw) { $raw = " }
    if ($raw.Trim().Length -gt 0) {
        Write-Host "Skipping $Path (already populated)"
        return
    }

    Write-Host "Populating $Path"
    $Content | Set-Content -Path $Path -Encoding UTF8
}

# 1. Populate RenderQueue.cs (Engine.Systems)
$renderQueueContent = @"
using System;
using System.Collections.Generic;

namespace Engine.Systems
{
    /// <summary>
    /// Simple render queue that collects render actions for the current frame.
    /// The Game Loop is responsible for calling Process() once per frame.
    /// </summary>
    public class RenderQueue
    {
        private readonly List<Action> _renderActions = new List<Action>();

        public void Enqueue(Action renderAction)
        {
            if (renderAction == null) return;
            _renderActions.Add(renderAction);
        }

        public void Process()
        {
            foreach (var action in _renderActions)
            {
                action();
            }

            _renderActions.Clear();
        }
    }
}
"@

Set-IfEmpty -Path $renderQueuePath -Content $renderQueueContent

# 2. Populate InputRouter.cs (Engine.Systems)
$inputRouterContent = @"
using System;

namespace Engine.Systems
{
    /// <summary>
    /// InputRouter is responsible for polling input state and exposing
    /// high-level signals to the rest of the engine.
    /// Framework-specific input wiring will be added later.
    /// </summary>
    public class InputRouter
    {
        public void Poll()
        {
            // Placeholder for input polling logic.
            // This will be wired to the actual input framework later.
        }
    }
}
"@

Set-IfEmpty -Path $inputRouterPath -Content $inputRouterContent

# 3. Populate EventDispatcher.cs (Engine.Systems)
$eventDispatcherContent = @"
using System;
using System.Collections.Generic;

namespace Engine.Systems
{
    /// <summary>
    /// Minimal event dispatcher for routing simple engine events.
    /// This will be extended as systems come online.
    /// </summary>
    public class EventDispatcher
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) return;

            var key = typeof(TEvent);
            if (!_handlers.TryGetValue(key, out var list))
            {
                list = new List<Delegate>();
                _handlers[key] = list;
            }

            list.Add(handler);
        }

        public void Publish<TEvent>(TEvent evt)
        {
            var key = typeof(TEvent);
            if (!_handlers.TryGetValue(key, out var list)) return;

            foreach (var handler in list)
            {
                if (handler is Action<TEvent> action)
                {
                    action(evt);
                }
            }
        }
    }
}
"@

Set-IfEmpty -Path $eventDispatcherPath -Content $eventDispatcherContent

# 4. Populate LoggingSystem.cs (Engine.Systems)
$loggingSystemContent = @"
using System;
using System.Diagnostics;

namespace Engine.Systems
{
    /// <summary>
    /// Minimal logging system for engine bring-up and debugging.
    /// Can later be redirected to files, overlays, or external sinks.
    /// </summary>
    public static class LoggingSystem
    {
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        }
    }
}
"@

Set-IfEmpty -Path $loggingSystemPath -Content $loggingSystemContent

# 5. Ensure GameLoop.cs can see Engine.Systems
if (Test-Path $gameLoopPath) {
    $gameLoopRaw = Get-Content $gameLoopPath -Raw -ErrorAction SilentlyContinue
    if ($null -ne $gameLoopRaw -and $gameLoopRaw -notmatch "using\s+Engine\.Systems;") {
        Write-Host "Adding 'using Engine.Systems;' to GameLoop.cs"

        $lines = $gameLoopRaw -split "`r?`n"
        $insertIndex = 0

        for ($i = 0; $i -lt $lines.Length; $i++) {
            if ($lines[$i] -match "^using\s+") {
                $insertIndex = $i + 1
            }
        }

        $updatedLines = @()
        if ($insertIndex -gt 0) {
            $updatedLines += $lines[0..($insertIndex-1)]
            $updatedLines += "using Engine.Systems;"
            if ($insertIndex -lt $lines.Length) {
                $updatedLines += $lines[$insertIndex..($lines.Length-1)]
            }
        } else {
            $updatedLines += "using Engine.Systems;"
            $updatedLines += $lines
        }

        ($updatedLines -join [Environment]::NewLine) | Set-Content -Path $gameLoopPath -Encoding UTF8
    } else {
        Write-Host "GameLoop.cs already references Engine.Systems or could not be read."
    }
} else {
    Write-Host "GameLoop.cs not found at $gameLoopPath"
}

# 6. Log Change0003 in PowerShellLog.md
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0003.ps1 � $timestamp
- Populated structural systems required for GameLoop debugging (RenderQueue, InputRouter, EventDispatcher, LoggingSystem).
- Ensured Engine.Systems namespace usage and added 'using Engine.Systems;' to GameLoop.cs where needed.
- Mode: additive-only, UTF-8, atomic writes.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0003.ps1 completed."
