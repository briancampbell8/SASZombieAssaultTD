# Script to replace all Vector2 with Vector3 throughout the codebase
Write-Host "Replacing all Vector2 with Vector3 in C# files..."

# Get all C# files
$csFiles = Get-ChildItem -Path "E:\BDC\Projects\SASZombieAssaultTD" -Filter "*.cs" -Recurse

$processedFiles = 0
$totalFiles = $csFiles.Count

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    
    # Replace all Vector2 with Vector3
    $content = $content -replace "Vector2", "Vector3"
    
    # Only write back if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Processed: $($file.FullName)"
        $processedFiles++
    }
}

Write-Host "Complete! Processed $processedFiles out of $totalFiles files."
