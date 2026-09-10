// ====================================================================================================
//  FILE: AssetLoader_BmpTests.cs
//  PATH: AssetLoaderTest\AssetLoader_BmpTests.cs
//  PROGRAM: AssertionExtensions.cs
//  MODULE: Diagnostics & Engine Pipeline (AssertionExtensions)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for SASZombieAssaultTD.Tests.AssetLoader.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide public interface and handling execution for NotNull().
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

using System;
using Xunit;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Tests.AssetLoader
{
    using Xunit;

    public static class AssertionExtensions
    {
        public static void NotNull(this object obj)
        {
            Assert.NotNull(obj);
        }

        public static void IsType<T>(this object obj)
        {
            Assert.IsType<T>(obj);
        }
    }


    internal class FactAttribute : Attribute
    {
    }
}
