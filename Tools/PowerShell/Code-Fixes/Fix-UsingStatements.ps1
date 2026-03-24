# Fix-UsingStatements.ps1
# PowerShell script to fix using statements to match corrected namespaces

Write-Host "Starting using statement fixes..." -ForegroundColor Green

# Define the project root
$projectRoot = "E:\BDC\Projects\SASZombieAssaultTD"

# Define using statement mappings
$usingFixes = @{
    # Core Managers
    "using SASZombieAssaultTD.Engine.Managers" = "using SASZombieAssaultTD.Engine.Core.Managers"
    
    # Core Input
    "using SASZombieAssaultTD.Engine.Input" = "using SASZombieAssaultTD.Engine.Core.Input"
    
    # Core Timing
    "using SASZombieAssaultTD.Engine.Timing" = "using SASZombieAssaultTD.Engine.Core.Timing"
    
    # Core Diagnostics
    "using SASZombieAssaultTD.Engine.Diagnostics" = "using SASZombieAssaultTD.Engine.Core.Diagnostics"
    
    # Core Interfaces
    "using SASZombieAssaultTD.Engine.Interfaces" = "using SASZombieAssaultTD.Engine.Core.Interfaces"
    
    # Animation Diagnostics
    "using SASZombieAssaultTD.Engine.Animation.Diagnostics" = "using SASZombieAssaultTD.Engine.Animation.Diagnostics"
    
    # Animation Visualization
    "using SASZombieAssaultTD.Engine.Animation.Visualization" = "using SASZombieAssaultTD.Engine.Animation.Visualization"
}

# Files that need using statement fixes
$filesToFix = @(
    "$projectRoot\Engine\GameRoot.cs"
    "$projectRoot\Engine\Core\GameLoop.cs"
    "$projectRoot\Engine\Animation\AnimationDebugTools.cs"
)

Write-Host "Processing $($filesToFix.Count) files for using statement fixes..." -ForegroundColor Yellow

# Process each file
foreach ($file in $filesToFix) {
    if (Test-Path $file) {
        Write-Host "Processing: $file" -ForegroundColor Cyan
        
        try {
            # Read the file content
            $content = Get-Content $file -Raw
            
            # Apply using statement fixes
            foreach ($oldUsing in $usingFixes.Keys) {
                $newUsing = $usingFixes[$oldUsing]
                
                # Replace using statements
                $content = $content -replace $oldUsing, $newUsing
            }
            
            # Add missing using statements for AnimationDebugTools.cs
            if ($file -like "*AnimationDebugTools.cs") {
                if ($content -notmatch "using SASZombieAssaultTD.Engine.Animation") {
                    $content = $content -replace "(using System.Collections.Generic;)", "`$1`nusing SASZombieAssaultTD.Engine.Animation;"
                }
            }
            
            # Add missing using statements for GameRoot.cs
            if ($file -like "*GameRoot.cs") {
                if ($content -notmatch "using SASZombieAssaultTD.Engine.Core.Registry") {
                    $content = $content -replace "(using SASZombieAssaultTD.Engine.Rendering;)", "`$1`nusing SASZombieAssaultTD.Engine.Core.Registry;"
                }
            }
            
            # Write the fixed content back to the file
            Set-Content $file $content -NoNewline
            
            Write-Host "  ✓ Fixed using statements in $file" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Error processing $file : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ⚠ File not found: $file" -ForegroundColor Yellow
    }
}

Write-Host "Using statement fixes completed!" -ForegroundColor Green
Write-Host "Run 'dotnet build' to verify the fixes." -ForegroundColor Cyan
