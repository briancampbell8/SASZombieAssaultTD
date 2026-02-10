# AssetDiscovery-Build.ps1
# Subsystem Population Script � Structure-first, Additive-only, UTF-8, No BOM

$ErrorActionPreference = 'Stop'

$root = Get-Location
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

# Paths
$csPath            = Join-Path $root 'Engine/Systems/Assets/AssetDiscovery.cs'
$readmePath        = Join-Path $root 'README.md'
$structurePath     = Join-Path $root 'ProjectStructure.md'
$tasklistPath      = Join-Path $root 'Tasklist.md'
$logPath           = Join-Path $root 'PowerShellLog.md'

# UTF-8 without BOM
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Append-Line {
    param([string]$Path, [string]$Line)
    Add-Content -Path $Path -Value $Line -Encoding UTF8
}

# ---------------------------------------------------------
# 1. Create or Update AssetDiscovery.cs
# ---------------------------------------------------------

$initialContent = @"
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Systems.Assets
{
    /// <summary>
    /// AssetDiscovery
    /// --------------
    /// Scans the Assets directory, identifies available files,
    /// and produces a structured list for loaders and registries.
    ///
    /// Structure-first stub � logic will be added in later phases.
    /// </summary>
    public class AssetDiscovery
    {
        public IEnumerable<string> DiscoverAssets(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                yield break;

            foreach (var file in Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories))
                yield return file;
        }
    }
}
"@

$createdCs = $false

if (-not (Test-Path $csPath)) {
    [System.IO.File]::WriteAllText($csPath, $initialContent, $utf8NoBom)
    $createdCs = $true
}

# ---------------------------------------------------------
# 2. Update ProjectStructure.md
# ---------------------------------------------------------
Append-Line $structurePath "
Append-Line $structurePath "### Asset Discovery Build ($timestamp)"
if ($createdCs) {
    Append-Line $structurePath "- Created AssetDiscovery.cs"
} else {
    Append-Line $structurePath "- AssetDiscovery.cs already existed (skipped)"
}

# ---------------------------------------------------------
# 3. Update README.md
# ---------------------------------------------------------
Append-Line $readmePath "
Append-Line $readmePath "### Asset Discovery Subsystem Update ($timestamp)"
Append-Line $readmePath "- Asset Discovery subsystem stub created and integrated into the engine structure."

# ---------------------------------------------------------
# 4. Update Tasklist.md
# ---------------------------------------------------------
Append-Line $tasklistPath "
Append-Line $tasklistPath "### Asset Discovery Review Update ($timestamp)"
Append-Line $tasklistPath "- Marked 'Event Dispatcher' as reviewed."
Append-Line $tasklistPath "- Asset Discovery subsystem scaffolded."

# Replace the Event Dispatcher checkbox
(Get-Content $tasklistPath) |
    ForEach-Object { $_ -replace 'Event Dispatcher: \[ \]', 'Event Dispatcher: [x]' } |
    Set-Content -Encoding UTF8 $tasklistPath

# ---------------------------------------------------------
# 5. Update PowerShellLog.md
# ---------------------------------------------------------
Append-Line $logPath "
Append-Line $logPath "[$timestamp] AssetDiscovery-Build.ps1 executed."
if ($createdCs) {
    Append-Line $logPath "- Created AssetDiscovery.cs"
} else {
    Append-Line $logPath "- AssetDiscovery.cs already existed (skipped)"
}
Append-Line $logPath "- Marked Event Dispatcher as reviewed."
Append-Line $logPath "- Mode: structure-first, additive-only, UTF-8, no BOM."

Write-Host "AssetDiscovery-Build.ps1 completed successfully." -ForegroundColor Green
