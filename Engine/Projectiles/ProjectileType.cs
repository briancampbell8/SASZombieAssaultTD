// ====================================================================================================
//  FILE: ProjectileType.cs
//  PATH: ./Engine/Projectiles/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ProjectileType module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    ///<summary>
    ///Projectile types used by the projectile system and pools.
    ///Keep values stable if serialization or editor tooling depends on them.
    ///</summary>
    public enum ProjectileType
    {
        ///<summary>Unknown / default value.</summary>
        Unknown = 0,

        ///<summary>Small fast bullets.</summary>
        Bullet,

        ///<summary>Rockets with splash damage.</summary>
        Rocket,

        ///<summary>Arcing grenades with splash damage.</summary>
        Grenade,

        ///<summary>Instant or beam lasers.</summary>
        Laser,

        ///<summary>Energy projectiles.</summary>
        Plasma,

        ///<summary>Physical arrows.</summary>
        Arrow,

        ///<summary>Magic projectiles.</summary>
        Magic
    }
}

