// =========================================================
//  FILE: SceneTransition.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

using System;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
namespace SASZombieAssaultTD.Engine.Scenes
{
    ///<summary>
    ///Types of scene transitions.
    ///</summary>
    public enum SceneTransitionType
    {
        ///<summary>
        ///No transition (instant switch).
        ///</summary>
        None,

        ///<summary>
        ///Fade to black then fade in.
        ///</summary>
        Fade,

        ///<summary>
        ///Fade to white then fade in.
        ///</summary>
        FadeWhite,

        ///<summary>
        ///Slide from left.
        ///</summary>
        SlideLeft,

        ///<summary>
        ///Slide from right.
        ///</summary>
        SlideRight,

        ///<summary>
        ///Slide from top.
        ///</summary>
        SlideUp,

        ///<summary>
        ///Slide from bottom.
        ///</summary>
        SlideDown
    }

    ///<summary>
    ///Scene transition for smooth visual transitions between scenes.
    ///P120-06: Implements transition effects with duration control and callbacks.
    ///</summary>
    public class SceneTransition
    {
        private SceneTransitionType _transitionType;
        private float _duration;
        private float _elapsedTime;
        private bool _isPlaying;
        private BaseScene? _fromScene;
        private BaseScene? _toScene;
        private Action? _onComplete;
        private Action<float>? _onProgress;

        ///<summary>
        ///Gets the transition type.
        ///</summary>
        public SceneTransitionType TransitionType => _transitionType;

        ///<summary>
        ///Gets the transition duration in seconds.
        ///</summary>
        public float Duration => _duration;

        ///<summary>
        ///Gets the elapsed time since transition started.
        ///</summary>
        public float ElapsedTime => _elapsedTime;

        ///<summary>
        ///Gets whether the transition is currently playing.
        ///</summary>
        public bool IsPlaying => _isPlaying;

        ///<summary>
        ///Gets the transition progress (0.0 to 1.0).
        ///</summary>
        public float Progress => _duration > 0 ? _elapsedTime / _duration : 1.0f;

        ///<summary>
        ///Event fired when transition starts.
        ///</summary>
        public event Action? OnTransitionStarted;

        ///<summary>
        ///Event fired when transition completes.
        ///</summary>
        public event Action? OnTransitionCompleted;

        ///<summary>
        ///Initializes a new scene transition.
        ///</summary>
        ///<param name="transitionType">The type of transition.</param>
        ///<param name="duration">The duration in seconds.</param>
        public SceneTransition(SceneTransitionType transitionType, float duration = 1.0f)
        {
            _transitionType = transitionType;
            _duration = System.Math.Max(0.1f, duration);
            _elapsedTime = 0f;
            _isPlaying = false;

            Dlogger.Log("Info", $"SceneTransition: Created {_transitionType} transition with duration {_duration:F2}s");
        }

        ///<summary>
        ///Starts the transition between two scenes.
        ///</summary>
        ///<param name="fromScene">The scene to transition from.</param>
        ///<param name="toScene">The scene to transition to.</param>
        ///<param name="onComplete">Callback when transition completes.</param>
        public void Execute(BaseScene? fromScene, BaseScene toScene, Action? onComplete = null)
        {
            if (_isPlaying)
            {
                Dlogger.Log("Warning", "SceneTransition: Cannot start transition - already playing");
                return;
            }

            _fromScene = fromScene;
            _toScene = toScene ?? throw new ArgumentNullException(nameof(toScene));
            _onComplete = onComplete;
            _elapsedTime = 0f;
            _isPlaying = true;

            //Exit from scene
            _fromScene?.OnExit();

            Dlogger.Log("Info", $"SceneTransition: Starting {_transitionType} transition from '{fromScene?.GetType().Name ?? "None"}' to '{toScene.GetType().Name}'");
            OnTransitionStarted?.Invoke();
        }

        ///<summary>
        ///Updates the transition.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        ///<returns>True if transition is still playing, false if complete.</returns>
        public bool Update(float deltaTime)
        {
            if (!_isPlaying)
            {
                return false;
            }

            _elapsedTime += deltaTime;

            //Update progress callback
            _onProgress?.Invoke(Progress);

            //Check if transition is complete
            if (_elapsedTime >= _duration)
            {
                Complete();
                return false;
            }

            return true;
        }

        ///<summary>
        ///Completes the transition.
        ///</summary>
        private void Complete()
        {
            _isPlaying = false;
            _elapsedTime = _duration;

            //Enter to scene
            _toScene?.OnEnter();

            Dlogger.Log("Info", $"SceneTransition: Completed {_transitionType} transition");
            OnTransitionCompleted?.Invoke();
            _onComplete?.Invoke();
        }

        ///<summary>
        ///Cancels the transition.
        ///</summary>
        public void Cancel()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;
            Dlogger.Log("Warning", $"SceneTransition: Cancelled {_transitionType} transition");
        }

        ///<summary>
        ///Sets the progress callback.
        ///</summary>
        ///<param name="onProgress">Callback with progress value (0.0 to 1.0).</param>
        public void SetProgressCallback(Action<float>? onProgress)
        {
            _onProgress = onProgress;
        }

        ///<summary>
        ///Gets the current fade alpha value (for fade transitions).
        ///</summary>
        ///<returns>Alpha value from 0.0 to 1.0.</returns>
        public float GetFadeAlpha()
        {
            if (_transitionType != SceneTransitionType.Fade && _transitionType != SceneTransitionType.FadeWhite)
            {
                return 0f;
            }

            //Fade out first half, fade in second half
            var halfDuration = _duration * 0.5f;
            if (_elapsedTime < halfDuration)
            {
                //Fading out
                return _elapsedTime / halfDuration;
            }
            else
            {
                //Fading in
                return 1.0f - ((_elapsedTime - halfDuration) / halfDuration);
            }
        }

        ///<summary>
        ///Gets the current slide offset (for slide transitions).
        ///</summary>
        ///<returns>Offset value from 0.0 to 1.0.</returns>
        public float GetSlideOffset()
        {
            if (!IsSlideTransition())
            {
                return 0f;
            }

            return 1.0f - Progress;
        }

        ///<summary>
        ///Checks if this is a slide transition.
        ///</summary>
        ///<returns>True if transition type is a slide variant.</returns>
        private bool IsSlideTransition()
        {
            return _transitionType == SceneTransitionType.SlideLeft ||
                   _transitionType == SceneTransitionType.SlideRight ||
                   _transitionType == SceneTransitionType.SlideUp ||
                   _transitionType == SceneTransitionType.SlideDown;
        }

        ///<summary>
        ///Gets transition information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"SceneTransition: Type={_transitionType}, Duration={_duration:F2}s, " +
                   $"Elapsed={_elapsedTime:F2}s, Progress={Progress:P2}, Playing={_isPlaying}";
        }
    }
}
