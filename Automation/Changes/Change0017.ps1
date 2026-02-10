# ============================================
# Change0017.ps1
# Zombie Animation System
# Adds AnimationClip, AnimationFrame, AnimationPlayer
# Integrates animation into ZombieMovement + GameScene
# Deterministic, additive-only, ASCII-safe
# ============================================

$ChangeId = "Change0017"
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
    "Engine/Systems",
    "Engine/Systems/Gameplay",
    "Engine/Systems/Gameplay/Animation",
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
# 2. Create animation subsystem files
# --------------------------------------------

$FilesToCreate = @(
    "Engine/Systems/Gameplay/Animation/AnimationFrame.cs",
    "Engine/Systems/Gameplay/Animation/AnimationClip.cs",
    "Engine/Systems/Gameplay/Animation/AnimationPlayer.cs"
)

foreach ($file in $FilesToCreate) {
    if (!(Test-Path $file)) {
        New-Item -ItemType File -Path $file | Out-Null
        Log "Created file: $file"
    }
}

# --------------------------------------------
# 3. Inject AnimationFrame.cs
# --------------------------------------------

$AnimationFrameCode = @"
using System;

namespace Engine.Systems.Gameplay.Animation
{
    // <$ChangeId>
    public class AnimationFrame
    {
        public string SpriteKey;
        public float Duration;

        public AnimationFrame(string spriteKey, float duration)
        {
            SpriteKey = spriteKey;
            Duration = duration;
        }
    }
    // </$ChangeId>
}
"@

Add-Content -Path "Engine/Systems/Gameplay/Animation/AnimationFrame.cs" -Value $AnimationFrameCode
Log "Injected $ChangeId into AnimationFrame.cs"

# --------------------------------------------
# 4. Inject AnimationClip.cs
# --------------------------------------------

$AnimationClipCode = @"
using System.Collections.Generic;

namespace Engine.Systems.Gameplay.Animation
{
    // <$ChangeId>
    public class AnimationClip
    {
        public List<AnimationFrame> Frames = new List<AnimationFrame>();
        public bool Loop = true;

        public AnimationClip() {}

        public void AddFrame(string spriteKey, float duration)
        {
            Frames.Add(new AnimationFrame(spriteKey, duration));
        }
    }
    // </$ChangeId>
}
"@

Add-Content -Path "Engine/Systems/Gameplay/Animation/AnimationClip.cs" -Value $AnimationClipCode
Log "Injected $ChangeId into AnimationClip.cs"

# --------------------------------------------
# 5. Inject AnimationPlayer.cs
# --------------------------------------------

$AnimationPlayerCode = @"
using System;
using Engine.Systems.Gameplay.Animation;

namespace Engine.Systems.Gameplay.Animation
{
    // <$ChangeId>
    public class AnimationPlayer
    {
        private AnimationClip clip;
        private int index = 0;
        private float timer = 0f;

        public string CurrentSprite { get; private set; }

        public AnimationPlayer(AnimationClip clip)
        {
            this.clip = clip;
            if (clip.Frames.Count > 0)
                CurrentSprite = clip.Frames[0].SpriteKey;
        }

        public void Update(float dt)
        {
            if (clip.Frames.Count == 0)
                return;

            timer += dt;

            if (timer >= clip.Frames[index].Duration)
            {
                timer = 0f;
                index++;

                if (index >= clip.Frames.Count)
                {
                    if (clip.Loop)
                        index = 0;
                    else
                        index = clip.Frames.Count - 1;
                }

                CurrentSprite = clip.Frames[index].SpriteKey;
            }
        }
    }
    // </$ChangeId>
}
"@

Add-Content -Path "Engine/Systems/Gameplay/Animation/AnimationPlayer.cs" -Value $AnimationPlayerCode
Log "Injected $ChangeId into AnimationPlayer.cs"

# --------------------------------------------
# 6. Integrate animation into ZombieMovement.cs
# --------------------------------------------

$ZombieMovementFile = "Scripts/Zombies/ZombieMovement.cs"

if (Test-Path $ZombieMovementFile) {

$ZombieMovementInject = @"
// <$ChangeId>
public Engine.Systems.Gameplay.Animation.AnimationPlayer AnimPlayer;

public void AttachAnimation(Engine.Systems.Gameplay.Animation.AnimationClip clip)
{
    AnimPlayer = new Engine.Systems.Gameplay.Animation.AnimationPlayer(clip);
}
// </$ChangeId>
"@

Add-Content -Path $ZombieMovementFile -Value $ZombieMovementInject
Log "Injected $ChangeId into ZombieMovement.cs"

} else {
    Log "WARNING: ZombieMovement.cs not found — skipping animation integration"
}

# --------------------------------------------
# 7. Integrate animation update into GameScene.cs
# --------------------------------------------

$GameSceneFile = "Engine/Scenes/GameScene.cs"

if (Test-Path $GameSceneFile) {

$GameSceneInject = @"
// <$ChangeId>
// Update animations for all zombies
foreach (var z in spawner.ActiveZombies)
{
    z.AnimPlayer?.Update(deltaTime);
}
// </$ChangeId>
"@

Add-Content -Path $GameSceneFile -Value $GameSceneInject
Log "Injected $ChangeId into GameScene.cs"

} else {
    Log "WARNING: GameScene.cs not found — skipping animation update hook"
}

# --------------------------------------------
# 8. Completion
# --------------------------------------------

Log "Completed $ChangeId"