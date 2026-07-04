# 1. Verify and read local metadata table data
$jsonPath = "metadata.json"
if (-not (Test-Path $jsonPath)) {
    Write-Host "[-] Error: '$jsonPath' master table file is missing inside project root." -ForegroundColor Red
    return
}

$database = Get-Content $jsonPath -Raw | ConvertFrom-Json
$projectRoot = Get-Location

# 2. Iterate through each file node registry mapping inside the table
foreach ($relativeFilePath in $database.psobject.Properties.Name) {
    # Resolve the absolute physical path dynamically based on your environment
    $absolutePath = Join-Path $projectRoot ($relativeFilePath.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
    
    if (-not (Test-Path $absolutePath)) {
        Write-Host "[!] Target missing context path loop skipped: $relativeFilePath" -ForegroundColor Yellow
        continue
    }

    $data = $database.$relativeFilePath
    $action = $data.engine_action.ToLower()

    if ($action -eq "skip") {
        Write-Host "[→] Skipping standard compliant entry: $relativeFilePath" -ForegroundColor Cyan
        continue
    }

    # Read current source file raw data string streams
    $content = Get-Content $absolutePath -Raw

    # 3. Process Overwrite Operations: Strip old comment structures cleanly
    if ($action -eq "overwrite") {
        Write-Host "[⇄] Stripping previous malformed layout header from: $relativeFilePath" -ForegroundColor Magenta
        if ($content -match '^(?s)\s*//\s*=+.*?//\s*=+[\r\n]+') {
            $content = $content -replace '^(?s)\s*//\s*=+.*?//\s*=+[\r\n]+', ''
        } else {
            $lines = $content -split '\r?\n'
            $firstCodeIndex = 0
            for ($i = 0; $i -lt $lines.Length; $i++) {
                if ($lines[$i].Trim().StartsWith("//") -or [string]::IsNullOrWhiteSpace($lines[$i])) {
                    continue;
                }
                $firstCodeIndex = $i
                break
            }
            if ($firstCodeIndex -gt 0) {
                $content = ($lines[$firstCodeIndex..($lines.Length - 1)]) -join "`r`n"
            }
        }
    }

    # 4. Process Missing Operations Safeguards: Stop double injections
    if ($action -eq "add") {
        if ($content -like "*FILE:*" -and $content -like "*ROLE:*") {
            Write-Host "[→] Block layout already discovered on target, skipping file: $relativeFilePath" -ForegroundColor Cyan
            continue
        }
    }

    # 5. Format Text Fields Natively (Converts string "\n" into true PowerShell line breaks)
    $cleanRole = $data.role -replace '\\r\\n', "`r`n" -replace '\\n', "`r`n"
    $formattedRole = $cleanRole -replace "`r`n", "`r`n//      "
    $pureFileName = [System.IO.Path]::GetFileName($absolutePath)

    # 6. Format the Precise Custom Multi-Line Output String
    $header = New-Object System.Text.StringBuilder
    [void]$header.AppendLine("// ====================================================================================================")
    [void]$header.AppendLine("//  FILE: $pureFileName")
    [void]$header.AppendLine("//  PATH: $($data.folderPath)") # Outputs correct subfolder path dynamically
    [void]$header.AppendLine("//  MODULE: $($data.moduleName)")
    [void]$header.AppendLine("//")
    [void]$header.AppendLine("//  ROLE:")
    [void]$header.AppendLine("//      $formattedRole")
    [void]$header.AppendLine("//")
    [void]$header.AppendLine("//  RESPONSIBILITIES:")
    foreach ($resp in $data.responsibilities) {
        $cleanResp = $resp -replace '\\r\\n', "`r`n" -replace '\\n', "`r`n" -replace "`r`n", "`r`n//      - "
        [void]$header.AppendLine("//      - $cleanResp")
    }
    [void]$header.AppendLine("//")
    [void]$header.AppendLine("//  NON-RESPONSIBILITIES:")
    [void]$header.AppendLine("//      - Low-level data persistence or file serialization.")
    [void]$header.AppendLine("//")
    [void]$header.AppendLine("//  NOTES:")
    [void]$header.AppendLine("//      Auto-generated structure verified locally via file state scripts.")
    [void]$header.AppendLine("// ====================================================================================================")

    # 7. Prepend block data directly into file on disk line 1, safeguarding code structure underneath
    Set-Content $absolutePath ($header.ToString() + $content.TrimStart()) -Encoding UTF8
    Write-Host "[✓] Successfully updated: $relativeFilePath" -ForegroundColor Green
}
