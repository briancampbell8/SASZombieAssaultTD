using SASZombieAssaultTD.Engine.ECS;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    //Numeric standardization: All continuous values use double for precision
    ///<summary>
    ///Represents the damage dealt by an entity.
    ///</summary>
    public class DamageComponent : BaseComponent
    {
        public double Damage { get; set; }
    }
}
