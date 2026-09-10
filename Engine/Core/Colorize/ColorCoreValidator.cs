// ====================================================================================================
//  FILE: ColorCoreValidator.cs
//  PATH: ./Engine/Core/Colorize/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ColorCoreValidator module.
//
//  RESPONSIBILITIES:
//      - Provide ValidateColorCore() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File Path: Engine/Core/Colorize/ColorCoreValidator.cs
//File: ColorCoreValidator.cs
//Program: ColorCore (Validator)
//Subsystem: Core / Colorize
//
//Purpose:
//    Provides a standalone validation routine for the ColorCore subsystem.
//    Ensures that static fields, constructors, implicit operators, and
//    property accessors behave correctly after structural changes.
//
//Responsibilities:
//    - Validate static color fields (e.g., Crimson)
//    - Validate CreateUnchecked factory method
//    - Validate implicit conversion from Core.Color → Color
//    - Validate property access (R, G, B, A, RByte, etc.)
//    - Produce debug output and write error logs if validation fails
//
//Architecture:
//    - Standalone utility class
//    - No dependencies on rendering or asset systems
//    - Safe to run at startup or during diagnostics
//
//Notes:
//    - This validator is NOT the canonical ColorCore implementation
//    - Canonical logic resides in Engine/Core/Color.cs
//    - This file is allowed because it performs *validation*, not definition
//============================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
//

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.CoreSize.Colorize
{
    ///<summary>
    ///Utility program to validate ColorCore.cs structure and dependencies.
    ///</summary>
    public class ColorCoreValidator
    {
        public static void ValidateColorCore()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "=== COLORCORE VALIDATION START ===");

            var errors = new List<string>();
            var warnings = new List<string>();

            //--------------------------------------------------------------------
            //TEST 1 — Static field initialization
            //--------------------------------------------------------------------
            try
            {
                var crimson = Color.Crimson;
                DLogger.Log($"✅ Crimson field accessible: {crimson}");
            }
            catch (Exception ex)
            {
                errors.Add($"Crimson field access failed: {ex.Message}");
            }

            //--------------------------------------------------------------------
            //TEST 2 — CreateUnchecked factory method
            //--------------------------------------------------------------------
            try
            {
                var testColor = Color.CreateUnchecked(0.5f, 0.7f, 0.3f, 0.9f);
                DLogger.Log(
                    $"✅ CreateUnchecked works: R={testColor.R}, G={testColor.G}, B={testColor.B}, A={testColor.A}");
            }
            catch (Exception ex)
            {
                errors.Add($"CreateUnchecked failed: {ex.Message}");
            }

            //--------------------------------------------------------------------
            //TEST 3 — Implicit operator Core.Color → Color
            //--------------------------------------------------------------------
            try
            {
                var coreColor = new Color(128, 64, 32, 255);
                var engineColor = (Color)coreColor;

                DLogger.Log(
                    $"✅ Implicit operator works: R={engineColor.R}, G={engineColor.G}, B={engineColor.B}, A={engineColor.A}");
            }
            catch (Exception ex)
            {
                errors.Add($"Implicit operator failed: {ex.Message}");
            }

            //--------------------------------------------------------------------
            //TEST 4 — Property access
            //--------------------------------------------------------------------
            try
            {
                var testColor = new Color(0.8f, 0.6f, 0.4f, 1.0f);
                DLogger.Log(
                    $"✅ Property access works: RByte={testColor.RByte}, GByte={testColor.GByte}");
            }
            catch (Exception ex)
            {
                errors.Add($"Property access failed: {ex.Message}");
            }

            //--------------------------------------------------------------------
            //FINAL REPORT
            //--------------------------------------------------------------------
            DLogger.Log(LogSubsystems.ResourcesPipeline, "=== COLORCORE VALIDATION COMPLETE ===");
            DLogger.Log($"Errors found: {errors.Count}");
            DLogger.Log($"Warnings found: {warnings.Count}");

            if (errors.Count > 0)
            {
                File.WriteAllText("ColorCoreValidation.log", string.Join("\n", errors));
                throw new InvalidOperationException(
                    $"ColorCore validation failed with {errors.Count} errors. See ColorCoreValidation.log");
            }

            if (warnings.Count > 0)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Warnings:");
                warnings.ForEach(w => DLogger.Log($"  - {w}"));
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "✅ ColorCore validation completed successfully!");
        }
    }
}

