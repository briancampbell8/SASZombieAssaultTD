# SAS Zombie Assault TD – Restoration Workflow

## 1. Project Purpose
Rebuild SAS Zombie Assault TD as a native C# MonoGame application using extracted assets, documented behaviors, and modular engine systems.

## 2. Core Systems
- Map system (tilemaps, pathing, spawn nodes)
- Enemy system (AI, behaviors, wave logic)
- Turret system (placement, upgrades, targeting)
- SAS soldier system (placement, targeting, rooftop logic)
- Deployables (sandbags, barbed wire, grenades)
- Premium items (free in this version)
- UI system (HUD, item bar, wave tracker)
- Audio system (SFX for enemies, turrets, abilities)

## 3. Asset Sources
- Extracted SWF sprites
- Fandom wiki references
- Uploaded map screenshots
- Zombie glossary (internal names + behaviors)

## 4. Milestone Tracker
### Phase 1 — Documentation
- Populate all markdown files
- Define map flow and tile atlas
- Define enemy roster and behaviors
- Define turret and soldier logic

### Phase 2 — Engine Scaffolding
- Create Scene classes for all 8 maps
- Implement MapGrid loader
- Implement WaveManager
- Implement EnemyManager

### Phase 3 — Gameplay Systems
- Turret placement + upgrades
- Soldier placement + targeting
- Deployable logic
- Premium item effects

### Phase 4 — Integration
- HUD + Item Bar
- Sprite atlas loading
- Sound effects
- Boss logic (Devastator, Ruin)

### Phase 5 — Testing
- Map-by-map validation
- Wave pacing accuracy
- Enemy behavior accuracy
- Performance tuning

## 5. Notes
This file is the master reference for the entire restoration. All subsystem docs live in `/Docs/`.

