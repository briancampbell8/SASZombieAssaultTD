// =====================================================================================================
//  FILE: UIEnums.cs
//  PATH: Engine/UI/UIEnums.cs
//  SUBSYSTEM: UI
//
//  ROLE:
//      Defines all enumerations used within the UI subsystem. Implements the minimal deterministic lifecycle
//      contract for any engine-hosted program. Implemented
//
//  RESPONSIBILITIES:
//      - Provide a strict lifecycle surface: Initialize → Run → Update → Render → Shutdown.
//      - Allow engine hosts (e.g., GameRootMain) to expose deterministic lifecycle entry points.
//      - Serve as the base contract for any future top-level engine program modules.
//      - Support both GPU-context rendering and generic object-based render forwarding.
//
//  NON-RESPONSIBILITIES:
//      - Implementing update or render logic internally (delegated to subsystems).
//      - Managing system registration, asset loading, or state-machine orchestration.
//      - Handling GPU device creation, swap-chain management, or windowing.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces legacy partial lifecycle methods.
//      - GameRootMain implements this interface and delegates lifecycle operations to:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
//      - Includes legacy compatibility signatures (Render(object), Tick(object,...)) for transitional
//        subsystem support, though the GPU-only pipeline uses Render(D3D11Adapter_Core).
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI
{
    public class UIEnums
    {
        //-------------------------------------------------------------------------------------------------
        // UI ANCHOR ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum UIAnchor
        {
            TopLeft,
            TopRight,
            Center,
            BottomLeft,
            BottomRight,
            TopCenter,
            BottomCenter,
            LeftCenter,
            RightCenter,
            MiddleLeft,
            MiddleCenter,
            MiddleRight
            ///<summary>
            ///Anchor to the top-left corner
            ///</summary>

        }
        //-------------------------------------------------------------------------------------------------
        // UI DEBUG PAGE ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum DebugPage
        {
            Finalizer,
            Crosshair,
            RenderOrder,
            HUDBounds,
            EngineStats
        }
        //-------------------------------------------------------------------------------------------------
        // UI INPUT KEY ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum InputKey
        {
            F1,
            F2,
            F3,
            F4,
            F5

        }
        // -------------------------------------------------------------------------------------------------
        //  OVERLAY PAGE ENUMERATION
        // -------------------------------------------------------------------------------------------------

        public enum OverlayPage
        {
            Finalizer,
            Crosshair,
            RenderOrder,
            HUDBounds,
            EngineStats
        }
        public enum UIAlignment
        {
            ///<summary>
            ///Align to the left edge
            ///</summary>
            Left,

            ///<summary>
            ///Align to the center horizontally
            ///</summary>
            Center,

            ///<summary>
            ///Align to the right edge
            ///</summary>
            Right
        }
        //-------------------------------------------------------------------------------------------------
        // UI VERTICAL ALIGNMENT ENUMERATION
        //-------------------------------------------------------------------------------------------------
        ///<summary>
        ///Vertical alignment options for UI elements
        ///</summary>
        public enum UIVerticalAlignment
        {
            ///<summary>
            ///Align to the top edge
            ///</summary>
            Top,

            ///<summary>
            ///Align to the center vertically
            ///</summary>
            Middle,

            ///<summary>
            ///Align to the bottom edge
            ///</summary>
            Bottom
        }

        ///<summary>
        ///Anchor points for UI elements relative to their parent
        ///</summary>


        /// <summary>
        /// Layout direction for UI elements
        /// </summary>
        //------------------------------------------------------------------------------------------------
        // UI LAYOUT DIRECTION ENUMERATION
        //------------------------------------------------------------------------------------------------
        public enum UILayoutDirection
        {
            ///<summary>
            ///Layout elements horizontally
            ///</summary>
            Horizontal,

            ///<summary>
            ///Layout elements vertically
            ///</summary>
            Vertical
        }
        //-------------------------------------------------------------------------------------------------
        // UI UPGRADE TEXT TYPE ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum UpgradeTextType
        {
            Title,
            Description,
            Cost,
            PositiveStat,
            NegativeStat,
            Requirement,
            Locked,
            Maxed,
            SpecialAbility,
            Warning,
            Info
        }
        // --------------------------------------------------------------------
        // QUALITY DECISION ENUMERATION
        // --------------------------------------------------------------------
        public enum QualityDecision
        {
            None,
            Increase,
            Decrease
        }
        //-------------------------------------------------------------------------------------------------
        // UI UPGRADE STATUS ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum UpgradeStatus
        {
            Available,
            Locked,
            Maxed,
            Unaffordable,
            InProgress,
            Selected,
            PrerequisitesNotMet
        }
        //-------------------------------------------------------------------------------------------------
        // UI DRAW CALL TYPE ENUMERATION
        //-------------------------------------------------------------------------------------------------
        public enum UIDrawCallType
        {
            Rectangle,
            Text,
            Line,
            Circle,
            Triangle
        }
    }
}
