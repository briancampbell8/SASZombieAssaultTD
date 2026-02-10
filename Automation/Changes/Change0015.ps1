# ================================
# Change0001.ps1
# Zombie Movement System Bootstrap
# Creates files + injects code blocks
# ================================

$ChangeId = "Change0015"
$LogFile = "PowerShellLog.md"

function Write-Log {
    param([string]$Message)
    Add-Content -Path $LogFile -Value "[$ChangeId] $Message"
}

Write-Log "Starting $ChangeId"

# -------------------------------
# 1. Ensure folder structure exists
# -------------------------------

$folders = @(
    "Scripts",
    "Scripts\Paths",
    "Scripts\Zombies",
    "Scripts\TestScenes",
    "GameData",
    "GameData\Paths"
)

foreach ($folder in $folders) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
        Write-Log "Created folder: $folder"
    }
}

# -------------------------------
# 2. Create .cs files if missing
# -------------------------------

$files = @{
    "Scripts\Paths\PathLoader.cs" = "PathLoader.cs"
    "Scripts\Zombies\ZombieMovement.cs" = "ZombieMovement.cs"
    "Scripts\Zombies\ZombieSpawner.cs" = "ZombieSpawner.cs"
    "Scripts\TestScenes\MeanStreetsTest.cs" = "MeanStreetsTest.cs"
}

foreach ($file in $files.Keys) {
    if (!(Test-Path $file)) {
        New-Item -ItemType File -Path $file | Out-Null
        Write-Log "Created file: $file"
    }
}

# -------------------------------
# 3. Inject code blocks
# -------------------------------

function Inject-Code {
    param(
        [string]$FilePath,
        [string]$Marker,
        [string]$CodeBlock
    )

    $StartMarker = "// <$Marker>"
    $EndMarker   = "// </$Marker>"

    $Content = Get-Content $FilePath -Raw

    if ($Content -match [regex]::Escape($StartMarker)) {
        Write-Log "Skipped injection for $FilePath (already contains $Marker)"
        return
    }

    $Injection = "$StartMarker`r`n$CodeBlock`r`n$EndMarker`r`n"
    Add-Content -Path $FilePath -Value $Injection

    Write-Log "Injected $Marker into $FilePath"
}

# -------------------------------
# PathLoader.cs
# -------------------------------

$PathLoaderCode = @"
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public static class PathLoader
{
    public static PathData Load(string pathId)
    {
        string fullPath = $"GameData/Paths/{pathId}.json";
        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<PathData>(json);
    }
}

public class PathData
{
    public string pathId;
    public List<Waypoint> waypoints;
}

public class Waypoint
{
    public float x;
    public float y;
}
"@

Inject-Code "Scripts\Paths\PathLoader.cs" $ChangeId $PathLoaderCode

# -------------------------------
# ZombieMovement.cs
# -------------------------------

$ZombieMovementCode = @"
using System.Numerics;

public class ZombieMovement
{
    public Vector2 Position;
    public float Speed = 50f;
    public int CurrentIndex = 0;
    private PathData path;

    public ZombieMovement(PathData pathData, Vector2 spawnPos)
    {
        path = pathData;
        Position = spawnPos;
    }

    public void Update(float deltaTime)
    {
        if (CurrentIndex >= path.waypoints.Count)
            return;

        var target = new Vector2(path.waypoints[CurrentIndex].x, path.waypoints[CurrentIndex].y);
        var direction = Vector2.Normalize(target - Position);
        Position += direction * Speed * deltaTime;

        if (Vector2.Distance(Position, target) < 4f)
            CurrentIndex++;
    }
}
"@

Inject-Code "Scripts\Zombies\ZombieMovement.cs" $ChangeId $ZombieMovementCode

# -------------------------------
# ZombieSpawner.cs
# -------------------------------

$ZombieSpawnerCode = @"
using System.Collections.Generic;
using System.Numerics;

public class ZombieSpawner
{
    public Vector2 SpawnPoint = new Vector2(100, 600);
    public float SpawnInterval = 1.5f;
    private float timer = 0f;

    private PathData path;
    public List<ZombieMovement> ActiveZombies = new List<ZombieMovement>();

    public ZombieSpawner(PathData pathData)
    {
        path = pathData;
    }

    public void Update(float deltaTime)
    {
        timer -= deltaTime;

        if (timer <= 0f)
        {
            SpawnZombie();
            timer = SpawnInterval;
        }

        foreach (var z in ActiveZombies)
            z.Update(deltaTime);
    }

    private void SpawnZombie()
    {
        var zombie = new ZombieMovement(path, SpawnPoint);
        ActiveZombies.Add(zombie);
    }
}
"@

Inject-Code "Scripts\Zombies\ZombieSpawner.cs" $ChangeId $ZombieSpawnerCode

# -------------------------------
# MeanStreetsTest.cs
# -------------------------------

$TestSceneCode = @"
public class MeanStreetsTest
{
    private ZombieSpawner spawner;

    public void Start()
    {
        var path = PathLoader.Load("MeanStreets");
        spawner = new ZombieSpawner(path);
    }

    public void Update(float deltaTime)
    {
        spawner.Update(deltaTime);
    }
}
"@

Inject-Code "Scripts\TestScenes\MeanStreetsTest.cs" $ChangeId $TestSceneCode

Write-Log "Completed $ChangeId"