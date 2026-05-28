/*
// File: UIMaterial.cs
// Purpose: UI material definition for SAS Zombie Assault TD UI system.
// Features: Material properties for UI rendering including color, texture, shader, and blend mode.
// Created: Engine UI Implementation
//
// NOTE: Diagnostic CS0101 ("The namespace 'SASZombieAssaultTD.Engine.UI' already contains a definition for 'UIMaterial'")
// indicates there is at least one other file in the same namespace that defines a type named UIMaterial.
// This file below is the primary/valid definition. To resolve the CS0101 error, do one of the following:
// 
// 1) Preferred — Remove duplicate definitions:
//    - Find other files that declare `public class UIMaterial` in namespace SASZombieAssaultTD.Engine.UI
//      and remove or consolidate those duplicate full-class definitions so only this file defines the class.
//    - If the other files contain unique members, move those members into this file (or make the other files partial
//      and move only the additional members into them — see option 2).)
// 
// 2) Merge via partial classes:
//    - Convert duplicate full-class declarations into `partial class UIMaterial` and ensure only one file contains
//      the complete set of members, or split members across partial declarations so there are no two
//      conflicting complete definitions. Example for a duplicate file:
//        // In OtherFile.cs
//        namespace SASZombieAssaultTD.Engine.UI
//        {
//            public partial class UIMaterial
//            {
//                // move only additional members here (no duplicate property definitions)
//            }
//        }
// 
// 3) Rename one of the definitions:
//    - If there really are two different concepts, rename one of the classes to a distinct name
//      (e.g., UIMaterialV2 or UIMaterialSettings) and update usages accordingly.
// 
// 4) If you cannot find duplicates, search the project for "class UIMaterial" or a file that might be included twice
//    by linked files, accidental file copies, or conflicting generated code. Check for Generated files, partial classes,
//    and linked files included in multiple projects in a solution.
// 
// Recommended immediate code change (safe first step):
// - Make this declaration `partial` so other files that are meant to augment this class can be converted to `partial` as well.
// - If other files are full, duplicate definitions, convert them to `partial` and move unique members here, or delete duplicates.
// 
// If you want, provide the paths or contents of the other files that define UIMaterial and I will produce exact edits to merge them.
// 
// End of guidance.
using SASZombieAssaultTD.Engine.Diagnostics;

*/

using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Blend mode enumeration for UI rendering.
    /// </summary>
    public enum BlendMode
    {
        Opaque,
        AlphaBlend,
        Additive,
        Multiply
    }

    /// <summary>
    /// UI material for element rendering.
    /// Provides material properties for UI elements including color, texture, shader, and blend mode.
    /// </summary>
    // Converted to partial to allow merging with other partial declarations that may exist in the project.
    // DUPLICATE UIMaterial — DISABLED
    // REASON: Canonical definition exists in Engine/UI/UIMaterial.cs
    // STATUS: Commented out to resolve CS0101 namespace collision.

    // public class UIMaterial
    // {
    //     // DUPLICATE — DO NOT USE
    // }

    // public partial class UIMaterial // DUPLICATE — DO NOT USE
    // {
    //public Color BaseColor { get; set; }
      //  public object Texture { get; set; }
     //   public object Shader { get; set; }
     //   public BlendMode BlendMode { get; set; }

    //    public UIMaterial()
    //    {
    //        InitializeMaterial();
    //    }

    //    private void InitializeMaterial()
    //    {
    //        BaseColor = Color.White;
    //        Texture = null;
    //        Shader = null;
    //        BlendMode = BlendMode.AlphaBlend;
    //        ValidateProperties();
    //    }

   //     private void ValidateProperties()
     //   {
       //     if (BaseColor == null)
         //   {
           //     throw new ArgumentNullException(nameof(BaseColor));
        //    }
      //  }
    // }
}
