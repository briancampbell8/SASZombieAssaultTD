namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Parameter types for spawn pattern configuration.
    /// Defines the data types available for pattern parameters.
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Boolean parameter (true/false)
        /// </summary>
        Bool,
        
        /// <summary>
        /// Integer parameter
        /// </summary>
        Int,
        
        /// <summary>
        /// Floating point parameter
        /// </summary>
        Float,
        
        /// <summary>
        /// String parameter
        /// </summary>
        String,
        
        /// <summary>
        /// Vector3 parameter (x, y, z)
        /// </summary>
        Vector3,
        
        /// <summary>
        /// Color parameter (r, g, b, a)
        /// </summary>
        Color
    }
}
