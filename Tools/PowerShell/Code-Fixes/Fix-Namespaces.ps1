# Fix-Namespaces.ps1
# PowerShell script to fix namespace mismatches in the project

Write-Host "Starting namespace fixes..." -ForegroundColor Green

# Define the project root
$projectRoot = "E:\BDC\Projects\SASZombieAssaultTD"

# Define namespace mappings
$namespaceFixes = @{
    # Core Managers namespace fix
    "Engine.Managers" = "SASZombieAssaultTD.Engine.Core.Managers"
    
    # Core Input namespace fix  
    "Engine.Input" = "SASZombieAssaultTD.Engine.Core.Input"
    
    # Core Timing namespace fix
    "Engine.Timing" = "SASZombieAssaultTD.Engine.Core.Timing"
    
    # Core Diagnostics namespace fix
    "Engine.Diagnostics" = "SASZombieAssaultTD.Engine.Core.Diagnostics"
    
    # Core Interfaces namespace fix
    "Engine.Interfaces" = "SASZombieAssaultTD.Engine.Core.Interfaces"
    
    # Animation Diagnostics namespace fix
    "Engine.Animation.Diagnostics" = "SASZombieAssaultTD.Engine.Animation.Diagnostics"
    
    # Animation Visualization namespace fix
    "Engine.Animation.Visualization" = "SASZombieAssaultTD.Engine.Animation.Visualization"
}

# Files to fix with their actual locations
$filesToFix = @(
    "$projectRoot\Engine\Core\Managers\BaseManager.cs"
    "$projectRoot\Engine\Core\Managers\SystemManager.cs"
    "$projectRoot\Engine\Core\Managers\UpdateManager.cs"
    "$projectRoot\Engine\Core\Managers\RenderManager.cs"
    "$projectRoot\Engine\Core\Managers\InputManager.cs"
    "$projectRoot\Engine\Core\Input\InputModule.cs"
    "$projectRoot\Engine\Core\Timing\TimingModule.cs"
    "$projectRoot\Engine\Core\Timing\FrameDiagnostics.cs"
    "$projectRoot\Engine\Core\Diagnostics\ManagerDiagnostics.cs"
    "$projectRoot\Engine\Core\Interfaces\IManager.cs"
    "$projectRoot\Engine\Core\Interfaces\ISystemRegistry.cs"
    "$projectRoot\Engine\Core\Interfaces\IGameStateMachine.cs"
    "$projectRoot\Engine\Core\Interfaces\IDebugRenderer.cs"
    "$projectRoot\Engine\Animation\Diagnostics\AnimationDiagnostics.cs"
    "$projectRoot\Engine\Animation\Visualization\AnimationStateVisualization.cs"
)

Write-Host "Processing $($filesToFix.Count) files..." -ForegroundColor Yellow

# Process each file
foreach ($file in $filesToFix) {
    if (Test-Path $file) {
        Write-Host "Processing: $file" -ForegroundColor Cyan
        
        try {
            # Read the file content
            $content = Get-Content $file -Raw
            
            # Apply namespace fixes
            foreach ($oldNamespace in $namespaceFixes.Keys) {
                $newNamespace = $namespaceFixes[$oldNamespace]
                
                # Replace namespace declarations
                $content = $content -replace "namespace $oldNamespace", "namespace $newNamespace"
                
                # Replace using statements
                $content = $content -replace "using $oldNamespace", "using $newNamespace"
            }
            
            # Write the fixed content back to the file
            Set-Content $file $content -NoNewline
            
            Write-Host "  ✓ Fixed namespaces in $file" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Error processing $file : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ⚠ File not found: $file" -ForegroundColor Yellow
    }
}

# Also fix the new programs we created
$newPrograms = @(
    "$projectRoot\Engine\Animation\AnimationStateInspector.cs"
    "$projectRoot\Engine\Animation\AnimationPerformanceAnalyzer.cs"
    "$projectRoot\Engine\Animation\AnimationTransitionDebug.cs"
)

Write-Host "Processing new programs..." -ForegroundColor Yellow

foreach ($file in $newPrograms) {
    if (Test-Path $file) {
        Write-Host "Processing: $file" -ForegroundColor Cyan
        
        try {
            # Read the file content
            $content = Get-Content $file -Raw
            
            # Fix animation namespace
            $content = $content -replace "namespace SASZombieAssaultTD.Engine", "namespace SASZombieAssaultTD.Engine.Animation"
            
            # Write the fixed content back to the file
            Set-Content $file $content -NoNewline
            
            Write-Host "  ✓ Fixed namespace in $file" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Error processing $file : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ⚠ File not found: $file" -ForegroundColor Yellow
    }
}

Write-Host "Namespace fixes completed!" -ForegroundColor Green
Write-Host "Run 'dotnet build' to verify the fixes." -ForegroundColor Cyan
