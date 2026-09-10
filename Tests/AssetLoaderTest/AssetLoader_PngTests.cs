// ====================================================================================================
//  FILE: AssetLoader_PngTests.cs
//  PATH: AssetLoaderTest\AssetLoader_PngTests.cs
//  PROGRAM: AssetLoader_PngTests.cs
//  MODULE: Diagnostics & Engine Pipeline (AssetLoader_PngTests)
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
    public class AssetLoader_PngTests
    {
        [Fact]
        public async Task LoadPng_ReturnsTexture2D()
        {
            //PRECONDITION:
            //    The file Assets/logo.png must exist and be a valid PNG image.

            //ARRANGE:
            var loader = new Engine.Rendering.AssetLoader();
            string path = "Assets/logo.png";

            //ACT:
            var result = await loader.LoadAsync<Engine.Rendering.Texture2D>(path);

            //ASSERT:
            Assert.NotNull(result);
            Assert.IsType<Engine.Rendering.Texture2D>(result);
        }
    }
}
