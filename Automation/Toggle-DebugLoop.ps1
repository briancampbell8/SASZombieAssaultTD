# =====================================================================
# Toggle-DebugLoop.ps1
# Inserts or removes the temporary 4-iteration debug loop limiter
# inside GameLoop.cs. This script is safe, reversible, and idempotent.
# =====================================================================

$target = "E:\BDC\Projects\SASZombieAssaultTD\Engine\Core\GameLoop.cs"

# Comment banners used to detect the block
$startTag = "// ============================================================================ DEBUG MODE: TEMPORARY LOOP LIMITER"
$endTag   = "// ============================================================================ END DEBUG MODE: TEMPORARY LOOP LIMITER"

# Read file
$content = Get-Content $target

# Check if block already exists
$blockExists = $content -contains $startTag

if (-not $blockExists) {

    Write-Host "Debug block not found. Inserting debug loop limiter..."

    # Build the block as an array of lines
    $debugBlock = @(
        $startTag
        "// Purpose: Restrict main loop to 4 iterations for console-based debugging."
        "// This block remains active until DebugSession-0001 is fully completed."
        "// DO NOT REMOVE until baseline engine initialization is verified."
        "int debugIterations = 0;"
        "
        "while (debugIterations < 4)"
        "{"
        '    Console.WriteLine("Loop iteration: {0}", debugIterations + 1);'
        "    Update();"
        "    Render();"
        "    debugIterations++;"
        "}"
        $endTag
    )

    # Insert block after the opening brace of Run()
    $newContent = @()
    $inserted = $false

    foreach ($line in $content) {
        $newContent += $line

        if (-not $inserted -and $line -match "public void Run\(\)") {
            # Wait for the next line containing "{"
            continue
        }

        if (-not $inserted -and $line.Trim() -eq "{") {
            $newContent += $debugBlock
            $inserted = $true
        }
    }

    $newContent | Set-Content $target -Encoding ASCII
    Write-Host "Debug loop limiter inserted successfully."
    exit
}

# If block exists, ask whether to remove it
Write-Host "Debug block detected."
$answer = Read-Host "Is debugging complete (yes/no)"

if ($answer -eq "yes") {

    Write-Host "Removing debug block..."

    $newContent = @()
    $skip = $false

    foreach ($line in $content) {
        if ($line -eq $startTag) {
            $skip = $true
            continue
        }
        if ($line -eq $endTag) {
            $skip = $false
            continue
        }
        if (-not $skip) {
            $newContent += $line
        }
    }

    $newContent | Set-Content $target -Encoding ASCII
    Write-Host "Debug block removed successfully."

} else {
    Write-Host "Debug block left in place."
}
