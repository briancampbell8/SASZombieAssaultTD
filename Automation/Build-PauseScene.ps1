# Build-PauseScene.ps1
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

$sceneDir = Join-Path $RootPath 'Engine\Scenes'
New-Item -ItemType Directory -Path $sceneDir -Force | Out-Null

$pausePath = Join-Path $sceneDir 'PauseScene.cs'

$pauseContent = @'
using System;
using Engine.Rendering;
using Engine.Systems;
using Engine.Systems.UI;

namespace Engine.Scenes
{
    public sealed class PauseScene : Scene
    {
        private readonly RenderQueue _renderQueue;
        private readonly LayoutSystem _layoutSystem;
        private readonly UIElementBase _rootPanel;

        public PauseScene(RenderQueue renderQueue, LayoutSystem layoutSystem)
        {
            _renderQueue = renderQueue ?? throw new ArgumentNullException(nameof(renderQueue));
            _layoutSystem = layoutSystem ?? throw new ArgumentNullException(nameof(layoutSystem));

            _rootPanel = BuildLayout();
        }

        private UIElementBase BuildLayout()
        {
            var panel = new Panel
            {
                Id = "Pause.Root",
                IsVisible = true
            };

            var resumeButton = new Button
            {
                Id = "Pause.Resume",
                Text = "Resume"
            };

            var quitButton = new Button
            {
                Id = "Pause.Quit",
                Text = "Quit to Menu"
            };

            panel.AddChild(resumeButton);
            panel.AddChild(quitButton);

            return panel;
        }

        public override void Update(TimeSpan deltaTime)
        {
            base.Update(deltaTime);
            _layoutSystem.Update(_rootPanel, deltaTime);
        }

        public override void Render(IRenderContext context)
        {
            base.Render(context);
            _layoutSystem.Render(_rootPanel, _renderQueue, context);
        }
    }
}
'@

Set-Content -Path $pausePath -Value $pauseContent -Encoding ASCII

Write-Host "Build-PauseScene.ps1 — Wrote Engine\Scenes\PauseScene.cs (ASCII, deterministic)."
