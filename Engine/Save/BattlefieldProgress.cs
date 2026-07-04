/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\BattlefieldProgress.cs
Purpose: Battlefield progress data structure for save/load system.
Features: P120 Integration for battlefield-specific progress tracking.
*/

using System;
//


using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
namespace SASZombieAssaultTD.Engine.Save
{
    ///<summary>
    ///Battlefield progress information.
    ///P140-04: P120 Integration for battlefield-specific progress tracking.
    ///</summary>
    public class BattlefieldProgress
    {
        public BattlefieldType Battlefield { get; set; }
        public bool Completed { get; set; }
        public int HighestWave { get; set; }
        public int HighScore { get; set; }
        public DateTime LastPlayed { get; set; }
        public float PlayTime { get; set; }
        public int Attempts { get; set; }

        public BattlefieldProgress(BattlefieldType battlefield)
        {
            Battlefield = battlefield;
            Completed = false;
            HighestWave = 0;
            HighScore = 0;
            LastPlayed = DateTime.UtcNow;
            PlayTime = 0f;
            Attempts = 0;

            DLogger.Log(LogSubsystems.Save, LogLevel.Debug, "Debug", $"BattlefieldProgress: Created for {battlefield.GetDisplayName()}");
        }

        ///<summary>
        ///Updates the battlefield progress.
        ///</summary>
        ///<param name="completed">Whether the battlefield was completed.</param>
        ///<param name="highestWave">The highest wave reached.</param>
        ///<param name="highScore">The high score achieved.</param>
        public void UpdateProgress(bool completed, int highestWave, int highScore)
        {
            Completed = completed;
            HighestWave = System.Math.Max(HighestWave, highestWave);
            HighScore = System.Math.Max(HighScore, highScore);
            Attempts++;
            LastPlayed = DateTime.UtcNow;

            DLogger.Log(
                LogSubsystems.Save,
                LogLevel.Info,
                "Info",
                $"BattlefieldProgress: Updated {Battlefield.GetDisplayName()} - Completed={completed}, " +
                $"HighestWave={HighestWave}, HighScore={HighScore}");


        }

        ///<summary>
        ///Adds play time to the total.
        ///</summary>
        ///<param name="playTime">Play time to add in seconds.</param>
        public void AddPlayTime(float playTime)
        {
            PlayTime += System.Math.Max(0f, playTime);
        }

        ///<summary>
        ///Creates a clone of this battlefield progress.
        ///</summary>
        ///<returns>A new BattlefieldProgress instance with the same values.</returns>
        public BattlefieldProgress Clone()
        {
            return new BattlefieldProgress(Battlefield)
            {
                Completed = Completed,
                HighestWave = HighestWave,
                HighScore = HighScore,
                LastPlayed = LastPlayed,
                PlayTime = PlayTime,
                Attempts = Attempts
            };
        }

        ///<summary>
        ///Gets battlefield progress information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"BattlefieldProgress: {Battlefield.GetDisplayName()}, Completed={Completed}, " +
                   $"HighestWave={HighestWave}, HighScore={HighScore}, Attempts={Attempts}, PlayTime={PlayTime:F1}s";
        }
    }
}
