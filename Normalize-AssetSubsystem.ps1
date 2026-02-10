$root   = "E:\BDC\Projects\SASZombieAssaultTD"
$assets = Join-Path $root "Engine\Systems\Assets"
$csproj = Join-Path $root "SASZombieAssaultTD.csproj"

function Write-Atomic($path, $content) {
    $tmp = "$path.tmp"
    $content | Out-File -FilePath $tmp -Encoding utf8
    Move-Item -Force $tmp $path
}

# ---------------------------------------------------------
# 1. Normalize namespaces in all asset subsystem files
# ---------------------------------------------------------

$assetFiles = @(
    "AssetRegistry.cs",
    "AssetDiscovery.cs",
    "TextureLoader.cs",
    "DataLoader.cs"
)

foreach ($file in $assetFiles) {
    $path = Join-Path $assets $file
    if (Test-Path $path) {
        $content = Get-Content $path -Raw

        # Replace any incorrect namespace with the correct one
        $content = $content -replace 'namespace\s+[\w\.]+', 'namespace Engine.Systems.Assets'

        Write-Atomic $path $content
        Write-Host "✔ Normalized namespace in $file"
    }
}

# ---------------------------------------------------------
# 2. Rewrite AssetInitializer.cs cleanly
# ---------------------------------------------------------

$initializer = Join-Path $assets "AssetInitializer.cs"

Write-Atomic $initializer @"
using Engine.Systems.Assets;

namespace Engine.Systems.Assets
{
    public static class AssetInitializer
    {
        public static void Initialize()
        {
            AssetRegistry.Clear();
            AssetDiscovery.Discover();
            TextureLoader.LoadAll();
            DataLoader.LoadAll();
        }
    }
}
"@

Write-Host "✔ Rewrote AssetInitializer.cs"


# ---------------------------------------------------------
# 3. Remove duplicate <Compile Include> entries from .csproj
# ---------------------------------------------------------

$proj = Get-Content $csproj

$unique = @{}
$cleaned = @()

foreach ($line in $proj) {
    if ($line -match '<Compile Include=') {
        if ($unique.ContainsKey($line)) {
            Write-Host "✔ Removed duplicate entry: $line"
            continue
        }
        $unique[$line] = $true
    }
    $cleaned += $line
}

Write-Atomic $csproj ($cleaned -join "`n")

Write-Host "
Write-Host "🎉 Asset subsystem fully normalized. Clean + rebuild your solution."
