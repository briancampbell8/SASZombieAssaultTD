//============================================================================
//File: RenderStats.cs
//Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\D3D11\RenderStats.cs
//Program: RenderStats
//Subsystem: Rendering / D3D11
//
//Purpose:
//    Tracks per-frame rendering statistics for the D3D11 backend.
//    Used by diagnostics overlays, performance tools, and backend reporting.
//
//Doctrine:
//    - Pure data container
//    - No System.Diagnostics
//    - No DLogger.Log() inside this class
//    - Deterministic, grep‑friendly, stable field names
//============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
//
{
    ///<summary>
    ///Per-frame rendering statistics for the D3D11 backend.
    ///</summary>
    public sealed class RenderStats
    {
        internal object AverageFPS;
        internal object GPULoad;
        internal object CPULoad;

        //--------------------------------------------------------------------
        //Core metrics
        //--------------------------------------------------------------------

        ///<summary>
        ///Number of draw calls executed this frame.
        ///</summary>
        public int DrawCalls { get; set; }

        ///<summary>
        ///Number of state changes (shaders, blend, rasterizer, etc.).
        ///</summary>
        public int StateChanges { get; set; }

        ///<summary>
        ///Number of frames rendered since startup.
        ///</summary>
        public int FramesRendered { get; set; }

        //--------------------------------------------------------------------
        //Timing metrics
        //--------------------------------------------------------------------

        ///<summary>
        ///CPU frame time in milliseconds.
        ///</summary>
        public double CpuFrameTimeMs { get; set; }

        ///<summary>
        ///GPU frame time in milliseconds.
        ///</summary>
        public double GpuFrameTimeMs { get; set; }

        //--------------------------------------------------------------------
        //Constructor
        //--------------------------------------------------------------------

        public RenderStats()
        {
            Reset();
        }

        //--------------------------------------------------------------------
        //Reset
        //--------------------------------------------------------------------

        ///<summary>
        ///Resets all counters to zero.
        ///</summary>
        public void Reset()
        {
            DrawCalls = 0;
            StateChanges = 0;
            FramesRendered = 0;
            CpuFrameTimeMs = 0.0;
            GpuFrameTimeMs = 0.0;
        }
    }
}
