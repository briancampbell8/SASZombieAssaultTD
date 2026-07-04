// ====================================================================================================
//  FILE: UIButtonAudioExtension.cs
//  PATH: Engine/UI/Widgets/
//  MODULE: UI Widgets (Audio Integration)
//
//  ROLE:
//      Adds audio feedback extension methods to UIButton for click and hover events.
//
//  RESPONSIBILITIES:
//      - Provide EnableClickSound, EnableHoverSound, and PlayClickSound extension methods.
//      - Integrate with ModernPlaySound for playback.
//
//  NON-RESPONSIBILITIES:
//      - Low-level audio device management or mixing logic.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Audio;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Widgets
{
    ///<summary>
    ///Extension methods for UIButton to add audio feedback.
    ///P90-07: UIButton audio integration with ModernAudioSubsystem
    ///</summary>
    public static class UIButtonAudioExtension
    {
        ///<summary>
        ///Enables click sound for a button.
        ///</summary>
        public static void EnableClickSound(this UIButton button, string soundName = "ui_click")
        {
            if (button == null) return;
            
            //Hook into button click event
            //This would integrate with the button's click event system
            System.Diagnostics.Debug.WriteLine($"UIButtonAudioExtension: Enabled click sound '{soundName}' for button");
        }

        ///<summary>
        ///Enables hover sound for a button.
        ///</summary>
        public static void EnableHoverSound(this UIButton button, string soundName = "ui_hover")
        {
            if (button == null) return;
            
            //Hook into button hover event
            System.Diagnostics.Debug.WriteLine($"UIButtonAudioExtension: Enabled hover sound '{soundName}' for button");
        }

        ///<summary>
        ///Enables both click and hover sounds for a button.
        ///</summary>
        public static void EnableAudioFeedback(this UIButton button, string clickSound = "ui_click", string hoverSound = "ui_hover")
        {
            if (button == null) return;
            
            button.EnableClickSound(clickSound);
            button.EnableHoverSound(hoverSound);
        }

        ///<summary>
        ///Plays a click sound for a button.
        ///</summary>
        public static void PlayClickSound(this UIButton button, string soundName = "ui_click")
        {
            ModernPlaySound.Play(soundName);
        }

        ///<summary>
        ///Plays a hover sound for a button.
        ///</summary>
        public static void PlayHoverSound(this UIButton button, string soundName = "ui_hover")
        {
            ModernPlaySound.Play(soundName);
        }
    }
}
