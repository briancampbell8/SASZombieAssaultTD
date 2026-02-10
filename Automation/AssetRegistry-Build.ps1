# =====================================================================
# Asset Registry Subsystem Build Script
# SAS Zombie Assault TD � Engine Restoration Project
# Author: BDC
# Purpose: Populate and validate the Asset Registry subsystem structure
# =====================================================================

param(
    [string]$ProjectRoot = (Resolve-Path "..\..").Path
)

Write-Host "=== Asset Registry Subsystem Build Starting ===" -ForegroundColor Cyan

# ---------------------------------------------------------------------
# 1. Define Paths
# ---------------------------------------------------------------------
$SubsystemName = "AssetRegistry"
$SubsystemPath = Join-Path $ProjectRoot "Engine\$SubsystemName"
$LogPath       = Join-Path $ProjectRoot "Logs\AssetRegistry-Build.log"

# Ensure directories exist
$null = New-Item -ItemType Directory -Force -Path $SubsystemPath
$null = New-Item -ItemType Directory -Force -Path (Split-Path $LogPath)

# ---------------------------------------------------------------------
# 2. Files to Generate
# ---------------------------------------------------------------------
$Files = @(
    "AssetRegistry.cs",
    "AssetKey.cs",
    "AssetMetadata.cs",
    "AssetRegistryValidator.cs",
    "README.md"
)

foreach ($file in $Files) {
    $FullPath = Join-Path $SubsystemPath $file
    if (-not (Test-Path $FullPath)) {
        New-Item -ItemType File -Path $FullPath -Force | Out-Null
        Add-Content -Path $LogPath -Value "Created: $file"
        Write-Host "Created: $file"
    }
    else {
        Add-Content -Path $LogPath -Value "Exists: $file"
        Write-Host "Exists: $file"
    }
}

# ---------------------------------------------------------------------
# 3. Registry Template Injection
# ---------------------------------------------------------------------
$RegistryTemplate = @"
using System.Collections.Generic;

namespace Engine.AssetRegistry
{
    public static class AssetRegistry
    {
        private static readonly Dictionary<string, AssetMetadata> _assets =
            new Dictionary<string, AssetMetadata>();

        public static void Register(string key, AssetMetadata metadata)
        {
            if (!_assets.ContainsKey(key))
                _assets.Add(key, metadata);
        }

        public static AssetMetadata Get(string key)
        {
            return _assets.ContainsKey(key) ? _assets[key] : null;
        }

        public static IReadOnlyDictionary<string, AssetMetadata> All => _assets;
    }
}
"@

Set-Content -Path (Join-Path $SubsystemPath "AssetRegistry.cs") -Value $RegistryTemplate
Add-Content -Path $LogPath -Value "Injected: AssetRegistry.cs template"

# ---------------------------------------------------------------------
# 4. Completion
# ---------------------------------------------------------------------
Write-Host "=== Asset Registry Subsystem Build Complete ===" -ForegroundColor Green
Add-Content -Path $LogPath -Value "Build Completed: $(Get-Date)"
