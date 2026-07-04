/*
File:    InputData.cs
Purpose:  Input data structure for handling user input.
Features: Contains keyboard, mouse, and other input states.
*/

using System;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine
{
    ///<summary>
    ///Input data structure for handling user input.
    ///</summary>
    public class InputData
    {
        ///<summary>
        ///Mouse position.
        ///</summary>
        public Vector3 MousePosition { get; set; }

        ///<summary>
        ///Mouse delta movement.
        ///</summary>
        public Vector3 MouseDelta { get; set; }

        ///<summary>
        ///Mouse wheel delta.
        ///</summary>
        public float MouseWheelDelta { get; set; }
        //<summary>
        ///Indicates that the player performed a selection action during this frame.
        ///This represents the semantic “select” input mapped from mouse, keyboard, or controller.
        ///</summary>
        public bool IsSelectPressed { get; set; }

        ///<summary>
        ///Whether left mouse button is pressed.
        ///</summary>
        public bool LeftMousePressed { get; set; }

        ///<summary>
        ///Whether right mouse button is pressed.
        ///</summary>
        public bool RightMousePressed { get; set; }

        ///<summary>
        ///Whether middle mouse button is pressed.
        ///</summary>
        public bool MiddleMousePressed { get; set; }

        ///<summary>
        ///Whether up arrow or W key is pressed.
        ///</summary>
        public bool IsUpPressed { get; set; }

        ///<summary>
        ///Whether down arrow or S key is pressed.
        ///</summary>
        public bool IsDownPressed { get; set; }

        ///<summary>
        ///Whether left arrow or A key is pressed.
        ///</summary>
        public bool IsLeftPressed { get; set; }

        ///<summary>
        ///Whether right arrow or D key is pressed.
        ///</summary>
        public bool IsRightPressed { get; set; }

        ///<summary>
        ///Whether Enter key is pressed.
        ///</summary>
        public bool IsEnterPressed { get; set; }

        ///<summary>
        ///Whether Escape key is pressed.
        ///</summary>
        public bool IsEscapePressed { get; set; }

        ///<summary>
        ///Whether Space key is pressed.
        ///</summary>
        public bool IsSpacePressed { get; set; }

        ///<summary>
        ///Whether F5 key is pressed.
        ///</summary>
        public bool F5Pressed { get; set; }

        ///<summary>
        ///Whether any key is pressed.
        ///</summary>
        public bool AnyKeyPressed { get; set; }

        ///<summary>
        ///Creates a new InputData instance.
        ///</summary>
        public InputData()
        {
            MousePosition = Vector3.Zero;
            MouseDelta = Vector3.Zero;
            MouseWheelDelta = 0f;
            LeftMousePressed = false;
            RightMousePressed = false;
            MiddleMousePressed = false;
            IsUpPressed = false;
            IsDownPressed = false;
            IsLeftPressed = false;
            IsRightPressed = false;
            IsEnterPressed = false;
            IsEscapePressed = false;
            IsSpacePressed = false;
            F5Pressed = false;
            AnyKeyPressed = false;
        }
    }
}
