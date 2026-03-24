/*
File:    Button.cs
Author:  BDC
Created: 2026-02-10

Purpose:
UI button element with text label and optional click action.

Notes:
<Any architectural notes, constraints, or special behaviors.>

*/
using SASZombieAssaultTD.Engine.Rendering;
using System;

namespace SASZombieAssaultTD.Engine.UI
{
    public class Button : UIElementBase
    {
        public string ButtonId { get; set; } = string.Empty;
        public bool ButtonVisible { get; set; } = true;
        public string Text { get; set; } = string.Empty;
        public Action? OnClick { get; set; }

        public override void Render(IRenderContext context)
        {
            // placeholder rendering
        }
    }
}


