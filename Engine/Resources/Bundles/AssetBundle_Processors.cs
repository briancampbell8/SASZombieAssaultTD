/*
Program Name: SASZombieAssaultTD
File Path: Engine\Resources\Bundles\AssetBundle_Processors.cs
Purpose: Resource management, save system, scene management, and state machine systems.
Features: Asset bundles, save data persistence, scene transitions, and enhanced state management.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;




namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Bundle compression handler.
    /// </summary>
    public class BundleCompression : IDisposable
    {
        private bool _disposed = false;

        public async Task CompressAsync(Stream input, Stream output, CompressionLevel level = CompressionLevel.Optimal)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BundleCompression));

            using var compressionStream = new GZipStream(output, GetCompressionLevel(level), true);
            await input.CopyToAsync(compressionStream);
        }

        public async Task DecompressAsync(Stream input, Stream output)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BundleCompression));

            using var compressionStream = new GZipStream(input, CompressionMode.Decompress, true);
            await compressionStream.CopyToAsync(output);
        }

        private System.IO.Compression.CompressionLevel GetCompressionLevel(CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.Fastest => System.IO.Compression.CompressionLevel.Fastest,
                CompressionLevel.NoCompression => System.IO.Compression.CompressionLevel.NoCompression,
                CompressionLevel.Optimal => System.IO.Compression.CompressionLevel.Optimal,
                CompressionLevel.Maximum => System.IO.Compression.CompressionLevel.SmallestSize,
                _ => System.IO.Compression.CompressionLevel.Optimal
            };
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }

    /// <summary>
    /// Bundle encryption handler.
    /// </summary>
    public class BundleEncryption : IDisposable
    {
        private bool _disposed = false;

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
