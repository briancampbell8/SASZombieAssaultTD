$root = "E:\BDC\Projects\SASZombieAssaultTD"

Write-Host "=== PowerShell Verification Pass ===" -ForegroundColor Cyan
Write-Host ""

# Only patterns that indicate REAL corruption
$patterns = @(
    '""""',              # quadruple quotes
    '"""',               # triple quotes
    '"[^"]*$' ,          # unterminated quote
    '\\+"'               # stray backslash before quote
)

$issuesFound = $false

Get-ChildItem -Path $root -Recurse -Filter *.ps1 | ForEach-Object {
    $file = $_.FullName
    $lines = Get-Content $file

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]

        foreach ($pattern in $patterns) {
            if ($line -match $pattern) {
                if (-not $issuesFound) {
                    Write-Host "Issues detected:" -ForegroundColor Yellow
                    $issuesFound = $true
                }

                Write-Host "[$file] Line $($i+1):" -ForegroundColor Red
                Write-Host "    $line" -ForegroundColor Gray
                break
            }
        }
    }
}

if (-not $issuesFound) {
    Write-Host "All PowerShell files are clean." -ForegroundColor Green
}