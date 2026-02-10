/*
    File:    Button.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        UI button element with text label and optional click action.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using UIButton = SASZombieAssaultTD.Engine.Systems.UI.Button;
using UIPanel = SASZombieAssaultTD.Engine.Systems.UI.Panel;
using System;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public class Button : UIElementBase
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public Action? OnClick { get; set; }

        public override void Render(IRenderContext context)
        {
            // placeholder rendering
        }
    }
}