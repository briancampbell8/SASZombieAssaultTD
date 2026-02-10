$root = "E:\BDC\Projects\SASZombieAssaultTD"

Write-Host "=== PowerShell Repair Pass Starting ===" -ForegroundColor Cyan

Get-ChildItem -Path $root -Recurse -Filter *.ps1 | ForEach-Object {
    $file = $_.FullName
    $backup = "$file.bak"

    # Backup original
    Copy-Item $file $backup -Force

    $content = Get-Content $file -Raw

    # Replace quadruple quotes with single quotes
    $fixed = $content -replace '"', '"'

    # Replace triple quotes (rare cases)
    $fixed = $fixed -replace '"', '"'

    # Replace doubled quotes (safe for PS1)
    $fixed = $fixed -replace '"', '"'

    # Write repaired file
    Set-Content -Path $file -Value $fixed -Encoding UTF8

    Write-Host "Repaired: $file" -ForegroundColor Green
}

Write-Host "=== Repair Complete ===" -ForegroundColor Cyan
Write-Host "Backups created as *.bak next to each file." -ForegroundColor Yellow
