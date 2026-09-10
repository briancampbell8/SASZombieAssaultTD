// ====================================================================================================
//  FILE: ElementDamageType.cs
//  PATH: ./Engine/Projectiles/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ElementDamageType module.
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
    ///Damage element types used by projectiles.
    ///Added to resolve CS0246 when referenced by ProjectileFactory.
    ///</summary>
    public enum ElementDamageType
    {
        Physical = 0,
        Fire = 1,
        Lightning = 2,
        Energy = 3,
        Magic = 4
    }
}

