$rootPath = "E:\BDC\Projects\SASZombieAssaultTD"
$outputCsv = "E:\BDC\Projects\SASZombieAssaultTD\Results.csv"

$results = @()

Get-ChildItem -Path $rootPath -Recurse -Filter *.cs | ForEach-Object {
    $file = $_
    $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue

    $isEmpty = $false
    $hasInfoComments = $false
    $hasNamespace = $false
    $hasClass = $false
    $anythingElse = $false

    if ([string]::IsNullOrWhiteSpace($content)) {
        $isEmpty = $true
    }
    else {
        if ($content -match "/\*\s*={3,}") {
            $hasInfoComments = $true
        }

        if ($content -match "\bnamespace\b") {
            $hasNamespace = $true
        }

        if ($content -match "\bclass\b") {
            $hasClass = $true
        }

        # Flag anomalies
        if (-not $hasNamespace -or -not $hasClass) {
            $anythingElse = $true
        }
    }

    $results += [PSCustomObject]@{
        ProgramName           = $file.Name
        FilePath              = $file.FullName
        Empty                 = $(if ($isEmpty) { "YES" } else { "NO" })
        InformationalComments = $(if ($hasInfoComments) { "YES" } else { "NO" })
        HasNamespace          = $(if ($hasNamespace) { "YES" } else { "NO" })
        HasClassDefinition    = $(if ($hasClass) { "YES" } else { "NO" })
        AnythingElse          = $(if ($anythingElse) { "YES" } else { "NO" })
    }
}

$results | Export-Csv -Path $outputCsv -NoTypeInformation -Encoding UTF8