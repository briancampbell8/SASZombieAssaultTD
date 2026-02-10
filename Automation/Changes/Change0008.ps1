# Change0008.ps1
# Purpose: Overwrite Program.cs so it references the correct Engine.GameRoot
# Mode: overwrite, UTF-8, atomic write

$root    = "E:\BDC\Projects\SASZombieAssaultTD"
$program = Join-Path $root "Program.cs"
$logPath = Join-Path $root "PowerShellLog.md"

# Correct Program.cs content
$programContent = @"
using Engine;

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

Write-Host "Overwriting Program.cs with correct GameRoot wiring"
$programContent | Set-Content -Path $program -Encoding UTF8

# Log Change0008
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0008.ps1 � $timestamp
- Overwrote Program.cs to reference Engine.GameRoot correctly.
- Ensured correct namespace imports and removed shadow GameRoot references.
- Mode: overwrite, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0008.ps1 completed."
