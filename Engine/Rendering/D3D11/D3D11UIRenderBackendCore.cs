/*
Program Name: SASZombieAssaultTD
File Path: Engine\Rendering\D3D11\D3D11UIRenderBackendCore.cs
Purpose: Rendering system for D3D11 graphics, sprites, text, and visual effects.
Features: Render command queuing, sprite batching, D3D11 presentation, and zombie rendering.
*/

// File:    D3D11UIRenderBackendCore.cs
// Purpose: Core implementation of the D3D11 UI render backend.
//

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    /// <summary>
    /// Core D3D11 UI render backend implementation.
    /// </summary>
    public class D3D11UIRenderBackendCore : IDisposable
    {
        private readonly Renderer _renderer;
        private int _drawCalls;
        private int _stateChanges;
        private int _framesRendered;

        public D3D11UIRenderBackendCore(Renderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public void ExecuteCommands(IEnumerable<IRenderCommand> commands)
        {
            if (commands == null)
                return;

            foreach (var command in commands)
            {
                command?.Execute(_renderer);
                _drawCalls++;
            }
        }

        public void Present()
        {
            _framesRendered++;
        }

        public (int DrawCalls, int StateChanges, int FramesRendered) GetStats()
        {
            return (_drawCalls, _stateChanges, _framesRendered);
        }

        public void ResetStats()
        {
            _drawCalls = 0;
            _stateChanges = 0;
            _framesRendered = 0;
        }

		public void Dispose()
		{
			Engine.Diagnostics.DebugLogger.Log(
			"PassThru",
			"D3D11UIRenderBackendCore.Dispose() invoked; no GPU resources owned by this component."
		);
}


    }
}
