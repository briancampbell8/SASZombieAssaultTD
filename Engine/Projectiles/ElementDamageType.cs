using System;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    /// <summary>
    /// Damage element types used by projectiles.
    /// Added to resolve CS0246 when referenced by ProjectileFactory.
    /// </summary>
    public enum ElementDamageType
    {
        Physical = 0,
        Fire = 1,
        Lightning = 2,
        Energy = 3,
        Magic = 4
    }
}
