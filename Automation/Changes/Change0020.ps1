# =====================================================================
# Change0020.ps1
# Purpose: Install Zombie AI Foundation (movement, pathing, behavior)
# Author: Automation Engine
# Mode: overwrite-safe, ASCII, deterministic
# Timestamp: (auto-generated at runtime)
# =====================================================================

Write-Host "[Change0020] Starting Change0020..." -ForegroundColor Cyan

# ---------------------------------------------------------
# 1. Detect project root
# ---------------------------------------------------------
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
while (-not (Test-Path "$root\SASZombieAssaultTD.csproj")) {
    $root = Split-Path -Parent $root
    if ($root -eq "") {
        Write-Host "[Change0020] ERROR: Could not locate project root." -ForegroundColor Red
        exit 1
    }
}

Write-Host "[Change0020] Project root detected: $root" -ForegroundColor Green

# ---------------------------------------------------------
# 2. Define Zombie AI subsystem paths
# ---------------------------------------------------------
$gameplayDir = Join-Path $root "Engine\Systems\Gameplay"
$zombieDir   = Join-Path $gameplayDir "Zombies"

# ---------------------------------------------------------
# 3. Ensure directories exist
# ---------------------------------------------------------
if (-not (Test-Path $zombieDir)) {
    Write-Host "[Change0020] Creating Zombies directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $zombieDir | Out-Null
} else {
    Write-Host "[Change0020] PASS: Zombies directory exists." -ForegroundColor Green
}

# ---------------------------------------------------------
# 4. Define files to write
# ---------------------------------------------------------
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

# ---------------------------------------------------------
# 5. Write files (overwrite, ASCII, deterministic)
# ---------------------------------------------------------
foreach ($file in $files.Keys) {
    $path = Join-Path $zombieDir $file
    Write-Host "[Change0020] Writing $file..." -ForegroundColor Cyan
    $content = $files[$file]

    $bytes = [System.Text.Encoding]::ASCII.GetBytes($content)
    [System.IO.File]::WriteAllBytes($path, $bytes)
}

# ---------------------------------------------------------
# 6. Completion
# ---------------------------------------------------------
Write-Host "[Change0020] Completed Change0020 successfully." -ForegroundColor Green