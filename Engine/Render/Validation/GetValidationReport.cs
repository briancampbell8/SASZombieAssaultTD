// ====================================================================================================
//  FILE: GetValidationReport.cs
//  PATH: Engine/Render/Validation/GetValidationReport.cs
//  SUBSYSTEM: Render Validation
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Text;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public class GetValidationReport
    {
        //===============================================================================================
        // CRITICAL FIX (CS0103): Declare missing backing fields to support the reporting engine
        //===============================================================================================
        private readonly List<string> _validationResults = new List<string>();

        public int CriticalErrorCount { get; private set; }

        //===============================================================================================
        // OPERATIONAL ENDPOINTS
        //===============================================================================================

        // CRITICAL FIX (CS0542): Renamed the method to 'ExecuteReportGeneration' 
        // so it no longer collides with the class name 'GetValidationReport'.
        public string ExecuteReportGeneration()
        {
            if (_validationResults == null || _validationResults.Count == 0)
            {
                return "No validation data recorded.";
            }

            var reportBuilder = new StringBuilder();
            reportBuilder.AppendLine("=== RENDER PIPELINE VALIDATION REPORT ===");
            reportBuilder.AppendLine($"Generated: {DateTime.Now}");
            reportBuilder.AppendLine($"Total Issues Found: {_validationResults.Count}");
            reportBuilder.AppendLine($"Critical Errors: {CriticalErrorCount}");
            reportBuilder.AppendLine("-----------------------------------------");

            foreach (var result in _validationResults)
            {
                reportBuilder.AppendLine($"- {result}");
            }

            return reportBuilder.ToString();
        }

        // Helper method to feed data into the fields
        public void AddResult(string message, bool isCritical)
        {
            _validationResults.Add(message);
            if (isCritical)
            {
                CriticalErrorCount++;
            }
        }
    }
}
