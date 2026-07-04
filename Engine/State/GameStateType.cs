using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///Enumeration of all possible game states.
    ///P20-02-04: Defines the state identifiers used by the state machine.
    ///</summary>
    public enum GameStateType
    {
        ///<summary>
        ///Initial boot state - loads core resources and transitions to MainMenu.
        ///</summary>
        Boot,
        
        ///<summary>
        ///Main menu state - handles menu navigation and game start/exit.
        ///</summary>
        MainMenu,
        
        ///<summary>
        ///Active gameplay state - handles game logic, input, and world updates.
        ///</summary>
        Gameplay,
        
        ///<summary>
        ///Paused state - freezes gameplay and handles pause menu options.
        ///</summary>
        Paused
    }
}




