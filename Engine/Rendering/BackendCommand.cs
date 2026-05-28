// File:    BackendCommand.cs
// Purpose: Backend command type for graphics device interface operations.
// Created: Fix CS0246 missing type errors.
//

using SASZombieAssaultTD.Engine.Diagnostics;

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Backend command for graphics device operations.
    /// </summary>
    public class BackendCommand
    {
        /// <summary>
        /// Command type identifier.
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// Command parameters.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Initializes a new backend command.
        /// </summary>
        public BackendCommand(string commandType)
        {
            CommandType = commandType;
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new backend command with parameters.
        /// </summary>
        public BackendCommand(string commandType, Dictionary<string, object> parameters)
        {
            CommandType = commandType;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }
}
