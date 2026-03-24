/*
File:    IGameStateMachine.cs
Path:    Engine/Interfaces/IGameStateMachine.cs
Purpose:   P11-09-02 - Core interface for game state machine management.
           Defines the contract for game state transitions and lifecycle management.

Role:      Essential game state machine interface for engine state coordination.
           - Provides game state transition management and control
           - Handles state initialization, update, and shutdown sequences
           - Manages state stack for nested state scenarios
           - Integrates with GameRoot for proper engine state coordination
           - Supports state-specific rendering and input handling

Features:   Game state transition management with proper validation.
           State lifecycle management with initialization and shutdown.
           State stack support for nested and hierarchical states.
           Thread-safe state operations for concurrent access.
           Integration with GameRoot for engine-wide state coordination.
           Support for state-specific update and render phases.

Notes:      This interface is implemented by concrete game state machines.
           State transitions are validated and logged for debugging.
           All state operations are designed for real-time performance.
           Interface supports both push and pop state operations.
           State machine integrates seamlessly with GameRoot coordination.

*/

using SASZombieAssaultTD.Engine.VectorMath;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Defines the contract for game state machine management.
    /// Implements P11-09-02: Game state transition management and lifecycle control.
    /// </summary>
    public interface IGameStateMachine
    {
        /// <summary>
        /// Gets the current active game state.
        /// </summary>
        IGameState CurrentState { get; }

        /// <summary>
        /// Gets whether the state machine is currently initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the number of states in the state stack.
        /// </summary>
        int StateCount { get; }

        /// <summary>
        /// Initializes the state machine with the initial state.
        /// </summary>
        /// <param name="initialState">The initial game state to start with.</param>
        Task InitializeAsync(IGameState initialState);

        /// <summary>
        /// Shuts down the state machine and all active states.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        /// Updates the current state and handles state transitions.
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds. Defaults to 0.016 (60 FPS).</param>
        Task UpdateAsync(float deltaTime = 0.016f);

        /// <summary>
        /// Renders the current state.
        /// </summary>
        /// <param name="renderContext">The render context for rendering operations.</param>
        void Render(IRenderContext renderContext);

        /// <summary>
        /// Pushes a new state onto the state stack.
        /// </summary>
        /// <param name="state">The state to push onto the stack.</param>
        Task PushStateAsync(IGameState state);

        /// <summary>
        /// Pushes multiple states onto the state stack.
        /// </summary>
        /// <param name="states">The states to push onto the stack.</param>
        Task PushStatesAsync(IEnumerable<IGameState> states);

        /// <summary>
        /// Pops the current state from the state stack.
        /// </summary>
        Task PopStateAsync();

        /// <summary>
        /// Pops multiple states from the state stack.
        /// </summary>
        /// <param name="count">The number of states to pop.</param>
        Task PopStatesAsync(int count);

        /// <summary>
        /// Changes to a new state, replacing the current state.
        /// </summary>
        /// <param name="state">The new state to change to.</param>
        Task ChangeStateAsync(IGameState state);

        /// <summary>
        /// Gets all states in the state stack.
        /// </summary>
        /// <returns>ReadOnly collection of states in the stack.</returns>
        IReadOnlyList<IGameState> GetStateStack();
    }

    /// <summary>
    /// Defines the contract for individual game states.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Gets the name of the game state.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets whether the state is currently active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets whether the state is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        Task EnterAsync();

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        Task ExitAsync();

        /// <summary>
        /// Called when the state is paused.
        /// </summary>
        Task PauseAsync();

        /// <summary>
        /// Called when the state is resumed.
        /// </summary>
        Task ResumeAsync();

        /// <summary>
        /// Updates the state logic.
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds. Defaults to 0.016 (60 FPS).</param>
        Task UpdateAsync(float deltaTime = 0.016f);

        /// <summary>
        /// Renders the state.
        /// </summary>
        /// <param name="renderContext">The render context for rendering operations.</param>
        void Render(IRenderContext renderContext);

        /// <summary>
        /// Handles input for the state.
        /// </summary>
        /// <param name="inputState">The current input state.</param>
        void HandleInput(IInputState inputState);
    }

    /// <summary>
    /// Defines the contract for render context.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Gets the current render target.
        /// </summary>
        IRenderTarget RenderTarget { get; }

        /// <summary>
        /// Gets the current camera.
        /// </summary>
        ICamera Camera { get; }

        /// <summary>
        /// Gets the current lighting configuration.
        /// </summary>
        ILightingConfiguration Lighting { get; }

        /// <summary>
        /// Begins a new render frame.
        /// </summary>
        void BeginFrame();

        /// <summary>
        /// Ends the current render frame.
        /// </summary>
        void EndFrame();

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="color">The clear color.</param>
        void Clear(System.Drawing.Color color);

        /// <summary>
        /// Sets the current camera.
        /// </summary>
        /// <param name="camera">The camera to set.</param>
        void SetCamera(ICamera camera);

        /// <summary>
        /// Sets current render target.
        /// </summary>
        /// <param name="renderTarget">The render target to set.</param>
        void SetRenderTarget(IRenderTarget renderTarget);

        /// <summary>
        /// Initializes the render context.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shuts down the render context.
        /// </summary>
        void Shutdown();
    }

    /// <summary>
    /// Defines the contract for render targets.
    /// </summary>
    public interface IRenderTarget
    {
        /// <summary>
        /// Gets the width of the render target.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the render target.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the pixel format of the render target.
        /// </summary>
        PixelFormat Format { get; }

        /// <summary>
        /// Gets the texture data from the render target.
        /// </summary>
        /// <returns>The texture data.</returns>
        byte[] GetTextureData();
    }

    /// <summary>
    /// Defines the contract for cameras.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Gets the camera position.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets the camera rotation.
        /// </summary>
        Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets the camera field of view.
        /// </summary>
        float FieldOfView { get; set; }

        /// <summary>
        /// Gets the camera aspect ratio.
        /// </summary>
        float AspectRatio { get; set; }

        /// <summary>
        /// Gets the camera near plane distance.
        /// </summary>
        float NearPlane { get; set; }

        /// <summary>
        /// Gets the camera far plane distance.
        /// </summary>
        float FarPlane { get; set; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        Matrix4x4 ViewMatrix { get; }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        Matrix4x4 ProjectionMatrix { get; }

        /// <summary>
        /// Gets the combined view-projection matrix.
        /// </summary>
        Matrix4x4 ViewProjectionMatrix { get; }

        /// <summary>
        /// Looks at a specific target position.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        void LookAt(Vector3 target);

        /// <summary>
        /// Projects a world position to screen coordinates.
        /// </summary>
        /// <param name="worldPosition">The world position to project.</param>
        /// <returns>The screen coordinates.</returns>
        Vector3 ProjectToScreen(Vector3 worldPosition);

        /// <summary>
        /// Unprojects a screen position to world coordinates.
        /// </summary>
        /// <param name="screenPosition">The screen position to unproject.</param>
        /// <returns>The world coordinates.</returns>
        Vector3 UnprojectFromScreen(Vector3 screenPosition);
    }

    /// <summary>
    /// Defines the contract for lighting configuration.
    /// </summary>
    public interface ILightingConfiguration
    {
        /// <summary>
        /// Gets the ambient light color.
        /// </summary>
        Vector3 AmbientColor { get; set; }

        /// <summary>
        /// Gets the ambient light intensity.
        /// </summary>
        float AmbientIntensity { get; set; }

        /// <summary>
        /// Gets the directional light.
        /// </summary>
        IDirectionalLight DirectionalLight { get; set; }

        /// <summary>
        /// Gets the collection of point lights.
        /// </summary>
        IReadOnlyList<IPointLight> PointLights { get; }

        /// <summary>
        /// Gets the collection of spot lights.
        /// </summary>
        IReadOnlyList<ISpotLight> SpotLights { get; }

        /// <summary>
        /// Adds a point light to the configuration.
        /// </summary>
        /// <param name="light">The point light to add.</param>
        void AddPointLight(IPointLight light);

        /// <summary>
        /// Removes a point light from the configuration.
        /// </summary>
        /// <param name="light">The point light to remove.</param>
        void RemovePointLight(IPointLight light);

        /// <summary>
        /// Adds a spot light to the configuration.
        /// </summary>
        /// <param name="light">The spot light to add.</param>
        void AddSpotLight(ISpotLight light);

        /// <summary>
        /// Removes a spot light from the configuration.
        /// </summary>
        /// <param name="light">The spot light to remove.</param>
        void RemoveSpotLight(ISpotLight light);
    }

    /// <summary>
    /// Defines the contract for directional lights.
    /// </summary>
    public interface IDirectionalLight
    {
        /// <summary>
        /// Gets the light direction.
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// Gets the light color.
        /// </summary>
        Vector3 Color { get; set; }

        /// <summary>
        /// Gets the light intensity.
        /// </summary>
        float Intensity { get; set; }
    }

    /// <summary>
    /// Defines the contract for point lights.
    /// </summary>
    public interface IPointLight
    {
        /// <summary>
        /// Gets the light position.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets the light color.
        /// </summary>
        Vector3 Color { get; set; }

        /// <summary>
        /// Gets the light intensity.
        /// </summary>
        float Intensity { get; set; }

        /// <summary>
        /// Gets the light range.
        /// </summary>
        float Range { get; set; }

        /// <summary>
        /// Gets the light attenuation.
        /// </summary>
        Vector3 Attenuation { get; set; }
    }

    /// <summary>
    /// Defines the contract for spot lights.
    /// </summary>
    public interface ISpotLight
    {
        /// <summary>
        /// Gets the light position.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets the light direction.
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// Gets the light color.
        /// </summary>
        Vector3 Color { get; set; }

        /// <summary>
        /// Gets the light intensity.
        /// </summary>
        float Intensity { get; set; }

        /// <summary>
        /// Gets the light range.
        /// </summary>
        float Range { get; set; }

        /// <summary>
        /// Gets the light cone angle.
        /// </summary>
        float ConeAngle { get; set; }

        /// <summary>
        /// Gets the light attenuation.
        /// </summary>
        Vector3 Attenuation { get; set; }
    }

    /// <summary>
    /// Defines the contract for input state.
    /// </summary>
    public interface IInputState
    {
        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        Vector3 MousePosition { get; }

        /// <summary>
        /// Gets the mouse delta.
        /// </summary>
        Vector3 MouseDelta { get; }

        /// <summary>
        /// Gets the mouse wheel delta.
        /// </summary>
        float MouseWheelDelta { get; }

        /// <summary>
        /// Gets whether a mouse button is pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>True if the button is pressed.</returns>
        bool IsMouseButtonPressed(MouseButton button);

        /// <summary>
        /// Gets whether a key is pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is pressed.</returns>
        bool IsKeyPressed(Key key);

        /// <summary>
        /// Gets whether a key was just pressed this frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed.</returns>
        bool IsKeyJustPressed(Key key);

        /// <summary>
        /// Gets whether a key was just released this frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just released.</returns>
        bool IsKeyJustReleased(Key key);
    }

    /// <summary>
    /// Pixel format enumeration.
    /// </summary>
    public enum PixelFormat
    {
        R8G8B8A8,
        R8G8B8,
        R16G16B16A16,
        R32G32B32A32
    }

    /// <summary>
    /// Mouse button enumeration.
    /// </summary>
    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        X1,
        X2
    }

    /// <summary>
    /// Key enumeration.
    /// </summary>
    public enum Key
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Num0, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9,
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
        Space, Enter, Escape, Tab, Backspace, Delete,
        Left, Right, Up, Down,
        LeftShift, RightShift, LeftCtrl, RightCtrl, LeftAlt, RightAlt
    }

    /// <summary>
    /// Quaternion structure.
    /// </summary>
    public struct Quaternion
    {
        public float X, Y, Z, W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x; Y = y; Z = z; W = w;
        }

        public static Quaternion Identity => new Quaternion(0, 0, 0, 1);
    }

    /// <summary>
    /// 4x4 matrix structure.
    /// </summary>
    public struct Matrix4x4
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;

        public static Matrix4x4 Identity => new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public Matrix4x4(float m11, float m12, float m13, float m14,
                         float m21, float m22, float m23, float m24,
                         float m31, float m32, float m33, float m34,
                         float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }
    }
}

