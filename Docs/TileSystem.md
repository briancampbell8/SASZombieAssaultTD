# Tile System

## Overview
The tile system defines how maps are constructed, how zombies navigate, and how players place turrets and deployables.

---

## Tile Types

### Path Tiles
- Zombies walk on these
- Define primary lanes

### Block Tiles
- Impassable
- Used for walls, shelves, fences

### Rooftop Tiles
- Allow SAS soldiers
- Block zombies

### Slow Tiles (optional)
- Snow tiles in Sub-Zero
- Reduce zombie speed

### Decorative Tiles
- Props, scenery, non-interactive elements

---

## Tilemap Structure
Tilemaps are grid-based and loaded from:
- PNG tile layers
- JSON metadata
- Spawn node definitions

---

## Tile Metadata
Each tile may include:
- Collision flag
- Height level
- Placement permission
- Pathing weight

---

## Tile Atlas Organization
- /Assets/Tiles/Urban/
- /Assets/Tiles/Snow/
- /Assets/Tiles/Warehouse/
- /Assets/Tiles/Grocery/
- /Assets/Tiles/Hilltop/
- /Assets/Tiles/Stadium/
- /Assets/Tiles/Mansion/

