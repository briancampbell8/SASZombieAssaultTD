using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Rendering
{
    // Stub implementations for XNA/MonoGame compatibility
    public enum SpriteSortMode
    {
        Deferred,
        Immediate,
        Texture,
        BackToFront,
        FrontToBack
    }
    
    public enum SpriteEffects
    {
        None,
        FlipHorizontally,
        FlipVertically
    }
    
    public class SpriteBatch
    {
        public static readonly SamplerState DefaultSamplerState = SamplerState.LinearClamp;
        public static readonly BlendState DefaultBlendState = BlendState.AlphaBlend;
        
        public void Begin(SpriteSortMode sortMode) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState) { }
        public void End() { }
        public void Draw(object texture, System.Numerics.Vector3 vector3, System.Numerics.Vector2 position, Color color) { }
        public void Draw(object texture, System.Numerics.Vector2 position, Rectangle? sourceRectangle, Color color) { }
        public void DrawString(object spriteFont, string text, System.Numerics.Vector2 position, Color color) { }
        public void DrawString(object spriteFont, string text, System.Numerics.Vector2 position, Color color, float rotation, System.Numerics.Vector2 origin, float scale, SpriteEffects effects, float layerDepth) { }

        internal void Draw(nint v, Vector3 vector31, Rectangle sourceRect, Color color, float rotation, Vector3 vector32, Vector3 vector33, SpriteEffects none, float layerDepth)
        {
            throw new NotImplementedException();
        }
    }
    
    public class SamplerState
    {
        public static SamplerState LinearClamp { get; } = new SamplerState();
        public static SamplerState PointClamp { get; } = new SamplerState();
        public static SamplerState AnisotropicClamp { get; } = new SamplerState();
        public static SamplerState LinearWrap { get; } = new SamplerState();
        public static SamplerState PointWrap { get; } = new SamplerState();
        public static SamplerState AnisotropicWrap { get; } = new SamplerState();
    }
    
    public class BlendState
    {
        public static BlendState AlphaBlend { get; } = new BlendState();
        public static BlendState Additive { get; } = new BlendState();
        public static BlendState Opaque { get; } = new BlendState();
        public static BlendState NonPremultiplied { get; } = new BlendState();
    }
}
