$Id = "0001"
$path = ".\Tools\Harry\Changes\Change_$Id.md"

$template = '# Change Proposal - ID: {ID}`n`n**Date:** `n**Target File(s):**  `n**Subsystem:**  `n**Proposal Summary:**  `n`n---`n`n## Proposed Code`n```csharp`n// Harry''s code-only output goes here`n``` `n`n---`n`n## Intent`n-  `n`n## Expected Behavior`n-  `n`n## Notes (Optional)`n-  `n`n---'

# Convert literal `n to real newlines
$final = $template -replace '`n', "`n"

# Write output
Set-Content -Path $path -Value $final -Encoding UTF8BOM

