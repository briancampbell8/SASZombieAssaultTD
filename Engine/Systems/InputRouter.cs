using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    public interface IInputListener
    {
        void OnInputEvent(InputEvent inputEvent);
    }

    public enum InputEventType
    {
        None = 0,
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp,
        MouseMove
    }

    public readonly struct InputEvent
    {
        public InputEventType Type { get; }
        public int KeyCode { get; }
        public int MouseX { get; }
        public int MouseY { get; }

        public InputEvent(InputEventType type, int keyCode = 0, int mouseX = 0, int mouseY = 0)
        {
            Type = type;
            KeyCode = keyCode;
            MouseX = mouseX;
            MouseY = mouseY;
        }
    }

    public sealed class InputRouter
    {
        private readonly List<IInputListener> _listeners;

        public InputRouter()
        {
            _listeners = new List<IInputListener>();
#if DEBUG
            Debug.WriteLine("[InputRouter] Constructed.");
#endif
        }

        public void RegisterListener(IInputListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
#if DEBUG
                Debug.WriteLine("[InputRouter] Listener registered.");
#endif
            }
        }

        public void UnregisterListener(IInputListener listener)
        {
            if (listener == null) return;
            if (_listeners.Remove(listener))
            {
#if DEBUG
                Debug.WriteLine("[InputRouter] Listener unregistered.");
#endif
            }
        }

        public void Route(InputEvent inputEvent)
        {
#if DEBUG
            Debug.WriteLine($"[InputRouter] Routing event: {inputEvent.Type}");
#endif
            foreach (var listener in _listeners)
            {
                listener.OnInputEvent(inputEvent);
            }
        }

#if DEBUG
        public void DebugPrint()
        {
            Debug.WriteLine($"[InputRouter] ListenerCount={_listeners.Count}");
        }
#endif
    }
}
