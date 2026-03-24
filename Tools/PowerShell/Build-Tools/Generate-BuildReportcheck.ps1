param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD",
    [string]$OutputCsv = ".\StrayCsFiles.csv"
)

Write-Host "Scanning for stray .cs files under: $ProjectRoot"
Write-Host "Output CSV: $OutputCsv"

# 1. Get all .cs files recursively
$allFiles = Get-ChildItem -Path $ProjectRoot -Recurse -Filter *.cs

# 2. Load .csproj includes (explicit or wildcard)
$csproj = Get-ChildItem -Path $ProjectRoot -Recurse -Filter *.csproj | Select-Object -First 1
$projXml = [xml](Get-Content $csproj.FullName)

# Collect all <Compile Include="..."> entries
$includedFiles = @()
foreach ($node in $projXml.Project.ItemGroup.Compile) {
    $includedFiles += (Join-Path $csproj.DirectoryName $node.Include)
}

# Normalize paths
$includedFiles = $includedFiles | ForEach-Object { $_.ToLower() }

# 3. Detect stray files
$results = foreach ($file in $allFiles) {

    $full = $file.FullName.ToLower()

    # Flags
    $isIncluded = $includedFiles -contains $full
    $isBackup = $file.Name -match 'copy|backup|old|\(\d+\)|~'
    $isTemp = $file.DirectoryName -match 'temp|backup|old|recovered'
    $isDuplicate = ($allFiles | Where-Object { $_.Name -eq $file.Name }).Count -gt 1

    if (-not $isIncluded -or $isBackup -or $isTemp -or $isDuplicate) {
        [pscustomobject]@{
            FileName     = $file.Name
            FullPath     = $file.FullName
            IncludedInCsproj = $isIncluded
            DuplicateName    = $isDuplicate
            BackupPattern    = $isBackup
            SuspiciousFolder = $isTemp
        }
    }
}

if ($results.Count -eq 0) {
    Write-Host "No stray or duplicate .cs files detected."
} else {
    Write-Host "Stray or duplicate files found: $($results.Count)"
    $results | Export-Csv -Path $OutputCsv -NoTypeInformation -Encoding UTF8
    Write-Host "CSV written to: $OutputCsv"
}