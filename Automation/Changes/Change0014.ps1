<#
Change0014.ps1
Purpose:
    Forward injection of debug-ready implementations for:
        - GameStateManager.cs
        - RenderQueue.cs
        - InputRouter.cs
        - EventDispatcher.cs

Behavior:
    - Creates .bak backups if not present
    - Writes full C# implementations
    - Verifies content matches expected blocks (strict byte-for-byte)
    - Logs results to PowerShellLog.md
#>

$ErrorActionPreference = "Stop"

$root = "E:\BDC\Projects\SASZombieAssaultTD\Engine\Systems"

$targets = @{
    "GameStateManager.cs" = "$root\GameStateManager.cs"
    "RenderQueue.cs"      = "$root\RenderQueue.cs"
    "InputRouter.cs"      = "$root\InputRouter.cs"
    "EventDispatcher.cs"  = "$root\EventDispatcher.cs"
}

# -------------------------
# C# Injection Blocks
# -------------------------

$GameStateManager = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Systems
{
    public enum GameStateType
    {
        None = 0,
        Boot,
        MainMenu,
        InGame,
        Paused
    }

    public interface IGameState
    {
        GameStateType StateType { get; }
        void Enter();
        void Exit();
        void Update(float deltaTime);
    }

    public class GameStateManager
    {
        public GameStateType CurrentStateType => _currentState?.StateType ?? GameStateType.None;

        private readonly Dictionary<GameStateType, IGameState> _states;
        private IGameState _currentState;

        public GameStateManager()
        {
            _states = new Dictionary<GameStateType, IGameState>();

#if DEBUG
            Debug.WriteLine("[GameStateManager] Constructed.");
#endif
        }

        public void RegisterState(IGameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            _states[state.StateType] = state;

#if DEBUG
            Debug.WriteLine($"[GameStateManager] Registered state: {state.StateType}");
#endif
        }

        public void ChangeState(GameStateType newStateType)
        {
            if (_currentState != null && _currentState.StateType == newStateType)
                return;

            if (_currentState != null)
            {
#if DEBUG
                Debug.WriteLine($"[GameStateManager] Exiting state: {_currentState.StateType}");
#endif
                _currentState.Exit();
            }

            if (!_states.TryGetValue(newStateType, out var nextState))
            {
#if DEBUG
                Debug.WriteLine($"[GameStateManager] Requested state not registered: {newStateType}");
#endif
                _currentState = null;
                return;
            }

            _currentState = nextState;

#if DEBUG
            Debug.WriteLine($"[GameStateManager] Entering state: {_currentState.StateType}");
#endif
            _currentState.Enter();
        }

        public void Update(float deltaTime)
        {
            _currentState?.Update(deltaTime);
        }

#if DEBUG
        public void DebugPrint()
        {
            var stateName = _currentState?.StateType.ToString() ?? "None";
            Debug.WriteLine($"[GameStateManager] CurrentState={stateName}");
        }
#endif
    }
}
"@

$RenderQueue = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Systems
{
    public interface IRenderable
    {
        int RenderOrder { get; }
        void Render();
    }

    public class RenderQueue
    {
        private readonly List<IRenderable> _renderables;

        public RenderQueue()
        {
            _renderables = new List<IRenderable>();

#if DEBUG
            Debug.WriteLine("[RenderQueue] Constructed.");
#endif
        }

        public void Add(IRenderable renderable)
        {
            if (renderable == null) throw new ArgumentNullException(nameof(renderable));
            _renderables.Add(renderable);

#if DEBUG
            Debug.WriteLine($"[RenderQueue] Added renderable with order {renderable.RenderOrder}");
#endif
        }

        public void Remove(IRenderable renderable)
        {
            if (renderable == null) return;
            _renderables.Remove(renderable);

#if DEBUG
            Debug.WriteLine("[RenderQueue] Removed renderable.");
#endif
        }

        public void Process()
        {
            _renderables.Sort((a, b) => a.RenderOrder.CompareTo(b.RenderOrder));

#if DEBUG
            Debug.WriteLine($"[RenderQueue] Processing {_renderables.Count} renderables.");
#endif

            foreach (var renderable in _renderables)
            {
                renderable.Render();
            }
        }

#if DEBUG
        public void DebugPrint()
        {
            Debug.WriteLine($"[RenderQueue] Count={_renderables.Count}");
        }
#endif
    }
}
"@

