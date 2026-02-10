# Root directory of the project
$Root = "E:\BDC\Projects\SASZombieAssaultTD"

# Output CSV path
$CsvOut = Join-Path $Root "SAS_TD_FileAudit.csv"

# Directories that contain C# files and must be debugged
$CsDirectories = @(
    "Engine\Core",
    "Engine\Entities",
    "Engine\Managers",
    "Engine\Platform",
    "Engine\Rendering",
    "Engine\Rendering\Debug",
    "Engine\Rendering\Zombies",
    "Engine\Scenes",
    "Engine\Systems",
    "Engine\Systems\Assets",
    "Engine\Systems\Diagnostics",
    "Engine\Systems\Gameplay",
    "Engine\Systems\Gameplay\Animation",
    "Engine\Systems\Gameplay\Zombies",
    "Engine\UI",
    "Engine\Tools",

    "Scripts\AI",
    "Scripts\AI\Behaviors",
    "Scripts\AI\Blackboard",
    "Scripts\Paths",
    "Scripts\TestScenes",
    "Scripts\Zombies",

    "Scenes\Game",
    "Scenes\MainMenu",
    "Scenes\Pause",
    "Scenes\Shared",

    "Systems\AI",
    "Systems\Collision",
    "Systems\Combat",
    "Systems\Economy",
    "Systems\Placement",
    "Systems\Survival",
    "Systems\Upgrades",
    "Systems\Waves",

    "Entities\Abilities",
    "Entities\AirSupport",
    "Entities\Environment",
    "Entities\Graphics",
    "Entities\Graphics\Animations",
    "Entities\Graphics\Backgrounds",
    "Entities\Graphics\Particles",
    "Entities\Graphics\Sprites",
    "Entities\Graphics\UIArt",
    "Entities\Projectiles",
    "Entities\Soldiers",
    "Entities\Towers",
    "Entities\Zombies"
)

# Resolve full paths
$FullPaths = $CsDirectories | ForEach-Object { Join-Path $Root $_ }

# Function to classify file content
function Get-FileStatus {
    param($Content)

    if ([string]::IsNullOrWhiteSpace($Content)) {
        return "Empty"
    }

    $Trimmed = $Content.Trim()

    # Remove comments
    $NoComments = ($Trimmed -replace '(?s)/\*.*?\*/', '' -replace '//.*', '').Trim()

    # If nothing left after removing comments
    if ([string]::IsNullOrWhiteSpace($NoComments)) {
        return "NearEmpty"
    }

    # Remove using statements and namespace declarations
    $Core = $NoComments -replace '^using\s+.*?;', '' -replace 'namespace\s+\w+(\.\w+)*\s*{?', ''
    $Core = $Core.Trim()

    # Detect empty class shells
    if ($Core -match 'class\s+\w+' -and $Core -notmatch '{.*?}') {
        return "NearEmpty"
    }

    # Detect class with no members
    if ($Core -match 'class\s+\w+' -and $Core -match '{\s*}') {
        return "NearEmpty"
    }

    # Detect files with structure but no logic
    if ($Core -match 'class\s+\w+' -and $Core -notmatch '\(' -and $Core -notmatch '=') {
        return "NeedsChanges"
    }

    return "Populated"
}

# Collect results
$Results = @()

foreach ($Dir in $FullPaths) {
    if (Test-Path $Dir) {
        Get-ChildItem -Path $Dir -Filter *.cs -Recurse | ForEach-Object {
            $Content = Get-Content $_.FullName -Raw
            $Status = Get-FileStatus -Content $Content

            $Results += [PSCustomObject]@{
                FileName  = $_.Name
                FilePath  = $_.FullName
                Subsystem = Split-Path $_.DirectoryName -Leaf
                Status    = $Status
                Notes     = ""
            }
        }
    }
}

# Sort and export
$Results |
    Sort-Object Subsystem, FileName |
    Export-Csv -Path $CsvOut -NoTypeInformation -Encoding UTF8

Write-Host "CSV audit complete:"
Write-Host $CsvOut