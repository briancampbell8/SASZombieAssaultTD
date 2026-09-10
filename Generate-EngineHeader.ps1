param (
    [Parameter(Mandatory=$false)]
    [string]$TargetDirectory = (Get-Location).Path
)

# 1. Gather all C# source files while explicitly ignoring binary and tracking build assets
$ExcludeFolders = @('obj', 'bin', 'packages', '.git', '.vs')
$FilterBlock = {
    param($CurrentItem) # Fixed: Changed from $File to $CurrentItem to prevent PowerShell optimization lockouts
    foreach ($Folder in $ExcludeFolders) {
        if ($CurrentItem.FullName -like "*\\$Folder\\*") { return $false }
    }
    return $true
}

$CSFiles = Get-ChildItem -Path $TargetDirectory -Filter "*.cs" -Recurse | Where-Object $FilterBlock

Write-Host "Found $($CSFiles.Count) total source files to verify..." -ForegroundColor Cyan

# Initialize processing metrics tracking counter
$ProcessedCount = 0

# 2. Process each file in the automation loop
foreach ($ItemFile in $CSFiles) {
    try {
        $FilePath = $ItemFile.FullName
        $Lines = Get-Content -Path $FilePath -Raw
        
        # Extract metadata metrics
        $FileName = $ItemFile.Name
        $RelativePath = $FilePath -replace '^(?:.*?[\\/])?(?=[^\\/]+[\\/][^\\/]+$)', ''
        
        $Namespace = "UnknownNamespace"
        if ($Lines -match 'namespace\s+([\w\.]+)') { $Namespace = $Matches[1] }
        
        $ClassName = "UnknownClass"
        if ($Lines -match '(?:public|internal|private|protected)?\s+(?:static\s+)?(?:class|struct|interface|enum)\s+(\w+)') { $ClassName = $Matches[1] }
        
        # Extract public/internal methods for responsibilities
        $Responsibilities = [System.Collections.Generic.List[string]]::new()
        $Matches_Methods = [regex]::Matches($Lines, '(?:public|internal)\s+(?:static\s+)?(?:\w+)\s+(\w+)\s*\(')
        foreach ($Match in $Matches_Methods) {
            $MethodName = $Match.Groups[1].Value
            if ($MethodName -notmatch '^(if|while|switch|for)$') {
                $Responsibilities.Add("//      - Provide public interface and handling execution for ${MethodName}().")
            }
        }
        
        if ($Responsibilities.Count -eq 0) {
            $Responsibilities.Add("//      - Provide deterministic engine pipeline handling execution logic.")
            $Responsibilities.Add("//      - Maintain runtime flow and process core thread states safely.")
        }
        
        # Format the Header Banner
        $HeaderString = @"
// ====================================================================================================
//  FILE: $FileName
//  PATH: $RelativePath
//  PROGRAM: $ClassName.cs
//  MODULE: Diagnostics & Engine Pipeline ($ClassName)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for $Namespace.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
$($Responsibilities -join "`r`n")
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================
param (
    [Parameter(Mandatory=$false)]
    [string]$TargetDirectory = (Get-Location).Path
)

# 1. Gather all C# source files while explicitly ignoring binary and tracking build assets
$ExcludeFolders = @('obj', 'bin', 'packages', '.git', '.vs')
$FilterBlock = {
    param($CurrentItem)
    foreach ($Folder in $ExcludeFolders) {
        if ($CurrentItem.FullName -like "*\\$Folder\\*") { return $false }
    }
    return $true
}

$CSFiles = Get-ChildItem -Path $TargetDirectory -Filter "*.cs" -Recurse | Where-Object $FilterBlock

Write-Host "Found $($CSFiles.Count) total source files to verify..." -ForegroundColor Cyan

# Initialize processing metrics tracking counter
$ProcessedCount = 0

# 2. Process each file in the automation loop
foreach ($ItemFile in $CSFiles) {
    try {
        $FilePath = $ItemFile.FullName
        $Lines = Get-Content -Path $FilePath -Raw

        # --------------------------------------------------------------------
        # SKIP IF MODERN HEADER ALREADY PRESENT
        # --------------------------------------------------------------------
        if ($Lines -match '^// ====================================================================================================') {
            Write-Host "Skipping (modern header exists) -> $($ItemFile.Name)" -ForegroundColor Yellow
            continue
        }

        # --------------------------------------------------------------------
        # STRIP HEADERS: remove /*...*/ or //... until first code line
        # --------------------------------------------------------------------
        $CleanedContent = New-Object System.Collections.Generic.List[string]
        $inBlockComment = $false
        $started = $false

        foreach ($line in ($Lines -split "`r`n")) {

            $trim = $line.Trim()

            if (-not $started) {

                # Multi-line comment start
                if ($trim.StartsWith("/*")) {
                    $inBlockComment = $true
                    continue
                }

                # Inside multi-line comment
                if ($inBlockComment) {
                    if ($trim.EndsWith("*/")) {
                        $inBlockComment = $false
                    }
                    continue
                }

                # Single-line comment
                if ($trim.StartsWith("//")) { continue }

                # Blank line
                if ([string]::IsNullOrWhiteSpace($trim)) { continue }

                # FIRST REAL CODE LINE FOUND
                $started = $true
            }

            # Preserve all lines after first code line
            $CleanedContent.Add($line)
        }

        # Extract metadata metrics
        $FileName = $ItemFile.Name
        $RelativePath = $FilePath -replace '^(?:.*?[\\/])?(?=[^\\/]+[\\/][^\\/]+$)', ''

        $Namespace = "UnknownNamespace"
        if ($Lines -match 'namespace\s+([\w\.]+)') { $Namespace = $Matches[1] }

        $ClassName = "UnknownClass"
        if ($Lines -match '(?:public|internal|private|protected)?\s+(?:static\s+)?(?:class|struct|interface|enum)\s+(\w+)') { $ClassName = $Matches[1] }

        # Extract public/internal methods for responsibilities
        $Responsibilities = [System.Collections.Generic.List[string]]::new()
        $Matches_Methods = [regex]::Matches($Lines, '(?:public|internal)\s+(?:static\s+)?(?:\w+)\s+(\w+)\s*\(')
        foreach ($Match in $Matches_Methods) {
            $MethodName = $Match.Groups[1].Value
            if ($MethodName -notmatch '^(if|while|switch|for)$') {
                $Responsibilities.Add("//      - Provide public interface and handling execution for ${MethodName}().")
            }
        }

        if ($Responsibilities.Count -eq 0) {
            $Responsibilities.Add("//      - Provide deterministic engine pipeline handling execution logic.")
            $Responsibilities.Add("//      - Maintain runtime flow and process core thread states safely.")
        }

        # Format the Header Banner
        $HeaderString = @"
// ====================================================================================================
//  FILE: $FileName
//  PATH: $RelativePath
//  PROGRAM: $ClassName.cs
//  MODULE: Diagnostics & Engine Pipeline ($ClassName)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for $Namespace.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
$($Responsibilities -join "`r`n")
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================


"@

        # Prepend header and overwrite file using exact UTF8 signature matching
        $FinalOutput = $HeaderString + ($CleanedContent -join "`r`n")
        [System.IO.File]::WriteAllText($FilePath, $FinalOutput, [System.Text.Encoding]::UTF8)

        # Increment our successful file operations counter metric
        $ProcessedCount++
        Write-Host "Processed Header -> $FileName" -ForegroundColor Green
    }
    catch {
        Write-Warning "Skipped file due to system access lock: $($ItemFile.Name)"
    }
}

# 3. Final metrics output execution report
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Yellow
Write-Host "Successfully added or updated headers on ($ProcessedCount) C# source files." -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Yellow
