# Boss Logic

## Overview
Bosses introduce unique mechanics that require special handling in the engine. SAS TD has two boss classes: Devastator and Ruin.

---

## Devastator

### Abilities
- Summons Skeletons
- Aura damages turrets and walls
- High health

### Engine Notes
- Aura is a periodic AoE tick
- Summons occur at health thresholds
- Skeletons inherit pathing from Devastator

---

## Ruin (Final Boss)

### Abilities
- Fiery tendrils knock down turrets and soldiers
- Massive health pool
- Immune to most instant-kill effects
- Takes heavy damage from Necro Nuke

### Spawn Pattern
- Wave 40
- Wave 50
- Wave 55
- Every wave after 60

### Engine Notes
- Tendril attack is a radial AoE
- Knockdown temporarily disables turrets
- Ruin ignores slow effects

