# Change0012 — Corruption Repair Sweep

- Date: 2026-01-26 13:49:22
- ProjectRoot: E:\BDC\Projects\SASZombieAssaultTD
- BackupRoot: E:\BDC\Projects\SASZombieAssaultTD\Backups\Change0012_RepairCorruption

| File | Action | Notes |
|------|--------|-------|
| Automation\Engine\Scenes\GameScene.cs | Repaired | Removed Unicode replacement char (�) |
| Automation\Engine\Scenes\MainMenuScene.cs | Repaired | Removed Unicode replacement char (�) |
| Backups\Change0012_RepairCorruption\Engine\Rendering\DebugOverlay.cs | Repaired | Removed stray leading '@' at file start |
| Backups\Change0012_RepairCorruption\Engine\Rendering\IRenderContext.cs | Repaired | Removed stray leading '@' at file start |
| Backups\Change0012_RepairCorruption\Engine\Rendering\RenderSurface.cs | Repaired | Removed stray leading '@' at file start |
| Backups\Change0012_RepairCorruption\Engine\Rendering\WindowHost.cs | Repaired | Removed stray leading '@' at file start |
| Engine\Scenes\MainMenuScene.cs | Repaired | Removed Unicode replacement char (�) |
Done. Review the log and rebuild in VS.
