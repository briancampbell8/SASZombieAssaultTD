# Update-ProjectFile.ps1
# PowerShell script to add all missing files to the project file

Write-Host "Updating project file with missing files..." -ForegroundColor Green

$projectFile = "E:\BDC\Projects\SASZombieAssaultTD\SASZombieAssaultTD.csproj"
$projectRoot = "E:\BDC\Projects\SASZombieAssaultTD"

# Read the current project file
$projectContent = Get-Content $projectFile -Raw

# Define files to add
$filesToAdd = @(
    # Core Interfaces
    "Engine\Core\Interfaces\IManager.cs",
    "Engine\Core\Interfaces\ISystemRegistry.cs", 
    "Engine\Core\Interfaces\IGameStateMachine.cs",
    "Engine\Core\Interfaces\IDebugRenderer.cs",
    
    # Core Managers
    "Engine\Core\Managers\BaseManager.cs",
    "Engine\Core\Managers\SystemManager.cs",
    "Engine\Core\Managers\UpdateManager.cs",
    "Engine\Core\Managers\RenderManager.cs",
    "Engine\Core\Managers\InputManager.cs",
    
    # Core Registry
    "Engine\Core\Registry\SystemRegistry.cs",
    
    # Core Timing
    "Engine\Core\Timing\TimingModule.cs",
    "Engine\Core\Timing\FrameDiagnostics.cs",
    
    # Core Input
    "Engine\Core\Input\InputModule.cs",
    
    # Core Diagnostics
    "Engine\Core\Diagnostics\ManagerDiagnostics.cs",
    
    # Core Logging
    "Engine\Core\Logging\DebugLogger.cs",
    
    # Animation System
    "Engine\Animation\AnimationController.cs",
    "Engine\Animation\AnimationClip.cs",
    "Engine\Animation\AnimationParameters.cs",
    "Engine\Animation\AnimationStateMachine.cs",
    "Engine\Animation\AnimationTrack.cs",
    "Engine\Animation\AnimationStateInspector.cs",
    "Engine\Animation\AnimationPerformanceAnalyzer.cs",
    "Engine\Animation\AnimationTransitionDebug.cs",
    
    # Animation Diagnostics
    "Engine\Animation\Diagnostics\AnimationDiagnostics.cs",
    
    # Animation Visualization
    "Engine\Animation\Visualization\AnimationStateVisualization.cs",
    
    # Animation Events
    "Engine\Animation\Events\AnimationEvent.cs",
    "Engine\Animation\Events\AnimationEventHandler.cs",
    "Engine\Animation\Events\AnimationEventValidator.cs",
    
    # Animation States
    "Engine\Animation\States\AnimationState.cs",
    "Engine\Animation\States\AnimationStateMachine.cs",
    
    # Animation BlendTrees
    "Engine\Animation\BlendTrees\AnimationBlendTree.cs",
    "Engine\Animation\BlendTrees\BlendTree1D.cs",
    "Engine\Animation\BlendTrees\BlendTree2D.cs",
    
    # Core Components
    "Engine\Components\TransformComponent.cs",
    "Engine\Components\PhysicsComponent.cs",
    "Engine\Components\SpriteComponent.cs",
    "Engine\Components\CollisionComponent.cs",
    "Engine\Components\StatsComponent.cs",
    "Engine\Components\InventoryComponent.cs",
    "Engine\Components\ParticleEmitterComponent.cs",
    
    # Core Systems
    "Engine\Systems\CollisionSystem.cs",
    "Engine\Systems\PhysicsSystem.cs",
    "Engine\Systems\RenderingSystem.cs",
    
    # Audio System
    "Engine\Audio\AudioEngine.cs",
    "Engine\Audio\SoundEffect.cs",
    "Engine\Audio\MusicTrack.cs",
    
    # Rendering
    "Engine\Rendering\RenderDiagnostics.cs"
)

# Find the closing </Project> tag
$projectEndIndex = $projectContent.LastIndexOf("</Project>")
if ($projectEndIndex -eq -1) {
    Write-Host "Error: Could not find closing </Project> tag" -ForegroundColor Red
    exit 1
}

# Build the new content
$newContent = $projectContent.Substring(0, $projectEndIndex)

