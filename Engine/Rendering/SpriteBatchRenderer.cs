/*
File:    SpriteBatchRenderer.cs
Purpose: Internal batching list; Flush() inside End(); texture draw batching.
*/
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public class SpriteBatchRenderer
    {
        private IRenderContext? _context;
        private readonly List<DrawCall> _batch = new List<DrawCall>();

        private struct DrawCall
        {
            public Texture2D Texture;
            public Rectangle Dest;
        }

        public void Begin(IRenderContext context)
        {
            _context = context;
            _batch.Clear();
        }

        public void Draw(Texture2D texture, Rectangle dest)
        {
            if (texture == null)
                return;
            _batch.Add(new DrawCall { Texture = texture, Dest = dest });
        }

        public void End()
        {
            Flush();
            _context = null;
        }

        public void Flush()
        {
            if (_context == null)
                return;
            foreach (var call in _batch)
                _context.DrawTexture(call.Texture, new Vector3(call.Dest.X, call.Dest.Y, 0f), Color.White);
            _batch.Clear();
        }
    }
}




