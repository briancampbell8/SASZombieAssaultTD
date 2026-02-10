# Generate-ProjectStructure.ps1
# Produces a clean, canonical ProjectStructure.md

$OutputFile = "ProjectStructure.md"
$Timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")

# Folders to exclude entirely
$ExcludeDirs = @(
    "bin",
    "obj",
    ".vs",
    ".vscode",
    ".git",
    ".idea",
    ".slnx",
    "ProjectEvaluation",
    "CopilotIndices",
    "DesignTimeBuild",
    "FileContentIndex",
    "v18",
    "sessions",
    "runtimes",
    "cs",
    "de","es","fr","hu","it","ja","ko","nl","pl","pt-BR","pt-PT","ru","sv","tr","zh-Hans","zh-Hant"
)

# Files to exclude
$ExcludeFiles = @(
    "*.dll",
    "*.pdb",
    "*.json",
    "*.cache",
    "*.bin",
    "*.idx",
    "*.db",
    "*.exe",
    "*.config",
    "*.editorconfig"
)

# Header
@"
# Project Structure Snapshot (Auto-Generated)
Generated: $Timestamp

## Directory Tree
"@ | Out-File $OutputFile -Encoding UTF8

# Recursive directory listing with filtering
function Write-Tree {
    param(
        [string]$Path,
        [int]$Depth = 0
    )

    $Indent = " " * ($Depth * 2)

    # List directories
    Get-ChildItem $Path -Directory |
        Where-Object { $ExcludeDirs -notcontains $_.Name } |
        Sort-Object Name |
        ForEach-Object {
            "$Indent- $($_.Name)/" | Out-File $OutputFile -Append -Encoding UTF8
            Write-Tree -Path $_.FullName -Depth ($Depth + 1)
        }

    # List files
    Get-ChildItem $Path -File |
        Where-Object {
            foreach ($pattern in $ExcludeFiles) {
                if ($_.Name -like $pattern) { return $false }
            }
            return $true
        } |
        Sort-Object Name |
        ForEach-Object {
            "$Indent- $($_.Name)" | Out-File $OutputFile -Append -Encoding UTF8
        }
}

# Start at project root
Write-Tree -Path $PWD.Path
