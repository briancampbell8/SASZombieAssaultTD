# Harry_PostReview.ps1
# Run after reviewing and implementing Harry's proposal

$root = $PWD.Path
$logPath = "$root\Tools\Harry\Harry_TrainingLog.md"

Write-Host "=== Harry Post-Review Logging ==="

$EntryId        = Read-Host "Entry ID (e.g., 0001)"
$ProposalFile   = Read-Host "Proposal File (e.g., Change_0001.md)"
$TargetFiles    = Read-Host "Target File(s)"
$Correct        = Read-Host "Correct Behaviors"
$Incorrect      = Read-Host "Incorrect Behaviors"
$RulesFollowed  = Read-Host "Rules Followed"
$RulesViolated  = Read-Host "Rules Violated"
$ReviewerNotes  = Read-Host "Reviewer Notes"
$LessonsSent    = Read-Host "Lessons Learned ID (optional)"
$NextOutput     = Read-Host "Next Output ID (optional)"

$entry = @"
## Entry $EntryId
**Date:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
**Proposal File:** $ProposalFile
**Target File(s):** $TargetFiles
**Reviewer:** Human

### Evaluation
- **Correct Behaviors:** $Correct
- **Incorrect Behaviors:** $Incorrect
- **Rules Followed:** $RulesFollowed
- **Rules Violated:** $RulesViolated

### Reviewer Notes
$ReviewerNotes

### Lessons Sent (ID): $LessonsSent
### Harry�s Next Output (ID): $NextOutput

---
"@

Add-Content -Path $logPath -Value $entry -Encoding UTF8BOM

Write-Host "Training log updated successfully."
