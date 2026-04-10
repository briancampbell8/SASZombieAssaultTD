/*
File:    TowerType.cs
Purpose: Enumeration of tower types for SAS Zombie Assault TD.
Features: Tower type definitions with associated properties.

P11-04-07-B: Tower type enumeration following established enum pattern.
*/

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Enumeration of available tower types.
    /// </summary>
    public enum TowerType
    {
        /// <summary>
        /// Basic tower with standard damage and range.
        /// </summary>
        Basic,

        /// <summary>
        /// Sniper tower with long range and high damage.
        /// </summary>
        Sniper,

        /// <summary>
        /// Splash damage tower with area effect.
        /// </summary>
        Splash,

        /// <summary>
        /// Freeze tower that slows enemies.
        /// </summary>
        Freeze,

        /// <summary>
        /// Rapid fire tower with low damage but high fire rate.
        /// </summary>
        Rapid,

        /// <summary>
        /// Poison tower with damage over time.
        /// </summary>
        Poison,

        /// <summary>
        /// Laser tower with instant hit capability.
        /// </summary>
        Laser,

        /// <summary>
        /// Mortar tower with area damage.
        /// </summary>
        Mortar,

        /// <summary>
        /// Flame tower with continuous damage.
        /// </summary>
        Flame,

        /// <summary>
        /// Ice tower with freezing capability.
        /// </summary>
        Ice,

        /// <summary>
        /// Electric tower with chain lightning.
        /// </summary>
        Electric,

        /// <summary>
        /// Tesla tower with chain lightning.
        /// </summary>
        Tesla
    }
}
