# Wave Configuration System

## Overview
This document defines how waves are structured, triggered, and escalated across all eight maps. It serves as the blueprint for WaveManager.cs and ensures accurate pacing that matches the original game.

---

## Wave Structure

### Wave Definition
Each wave contains:
- Enemy types
- Spawn counts
- Spawn intervals
- Spawn lanes
- Special triggers (bosses, splits, summons)

### Example JSON Structure (for reference)
{
  """"wave"""": 10,
  """"spawns"""": [
    { """"type"""": """"Bloater"""", """"count"""": 3, """"lane"""": 1, """"interval"""": 1.5 },
    { """"type"""": """"Sprinter"""", """"count"""": 10, """"lane"""": 2, """"interval"""": 0.8 }
  ]
}

---

## Global Wave Rules

### Early Waves (1–10)
- Introduce basic enemies
- Low density
- Single-lane pressure

### Mid Waves (11–25)
- Introduce Shadows, Mamushkas
- Multi-lane pressure
- Higher density

### Late Waves (26–39)
- Devastators appear
- Skeleton summons
- High density and mixed compositions

### Final Waves (40+)
- Ruin appears
- Recurring boss cycles:
  - Wave 40
  - Wave 50
  - Wave 55
  - Every wave after 60

---

## Map-Specific Wave Notes

### Mean Street
- Linear waves
- Early introduction of Sprinters

### Sub-Zero
- Slower pacing early
- Multi-lane pressure later

### Dead Warehouse
- High-density waves
- Tight corridors amplify difficulty

### Shop Til You Drop
- Player-created pathing affects wave flow

### Killtop
- Multi-directional spawns from early waves

### Touchdown
- Long curved paths increase travel time

### Cleanup On Aisle 13
- Heavy pressure from the west wall

### Outbreak Mansion
- Final boss map
- Highest density and mixed compositions

