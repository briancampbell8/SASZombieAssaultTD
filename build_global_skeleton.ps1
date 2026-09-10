# 1. Target the absolute root Engine workspace directory
$projectRoot = Get-Location
$engineRoot = Join-Path $projectRoot "Engine"
$jsonPath = "metadata.json"

if (-not (Test-Path $engineRoot)) {
    Write-Host "[-] Error: 'Engine' directory not found at $projectRoot" -ForegroundColor Red
    return
}

# 2. Check for an existing metadata database to preserve manual edits
$existingDatabase = @{}
if (Test-Path $jsonPath) {
    Write-Host "[*] Existing metadata.json detected. Loading entries to preserve manual edits..." -ForegroundColor Cyan
    try {
        if ((Get-Item $jsonPath).Length -gt 10) {
            $existingDatabase = Get-Content $jsonPath -Raw | ConvertFrom-Json -AsHashtable
        }
    } catch {
        Write-Host "[!] Warning: Existing metadata file format is broken. Rebuilding structural table..." -ForegroundColor Yellow
    }
}

# 3. Gather all C# files recursively across all Engine subfolders
$csFiles = Get-ChildItem -Path $engineRoot -Filter "*.cs" -Recurse | Where-Object { 
    $_.Name -notlike "*.Designer.cs" -and $_.Name -notlike "*.g.cs" 
}

Write-Host "[*] Found $($csFiles.Count) C# source files. Safely mapping and assembling structural table arrays..." -ForegroundColor Cyan

$jsonOutput = New-Object System.Text.StringBuilder
[void]$jsonOutput.AppendLine("{")
$validEntries = New-Object System.Collections.Generic.List[string]

# Subsystem inference based on folder names
function Get-Subsystem([string]$path) {
    if ($path -match "/Waves/") { return "WaveDirector" }
    if ($path -match "/AI/") { return "AI" }
    if ($path -match "/Combat/") { return "Combat" }
    if ($path -match "/Rendering/") { return "Rendering" }
    if ($path -match "/Audio/") { return "Audio" }
    if ($path -match "/UI/") { return "UI" }
    if ($path -match "/Diagnostics/") { return "Diagnostics" }
    if ($path -match "/Serialization/") { return "Serialization" }
    return "Core"
}

# Role inference based on subsystem
function Get-Role([string]$className, [string]$subsystem) {
    switch ($subsystem) {
        "WaveDirector" { return "Load, validate, and construct wave definitions for the WaveDirector subsystem." }
        "AI"           { return "Provide deterministic AI behavior, decision logic, or state evaluation." }
        "Combat"       { return "Manage combat interactions, damage flow, or projectile behavior." }
        "Rendering"    { return "Provide rendering logic, draw calls, batching, or GPU resource management." }
        "Audio"        { return "Manage audio playback, mixing, or spatial sound behavior." }
        "UI"           { return "Provide UI layout, interaction logic, or HUD rendering." }
        "Diagnostics"  { return "Provide logging, profiling, or diagnostic instrumentation." }
        "Serialization"{ return "Provide serialization, deserialization, or data transformation logic." }
        default        { return "Encapsulate core engine behavior for the $className module." }
    }
}

# Improved method regex (handles generics, async, attributes, modifiers)
$methodPattern = '(?m)^\s*(?:

\[.*?\]

