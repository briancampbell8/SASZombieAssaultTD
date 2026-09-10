// ====================================================================================================
//  FILE: IAIBehavior.cs
//  PATH: ./Engine/AI/
//  MODULE: AI
//
//  ROLE:
//      Provide deterministic AI behavior, decision logic, or state evaluation.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the AI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//CANONICAL IAIBehavior — CLEAN VERSION
//REASON: Decompiler added invalid members (DeltaTime, Owner, GetTarget, SetTarget)
//STATUS: Restored to original engine design
//DATE: 2026‑05‑14

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.AI
{
    public interface IAIBehavior
    {
        ///<summary>
        ///Executes one tick of this behavior.
        ///</summary>
        ///<param name="context">Shared AI context containing state and timing information.</param>
        void Tick(AIContext context);
    }
}

