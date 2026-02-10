$changesRoot = "E:\BDC\Projects\SASZombieAssaultTD\Automation\Changes"
$quarantine  = Join-Path $changesRoot "_DISABLED"

if (!(Test-Path $quarantine)) {
    New-Item -ItemType Directory -Path $quarantine | Out-Null
}

$targets = @(
    "Change0010.ps1",
    "Change0011.ps1"
)

foreach ($file in $targets) {
    $src = Join-Path $changesRoot $file
    if (Test-Path $src) {
        $dst = Join-Path $quarantine $file
        Move-Item -Force $src $dst
        Write-Host "Disabled: $file (moved to _DISABLED)"
    } else {
        Write-Host "Not found (already disabled?): $file"
    }
}

$log = Join-Path $changesRoot "DisableLog.txt"
"[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] Disabled Change0010.ps1 and Change0011.ps1" |
    Out-File -FilePath $log -Append -Encoding utf8

Write-Host "Automation change scripts disabled safely."
