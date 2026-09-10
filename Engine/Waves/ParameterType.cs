// ====================================================================================================
//  FILE: ParameterType.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Parameter types for spawn pattern configuration. Defines the data types available for pattern parameters.
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
