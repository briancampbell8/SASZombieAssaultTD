// =====================================================================================================
//  FILE: TowerSaveData.cs
//  PATH: Engine/Towers/Save/TowerSaveData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the tower save data model used by the GameSave subsystem.
//      Provides a compact, serializable representation of tower state for capture, backup, and restore.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers.Save
{
    public class TowerSaveData
    {
        public int TowerCount { get; set; }
        public int TotalValue { get; set; }

        public List<string> TowerTypes { get; set; }
        public Dictionary<string, Vector3> TowerPositions { get; set; }
        public Dictionary<string, int> TowerLevels { get; set; }

        public TowerSaveData()
        {
            TowerTypes = new List<string>();
            TowerPositions = new Dictionary<string, Vector3>();
            TowerLevels = new Dictionary<string, int>();
        }

        public TowerSaveData Clone()
        {
            return new TowerSaveData
            {
                TowerCount = TowerCount,
                TotalValue = TotalValue,
                TowerTypes = new List<string>(TowerTypes),
                TowerPositions = new Dictionary<string, Vector3>(TowerPositions),
                TowerLevels = new Dictionary<string, int>(TowerLevels)
            };
        }
    }
}
