// ====================================================================================================
//  FILE: ValidationReport.cs
//  PATH: ./Engine/Animation/BlendTree/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ValidationReport module.
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
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    ///<summary>
    ///Validation report for blend tree validation.
    ///</summary>
    public class ValidationReport
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public bool IsValid => Errors.Count == 0;
    }
}

