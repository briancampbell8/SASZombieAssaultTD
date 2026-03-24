/*
File:    AnimationStateVisualization.cs
Path:    Engine/Animation/Visualization/AnimationStateVisualization.cs
Purpose: P11-16-05 - Animation state visualization data and rendering.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.VectorMath;

// FORCE THIS FILE TO USE SYSTEM.DRAWING.COLOR
using DrawingColor = System.Drawing.Color;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    public class AnimationStateVisualization
    {
        private readonly object _visualizationLock = new();

        private int _entityId;
        private string _stateName;
        private Vector3 _position;
        private Vector3 _size = new(1f, 1f, 1f);

        private DrawingColor _color = DrawingColor.White;
        private float _opacity = 1f;
        private bool _isVisible = true;

        private float _animationTime;
        private float _transitionProgress;
        private bool _isTransitioning;
        private string _fromState = string.Empty;
        private string _toState = string.Empty;

        private readonly List<VisualEffect> _visualEffects = new();
        private readonly Dictionary<string, float> _parameters = new();

        private DateTime _lastUpdate = DateTime.Now;

        public int EntityId => _entityId;
        public string StateName => _stateName;
        public Vector3 Position => _position;
        public Vector3 Size => _size;
        public DrawingColor Color => _color;
        public float Opacity => _opacity;
        public bool IsVisible => _isVisible;
        public float AnimationTime => _animationTime;
        public float TransitionProgress => _transitionProgress;
        public bool IsTransitioning => _isTransitioning;
        public string FromState => _fromState;
        public string ToState => _toState;
        public IReadOnlyList<VisualEffect> VisualEffects => _visualEffects.AsReadOnly();
        public IReadOnlyDictionary<string, float> Parameters => _parameters;

        public AnimationStateVisualization(int entityId, string stateName, Vector3 position)
        {
            _entityId = entityId;
            _stateName = stateName;
            _position = position;
        }

        public void UpdateState(string stateName, Vector3 position, float animationTime)
        {
            lock (_visualizationLock)
            {
                _stateName = stateName;
                _position = position;
                _animationTime = animationTime;
                _lastUpdate = DateTime.Now;
                UpdateVisualEffects();
            }
        }

        public void UpdateTransition(string fromState, string toState, float progress)
        {
            lock (_visualizationLock)
            {
                _fromState = fromState;
                _toState = toState;

                _transitionProgress = System.Math.Clamp(progress, 0f, 1f);
                _isTransitioning = _transitionProgress > 0f && _transitionProgress < 1f;

                _lastUpdate = DateTime.Now;
                UpdateTransitionEffects();
            }
        }

        public void SetColor(DrawingColor color)
        {
            lock (_visualizationLock)
            {
                _color = color;
                _lastUpdate = DateTime.Now;
            }
        }

        public void SetSize(Vector3 size)
        {
            lock (_visualizationLock)
            {
                _size = size;
                _lastUpdate = DateTime.Now;
            }
        }

        public void SetOpacity(float opacity)
        {
            lock (_visualizationLock)
            {
                _opacity = System.Math.Clamp(opacity, 0f, 1f);
                _lastUpdate = DateTime.Now;
            }
        }

        public void SetVisible(bool visible)
        {
            lock (_visualizationLock)
            {
                _isVisible = visible;
                _lastUpdate = DateTime.Now;
            }
        }

        public void AddVisualEffect(VisualEffect effect)
        {
            if (effect == null) return;

            lock (_visualizationLock)
            {
                _visualEffects.Add(effect);
                _lastUpdate = DateTime.Now;
            }
        }

        public void RemoveVisualEffect(VisualEffect effect)
        {
            if (effect == null) return;

            lock (_visualizationLock)
            {
                _visualEffects.Remove(effect);
                _lastUpdate = DateTime.Now;
            }
        }

        public void SetParameter(string name, float value)
        {
            if (string.IsNullOrEmpty(name)) return;

            lock (_visualizationLock)
            {
                _parameters[name] = value;
                _lastUpdate = DateTime.Now;
            }
        }

        public float GetParameter(string name, float defaultValue = 0f)
        {
            if (string.IsNullOrEmpty(name)) return defaultValue;

            lock (_visualizationLock)
            {
                return _parameters.TryGetValue(name, out var value) ? value : defaultValue;
            }
        }

        public void Render(IDebugRenderer debugRenderer)
        {
            if (!_isVisible || debugRenderer == null) return;

            lock (_visualizationLock)
            {
                try
                {
                    RenderMainVisualization(debugRenderer);

                    if (_isTransitioning)
                        RenderTransitionVisualization(debugRenderer);

                    RenderVisualEffects(debugRenderer);
                    RenderStateName(debugRenderer);
                }
                catch (Exception ex)
                {
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogError(
                        $"Failed to render animation state visualization: {ex.Message}", ex);
                }
            }
        }

        private void RenderMainVisualization(IDebugRenderer debugRenderer)
        {
            int alpha = (int)(System.Math.Clamp(_opacity, 0f, 1f) * 255f);

            var sphereColor = DrawingColor.FromArgb(alpha, _color.R, _color.G, _color.B);

            debugRenderer.DrawSphere(_position, (int)_size.X, sphereColor);
        }

        private void RenderTransitionVisualization(IDebugRenderer debugRenderer)
        {
            var barPosition = _position + new Vector3(0f, _size.Y + 0.5f, 0f);
            var barSize = new Vector3(_size.X, 0.1f, 0.1f);

            debugRenderer.DrawBox(barPosition, barSize, DrawingColor.Gray);

            var progressSize = new Vector3(_size.X * _transitionProgress, 0.1f, 0.1f);
            var progressPosition = barPosition - new Vector3(_size.X * (1f - _transitionProgress) / 2f, 0f, 0f);

            debugRenderer.DrawBox(progressPosition, progressSize, DrawingColor.Green);
        }

        private void RenderVisualEffects(IDebugRenderer debugRenderer)
        {
            foreach (var effect in _visualEffects)
                effect.Render(debugRenderer, _position, _animationTime);
        }

        private void RenderStateName(IDebugRenderer debugRenderer)
        {
            var textPosition = _position + new Vector3(0f, _size.Y + 1f, 0f);

            int alpha = (int)(System.Math.Clamp(_opacity, 0f, 1f) * 255f);
            var textColor = DrawingColor.FromArgb(alpha, 255, 255, 255);

            debugRenderer.DrawText(textPosition, _stateName, textColor, 12);
        }

        private void UpdateVisualEffects()
        {
            for (int i = _visualEffects.Count - 1; i >= 0; i--)
            {
                var effect = _visualEffects[i];
                effect.Update(_animationTime);

                if (effect.IsExpired)
                    _visualEffects.RemoveAt(i);
            }
        }

        private void UpdateTransitionEffects()
        {
            if (_isTransitioning && _visualEffects.Count == 0)
            {
                AddVisualEffect(new PulseEffect
                {
                    Duration = 1f,
                    Intensity = 0.5f,
                    Color = DrawingColor.Yellow
                });
            }
        }
    }

    public abstract class VisualEffect
    {
        public float Duration { get; set; }
        public float Intensity { get; set; }

        // FORCE System.Drawing.Color
        public DrawingColor Color { get; set; }

        public bool IsExpired => Duration > 0f && CalculateElapsedTime() >= Duration;

        private float CalculateElapsedTime()
        {
            var startTime = DateTime.Now.AddSeconds(-Duration);
            var elapsed = DateTime.Now - startTime;
            return (float)elapsed.TotalSeconds;
        }

        public abstract void Update(float animationTime);
        public abstract void Render(IDebugRenderer debugRenderer, Vector3 position, float animationTime);
    }

    public class PulseEffect : VisualEffect
    {
        public override void Update(float animationTime) { }

        public override void Render(IDebugRenderer debugRenderer, Vector3 position, float animationTime)
        {
            float pulseScale = 1f + MathF.Sin(animationTime * (float)System.Math.PI * 2f) * Intensity;
            var pulseSize = new Vector3(pulseScale, pulseScale, pulseScale);

            debugRenderer.DrawSphere(position, (int)pulseSize.X, DrawingColor.White);
        }
    }
}