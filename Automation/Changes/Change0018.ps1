# ============================================
# Change0018.ps1
# Animated Zombie Rendering System
# Adds ZombieRenderer + integrates into GameScene
# Deterministic, additive-only, ASCII-safe
# ============================================

$ChangeId = "Change0018"
$LogFile = "PowerShellLog.md"

function Log {
    param([string]$Message)
    Add-Content -Path $LogFile -Value "[$ChangeId] $Message"
}

Log "Starting $ChangeId"

# --------------------------------------------
# 1. Ensure folder structure exists
# --------------------------------------------

$folders = @(
    "Engine/Rendering",
    "Engine/Rendering/Zombies",
    "Engine/Scenes",
    "Scripts/Zombies"
)

foreach ($folder in $folders) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
        Log "Created folder: $folder"
    }
}

# --------------------------------------------
# 2. Create ZombieRenderer.cs
# --------------------------------------------

$ZombieRendererFile = "Engine/Rendering/Zombies/ZombieRenderer.cs"

if (!(Test-Path $ZombieRendererFile)) {
    New-Item -ItemType File -Path $ZombieRendererFile | Out-Null
    Log "Created file: $ZombieRendererFile"
}

# --------------------------------------------
# 3. Inject ZombieRenderer.cs content
# --------------------------------------------

$ZombieRendererCode = @"
using System;
using System.Numerics;
using Engine.Rendering;
using Engine.Systems.Gameplay.Animation;

namespace Engine.Rendering.Zombies
{
    // <$ChangeId>
    public class ZombieRenderer
    {
        private TextureAtlas atlas;
        private SpriteBatch batch;

        public ZombieRenderer(TextureAtlas atlas, SpriteBatch batch)
        {
            this.atlas = atlas;
            this.batch = batch;
        }

        public void DrawZombie(IRenderContext ctx, Vector2 position, AnimationPlayer anim)
        {
            if (anim == null || string.IsNullOrEmpty(anim.CurrentSprite))
                return;

            var sprite = atlas.GetSprite(anim.CurrentSprite);
            if (sprite == null)
                return;

            batch.Draw(ctx, sprite, position);
        }
    }
    // </$ChangeId>
}
"@

Add-Content -Path $ZombieRendererFile -Value $ZombieRendererCode
Log "Injected $ChangeId into $ZombieRendererFile"

# --------------------------------------------
# 4. Inject renderer creation into GameScene.cs
# --------------------------------------------

$GameSceneFile = "Engine/Scenes/GameScene.cs"

if (Test-Path $GameSceneFile) {

$GameSceneInjectRenderer = @"
// <$ChangeId>
// Create zombie renderer
private Engine.Rendering.Zombies.ZombieRenderer zombieRenderer;
// </$ChangeId>
"@

Add-Content -Path $GameSceneFile -Value $GameSceneInjectRenderer
Log "Injected $ChangeId (renderer field) into GameScene.cs"

} else {
    Log "WARNING: GameScene.cs not found — skipping renderer field injection"
}

# --------------------------------------------
# 5. Inject renderer initialization into GameScene.cs
# --------------------------------------------

if (Test-Path $GameSceneFile) {

$GameSceneInitInject = @"
// <$ChangeId>
// Initialize zombie renderer
zombieRenderer = new Engine.Rendering.Zombies.ZombieRenderer(atlas, spriteBatch);
// </$ChangeId>
"@

Add-Content -Path $GameSceneFile -Value $GameSceneInitInject
Log "Injected $ChangeId (renderer init) into GameScene.cs"

}

# --------------------------------------------
# 6. Inject zombie rendering loop into GameScene.cs
# --------------------------------------------

if (Test-Path $GameSceneFile) {

$GameSceneRenderInject = @"
// <$ChangeId>
// Render zombies
foreach (var z in spawner.ActiveZombies)
{
    var pos = new System.Numerics.Vector2(z.x, z.y);
    zombieRenderer.DrawZombie(ctx, pos, z.AnimPlayer);
}
// </$ChangeId>
"@

Add-Content -Path $GameSceneFile -Value $GameSceneRenderInject
Log "Injected $ChangeId (render loop) into GameScene.cs"

}

# --------------------------------------------
# 7. Completion
# --------------------------------------------

Log "Completed $ChangeId"