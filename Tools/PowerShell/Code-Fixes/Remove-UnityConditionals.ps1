# Script to remove Unity preprocessor directives from all C# files

Write-Host "Removing Unity preprocessor directives from C# files..."

# Get all C# files
$csFiles = Get-ChildItem -Path "E:\BDC\Projects\SASZombieAssaultTD" -Filter "*.cs" -Recurse

$processedFiles = 0
$totalFiles = $csFiles.Count

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    
    # Remove #if !DISABLE_UNITY at the beginning of files
    $content = $content -replace "^#if !DISABLE_UNITY`r?`n", ""
    
    # Remove #endif at the end of files
    $content = $content -replace "`r?`n#endif$", ""
    
    # Only write back if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Processed: $($file.FullName)"
        $processedFiles++
    }
}

Write-Host "Complete! Processed $processedFiles out of $totalFiles files."
