# SteveScript.ps1 — DOCX-Only Hybrid Formatting Version
# Purpose:
# - Steve.docx is now the sole source and output artifact
# - BEFORE 3 PM:
#       Prompt: "Early end to debugging?"
#       YES → Morning Mode (Start-of-Day header + Comments-To-Address section)
#       NO  → exit with "Decision made to keep debugging"
# - AT or AFTER 3 PM:
#       Automatically run Evening Mode
# - Evening Mode:
#       Append Latest Log Entry + End-of-Day Summary sections
# - Uses Word COM automation with hybrid formatting (professional + highlighted sections)
# - Deterministic, byte-stable writes

$ErrorActionPreference = "Stop"

# ---------------------------------------------------------
# Resolve paths
# ---------------------------------------------------------
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$docxPath = Join-Path $root "Steve.docx"

if (-not (Test-Path $docxPath)) {
    Write-Host "Steve.docx not found at: $docxPath" -ForegroundColor Red
    exit 1
}

Write-Host "Loading Steve.docx" -ForegroundColor Cyan

# ---------------------------------------------------------
# Start Word COM automation
# ---------------------------------------------------------
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Open($docxPath)

# Helper: Insert a heading
function Add-Heading($text, $level) {
    $range = $doc.Content
    $range.Collapse(0)
    $range.InsertParagraphAfter()
    $range = $doc.Content
    $range.Collapse(0)
    $range.Text = $text
    $range.Style = "Heading $level"
    $range.InsertParagraphAfter()
}

# Helper: Insert a normal paragraph
function Add-Paragraph($text) {
    $range = $doc.Content
    $range.Collapse(0)
    $range.Text = $text
    $range.Style = "Normal"
    $range.InsertParagraphAfter()
}

# Helper: Insert a highlighted block
function Add-HighlightBlock($text) {
    $range = $doc.Content
    $range.Collapse(0)
    $range.Text = $text
    $range.Style = "Normal"
    $range.Shading.BackgroundPatternColor = 15921906  # Light yellow highlight
    $range.InsertParagraphAfter()
}

# Helper: Insert bullet list
function Add-BulletList($items) {
    foreach ($item in $items) {
        $range = $doc.Content
        $range.Collapse(0)
        $range.Text = $item
        $range.Style = "Normal"
        $range.ListFormat.ApplyBulletDefault()
        $range.InsertParagraphAfter()
    }
}

# ---------------------------------------------------------
# Date and mode logic
# ---------------------------------------------------------
$todayDisplay = (Get-Date).ToString("dddd, MMMM dd, yyyy")
$todayIso     = (Get-Date).ToString("yyyy-MM-dd")
$currentHour  = (Get-Date).Hour

$mode = $null

if ($currentHour -lt 15) {
    Write-Host "It is before 3 PM." -ForegroundColor Yellow
    Write-Host "Early end to debugging? (yes/no)" -ForegroundColor Cyan
    $response = Read-Host

    if ($response -match "^(?i)y(es)?$") {
        Write-Host "Running MORNING MODE..." -ForegroundColor Green
        $mode = "Morning"
    }
    else {
        Write-Host "Decision made to keep debugging" -ForegroundColor Yellow
        $doc.Close()
        $word.Quit()
        exit 0
    }
}
else {
    Write-Host "It is 3 PM or later — running EVENING MODE" -ForegroundColor Green
    $mode = "Evening"
}

# ---------------------------------------------------------
# MORNING MODE
# ---------------------------------------------------------
if ($mode -eq "Morning") {

    $startStamp = (Get-Date).ToString("MM-dd-yyyy HH:mm:ss")

    Add-Heading "$todayDisplay — Start of Day" 1
    Add-Paragraph "Started at: $startStamp"

    Add-Heading "Comments To Address First" 2
    Add-HighlightBlock "- List the comments or review notes that must be handled before any other work"

    Write-Host "Writing updated Steve.docx" -ForegroundColor Cyan
    $doc.Save()
    $doc.Close()
    $word.Quit()

    Write-Host "SteveScript complete. Morning Mode executed." -ForegroundColor Green
    exit 0
}

# ---------------------------------------------------------
# EVENING MODE
# ---------------------------------------------------------
if ($mode -eq "Evening") {

    Add-Heading "Latest Log Entry — $todayIso" 2
    Add-BulletList @(
        "Fill in key accomplishments for the day",
        "Example: Completed subsystem work, verified builds, etc."
    )

    Add-Heading "End-of-Day Summary — $todayIso" 1

    Add-Heading "What We Accomplished Today" 3
    Add-BulletList @(
        "Fill in accomplishments"
    )

    Add-Heading "What We Need To Do Tomorrow" 3
    Add-BulletList @(
        "Fill in next actions"
    )

    Add-Heading "What We Intend To Accomplish Next" 3
    Add-BulletList @(
        "Fill in forward-looking goals"
    )

    Write-Host "Writing updated Steve.docx" -ForegroundColor Cyan
    $doc.Save()
    $doc.Close()
    $word.Quit()

    Write-Host "SteveScript complete. Evening Mode executed." -ForegroundColor Green
    exit 0
}