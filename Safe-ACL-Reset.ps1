# Safe-ACL-Reset.ps1
param([string]$Path = ".", [switch]$DryRun, [switch]$Force)

if (-not $Force) {
    Write-Host "=== SAFETY CHECK ===" -ForegroundColor Red
    Write-Host "Path: $(Resolve-Path $Path)" -ForegroundColor Yellow
    Write-Host "Scope: Root only (not recursive)" -ForegroundColor Yellow
    Write-Host "Permission: Modify (M)" -ForegroundColor Yellow
    $confirm = Read-Host "Type 'CONFIRM' to proceed (or use -Force)"
    if ($confirm -ne "CONFIRM") { Write-Host "Aborted." -ForegroundColor Green; exit }
}

if ($DryRun) {
    Write-Host "=== DRY RUN ===" -ForegroundColor Cyan
    Write-Host "Would run: icacls `"$(Resolve-Path $Path)`" /reset"
    Write-Host "Would run: icacls `"$(Resolve-Path $Path)`" /grant `"SPECTRE\BDC:(OI)(CI)M`"
    Write-Host "Would run: icacls `"$(Resolve-Path $Path)`" /grant `"SYSTEM:(OI)(CI)M`"
    Write-Host "Would run: icacls `"$(Resolve-Path $Path)`" /grant `"Administrators:(OI)(CI)M`"
    exit
}

Write-Host "=== APPLYING MINIMAL ACL CHANGES ===" -ForegroundColor Green
icacls "$(Resolve-Path $Path)" /reset
icacls "$(Resolve-Path $Path)" /grant "SPECTRE\BDC:(OI)(CI)M"
icacls "$(Resolve-Path $Path)" /grant "SYSTEM:(OI)(CI)M"
icacls "$(Resolve-Path $Path)" /grant "Administrators:(OI)(CI)M"
Write-Host "`n=== COMPLETE ===" -ForegroundColor Green

