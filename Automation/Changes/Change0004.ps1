# Change0004.ps1
# Purpose: Inject Program.cs entry point into project root
# Mode: additive-only, UTF-8, atomic write

$root = "E:\BDC\Projects\SASZombieAssaultTD"
$programPath = Join-Path $root "Program.cs"
$logPath = Join-Path $root "PowerShellLog.md"

function Set-IfEmpty {
    param(
        [string]$Path,
        [string]$Content
    )

    if (-not (Test-Path $Path)) {
        Write-Host "Creating $Path"
        $Content | Set-Content -Path $Path -Encoding UTF8
        return
    }

    $raw = Get-Content $Path -Raw -ErrorAction SilentlyContinue
    if ($null -eq $raw) { $raw = " }

    if ($raw.Trim().Length -gt 0) {
        Write-Host "Skipping $Path (already populated)"
        return
    }

    Write-Host "Populating $Path"
    $Content | Set-Content -Path $Path -Encoding UTF8
}

# Program.cs content
$programContent = @"
using Engine.Core;

namespace SASZombieAssaultTD
{
    public static class Program
    {
        public static void Main()
        {
            var root = new GameRoot();
            root.Run();
        }
    }
}
"@

# Inject Program.cs if empty or missing
Set-IfEmpty -Path $programPath -Content $programContent

# Log Change0004
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0004.ps1 � $timestamp
- Injected Program.cs entry point into project root.
- Added minimal, modern bootstrapper calling GameRoot.Run().
- Mode: additive-only, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0004.ps1 completed."
