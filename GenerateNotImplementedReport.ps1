# ============================================================================
# SCRIPT:     GenerateNotImplementedReport.ps1
# PATH TARGET: E:\BDC\Projects\SASZombieAssaultTD\Reports\NotImplementedReport.md
# PURPOSE:    Aggregates files with open NotImplementedException stubs, calculates 
#             individual file volumetrics, and tracks the overall grand total.
# ============================================================================

\$targetDir = "E:\BDC\Projects\SASZombieAssaultTD\Engine"
\$reportFolder = "E:\BDC\Projects\SASZombieAssaultTD\Reports"
reportPath = Join-Path reportFolder "NotImplementedReport.md"

# Ensure the destination Reports directory exists securely on disk
if (!(Test-Path \$reportFolder)) {
    New-Item -ItemType Directory -Path \$reportFolder | Out-Null
}

Write-Host "[GeminiRunner] Commencing recursive PowerShell scan for NotImplementedException landmines..."

# 1. Gather all granular occurrences across loose .cs scripts
rawFindings = Get-ChildItem -Path targetDir -Filter *.cs -Recurse | 
    Where-Object { \$_.FullName -notmatch '\\(obj|bin|Output)\\' } | 
    Select-String -Pattern "throw new NotImplementedException"

grandTotal = rawFindings.Count

# 2. Group findings by filename to calculate file-specific volumetrics
groupedFindings = rawFindings | Group-Object FileName | Sort-Object Count -Descending

# 3. Stream and format the final Markdown compilation out to disk
\$sb = New-Object System.Text.StringBuilder
[void]\$sb.AppendLine("# NotImplementedException Structural Crash Audit Report")
[void]\$sb.AppendLine("* **Destination Directory Locked:** `E:/BDC/Projects/SASZombieAssaultTD/Reports`")
[void]$sb.AppendLine("* **Scan Generation Timestamp:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')`n")

[void]$sb.AppendLine("## 📊 Summary Operational Metrics")
[void]$sb.AppendLine("* **Grand Total Open Exception Landmines:** ``\$grandTotal``")
[void]$sb.AppendLine("* **Total Unique Impacted C# Modules:** ``(groupedFindings.Count)```n")

[void]$sb.AppendLine("## 🗂️ Unresolved Exception Volumetrics by Module")
[void]$sb.AppendLine("| Target C# Source File Name | Open Stubs Count | Threat Vector Status |")
[void]$sb.AppendLine("| :--- | :---: | :--- |")

foreach ($group in $groupedFindings) {
    $threatStatus = if ($group.Count -gt 2) { "Critical Operational Block" } else { "Isolated Subroutine Crash" }
    [void]$sb.AppendLine("| ``$($group.Name)`` | ``$($group.Count)`` | $threatStatus |")
}

[void]$sb.AppendLine("`n## 🔍 Granular Landmine Mapping Matrix")
[void]$sb.AppendLine("| File Target Module Name | Line Number | Unimplemented Block Signature Context |")
[void]$sb.AppendLine("| :--- | :--- | :--- |")

foreach ($finding in $rawFindings) {
    $cleanLine = $finding.Line.Trim().Replace("|", "I")
    [void]$sb.AppendLine("| ``$($finding.FileName)`` | ``$($finding.LineNumber)`` | $cleanLine |")
}

# Commit changes securely using clean UTF-8 encoding patterns
[System.IO.File]::WriteAllText($reportPath, $sb.ToString(), [System.Text.Encoding]::UTF8)

Write-Host "[GeminiRunner] Success! Generated exception file analysis saved under: $reportPath"
