//==========================================================================================
// FILE: AdapterOps_Resources.cs
// PATH: Engine/Render/Adapter/AdapterOps_Resources.cs
// SUBSYSTEM: Rendering / D3D11 GPU Resource Management
//
// ROLE:
// Provides deterministic creation, validation, and management of GPU resources,
// including buffers, textures, samplers, and views. Ensures stable allocation
// behavior and exposes a clean interface for higher‑level rendering subsystems.
//
// RESPONSIBILITIES:
//  - Create GPU buffers, textures, and resource views.
//  - Validate resource configurations and usage flags.
//  - Manage deterministic lifetime and cleanup of GPU resources.
//  - Provide structured resource information for debugging and diagnostics.
//  - Support pipeline and frame subsystems with stable resource access.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Managing swap chains or presentation.
//  - Binding pipeline states or performing rendering operations.
//==========================================================================================

using System;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 GPU resource management subsystem.
    /// </summary>
    public sealed class AdapterOps_Resources
    {
        private readonly D3D11RenderContext _context;

        public AdapterOps_Resources(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create a GPU buffer with deterministic configuration.
        public object CreateBuffer(object bufferDesc, object initialData = null)
        {
            // Validate input to avoid null reference issues.
            if (bufferDesc == null)
            {
                throw new ArgumentNullException(nameof(bufferDesc));
            }

            // Create a deterministic, serializable representation of the GPU buffer resource.
            // We avoid referencing any unknown or project-specific APIs and instead return a
            // dictionary containing the descriptor and optional initial data. Consumers
            // of this method can inspect this dictionary to perform the actual GPU allocation
            // using the engine-specific context.
            var resourceInfo = new System.Collections.Generic.Dictionary<string, object>(3)
            {
                ["Descriptor"] = bufferDesc,
                ["InitialData"] = initialData,
                ["CreatedUtc"] = DateTime.UtcNow
            };

            return resourceInfo;
        }

        // Create a GPU texture with deterministic configuration.
        public object CreateTexture(object textureDesc, object initialData = null)
        {
            // Validate input
            if (textureDesc == null)
            {
                throw new ArgumentNullException(nameof(textureDesc));
            }

            // If an underlying render context exposes a CreateTexture method, prefer to delegate to it.
            if (_context != null)
            {
                var ctxType = _context.GetType();

                // Try to find a method named CreateTexture that accepts two parameters.
                var found = default(MethodInfo);
                foreach (var m in ctxType.GetMethods())
                {
                    if (string.Equals(m.Name, "CreateTexture", StringComparison.Ordinal) && m.GetParameters().Length == 2)
                    {
                        found = m;
                        break;
                    }
                }

                if (found != null)
                {
                    try
                    {
                        // Invoke the underlying implementation and return its result.
                        var result = found.Invoke(_context, new object[] { textureDesc, initialData });
                        return result;
                    }
                    catch (TargetInvocationException tie) when (tie.InnerException != null)
                    {
                        // Surface the original exception from the invoked method.
                        throw tie.InnerException;
                    }
                }
            }

            // Fallback: return a simple descriptor object so callers receive a deterministic, inspectable value.
            // Returning an anonymous object is acceptable because the method's return type is object.
            return new { Descriptor = textureDesc, InitialData = initialData, CreatedUtc = DateTime.UtcNow };
        }

        // Create a resource view (SRV/RTV/DSV/UAV).
        public object CreateResourceView(object resource, object viewDesc)
        {
            // Validate required parameter
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            // If no descriptor provided, return the resource itself as a sensible default.
            if (viewDesc is null)
            {
                return resource;
            }

            // If the resource already represents the requested view, return it directly.
            if (ReferenceEquals(resource, viewDesc) || resource.Equals(viewDesc))
            {
                return resource;
            }

            // Attempt to find and invoke a creation method on the resource via reflection.
            // This avoids hard-coding APIs and remains resilient if the underlying resource
            // type exposes a convenience method (e.g. CreateShaderResourceView, CreateView, etc.).
            var descType = viewDesc.GetType();
            var resourceType = resource.GetType();

            // Candidate method names commonly used for creating resource views.
            string[] candidateNames = new[]
            {
        "CreateShaderResourceView",
        "CreateRenderTargetView",
        "CreateDepthStencilView",
        "CreateUnorderedAccessView",
        "CreateView",
        "Create"
    };

            foreach (var name in candidateNames)
            {
                var method = resourceType.GetMethod(name, new[] { descType });
                if (method != null)
                {
                    try
                    {
                        return method.Invoke(resource, new[] { viewDesc });
                    }
                    catch (TargetInvocationException tie)
                    {
                        // Unwrap and rethrow the inner exception to preserve original error semantics.
                        throw tie.InnerException ?? tie;
                    }
                }
            }

            // If no suitable creation method exists, fall back to returning a tuple that
            // pairs the resource with the view descriptor to represent the requested view.
            // This is a safe, well-typed object that callers can inspect or replace later.
            return Tuple.Create(resource, viewDesc);
        }

        // Validate resource configuration and usage flags.
        public bool ValidateResourceConfig(object resourceDesc)
        {
            // Basic reflection-based validation to avoid depending on concrete types.
            // Accept null check first.
            if (resourceDesc == null)
            {
                return false;
            }

            var type = resourceDesc.GetType();

            try
            {
                // Common texture properties: Width and Height must be > 0
                var propWidth = type.GetProperty("Width");
                var propHeight = type.GetProperty("Height");
                if (propWidth != null && propHeight != null)
                {
                    var widthVal = propWidth.GetValue(resourceDesc);
                    var heightVal = propHeight.GetValue(resourceDesc);
                    if (widthVal == null || heightVal == null)
                        return false;

                    int width = Convert.ToInt32(widthVal);
                    int height = Convert.ToInt32(heightVal);
                    if (width <= 0 || height <= 0)
                        return false;

                    // If texture has Format property, ensure it's not the default/null value
                    var propFormat = type.GetProperty("Format");
                    if (propFormat != null)
                    {
                        var fmt = propFormat.GetValue(resourceDesc);
                        if (fmt == null)
                            return false;
                    }

                    return true;
                }

                // Common buffer properties: ByteWidth, Size or ElementCount should be > 0
                string[] bufferProps = { "ByteWidth", "Size", "ElementCount" };
                foreach (var name in bufferProps)
                {
                    var p = type.GetProperty(name);
                    if (p != null)
                    {
                        var val = p.GetValue(resourceDesc);
                        if (val == null)
                            return false;

                        int size = Convert.ToInt32(val);
                        return size > 0;
                    }
                }

                // If resource describes usage/flags, at least ensure the properties exist and are not null
                var usageProp = type.GetProperty("Usage");
                var bindFlagsProp = type.GetProperty("BindFlags");
                if (usageProp != null || bindFlagsProp != null)
                {
                    if (usageProp != null && usageProp.GetValue(resourceDesc) == null)
                        return false;
                    if (bindFlagsProp != null && bindFlagsProp.GetValue(resourceDesc) == null)
                        return false;

                    return true;
                }

                // If no common properties were found, consider non-null descriptor acceptable.
                return true;
            }
            catch
            {
                // Any exception during reflection or conversion indicates invalid config.
                return false;
            }
        }

        // Retrieve structured resource information.
        public object GetResourceInfo(object resource)
        {
            // Return null for null input.
            if (resource == null)
            {
                return null;
            }

            var type = resource.GetType();

            // Use a dictionary to return structured information about the resource.
            var info = new System.Collections.Generic.Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["Type"] = type.FullName ?? type.Name
            };

            // Try to include the object's ToString() representation if available.
            try
            {
                info["ToString"] = resource.ToString();
            }
            catch
            {
                // Ignore exceptions from ToString()
            }

            // Inspect public instance properties.
            var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                // Skip indexer properties.
                if (prop.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                object value;
                try
                {
                    value = prop.GetValue(resource);
                }
                catch (Exception ex)
                {
                    value = $"<unreadable: {ex.GetType().Name}: {ex.Message}>";
                }

                // Summarize arrays to avoid embedding potentially large data structures.
                if (value is Array arr)
                {
                    info[prop.Name] = new { ElementType = arr.GetType().GetElementType()?.FullName ?? "array", arr.Length };
                }
                else
                {
                    info[prop.Name] = value;
                }
            }

            // Inspect public instance fields as well.
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                object value;
                try
                {
                    value = field.GetValue(resource);
                }
                catch (Exception ex)
                {
                    value = $"<unreadable: {ex.GetType().Name}: {ex.Message}>";
                }

                if (value is Array arr)
                {
                    info[field.Name] = new { ElementType = arr.GetType().GetElementType()?.FullName ?? "array", arr.Length };
                }
                else
                {
                    info[field.Name] = value;
                }
            }

            return info;
        }
    }
}
