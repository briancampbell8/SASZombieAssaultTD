namespace SASZombieAssaultTD.Engine.Dictionary
{
    public enum EnemyType
    {
        Basic, Fast, Tank, Swift, Armored, Flying, Boss,
        SwiftZombie, HeavyZombie, BomberZombie, SprinterZombie, BloaterZombie, MamushkaZombie, RuinZombie, ShadowZombie, RobotClownZombie, DevastatorZombie,
        BasicZombie, FastZombie, TankZombie, SwarmZombie, SpitterZombie
    }

    public enum ZombieType
    {
        Swift, Heavy, Bomber, Sprinter, Bloater, Mamushka, Ruin, Shadow, RobotClown, Devastator,
        Basic, Fast, Tank, Swarm, Spitter
    }

    public enum TowerType
    {
        SniperSAS, 
        Turret, 
        MGLTurret, 
        VickersTurret, 
        Flamethrower, 
        MortarPit, 
        AAATurret, 
        ShotgunTurret, 
        RailgunTurret, 
        TeslaCoil,
        LaserTurret, 
        PulseCannon,
        SASSoldier,
        SpecialTurret,
        PoisonDartTurret, 
        IceTurret, 
        LightningTurret, 
        PlasmaTurret, 
        QuantumTurret
    }

    public enum TerrainType
    {
        Any, Ground, Rooftop
    }

    public enum SpawnPatternType
    {
        Single, Flanking, Grid, V
    }

    public enum SpawnPattern
    {
        Line, Wave, Circle, Cluster, Flanking, Pincer, Single, Grid, Random, Spiral, Cross, Diamond, Square, Triangle, Hourglass, Scattered, Burst, Swarm, Surge, Flood
    }

    public class SpawnPatternParameter
    {
        // Missing properties
        public string Type { get; set; }
        public bool Required { get; set; }
        
        // Alias properties for compatibility
        public string ParameterType { get; set; }
        public bool IsRequired { get; set; }
    }

    public enum CollisionShapeType
    {
        Box, Circle, Polygon, Composite
    }

    public enum DeathType
    {
        Bullet, Fire, Melee, Explosion, Poison, Ice, Lightning, Plasma, Laser, Crush, Fall, Drowning, Acid, Radiation, Magic
    }
}
