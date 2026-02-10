
param(
    [Parameter(Mandatory=$true)]
    [string]$TaskName,

    [Switch]$Uncheck
)

$tasklistPath = "Tasklist.md"
$logPath = "PowerShellLog.md"

# Read all lines from the file
$lines = Get-Content $tasklistPath -Encoding UTF8

# Marker for the start of the Review Status Zone
$startMarker = "## Review Status (PowerShell‑Managed)"

# Find the index of the marker
$startIndex = $lines.IndexOf($startMarker)

if ($startIndex -lt 0) {
    Write-Host "Review Status Zone not found. Cannot update." -ForegroundColor Red
    exit
}


# Escape the task name for regex safety
$escaped = [regex]::Escape($TaskName)

# Search only within the Review Status Zone
for ($i = $startIndex + 1; $i -lt $lines.Count; $i++) {

    # Stop if we hit a new header
    if ($lines[$i].StartsWith("#")) { break }

if ($lines[$i] -match "^\s*-\s*$escaped\s*:\s*\[(.*?)\]") {

if ($Lines[$i] -match "^\s*-\s*$escaped\s*:\s*\[(.*?)\]") {

    if ($Uncheck) {
        $Lines[$i] = "- ${TaskName}: [ ]"
        Write-Host "Task '$TaskName' unchecked." -ForegroundColor Yellow
    } else {
        $Lines[$i] = "- ${TaskName}: [x]"
        Write-Host "Task '$TaskName' marked as reviewed." -ForegroundColor Green
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllLines($TaskListPath, $Lines, $utf8NoBom)

    Add-Content $LogPath "$(Get-Date) — $TaskName was $(
        if ($Uncheck) { 'unchecked' } else { 'checked' }
    ) by automation."

    exit
    }
}    
}

Write-Host "Task '$TaskName' not found in Review Status Zone." -ForegroundColor Yellow

