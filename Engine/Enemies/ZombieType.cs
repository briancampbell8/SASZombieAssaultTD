using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Zombie type enumeration for different enemy categories.
    ///</summary>
    public enum ZombieType
    {
        ///<summary>
        ///Basic zombie
        ///</summary>
        Basic,
        
        ///<summary>
        ///Fast zombie
        ///</summary>
        Fast,
        
        ///<summary>
        ///Tank zombie (high health)
        ///</summary>
        Tank,
        
        ///<summary>
        ///Spitter zombie (ranged attack)
        ///</summary>
        Spitter,
        
        ///<summary>
        ///Boss zombie
        ///</summary>
        Boss,
        
        ///<summary>
        ///Swarm zombie (appears in groups)
        ///</summary>
        Swarm,
        
        ///<summary>
        ///Armored zombie (damage resistance)
        ///</summary>
        Armored,
        
        ///<summary>
        ///Toxic zombie (damage over time)
        ///</summary>
        Toxic,

        ///<summary>
        ///Shadow zombie (stealthy)
        ///</summary>
        Shadow,

        ///<summary>
        ///Robot Clown zombie (explosive)
        ///</summary>
        RobotClown,

        ///<summary>
        ///Devastator zombie (high damage)
        ///</summary>
        Devastator
    }
}
