//
//* File:    BGFX.cs
//* Path:    Engine/Rendering/BGFX/BGFX.cs
//* Purpose: BGFX method stubs with exact signatures for interface compliance.
//*          These methods contain no logic and return default values.
////
//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX API method stubs.
    ///Minimal placeholder implementations that match BGFX signatures.
    ///</summary>
    public static class bgfx
    {
        ///<summary>
        ///BGFX reset method stub.
        ///</summary>
        public static void reset(int width, int height, BGFX_RESET_FLAGS flags)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.reset - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX shutdown method stub.
        ///</summary>
        public static void shutdown()
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.shutdown - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX create vertex buffer method stub.
        ///</summary>
        public static VertexBufferHandle createVertexBuffer(object memory, object layout)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.createVertexBuffer - STUB PROCESSED");
            return new VertexBufferHandle(); //Default handle
        }

        ///<summary>
        ///BGFX create index buffer method stub.
        ///</summary>
        public static IndexBufferHandle createIndexBuffer(object memory)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.createIndexBuffer - STUB PROCESSED");
            return new IndexBufferHandle(); //Default handle
        }

        ///<summary>
        ///BGFX set vertex buffer method stub.
        ///</summary>
        public static void setVertexBuffer(int index, VertexBufferHandle buffer)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.setVertexBuffer - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX set index buffer method stub.
        ///</summary>
        public static void setIndexBuffer(IndexBufferHandle buffer)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.setIndexBuffer - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX set texture method stub.
        ///</summary>
        public static void setTexture(int stage, object uniform, TextureHandle texture)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.setTexture - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX submit method stub.
        ///</summary>
        public static void submit(int view, ProgramHandle program)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.submit - STUB PROCESSED");
        }

        ///<summary>
        ///BGFX set state method stub.
        ///</summary>
        public static void setState(uint flags)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFX.setState - STUB PROCESSED");
        }
    }
}
