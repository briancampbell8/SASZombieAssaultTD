# Camera System

## Overview
The camera system controls what portion of the map is visible to the player. It must support smooth movement and clamped boundaries.

---

## Features

### Map Bounds
- Camera cannot move outside map edges
- Calculated from tilemap dimensions

### Smooth Panning
- Interpolated movement
- Adjustable speed

### Zoom (optional)
- Default zoom level fixed
- Optional zoom for accessibility

### Shake (optional)
- Triggered by explosions or boss attacks
- Short-duration screen shake

