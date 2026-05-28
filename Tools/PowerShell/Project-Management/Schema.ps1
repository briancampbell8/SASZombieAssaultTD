$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$schemaDir = Join-Path $root "Schemas"

if (!(Test-Path $schemaDir)) {
    New-Item -ItemType Directory -Path $schemaDir | Out-Null
}

$hudSchema = @'
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "title": "HUD Layout Schema",
  "type": "object",
  "properties": {
    "elements": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "id": { "type": "string" },
          "type": {
            "type": "string",
            "enum": ["text", "image", "button", "bar"]
          },
          "x": { "type": "number" },
          "y": { "type": "number" },
          "width": { "type": "number" },
          "height": { "type": "number" },
          "text": { "type": "string" },
          "image": { "type": "string" },
          "color": { "type": "string" },
          "fontSize": { "type": "number" },
          "anchor": {
            "type": "string",
            "enum": ["top-left", "top-right", "bottom-left", "bottom-right", "center"]
          }
        },
        "required": ["id", "type", "x", "y"]
      }
    }
  },
  "required": ["elements"]
}
'@

$staticSchema = @'
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "title": "Static Layout Schema",
  "type": "object",
  "properties": {
    "tiles": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "x": { "type": "number" },
          "y": { "type": "number" },
          "type": { "type": "string" }
        },
        "required": ["x", "y", "type"]
      }
    }
  },
  "required": ["tiles"]
}
'@

Set-Content -Path (Join-Path $schemaDir "HUD.schema.json") -Value $hudSchema -Encoding ASCII
Set-Content -Path (Join-Path $schemaDir "StaticLayout.schema.json") -Value $staticSchema -Encoding ASCII

$targets = @(
    "Assets/HUD/HUD.json",
    "Assets/HUD/SupportHUD.json",
    "Assets/Static/MeanStreets.json"
)

foreach ($file in $targets) {
    $path = Join-Path $root $file
    if (Test-Path $path) {
        $json = Get-Content $path -Raw
        if ($json -notmatch '"\$schema"') {
            $schemaPath = "../../Schemas/" + (Split-Path $file -Leaf).Replace(".json", ".schema.json")
            $updated = "{`n  `"$schema`": `"$schemaPath`"," + $json.TrimStart('{')
            Set-Content -Path $path -Value $updated -Encoding ASCII
        }
    }
}