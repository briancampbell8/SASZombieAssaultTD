# Script to remove all Unity-related preprocessor directives from all C# files

Write-Host "Removing all Unity preprocessor directives from C# files..."

# Get all C# files
$csFiles = Get-ChildItem -Path "E:\BDC\Projects\SASZombieAssaultTD" -Filter "*.cs" -Recurse

$processedFiles = 0
$totalFiles = $csFiles.Count

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    
    # Remove all Unity-related preprocessor directives
    $content = $content -replace "#if !DISABLE_UNITY`r?`n", ""
    $content = $content -replace "#endif`r?`n", ""
    $content = $content -replace "`r?`n#endif", ""
    $content = $content -replace "#endif", ""
    
    # Only write back if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Processed: $($file.FullName)"
        $processedFiles++
    }
}

Write-Host "Complete! Processed $processedFiles out of $totalFiles files."