# Add missing files section
$newContent += @"

  <!-- ========================================================= -->
  <!-- CORE INTERFACES                                          -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core interfaces
$coreInterfaces = $filesToAdd | Where-Object { $_ -like "Engine\Core\Interfaces\*" }
foreach ($file in $coreInterfaces) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE MANAGERS                                             -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core managers
$coreManagers = $filesToAdd | Where-Object { $_ -like "Engine\Core\Managers\*" }
foreach ($file in $coreManagers) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE REGISTRY                                             -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core registry
$coreRegistry = $filesToAdd | Where-Object { $_ -like "Engine\Core\Registry\*" }
foreach ($file in $coreRegistry) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE TIMING                                              -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core timing
$coreTiming = $filesToAdd | Where-Object { $_ -like "Engine\Core\Timing\*" }
foreach ($file in $coreTiming) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE INPUT                                               -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core input
$coreInput = $filesToAdd | Where-Object { $_ -like "Engine\Core\Input\*" }
foreach ($file in $coreInput) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE DIAGNOSTICS                                         -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core diagnostics
$coreDiagnostics = $filesToAdd | Where-Object { $_ -like "Engine\Core\Diagnostics\*" }
foreach ($file in $coreDiagnostics) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE LOGGING                                              -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core logging
$coreLogging = $filesToAdd | Where-Object { $_ -like "Engine\Core\Logging\*" }
foreach ($file in $coreLogging) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION SYSTEM                                          -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation system
$animationSystem = $filesToAdd | Where-Object { $_ -like "Engine\Animation\*" -and $_ -notlike "*Diagnostics*" -and $_ -notlike "*Visualization*" -and $_ -notlike "*Events*" -and $_ -notlike "*States*" -and $_ -notlike "*BlendTrees*" }
foreach ($file in $animationSystem) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION DIAGNOSTICS                                     -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation diagnostics
$animationDiagnostics = $filesToAdd | Where-Object { $_ -like "Engine\Animation\Diagnostics\*" }
foreach ($file in $animationDiagnostics) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION VISUALIZATION                                   -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation visualization
$animationVisualization = $filesToAdd | Where-Object { $_ -like "Engine\Animation\Visualization\*" }
foreach ($file in $animationVisualization) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION EVENTS                                          -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation events
$animationEvents = $filesToAdd | Where-Object { $_ -like "Engine\Animation\Events\*" }
foreach ($file in $animationEvents) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION STATES                                          -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation states
$animationStates = $filesToAdd | Where-Object { $_ -like "Engine\Animation\States\*" }
foreach ($file in $animationStates) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- ANIMATION BLEND TREES                                     -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add animation blend trees
$animationBlendTrees = $filesToAdd | Where-Object { $_ -like "Engine\Animation\BlendTrees\*" }
foreach ($file in $animationBlendTrees) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE COMPONENTS                                           -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core components
$coreComponents = $filesToAdd | Where-Object { $_ -like "Engine\Components\*" }
foreach ($file in $coreComponents) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- CORE SYSTEMS                                              -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add core systems
$coreSystems = $filesToAdd | Where-Object { $_ -like "Engine\Systems\*" }
foreach ($file in $coreSystems) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- AUDIO SYSTEM                                              -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add audio system
$audioSystem = $filesToAdd | Where-Object { $_ -like "Engine\Audio\*" }
foreach ($file in $audioSystem) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>

  <!-- ========================================================= -->
  <!-- RENDERING                                                -->
  <!-- ========================================================= -->
  <ItemGroup>
"@

# Add rendering
$rendering = $filesToAdd | Where-Object { $_ -like "Engine\Rendering\*" }
foreach ($file in $rendering) {
    $newContent += "    <Compile Include=`"$file`" />`n"
}

$newContent += @"  </ItemGroup>
"@

# Add the closing tag
$newContent += "</Project>"

# Write the updated project file
Set-Content $projectFile $newContent -NoNewline

Write-Host "Project file updated successfully!" -ForegroundColor Green
Write-Host "Added $($filesToAdd.Count) files to the project." -ForegroundColor Yellow
Write-Host "Run 'dotnet build' to verify the fixes." -ForegroundColor Cyan
