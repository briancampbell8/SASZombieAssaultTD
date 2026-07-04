using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Input;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Scenes.TestScenes
{
    public class MeanStreetsTest
    {
        public List<PointF> SpawnPoints { get; } = new();
        public EntityManager? EntityManager { get; set; }
        public UIInputRouter? InputSystem { get; set; }
        public PointF CameraFollowPosition { get; set; }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(float deltaTime)
        {
            if (EntityManager != null && SpawnPoints.Count > 0)
            {
            }
            CameraFollowPosition = new PointF { X = 0f, Y = 0f };
        }

        public void Render(IRenderContext context)
        {
            if (context != null)
                context.DrawText("MeanStreetsTest", new Vector3(10f, 10f, 0f), Color.White);
        }

        public void ConfigureDefaultSpawns()
        {
            SpawnPoints.Clear();
            SpawnPoints.Add(new PointF { x = 100f, y = 100f });
            SpawnPoints.Add(new PointF { x = 200f, y = 150f });
            SpawnPoints.Add(new PointF { x = 300f, y = 200f });
        }


        public void Run()
        {
            System.Diagnostics.Debug.WriteLine("[MeanStreetsTest] Running test scene with " + SpawnPoints.Count + " spawn points.");
        }
    }
}