$InputRouter = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Systems
{
    public interface IInputListener
    {
        void OnInputEvent(InputEvent inputEvent);
    }

    public enum InputEventType
    {
        None = 0,
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp,
        MouseMove
    }

    public readonly struct InputEvent
    {
        public InputEventType Type { get; }
        public int KeyCode { get; }
        public int MouseX { get; }
        public int MouseY { get; }

        public InputEvent(InputEventType type, int keyCode = 0, int mouseX = 0, int mouseY = 0)
        {
            Type = type;
            KeyCode = keyCode;
            MouseX = mouseX;
            MouseY = mouseY;
        }
    }

    public class InputRouter
    {
        private readonly List<IInputListener> _listeners;

        public InputRouter()
        {
            _listeners = new List<IInputListener>();

#if DEBUG
            Debug.WriteLine("[InputRouter] Constructed.");
#endif
        }

        public void RegisterListener(IInputListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);

#if DEBUG
                Debug.WriteLine("[InputRouter] Listener registered.");
#endif
            }
        }

        public void UnregisterListener(IInputListener listener)
        {
            if (listener == null) return;
            if (_listeners.Remove(listener))
            {
#if DEBUG
                Debug.WriteLine("[InputRouter] Listener unregistered.");
#endif
            }
        }

        public void Route(InputEvent inputEvent)
        {
#if DEBUG
            Debug.WriteLine($"[InputRouter] Routing event: {inputEvent.Type}");
#endif

            foreach (var listener in _listeners)
            {
                listener.OnInputEvent(inputEvent);
            }
        }

#if DEBUG
        public void DebugPrint()
        {
            Debug.WriteLine($"[InputRouter] ListenerCount={_listeners.Count}");
        }
#endif
    }
}
"@

$EventDispatcher = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Systems
{
    public interface IEventListener<TEvent>
    {
        void OnEvent(TEvent evt);
    }

    public class EventDispatcher
    {
        private readonly Dictionary<Type, List<object>> _listenersByType;

        public EventDispatcher()
        {
            _listenersByType = new Dictionary<Type, List<object>>();

#if DEBUG
            Debug.WriteLine("[EventDispatcher] Constructed.");
#endif
        }

        public void Subscribe<TEvent>(IEventListener<TEvent> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            var type = typeof(TEvent);
            if (!_listenersByType.TryGetValue(type, out var list))
            {
                list = new List<object>();
                _listenersByType[type] = list;
            }

            if (!list.Contains(listener))
            {
                list.Add(listener);

#if DEBUG
                Debug.WriteLine($"[EventDispatcher] Listener subscribed for {type.Name}");
#endif
            }
        }

        public void Unsubscribe<TEvent>(IEventListener<TEvent> listener)
        {
            if (listener == null) return;

            var type = typeof(TEvent);
            if (_listenersByType.TryGetValue(type, out var list))
            {
                if (list.Remove(listener))
                {
#if DEBUG
                    Debug.WriteLine($"[EventDispatcher] Listener unsubscribed from {type.Name}");
#endif
                }
            }
        }

        public void Publish<TEvent>(TEvent evt)
        {
            var type = typeof(TEvent);

#if DEBUG
            Debug.WriteLine($"[EventDispatcher] Publishing event: {type.Name}");
#endif

            if (_listenersByType.TryGetValue(type, out var list))
            {
                var snapshot = list.ToArray();
                foreach (var obj in snapshot)
                {
                    if (obj is IEventListener<TEvent> listener)
                    {
                        listener.OnEvent(evt);
                    }
                }
            }
        }

#if DEBUG
        public void DebugPrint()
        {
            Debug.WriteLine($"[EventDispatcher] EventTypesRegistered={_listenersByType.Count}");
        }
#endif
    }
}
"@

$blocks = @{
    "GameStateManager.cs" = $GameStateManager
    "RenderQueue.cs"      = $RenderQueue
    "InputRouter.cs"      = $InputRouter
    "EventDispatcher.cs"  = $EventDispatcher
}

# -------------------------
# Backup + Injection
# -------------------------

Write-Host "Starting Change0014 forward injection..." -ForegroundColor Cyan

foreach ($name in $targets.Keys)
{
    $path = $targets[$name]
    $bak  = "$path.bak"

    if ((Test-Path $path) -and -not (Test-Path $bak))
    {
        Write-Host "Creating backup for $name" -ForegroundColor Yellow
        Copy-Item -Path $path -Destination $bak -Force
    }

    Write-Host "Writing $name" -ForegroundColor Cyan
    [System.IO.File]::WriteAllText($path, $blocks[$name], [System.Text.Encoding]::UTF8)
}

# -------------------------
# Verification
# -------------------------

Write-Host "Verifying injection..." -ForegroundColor Cyan

$results = @()

foreach ($name in $targets.Keys)
{
    $path = $targets[$name]
    $content = Get-Content $path -Raw
    $expected = $blocks[$name]

    $pass = ($content -eq $expected)
	Write-Host "---- $name ----" -ForegroundColor Yellow
	Write-Host "Content length : $($content.Length)"
	Write-Host "Expected length: $($expected.Length)"
	Write-Host "Pass?          : $pass"
    $results += [PSCustomObject]@{
        File   = $name
        Path   = $path
        Result = if ($pass) { "PASS" } else { "FAIL" }
    }
}

# -------------------------
# Logging
# -------------------------

$logPath = "E:\BDC\Projects\SASZombieAssaultTD\PowerShellLog.md"

Add-Content -Path $logPath -Value "`n### Change0014 Injection $(Get-Date)"
foreach ($r in $results)
{
    Add-Content -Path $logPath -Value "- $($r.File): $($r.Result)"
}

# -------------------------
# Summary
# -------------------------

Write-Host "`nVerification Summary:" -ForegroundColor Yellow
$results | Format-Table

Write-Host "`nChange0014 forward injection complete." -ForegroundColor Green
