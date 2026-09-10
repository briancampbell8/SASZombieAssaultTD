// =====================================================================================================
// FILE: MainMenuScene.cs
// PATH: Engine/Scenes/MainMenuScene.cs
// SUBSYSTEM: Scene System / Main Menu Scene
//
// ROLE:
//     Deterministic main-menu scene providing the foundational UI surface for game startup navigation.
//     Responsible for constructing static menu UI elements, routing basic input, and rendering the
//     menu background + UI elements.
// =====================================================================================================

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.UI.Elements;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class MainMenuScene : BaseScene
    {
        private readonly List<UIElementBase> _menuElements = new();
        private ImageElement _backgroundElement;
        private int _selectedOption;
        private bool _initialized;
        private bool _started;

        public Button element { get; private set; }

        public override void Initialize()
        {
            if (_initialized)
                return;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] Initialize: Constructing main menu UI elements.");

            CreateBackgroundElement();
            CreateMenuElements();
            _initialized = true;
        }

        internal override void OnLoad()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] OnLoad: Scene loaded into SceneManager.");
        }

        internal override void OnStart()
        {
            if (_started)
                return;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] OnStart: Scene entering active state.");
            _selectedOption = 0;
            _started = true;
        }

        public override void Update(float deltaTime)
        {
            if (!_initialized || !_started)
                return;

            UpdateMenuAnimations(deltaTime);
            ProcessMenuInput();
        }

        private void UpdateMenuAnimations(float deltaTime)
        {
            if (element is Button btn)
                btn.Update(deltaTime);
        }

        private void ProcessMenuInput()
        {
            if (InputRouter == null)
                return;
            _selectedOption = 0;
        }

        // -------------------------------------------------------------------------------------------------
        // RENDER PIPELINE HOOKS
        // -------------------------------------------------------------------------------------------------

        // FIX: Swapped 'override' for 'new' to cleanly satisfy the CS0506 compiler rule
        public new void Render(D3D11Adapter_Core adapter)
        {
            Render(adapter, 1f / 60f);
        }

        public void Render(D3D11Adapter_Core adapter, float deltaTime)
        {
            if (!_initialized || adapter == null)
                return;

            RenderMenuBackground(adapter, deltaTime);
            RenderMenuElements(adapter, deltaTime);
        }

        private void RenderMenuBackground(D3D11Adapter_Core adapter, float deltaTime)
        {
            // Original clear: very dark navy
            // adapter.Clear(new Color(20, 20, 40));

            // DEBUG: draw a full-screen red rectangle so we can SEE the render path
            adapter.FillRectangle(new Rectangle(0, 0, 1280, 720), Color.Red);

            if (_backgroundElement != null)
                _backgroundElement.Render(adapter, deltaTime);
        }

        private void RenderMenuElements(D3D11Adapter_Core adapter, float deltaTime)
        {
            foreach (var item in _menuElements)
            {
                item.Render(adapter, deltaTime);
            }
        }

        // -------------------------------------------------------------------------------------------------
        // MENU ELEMENT CREATION
        // -------------------------------------------------------------------------------------------------

        private void CreateBackgroundElement()
        {
            _backgroundElement = new ImageElement
            {
                TextureName = "MainMenu.png",
                Position = new Point(0, 0),
                Size = new CoreSize.Size(1280, 720)
            };
        }

        private void CreateMenuElements()
        {
            _menuElements.Clear();

            Button[] buttons = new[]
            {
                new Button(
                    buttonId: "StartGameButton",
                    visible: true,
                    text: "Start Game",
                    onClick: OnStartGameClick,
                    position: new Vector2(400, 200),
                    size: new Vector2(200, 50)
                ),

                new Button(
                    buttonId: "OptionsButton",
                    visible: true,
                    text: "Options",
                    onClick: OnOptionsClick,
                    position: new Vector2(400, 270),
                    size: new Vector2(200, 50)
                ),

                new Button(
                    buttonId: "QuitButton",
                    visible: true,
                    text: "Quit",
                    onClick: OnQuitClick,
                    position: new Vector2(400, 340),
                    size: new Vector2(200, 50)
                )
            };

            _menuElements.AddRange(buttons);
        }

        // -------------------------------------------------------------------------------------------------
        // MENU ACTIONS
        // -------------------------------------------------------------------------------------------------

        private void OnStartGameClick()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] Start Game clicked — transitioning to gameplay scene.");
        }

        private void OnOptionsClick()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] Options clicked — opening options menu.");
        }

        private void OnQuitClick()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] Quit clicked — initiating shutdown.");
        }

        // -------------------------------------------------------------------------------------------------
        // CLEANUP
        // -------------------------------------------------------------------------------------------------

        public override void Cleanup()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MainMenuScene] Cleanup: Releasing menu resources.");

            _menuElements.Clear();
            _backgroundElement = null;

            _selectedOption = 0;
            _initialized = false;
            _started = false;
        }
    }
}
