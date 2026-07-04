//
//* File:    BGFXShaderManager.cs
//* Path:    Engine/Rendering/BGFX/BGFXShaderManager.cs
//* Purpose: BGFX shader management.
//*          Parallel implementation to BGFXShaderManager.
////
//

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX shader compilation and caching.
    ///- Provides same interface as BGFXGraphicsDevice.
    ///</summary>
    internal sealed class BGFXShaderManager : IDisposable
    {
        //    private BGFXDeviceCore _core;

        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************
        //public BGFXShaderManager(BGFXDeviceCore core)
        //{
        //    System.Diagnostics.Debug.WriteLine("[DIAG] BGFXShaderManager.constructor - STUB PROCESSED");
        //    _core = core ?? throw new ArgumentNullException(nameof(core));
        //}
        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************

        //    public BGFXShaderManager(BGFXDeviceCore core)
        //   {
        //        System.Diagnostics.Debug.WriteLine("[DIAG] BGFXShaderManager.constructor - STUB PROCESSED");
        //        _core = core; //Null check bypassed for BGFX stub implementation
        //  }

        public void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFXShaderManager.Initialize - STUB PROCESSED");
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXShaderManager.Initialize()");
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFXShaderManager.Dispose - STUB PROCESSED");
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXShaderManager.Dispose()");
        }
    }
}
