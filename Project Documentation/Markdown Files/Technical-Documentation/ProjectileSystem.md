# Projectile System

## Overview
Projectiles are spawned by turrets, soldiers, and some premium items. They handle movement, collision, and damage application.

---

## Projectile Types

### Bullet
- Used by Vickers and MG SAS
- Hits first enemy in path
- High fire rate, low damage

### Grenade
- Used by MGL turret
- Arcing trajectory
- AoE explosion on impact

### Sniper Round
- Used by Sniper SAS
- Pierces multiple enemies
- High damage, long range

### Flame Burst
- Used by Special Turret (flame mode)
- Short-range cone
- Applies burn DoT

### Acid Shot
- Used by Special Turret (acid mode)
- Applies armor-melting debuff

### Lightning Arc
- Used by Special Turret (lightning mode)
- Chains between enemies

---

## Collision Rules
- Bullets: stop on first hit
- Grenades: explode on impact
- Sniper rounds: pierce up to N targets
- Flame/acid/lightning: area or chain-based

---

## Object Pooling
- All projectiles use pooling
- Reduces allocations
- Improves performance

