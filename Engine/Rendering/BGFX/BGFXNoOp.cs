//
//* File:    BGFXNoOp.cs
//* Path:    Engine/Rendering/BGFX/BGFXNoOp.cs
//* Purpose: BGFX No-Op wrappers - inert, non-executing wrappers for BGFX API calls.
////

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX No-Op wrappers - inert, non-executing wrappers for BGFX API calls.
    ///This class contains only placeholder methods with TODO comments.
    ///</summary>
    internal class BGFXNoOp
    {
        ///<summary>
        ///Initialize BGFX - No-Op wrapper.
        ///</summary>
        public static void InitNoOp(object initData)
        {
            //TODO: Call bgfx.init(initData) once activation is enabled.
        }

        ///<summary>
        ///Reset BGFX - No-Op wrapper.
        ///</summary>
        public static void Reset(int width, int height, object resetFlags)
        {
            //TODO: Call bgfx.reset(width, height, resetFlags) once activation is enabled.
        }

        ///<summary>
        ///Set view rectangle - No-Op wrapper.
        ///</summary>
        public static void SetViewRect(ushort viewId, ushort x, ushort y, ushort width, ushort height)
        {
            //TODO: Call bgfx.setViewRect(viewId, x, y, width, height) once activation is enabled.
        }

        ///<summary>
        ///Set view clear state - No-Op wrapper.
        ///</summary>
        public static void SetViewClear(ushort viewId, uint clearFlags, uint clearColor, float depth, byte stencil)
        {
            //TODO: Call bgfx.setViewClear(viewId, clearFlags, clearColor, depth, stencil) once activation is enabled.
        }

        ///<summary>
        ///Set vertex buffer - No-Op wrapper.
        ///</summary>
        public static void SetVertexBuffer(byte stream, object vertexBufferHandle, uint startVertex, uint numVertices)
        {
            //TODO: Call bgfx.setVertexBuffer(stream, vertexBufferHandle, startVertex, numVertices) once activation is enabled.
        }

        ///<summary>
        ///Set index buffer - No-Op wrapper.
        ///</summary>
        public static void SetIndexBuffer(object indexBufferHandle, uint firstIndex, uint numIndices)
        {
            //TODO: Call bgfx.setIndexBuffer(indexBufferHandle, firstIndex, numIndices) once activation is enabled.
        }

        ///<summary>
        ///Set texture - No-Op wrapper.
        ///</summary>
        public static void SetTexture(byte stage, object uniformHandle, object textureHandle)
        {
            //TODO: Call bgfx.setTexture(stage, uniformHandle, textureHandle) once activation is enabled.
        }

        ///<summary>
        ///Set uniform - No-Op wrapper.
        ///</summary>
        public static void SetUniform(object uniformHandle, object value, ushort num)
        {
            //TODO: Call bgfx.setUniform(uniformHandle, value, num) once activation is enabled.
        }

        ///<summary>
        ///Submit draw call - No-Op wrapper.
        ///</summary>
        public static void Submit(ushort viewId, object programHandle, int depth, byte flags)
        {
            //TODO: Call bgfx.submit(viewId, programHandle, depth, flags) once activation is enabled.
        }

        ///<summary>
        ///Advance to next frame - No-Op wrapper.
        ///</summary>
        public static void Frame()
        {
            //TODO: Call bgfx.frame() once activation is enabled.
        }

        ///<summary>
        ///Shutdown BGFX - No-Op wrapper.
        ///</summary>
        public static void Shutdown()
        {
            //TODO: Call bgfx.shutdown() once activation is enabled.
        }
    }
}
