# Build-MainMenuScene.ps1
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

$mainMenuPath = Join-Path $sceneDir 'MainMenuScene.cs'

$mainMenuContent = @'
using System;
using Engine.Rendering;
using Engine.Systems;
using Engine.Systems.UI;

namespace Engine.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        private readonly RenderQueue _renderQueue;
        private readonly LayoutSystem _layoutSystem;
        private readonly UIElementBase _rootPanel;

        public MainMenuScene(RenderQueue renderQueue, LayoutSystem layoutSystem)
        {
            _renderQueue = renderQueue ?? throw new ArgumentNullException(nameof(renderQueue));
            _layoutSystem = layoutSystem ?? throw new ArgumentNullException(nameof(layoutSystem));

            _rootPanel = BuildLayout();
        }

        private UIElementBase BuildLayout()
        {
            // TODO: Wire up actual menu layout once UI assets are defined.
            var panel = new Panel
            {
                Id = "MainMenu.Root",
                IsVisible = true
            };

            var startButton = new Button
            {
                Id = "MainMenu.Start",
                Text = "Start Game"
            };

            var quitButton = new Button
            {
                Id = "MainMenu.Quit",
                Text = "Quit"
            };

            panel.AddChild(startButton);
            panel.AddChild(quitButton);

            return panel;
        }

        public override void Initialize()
        {
            base.Initialize();
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

Set-Content -Path $mainMenuPath -Value $mainMenuContent -Encoding ASCII

Write-Host "Build-MainMenuScene.ps1 — Wrote Engine\Scenes\MainMenuScene.cs (ASCII, deterministic)."