\s*)*(?:public|protected)\s+(?:virtual|override|static|async|sealed|partial)?\s*[^\s]+\s+([A-Za-z0-9_]+)\s*\('

for ($i = 0; $i -lt $csFiles.Count; $i++) {
    $file = $csFiles[$i]
    $fileName = $file.Name
    
    # Calculate uniform relative paths
    $relativeFilePath = $file.FullName.Replace($projectRoot.Path, "").TrimStart([System.IO.Path]::DirectorySeparatorChar).Replace('\', '/')
    $relativeFolderPath = [System.IO.Path]::GetDirectoryName($relativeFilePath).Replace('\', '/') + "/"
    if (-not $relativeFolderPath.StartsWith("./")) { $relativeFolderPath = "./" + $relativeFolderPath }

    # Safe check to skip empty or blank file allocations cleanly
    $fileContent = Get-Content $file.FullName -Raw
    if ([string]::IsNullOrWhiteSpace($fileContent)) { continue }

    # PRESERVATION CHECK: If this file entry already exists, reuse it exactly as-is
    if ($existingDatabase.Count -gt 0 -and $existingDatabase.ContainsKey($relativeFilePath)) {
        $existingNode = $existingDatabase[$relativeFilePath]
        
        $respStrings = New-Object System.Collections.Generic.List[string]
        foreach ($r in $existingNode.responsibilities) { 
            $respStrings.Add("      `"$($r.Replace('\', '\\').Replace('"', '\"'))`"") 
        }
        $respJsonBlock = $respStrings -join ",`r`n"

        $entrySb = New-Object System.Text.StringBuilder
        [void]$entrySb.AppendLine("  `"$relativeFilePath`": {")
        [void]$entrySb.AppendLine("    `"engine_action`": `"$($existingNode.engine_action)`",")
        [void]$entrySb.AppendLine("    `"moduleName`": `"$($existingNode.moduleName)`",")
        [void]$entrySb.AppendLine("    `"folderPath`": `"$($existingNode.folderPath)`",")
        [void]$entrySb.AppendLine("    `"role`": `"$($existingNode.role.Replace('\', '\\').Replace('"', '\"'))`",")
        [void]$entrySb.AppendLine("    `"responsibilities`": [")
        if ($respStrings.Count -gt 0) { [void]$entrySb.AppendLine($respJsonBlock) }
        [void]$entrySb.AppendLine("    ]")
        [void]$entrySb.Append("  }")
        
        $validEntries.Add($entrySb.ToString())
        continue
    }

    # NEW FILE DISCOVERY WORKFLOW
    $className = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
    $subsystem = Get-Subsystem $relativeFolderPath
    $moduleName = $subsystem

    # Extract responsibilities from method names
    $methodMatches = [regex]::Matches($fileContent, $methodPattern)
    $respList = New-Object System.Collections.Generic.List[string]

    foreach ($match in $methodMatches) {
        $mName = $match.Groups[1].Value
        if ($mName -ne $className) {
            $respList.Add("Provide $mName() behavior for the $moduleName subsystem.")
        }
    }

    if ($respList.Count -eq 0) {
        $respList.Add("Provide core functionality for the $moduleName subsystem.")
    }

    $role = Get-Role $className $subsystem

    # Format the clean, un-nested layout block entries accurately
    $entrySb = New-Object System.Text.StringBuilder
    [void]$entrySb.AppendLine("  `"$relativeFilePath`": {")
    [void]$entrySb.AppendLine("    `"engine_action`": `"add`",")
    [void]$entrySb.AppendLine("    `"moduleName`": `"$moduleName`",")
    [void]$entrySb.AppendLine("    `"folderPath`": `"$relativeFolderPath`",")
    [void]$entrySb.AppendLine("    `"role`": `"$($role.Replace('"','\"'))`",")
    [void]$entrySb.AppendLine("    `"responsibilities`": [")

    for ($j = 0; $j -lt $respList.Count; $j++) {
        $comma = if ($j -lt $respList.Count - 1) { "," } else { "" }
        [void]$entrySb.AppendLine("      `"$($respList[$j].Replace('"','\"'))`"$comma")
    }

    [void]$entrySb.AppendLine("    ]")
    [void]$entrySb.Append("  }")
    $validEntries.Add($entrySb.ToString())
}

# FIXED JOIN PIPELINE
$joinedEntries = $validEntries -join ",`r`n"
[void]$jsonOutput.AppendLine($joinedEntries)
[void]$jsonOutput.AppendLine("}")

# Write out the master skeleton file
[System.IO.File]::WriteAllText($jsonPath, $jsonOutput.ToString(), [System.Text.Encoding]::UTF8)
Write-Host "[✓] Master metadata registry table built error-free: metadata.json" -ForegroundColor Green
