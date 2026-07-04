using System.Collections.Generic;

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
