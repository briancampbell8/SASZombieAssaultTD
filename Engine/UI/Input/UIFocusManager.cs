using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Input
{
    ///<summary>
    ///Focus tracking and focus change logic for UI elements
    ///P80-05-03: UIFocusManager providing focus tracking and focus change logic for UI elements
    ///</summary>
    public class UIFocusManager
    {
        private UIElement _focusedElement;
        private readonly List<UIElement> _focusableElements;
        private bool _focusChangeRequested = false;

        ///<summary>
        ///Gets the currently focused element
        ///</summary>
        public UIElement FocusedElement => _focusedElement;

        ///<summary>
        ///Gets whether a focus change is requested
        ///</summary>
        public bool FocusChangeRequested => _focusChangeRequested;

        ///<summary>
        ///Initializes a new UIFocusManager
        ///</summary>
        public UIFocusManager()
        {
            _focusableElements = new List<UIElement>();
            System.Diagnostics.Debug.WriteLine("UIFocusManager: Initialized");
        }

        ///<summary>
        ///Adds a focusable element to the manager
        ///</summary>
        ///<param name="element">Element to add</param>
        public void RegisterFocusableElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Cannot register null element");
                    return;
                }

                if (_focusableElements.Contains(element))
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Element already registered");
                    return;
                }

                _focusableElements.Add(element);
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Registered focusable element, total: {_focusableElements.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error registering element - {ex.Message}");
            }
        }

        ///<summary>
        ///Removes a focusable element from the manager
        ///</summary>
        ///<param name="element">Element to remove</param>
        public void UnregisterFocusableElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Cannot unregister null element");
                    return;
                }

                if (_focusableElements.Remove(element))
                {
                    System.Diagnostics.Debug.WriteLine($"UIFocusManager: Unregistered focusable element, remaining: {_focusableElements.Count}");

                    //Clear focus if this element was focused
                    if (_focusedElement == element)
                    {
                        _focusedElement = null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Element not found in focusable elements");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error unregistering element - {ex.Message}");
            }
        }

        ///<summary>
        ///Sets focus to a specific element
        ///</summary>
        ///<param name="element">Element to focus</param>
        public void SetFocus(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Cannot focus null element");
                    return;
                }

                if (!_focusableElements.Contains(element))
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Cannot focus unregistered element");
                    return;
                }

                //Call focus lost on currently focused element
                if (_focusedElement != null && _focusedElement != element)
                {
                    _focusedElement.OnFocusLost();
                }

                //Set new focused element
                _focusedElement = element;
                element.OnFocused();

                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Set focus to element at {element.AbsolutePosition}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error setting focus - {ex.Message}");
            }
        }

        ///<summary>
        ///Clears focus from the current element
        ///</summary>
        public void ClearFocus()
        {
            try
            {
                if (_focusedElement != null)
                {
                    _focusedElement.OnFocusLost();
                    _focusedElement = null;

                    System.Diagnostics.Debug.WriteLine("UIFocusManager: Cleared focus");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("UIFocusManager: No element currently focused");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error clearing focus - {ex.Message}");
            }
        }

        ///<summary>
        ///Moves focus to the next element in the focus order
        ///</summary>
        public void FocusNext()
        {
            try
            {
                if (_focusedElement == null)
                {
                    //Focus first focusable element
                    if (_focusableElements.Count > 0)
                    {
                        SetFocus(_focusableElements[0]);
                    }
                }
                else
                {
                    //Find next element in the list
                    var currentIndex = _focusableElements.IndexOf(_focusedElement);
                    if (currentIndex >= 0 && currentIndex < _focusableElements.Count - 1)
                    {
                        SetFocus(_focusableElements[currentIndex + 1]);
                    }
                    else
                    {
                        //Wrap to first element
                        SetFocus(_focusableElements[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error focusing next element - {ex.Message}");
            }
        }

        ///<summary>
        ///Moves focus to the previous element in the focus order
        ///</summary>
        public void FocusPrevious()
        {
            try
            {
                if (_focusedElement == null)
                {
                    //Focus last focusable element
                    if (_focusableElements.Count > 0)
                    {
                        SetFocus(_focusableElements[_focusableElements.Count - 1]);
                    }
                }
                else
                {
                    //Find previous element in the list
                    var currentIndex = _focusableElements.IndexOf(_focusedElement);
                    if (currentIndex > 0)
                    {
                        SetFocus(_focusableElements[currentIndex - 1]);
                    }
                    else
                    {
                        //Wrap to last element
                        SetFocus(_focusableElements[_focusableElements.Count - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error focusing previous element - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates the focus manager
        ///</summary>
        public void Update()
        {
            try
            {
                //Update focus animations or effects here
                UpdateFocusAnimation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error during update - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates focus animations (placeholder implementation)
        ///</summary>
        private void UpdateFocusAnimation()
        {
            //Override in derived classes for focus animations
            //Examples: focus ring, pulse effect, color transitions
        }

        ///<summary>
        ///Gets a string representation of the focus state
        ///</summary>
        ///<returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                if (_focusedElement != null)
                {
                    return $"Focused: {_focusedElement.GetType().Name} at {_focusedElement.AbsolutePosition}";
                }
                else
                {
                    return "No element focused";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFocusManager: Error creating string representation - {ex.Message}");
                return "UIFocusManager: Error";
            }
        }
    }
}




