// ====================================================================================================
//  FILE: Menus.cs
//  PATH: ./Engine/UI/Menus/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide Add() behavior for the UI subsystem.
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide Update() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class Menus
    {
        private readonly List<UIElementBase> _elements = new List<UIElementBase>();

        public void Add(UIElementBase element)
        {
            if (element != null)
                _elements.Add(element);
        }

        public void Update(float deltaTime)
        {
            foreach (var element in _elements)
                element.Update(deltaTime);
        }

        public void Render(D3D11Adapter_Core context)
        {
            foreach (var element in _elements)
                element.RenderWithContext(context);
        }

        //Compatibility overloads for old scene code
        public void Update(UIElementBase _, TimeSpan deltaTime)
        {
            Update((float)deltaTime.TotalSeconds);
        }
    }
}



