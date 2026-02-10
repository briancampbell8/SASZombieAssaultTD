$root     = "E:\BDC\Projects\SASZombieAssaultTD"
$engine   = Join-Path $root "Engine\Systems\Assets"
$csproj   = Join-Path $root "SASZombieAssaultTD.csproj"

function Write-Atomic($path, $content) {
    $dir = Split-Path $path
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $tmp = "$path.tmp"
    $content | Out-File -FilePath $tmp -Encoding utf8
    Move-Item -Force $tmp $path
}

# ---------------------------------------------------------
# 1. Program.cs — create correct instance-based entry point
# ---------------------------------------------------------

$programPath = Join-Path $root "Program.cs"

Write-Atomic $programPath @"
using Engine;

namespace SASZombieAssaultTD
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var game = new GameRoot();
            game.Run();
        }
    }
}
"@

Write-Host "✔ Program.cs written"


# ---------------------------------------------------------
# 2. AssetInitializer.cs — correct using + clean content
# ---------------------------------------------------------

$initializerPath = Join-Path $engine "AssetInitializer.cs"

Write-Atomic $initializerPath @"
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

Write-Host "✔ AssetInitializer.cs rewritten"


# ---------------------------------------------------------
# 3. Ensure .csproj includes all required asset subsystem files
# ---------------------------------------------------------

$requiredIncludes = @(
    'Engine\Systems\Assets\AssetRegistry.cs',
    'Engine\Systems\Assets\AssetDiscovery.cs',
    'Engine\Systems\Assets\TextureLoader.cs',
    'Engine\Systems\Assets\DataLoader.cs',
    'Program.cs'
)

$projLines = Get-Content $csproj

foreach ($inc in $requiredIncludes) {
    $entry = "<Compile Include=`"$inc`" />"
    if (-not ($projLines -match [regex]::Escape($entry))) {
        # Insert after first <ItemGroup> that contains Compile entries
        $projLines = $projLines -replace '(?<=<ItemGroup>)', "`n    $entry"
        Write-Host "✔ Added to .csproj: $inc"
    }
}

# Write updated .csproj atomically
$tmp = "$csproj.tmp"
$projLines | Out-File -FilePath $tmp -Encoding utf8
Move-Item -Force $tmp $csproj

Write-Host "✔ .csproj updated"

Write-Host "
Write-Host "🎉 Fix script complete — clean + rebuild your solution."
