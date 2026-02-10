# =====================================================================
# Change0019.ps1
# Purpose: Install foundational Gameplay Entities Layer
# Author: Automation Engine
# Mode: overwrite-safe, ASCII, deterministic
# Timestamp: (auto-generated at runtime)
# =====================================================================

Write-Host "[Change0019] Starting Change0019..." -ForegroundColor Cyan

# ---------------------------------------------------------
# 1. Detect project root
# ---------------------------------------------------------
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
while (-not (Test-Path "$root\SASZombieAssaultTD.csproj")) {
    $root = Split-Path -Parent $root
    if ($root -eq "") {
        Write-Host "[Change0019] ERROR: Could not locate project root." -ForegroundColor Red
        exit 1
    }
}

Write-Host "[Change0019] Project root detected: $root" -ForegroundColor Green

# ---------------------------------------------------------
# 2. Define Gameplay subsystem paths
# ---------------------------------------------------------
$gameplayDir = Join-Path $root "Engine\Systems\Gameplay"

$files = @{
    "TowerBase.cs"        = @"
using System;

namespace Engine.Systems.Gameplay
{
    public abstract class TowerBase
    {
        public string Id { get; protected set; }
        public float Range { get; protected set; }
        public float FireRate { get; protected set; }

        public abstract void Initialize();
        public abstract void Update(float deltaTime);
        public abstract void Fire();
    }
}
"@

    "EnemyBase.cs"        = @"
using System;

namespace Engine.Systems.Gameplay
{
    public abstract class EnemyBase
    {
        public string Id { get; protected set; }
        public float Health { get; protected set; }
        public float Speed { get; protected set; }

        public abstract void Initialize();
        public abstract void Update(float deltaTime);
        public abstract void TakeDamage(float amount);
    }
}
"@

    "ProjectileSystem.cs" = @"
using System.Collections.Generic;

namespace Engine.Systems.Gameplay
{
    public class ProjectileSystem
    {
        private readonly List<object> _projectiles = new();

        public void Spawn(object projectile)
        {
            _projectiles.Add(projectile);
        }

        public void Update(float deltaTime)
        {
            // Placeholder for projectile updates
        }
    }
}
"@

    "WaveController.cs"   = @"
using System.Collections.Generic;

namespace Engine.Systems.Gameplay
{
    public class WaveController
    {
        private readonly Queue<object> _waves = new();

        public void EnqueueWave(object wave)
        {
            _waves.Enqueue(wave);
        }

        public void Update(float deltaTime)
        {
            // Placeholder for wave progression
        }
    }
}
"@

    "DamageResolver.cs"   = @"
namespace Engine.Systems.Gameplay
{
    public static class DamageResolver
    {
        public static float ApplyDamage(float baseDamage, float modifier)
        {
            return baseDamage * modifier;
        }
    }
}
"@
}

# ---------------------------------------------------------
# 3. Ensure directory exists
# ---------------------------------------------------------
if (-not (Test-Path $gameplayDir)) {
    Write-Host "[Change0019] Creating Gameplay directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $gameplayDir | Out-Null
} else {
    Write-Host "[Change0019] PASS: Gameplay directory exists." -ForegroundColor Green
}

# ---------------------------------------------------------
# 4. Write files (overwrite, ASCII, deterministic)
# ---------------------------------------------------------
foreach ($file in $files.Keys) {
    $path = Join-Path $gameplayDir $file
    Write-Host "[Change0019] Writing $file..." -ForegroundColor Cyan
    $content = $files[$file]

    $bytes = [System.Text.Encoding]::ASCII.GetBytes($content)
    [System.IO.File]::WriteAllBytes($path, $bytes)
}

# ---------------------------------------------------------
# 5. Completion
# ---------------------------------------------------------
Write-Host "[Change0019] Completed Change0019 successfully." -ForegroundColor Green