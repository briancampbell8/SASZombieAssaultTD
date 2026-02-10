# =====================================================================
# Change0020-Fix.ps1
# Purpose: Restore known-good Zombie AI Foundation files
# Author: Automation Engine
# Mode: overwrite, ASCII, deterministic
# =====================================================================

Write-Host "[Change0020-Fix] Starting fix pass..." -ForegroundColor Cyan

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
while (-not (Test-Path "$root\SASZombieAssaultTD.csproj")) {
    $root = Split-Path -Parent $root
    if ($root -eq "") {
        Write-Host "[Change0020-Fix] ERROR: Could not locate project root." -ForegroundColor Red
        exit 1
    }
}

$zombieDir = Join-Path $root "Engine\Systems\Gameplay\Zombies"

$files = @{

    "ZombieBase.cs" = @"
using System;
using Engine.Systems.Gameplay;

namespace Engine.Systems.Gameplay.Zombies
{
    public abstract class ZombieBase : EnemyBase
    {
        public float PathProgress { get; protected set; }
        public float MoveSpeed { get; protected set; }

        public abstract void FollowPath(float deltaTime);
        public abstract void Behave(float deltaTime);

        public override void Update(float deltaTime)
        {
            FollowPath(deltaTime);
            Behave(deltaTime);
        }
    }
}
"@

    "BasicZombie.cs" = @"
using Engine.Systems.Gameplay;

namespace Engine.Systems.Gameplay.Zombies
{
    public class BasicZombie : ZombieBase
    {
        public BasicZombie()
        {
            Id = ""BasicZombie"";
            Health = 100f;
            MoveSpeed = 1.0f;
        }

        public override void Initialize()
        {
            PathProgress = 0f;
        }

        public override void FollowPath(float deltaTime)
        {
            PathProgress += MoveSpeed * deltaTime;
        }

        public override void Behave(float deltaTime)
        {
            // Placeholder for attack or decision logic
        }

        public override void TakeDamage(float amount)
        {
            Health -= amount;
        }
    }
}
"@

    "ZombieController.cs" = @"
using System.Collections.Generic;

namespace Engine.Systems.Gameplay.Zombies
{
    public class ZombieController
    {
        private readonly List<ZombieBase> _zombies = new();

        public void Spawn(ZombieBase zombie)
        {
            zombie.Initialize();
            _zombies.Add(zombie);
        }

        public void Update(float deltaTime)
        {
            foreach (var z in _zombies)
                z.Update(deltaTime);
        }
    }
}
"@
}

foreach ($file in $files.Keys) {
    $path = Join-Path $zombieDir $file
    Write-Host "[Change0020-Fix] Restoring $file..." -ForegroundColor Yellow
    $content = $files[$file]

    $bytes = [System.Text.Encoding]::ASCII.GetBytes($content)
    [System.IO.File]::WriteAllBytes($path, $bytes)
}

Write-Host "[Change0020-Fix] Completed fix pass successfully." -ForegroundColor Green