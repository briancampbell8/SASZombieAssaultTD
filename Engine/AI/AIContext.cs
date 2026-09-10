// ====================================================================================================
//  FILE: AIContext.cs
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
/* 
//PROGRAM: AIContext
//FILE PATH: Engine/AI/AIContext.cs
//PURPOSE: Shared state container for AI behaviors.
//RESPONSIBILITIES:
//  - Provide timing, owner, and target information to behaviors.
//  - Act as a communication channel between AIController and behaviors.
//EXECUTION TRIGGER: Populated and updated by AIController.
//INPUTS:
//  - Delta time from AIController.
//  - Owner and target assignments from gameplay systems.
//OUTPUTS:
//  - Supplies state to behavior Tick() methods.
//DEPENDENCIES: None (lightweight data container).
//CONTENTS:
//  - AIContext class with DeltaTime, Owner, and Target properties.
//

*/

#nullable enable

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.AI
{
    ///<summary>
    ///Shared context container for AI behaviors.
    ///Provides state communication between behaviors and the AI controller.
    ///</summary>
    public class AIContext
    {
        public float DeltaTime { get; set; }
        public object? Owner { get; set; }
        public object? Target { get; set; }
    }
}

