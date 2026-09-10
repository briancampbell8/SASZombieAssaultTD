// ====================================================================================================
//  FILE: AssetLoader_JpegTests.cs
//  PATH: AssetLoaderTest\AssetLoader_JpegTests.cs
//  PROGRAM: AssetLoader_JpegTests.cs
//  MODULE: Diagnostics & Engine Pipeline (AssetLoader_JpegTests)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for SASZombieAssaultTD.Tests.AssetLoader.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide deterministic engine pipeline handling execution logic.
//      - Maintain runtime flow and process core thread states safely.
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

using System.Threading.Tasks;
using Xunit;

namespace SASZombieAssaultTD.Tests.AssetLoader
{
    public class AssetLoader_JpegTests
    {
        [Fact]
        public async Task LoadJpeg_ReturnsTexture2D()
        {
            //PRECONDITION:
            //    The file Assets/test.jpg must exist and be a valid JPEG image.

            //ARRANGE:
            var loader = new SASZombieAssaultTD.Engine.Rendering.AssetLoader();
            string path = "Assets/test.jpg";

            //ACT:
            var result = await loader.LoadAsync<SASZombieAssaultTD.Engine.Rendering.Texture2D>(path);

            //ASSERT:
            Assert.NotNull(result);
            Assert.IsType<SASZombieAssaultTD.Engine.Rendering.Texture2D>(result);
        }
    }
}
