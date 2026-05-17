using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.BlendTree
{
    /// <summary>
    /// P11-16-04: Parameters for blend tree animation blending.
    /// Stores blend tree parameter values for animation state mixing.
    /// </summary>
    public class BlendParameters
    {
        /// <summary>
        /// Dictionary of blend parameter values.
        /// Stores parameter names and their corresponding float values.
        /// </summary>
        private readonly Dictionary<string, float> _parameters = new();

        /// <summary>
        /// Sets a blend parameter value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to set.</param>
        /// <param name="value">Value to set for the parameter.</param>
        public void SetParameter(string parameterName, float value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new System.ArgumentException("Parameter name cannot be null or whitespace.", nameof(parameterName));

            _parameters[parameterName] = value;
        }

        /// <summary>
        /// Gets a blend parameter value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to get.</param>
        /// <returns>Parameter value, or 0 if not found.</returns>
        public float GetParameter(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new System.ArgumentException("Parameter name cannot be null or whitespace.", nameof(parameterName));

            return _parameters.TryGetValue(parameterName, out var value) ? value : 0f;
        }

        /// <summary>
        /// Tries to get a blend parameter value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to get.</param>
        /// <param name="value">Output parameter value.</param>
        /// <returns>True if parameter was found, false otherwise.</returns>
        public bool TryGetFloat(string parameterName, out float value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new System.ArgumentException("Parameter name cannot be null or whitespace.", nameof(parameterName));

            return _parameters.TryGetValue(parameterName, out value);
        }

        /// <summary>
        /// Gets all parameter names.
        /// </summary>
        /// <returns>Array of parameter names.</returns>
        public string[] GetParameterNames() => _parameters.Keys.ToArray();

        /// <summary>
        /// Clears all parameters.
        /// </summary>
        public void Clear() => _parameters.Clear();
    }
}
