using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Base UI element class with position, size, visibility, parent, and children fields
    /// P80-01-02: UIElement defining base UI element with position, size, visibility, parent, and children
    /// </summary>
    public class UIElement
    {
        private string _id;
        private System.Drawing.PointF _position;
        private System.Drawing.SizeF _size;
        private bool _isVisible = true;
        private bool _needsLayoutUpdate = true;
        private UIElement _parent;
        private readonly List<UIElement> _children;

        /// <summary>
        /// Gets or sets the unique identifier for this element.
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// Gets or sets the position of the element relative to its parent
        /// </summary>
        public System.Drawing.PointF Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the element
        /// </summary>
        public System.Drawing.SizeF Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the element is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent element
        /// </summary>
        public UIElement Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    // Remove from old parent if exists
                    if (_parent != null)
                    {
                        _parent.RemoveChild(this);
                    }

                    _parent = value;

                    // Add to new parent if exists
                    if (_parent != null)
                    {
                        _parent.AddChild(this);
                    }

                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets the collection of child elements
        /// </summary>
        public IReadOnlyList<UIElement> Children => _children.AsReadOnly();

        /// <summary>
        /// Gets whether the element needs a layout update
        /// </summary>
        public bool NeedsLayoutUpdate => _needsLayoutUpdate;

        /// <summary>
        /// Gets the absolute position of the element
        /// </summary>
        public System.Drawing.PointF AbsolutePosition
        {
            get
            {
                if (_parent == null)
                    return _position;

                return new System.Drawing.PointF(
                _parent.AbsolutePosition.X + _position.X,
                _parent.AbsolutePosition.Y + _position.Y
                );
            }
        }

        /// <summary>
        /// Initializes a new UI element
        /// </summary>
        public UIElement()
        {
            _children = new List<UIElement>();
            Console.WriteLine("UIElement: Created new UI element");
        }

        /// <summary>
        /// Updates the element
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public virtual void Update(float deltaTime)
        {
            try
            {
                // Update all children
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].Update(deltaTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the element
        /// </summary>
        public virtual void Render()
        {
            try
            {
                if (!_isVisible)
                    return;

                // Render all children
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].Render();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the element's layout
        /// </summary>
        public virtual void UpdateLayout()
        {
            try
            {
                // Update layouts for all children
                foreach (var child in _children)
                {
                    if (child.NeedsLayoutUpdate)
                    {
                        child.UpdateLayout();
                    }
                }

                _needsLayoutUpdate = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error updating layout - {ex.Message}");
            }
        }

        /// <summary>
        /// P80-01-03: Adds a child element to this element
        /// </summary>
        /// <param name="child">Child element to add</param>
        public virtual void AddChild(UIElement child)
        {
            try
            {
                if (child == null)
                {
                    Console.WriteLine("UIElement: Cannot add null child");
                    return;
                }

                if (_children.Contains(child))
                {
                    Console.WriteLine("UIElement: Child already exists");
                    return;
                }

                if (child.Parent != null && child.Parent != this)
                {
                    child.Parent.RemoveChild(child);
                }

                _children.Add(child);
                child._parent = this;
                InvalidateLayout();

                Console.WriteLine($"UIElement: Added child, total: {_children.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error adding child - {ex.Message}");
            }
        }

        /// <summary>
        /// P80-01-03: Removes a child element from this element
        /// </summary>
        /// <param name="child">Child element to remove</param>
        public virtual void RemoveChild(UIElement child)
        {
            try
            {
                if (child == null)
                {
                    Console.WriteLine("UIElement: Cannot remove null child");
                    return;
                }

                if (_children.Remove(child))
                {
                    child._parent = null;
                    InvalidateLayout();
                    Console.WriteLine($"UIElement: Removed child, remaining: {_children.Count}");
                }
                else
                {
                    Console.WriteLine("UIElement: Child not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error removing child - {ex.Message}");
            }
        }

        /// <summary>
        /// P80-01-03: Gets all child elements
        /// </summary>
        /// <returns>List of child elements</returns>
        public virtual List<UIElement> GetChildren()
        {
            try
            {
                return new List<UIElement>(_children);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error getting children - {ex.Message}");
                return new List<UIElement>();
            }
        }

        /// <summary>
        /// P80-01-03: Invalidates the layout of this element and its children
        /// </summary>
        public virtual void InvalidateLayout()
        {
            try
            {
                _needsLayoutUpdate = true;

                // Invalidate layout for all children
                foreach (var child in _children)
                {
                    child.InvalidateLayout();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error invalidating layout - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when the element receives focus
        /// </summary>
        public virtual void OnFocused()
        {
            try
            {
                Console.WriteLine($"UIElement: Element received focus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnFocused - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when the element loses focus
        /// </summary>
        public virtual void OnFocusLost()
        {
            try
            {
                Console.WriteLine($"UIElement: Element lost focus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnFocusLost - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when mouse enters the element
        /// </summary>
        public virtual void OnMouseEnter()
        {
            try
            {
                Console.WriteLine($"UIElement: Mouse entered element");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnMouseEnter - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when mouse exits the element
        /// </summary>
        public virtual void OnMouseExit()
        {
            try
            {
                Console.WriteLine($"UIElement: Mouse exited element");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnMouseExit - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when mouse button is pressed on the element
        /// </summary>
        public virtual void OnMousePress()
        {
            try
            {
                Console.WriteLine($"UIElement: Mouse pressed on element");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnMousePress - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when mouse button is released on the element
        /// </summary>
        public virtual void OnMouseRelease()
        {
            try
            {
                Console.WriteLine($"UIElement: Mouse released on element");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error in OnMouseRelease - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a point is inside this element
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is inside element</returns>
        public virtual bool ContainsPoint(System.Drawing.PointF point)
        {
            try
            {
                var absolutePos = AbsolutePosition;
                return point.X >= absolutePos.X &&
                point.X <= absolutePos.X + _size.Width &&
                point.Y >= absolutePos.Y &&
                point.Y <= absolutePos.Y + _size.Height;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error checking point containment - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the element at the specified point
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>Element at point, or null if none</returns>
        public virtual UIElement GetElementAt(System.Drawing.PointF point)
        {
            try
            {
                if (!_isVisible || !ContainsPoint(point))
                    return null;

                // Check children first (top-to-bottom)
                for (int i = _children.Count - 1; i >= 0; i--)
                {
                    var child = _children[i].GetElementAt(point);
                    if (child != null)
                        return child;
                }

                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIElement: Error getting element at point - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Handles input for this UI element.
        /// </summary>
        public virtual void HandleInput()
        {
            // Stub implementation
        }
    }
}




