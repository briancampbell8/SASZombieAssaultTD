# Scene Architecture

## Overview
Each map in SAS Zombie Assault TD is represented as a Scene class. Scenes handle tile loading, spawn node placement, camera setup, and wave progression.

---

## Scene Responsibilities
- Load tilemap and collision data
- Define spawn nodes and exit nodes
- Initialize turrets, soldiers, and deployables
- Handle wave triggers
- Manage camera bounds
- Render map layers and entities

---

## Scene Class Structure

### Base Class: GameScene.cs
- Shared logic for all maps
- Tilemap loader
- Entity manager references
- Update and Draw loops

### Derived Classes
- MeanStreetScene.cs
- SubZeroScene.cs
- DeadWarehouseScene.cs
- ShopTilYouDropScene.cs
- KilltopScene.cs
- TouchdownScene.cs
- CleanupScene.cs
- OutbreakMansionScene.cs

---

## Scene Lifecycle

### Initialize()
- Load tilemap
- Load spawn nodes
- Set camera bounds

### LoadContent()
- Load sprites
- Load audio
- Initialize UI

### Update()
- Handle waves
- Update entities
- Check win/loss conditions

### Draw()
- Draw tile layers
- Draw entities
- Draw UI overlays

