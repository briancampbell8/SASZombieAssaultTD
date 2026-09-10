// ====================================================================================================
//  FILE: AssetLoader_ShaderTests.cs
//  PATH: AssetLoaderTest\AssetLoader_ShaderTests.cs
//  PROGRAM: AssetLoader_ShaderTests.cs
//  MODULE: Diagnostics & Engine Pipeline (AssetLoader_ShaderTests)
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
    public class AssetLoader_ShaderTests
    {
        [Fact]
        public async Task LoadShader_ReturnsSourceText()
        {
            var loader = new SASZombieAssaultTD.Engine.Rendering.AssetLoader();
            string path = "Assets/test_shader.hlsl";

            var result = await loader.LoadAsync<string>(path);

            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.NotEmpty((System.Collections.Generic.IAsyncEnumerable<T>)result);
        }
    }

    internal class T
    {
        public T() { }
    }
}
