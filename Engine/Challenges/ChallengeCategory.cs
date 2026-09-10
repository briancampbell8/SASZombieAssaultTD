// ====================================================================================================
//  FILE: ChallengeCategory.cs
//  PATH: ./Engine/Challenges/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ChallengeCategory module.
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
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Challenges
{
    public enum ChallengeCategory
    {
        Daily,
        Weekly,
        Monthly,
        Special,
        Seasonal
    }
}

