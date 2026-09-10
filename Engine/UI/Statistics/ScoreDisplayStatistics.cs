// ====================================================================================================
//  FILE: ScoreDisplayStatistics.cs
//  PATH: ./Engine/UI/Statistics/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    ///<summary>
    ///Statistics about the score display system state.
    ///</summary>
    public class ScoreDisplayStatistics
    {
        public int CurrentScore { get; set; }
        public int DisplayedScore { get; set; }
        public bool IsAnimating { get; set; }
        public int ActivePopups { get; set; }
        public bool EnableAnimations { get; set; }

        public override string ToString()
        {
            return $"Score Display Statistics - Current: {CurrentScore}, Displayed: {DisplayedScore}, " +
            $"Animating: {IsAnimating}, Popups: {ActivePopups}, Animations: {EnableAnimations}";
        }
    }
}





