// ====================================================================================================
//  FILE: AssetLoader_TtfTests.cs
//  PATH: AssetLoaderTest\AssetLoader_TtfTests.cs
//  PROGRAM: AssetLoader_TtfTests.cs
//  MODULE: Diagnostics & Engine Pipeline (AssetLoader_TtfTests)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for SASZombieAssaultTD.Tests.AssetLoaderTest.
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

namespace SASZombieAssaultTD.Tests.AssetLoaderTest
{
    public class AssetLoader_TtfTests
    {
        [Fact]
        public async Task LoadTtf_ReturnsFont()
        {
            var loader = new SASZombieAssaultTD.Engine.Rendering.AssetLoader();
            string path = "Assets/test.ttf";

            var result = await loader.LoadAsync<SASZombieAssaultTD.Engine.Rendering.Font>(path);

            Assert.NotNull(result);
            Assert.IsType<SASZombieAssaultTD.Engine.Rendering.Font>(result);
        }
    }
}
