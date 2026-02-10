param([string]$LessonId,[string]$RelatedProposal,[string]$Issue,[string]$WentWrong,[string]$WentRight,[string]$Doctrine,[string]$Required)
$lessonPath = "$PWD\Tools\Harry\Harry_LessonsLearned.md"
$entry = "## Lesson $LessonId`n**Date:** $(Get-Date -Format yyyy-MM-dd HH:mm:ss)`n**Related Proposal ID:** $RelatedProposal`n**Summary of Issue:** $Issue`n`n### What Went Wrong`n$WentWrong`n`n### What Went Right`n$WentRight`n`n### Doctrine Reinforcement`n$Doctrine`n`n### Required Behavior Going Forward`n$Required`n`n---"
Add-Content -Path $lessonPath -Value $entry

