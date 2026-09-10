// ====================================================================================================
//  FILE: TowerInfoPanel.cs
//  PATH: ./Engine/UI/HUD/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering for Tower Information displays.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    internal class TowerInfoSection
    {
        private string v1;
        private float v2;

        public TowerInfoSection(string v1, float v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        internal void AddContent(string v)
        {
            throw new NotImplementedException();
        }

        internal void ClearContent()
        {
            throw new NotImplementedException();
        }
    }
}
