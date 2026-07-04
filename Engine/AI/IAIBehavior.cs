//CANONICAL IAIBehavior — CLEAN VERSION
//REASON: Decompiler added invalid members (DeltaTime, Owner, GetTarget, SetTarget)
//STATUS: Restored to original engine design
//DATE: 2026‑05‑14

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.AI
{
    public interface IAIBehavior
    {
        ///<summary>
        ///Executes one tick of this behavior.
        ///</summary>
        ///<param name="context">Shared AI context containing state and timing information.</param>
        void Tick(AIContext context);
    }
}
