// ====================================================================================================
//  FILE: GetUpgradeStatusColor.cs
//  PATH: ./Engine/UI/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide Available() behavior for the Rendering subsystem.
//      - Provide Locked() behavior for the Rendering subsystem.
//      - Provide Maxed() behavior for the Rendering subsystem.
//      - Provide Unaffordable() behavior for the Rendering subsystem.
//      - Provide InProgress() behavior for the Rendering subsystem.
//      - Provide Selected() behavior for the Rendering subsystem.
//      - Provide PrerequisitesNotMet() behavior for the Rendering subsystem.
//      - Provide GetStatusColor() behavior for the Rendering subsystem.
//      - Provide GetStatusColor() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    ///<summary>
    ///Returns color coding for upgrade states (locked, available, maxed).
    ///</summary>
    public static class GetUpgradeStatusColor
    {
        ///<summary>
        ///Gets color for available upgrade.
        ///</summary>
        ///<returns>Green color for available upgrades.</returns>
        public static Color Available()
        {
            return Color.Green;
        }

        ///<summary>
        ///Gets color for locked upgrade.
        ///</summary>
        ///<returns>Gray color for locked upgrades.</returns>
        public static Color Locked()
        {
            return Color.Gray;
        }

        ///<summary>
        ///Gets color for maxed upgrade.
        ///</summary>
        ///<returns>Gold color for maxed upgrades.</returns>
        public static Color Maxed()
        {
            return Color.Gold;
        }

        ///<summary>
        ///Gets color for upgrade that cannot be afforded.
        ///</summary>
        ///<returns>Red color for unaffordable upgrades.</returns>
        public static Color Unaffordable()
        {
            return Color.Red;
        }

        ///<summary>
        ///Gets color for upgrade in progress.
        ///</summary>
        ///<returns>Blue color for upgrades in progress.</returns>
        public static Color InProgress()
        {
            return Color.Blue;
        }

        ///<summary>
        ///Gets color for selected upgrade.
        ///</summary>
        ///<returns>Yellow color for selected upgrades.</returns>
        public static Color Selected()
        {
            return Color.Yellow;
        }

        ///<summary>
        ///Gets color for upgrade with prerequisites not met.
        ///</summary>
        ///<returns>Orange color for upgrades with unmet prerequisites.</returns>
        public static Color PrerequisitesNotMet()
        {
            return Color.Orange;
        }

        ///<summary>
        ///Gets color based on upgrade status.
        ///</summary>
        ///<param name="isAvailable">Whether upgrade is available.</param>
        ///<param name="isAffordable">Whether upgrade can be afforded.</param>
        ///<param name="isMaxed">Whether upgrade is maxed.</param>
        ///<param name="isSelected">Whether upgrade is selected.</param>
        ///<param name="isInProgress">Whether upgrade is in progress.</param>
        ///<returns>Appropriate color for the upgrade status.</returns>
        public static Color GetStatusColor(bool isAvailable, bool isAffordable, bool isMaxed, bool isSelected = false, bool isInProgress = false)
        {
            if (isInProgress)
                return InProgress();
            else if (isSelected)
                return Selected();
            else if (isMaxed)
                return Maxed();
            else if (isAvailable && isAffordable)
                return Available();
            else if (isAvailable && !isAffordable)
                return Unaffordable();
            else
                return Locked();
        }

        ///<summary>
        ///Gets color based on upgrade status enum.
        ///</summary>
        ///<param name="status">Upgrade status.</param>
        ///<returns>Appropriate color for the upgrade status.</returns>
        public static Color GetStatusColor(UpgradeStatus status)
        {
            return status switch
            {
                UpgradeStatus.Available => Available(),
                UpgradeStatus.Locked => Locked(),
                UpgradeStatus.Maxed => Maxed(),
                UpgradeStatus.Unaffordable => Unaffordable(),
                UpgradeStatus.InProgress => InProgress(),
                UpgradeStatus.Selected => Selected(),
                UpgradeStatus.PrerequisitesNotMet => PrerequisitesNotMet(),
                _ => Locked()
            };
        }
    }

    ///<summary>
    ///Enumeration for upgrade status.
    ///</summary>

}

