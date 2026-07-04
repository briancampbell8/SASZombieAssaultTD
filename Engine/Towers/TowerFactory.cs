/*
File:    TowerFactory.cs
Purpose: Factory for creating tower instances.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
using SASZombieAssaultTD.Engine.VectorMath;
namespace SASZombieAssaultTD.Engine.Towers
//
{
    ///<summary>
    ///Factory for creating tower instances.
    ///</summary>
    public static class TowerFactory
    {
        ///<summary>
        ///Creates a tower of the specified type at the given position.
        ///</summary>
        public static object CreateTower(string towerType, Vector3 position)
        {
            Dlogger.Log(LogSubsystems.Towers, LogLevel.Info, $"TowerFactory: Creating {towerType} at {position}");
            //Placeholder implementation
            return new object();
        }
    }
}
