// =====================================================================================================
//  FILE: IGameStateMachine.cs
//  PATH: Engine/State/IGameStateMachine.cs
//  SUBSYSTEM: State
// =====================================================================================================
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering.UI;

namespace SASZombieAssaultTD.Engine.State
{
    public interface IGameStateMachine
    {
        void StartGame();
        void PauseGame();
        void ResumeGame();
        void ResetGame();
        void GameOver();

        void Update(float deltaTime);

        void Shutdown();
        void NextLevel();
        string GetCurrentState();
        bool IsPaused();
        bool IsRunning();
        bool IsGameOver();

        // Render entry point - expose low-level GPU adapter render.
        void Render(D3D11Adapter_Core renderContext);
    }
}
