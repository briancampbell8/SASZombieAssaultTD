/*
File:    TowerDatabase.cs
Purpose: Database for tower information.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
namespace SASZombieAssaultTD.Engine.Towers
//
{
    ///<summary>
    ///Database for tower information.
    ///</summary>
    public static class TowerDatabase
    {
        ///<summary>
        ///Gets tower data for the specified tower type.
        ///</summary>
        public static object GetTowerData(string towerType)
        {
            Dlogger.Log(LogSubsystems.Towers, LogLevel.Info, $"TowerDatabase: Getting data for {towerType}");
            //Placeholder implementation
            return new object();
        }
    }
}
