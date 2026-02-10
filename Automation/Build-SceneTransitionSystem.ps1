# =====================================================================
# Build-SceneTransitionSystem.ps1
# Generates SceneStack.cs, SceneTransition.cs, and updates SceneManager.cs
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot   = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$scenePath    = Join-Path $engineRoot "Scenes"
$logPath      = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $scenePath | Out-Null

# ---------------------------------------------------------------------
# SceneStack.cs
# ---------------------------------------------------------------------
$sceneStackContent = @"
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class SceneStack
    {
        private readonly Stack<Scene> _stack = new Stack<Scene>();

        public Scene Current => _stack.Count > 0 ? _stack.Peek() : null;

        public void Push(Scene scene)
        {
            scene.Init();
            _stack.Push(scene);
        }

        public void Pop()
        {
            if (_stack.Count > 0)
                _stack.Pop();
        }

        public void Replace(Scene scene)
        {
            if (_stack.Count > 0)
                _stack.Pop();

            scene.Init();
            _stack.Push(scene);
        }
    }
}
"@

Set-Content -Path (Join-Path $scenePath "SceneStack.cs") -Value $sceneStackContent -Encoding ASCII

# ---------------------------------------------------------------------
# SceneTransition.cs
# ---------------------------------------------------------------------
$sceneTransitionContent = @"
namespace SASZombieAssaultTD.Engine.Scenes
{
    public enum SceneTransitionType
    {
        None,
        Fade
    }

    public sealed class SceneTransition
    {
        public SceneTransitionType Type { get; }
        public float Duration { get; }

        public SceneTransition(SceneTransitionType type, float duration = 0.5f)
        {
            Type = type;
            Duration = duration;
        }
    }
}
"@

Set-Content -Path (Join-Path $scenePath "SceneTransition.cs") -Value $sceneTransitionContent -Encoding ASCII

# ---------------------------------------------------------------------
# SceneManager.cs (overwrite with stack-enabled version)
# ---------------------------------------------------------------------
$sceneManagerContent = @"
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class SceneManager
    {
        private readonly SceneStack _stack = new SceneStack();

        public void SetScene(Scene scene)
        {
            _stack.Replace(scene);
        }

        public void PushScene(Scene scene)
        {
            _stack.Push(scene);
        }

        public void PopScene()
        {
            _stack.Pop();
        }

        public void Update()
        {
            _stack.Current?.Update();
        }

        public void Render(RenderQueue rq)
        {
            _stack.Current?.Render(rq);
        }
    }
}
"@

Set-Content -Path (Join-Path $scenePath "SceneManager.cs") -Value $sceneManagerContent -Encoding ASCII

# ---------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-SceneTransitionSystem.ps1 — $timestamp
- Wrote SceneStack.cs, SceneTransition.cs, and updated SceneManager.cs.
- Added push/pop/replace scene stack architecture.
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "Scene Transition System written and logged successfully."
