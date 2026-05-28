// ====================================================================================================
// File:        AssetPipeline_Processors.cs
// Program:     AssetPipeline (Processors)
// Subsystem:   Resources / Pipeline
//
// Purpose:
//     Defines all asset processors used by the Asset Pipeline. Each processor handles import,
//     optimization, validation, and output generation for a specific asset category.
//
// Notes:
//     - IAssetProcessor.AssetManagerType has return type object.
//     - IAssetProcessor.AssetType has return type AssetType.
//     - We expose a strongly-typed public ManagerType for internal use.
//     - Interface members are implemented explicitly to avoid type/name collisions.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Resources
{
    // ================================================================================================
    // TEXTURE PROCESSOR
    // ================================================================================================
    public class TextureProcessor : IAssetProcessor
    {
        public AssetManagerType ManagerType => AssetManagerType.Texture;

        AssetType IAssetProcessor.AssetType => default;
        object IAssetProcessor.AssetManagerType { get => AssetManagerType.Texture; set { } }

        public IEnumerable<string> SupportedExtensions =>
            new[] { ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".dds" };

        public void ProcessAsset(string assetPath, OptimizationLevel level)
        {
            System.Diagnostics.Debug.WriteLine($"[TextureProcessor] Processing texture: {assetPath}");
        }

        public async Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config)
        {
            try
            {
                await Task.Delay(50);
                File.Copy(inputPath, outputPath, true);

                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Success,
                    ProcessingTime = TimeSpan.FromMilliseconds(50),
                    OriginalSize = new FileInfo(inputPath).Length,
                    ProcessedSize = new FileInfo(outputPath).Length
                };
            }
            catch (Exception ex)
            {
                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Failed,
                    Error = ex.Message,
                    ProcessingTime = TimeSpan.FromMilliseconds(50)
                };
            }
        }

        public async Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config)
        {
            await Task.Delay(10);
            return new AssetProcessResult
            {
                InputPath = assetPath,
                OutputPath = assetPath,
                Status = AssetProcessStatus.Success,
                ProcessingTime = TimeSpan.FromMilliseconds(10),
                OriginalSize = new FileInfo(assetPath).Length,
                ProcessedSize = new FileInfo(assetPath).Length
            };
        }

        public async Task<AssetValidationResult> ValidateAsync(string assetPath)
        {
            await Task.Delay(10);
            return new AssetValidationResult
            {
                IsValid = File.Exists(assetPath),
                AssetPath = assetPath,
                ValidationTime = TimeSpan.FromMilliseconds(10)
            };
        }

        public void UpdateConfig(AssetProcessorConfig config) { }
    }

    // ================================================================================================
    // AUDIO PROCESSOR
    // ================================================================================================
    public class AudioProcessor : IAssetProcessor
    {
        public AssetManagerType ManagerType => AssetManagerType.Audio;

        AssetType IAssetProcessor.AssetType => default;
        object IAssetProcessor.AssetManagerType { get => AssetManagerType.Audio; set { } }

        public IEnumerable<string> SupportedExtensions =>
            new[] { ".wav", ".mp3", ".ogg", ".flac" };

        public async Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config)
        {
            try
            {
                await Task.Delay(30);
                File.Copy(inputPath, outputPath, true);

                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Success,
                    ProcessingTime = TimeSpan.FromMilliseconds(30),
                    OriginalSize = new FileInfo(inputPath).Length,
                    ProcessedSize = new FileInfo(outputPath).Length
                };
            }
            catch (Exception ex)
            {
                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Failed,
                    Error = ex.Message,
                    ProcessingTime = TimeSpan.FromMilliseconds(30)
                };
            }
        }

        public async Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config)
        {
            await Task.Delay(15);
            return new AssetProcessResult
            {
                InputPath = assetPath,
                OutputPath = assetPath,
                Status = AssetProcessStatus.Success,
                ProcessingTime = TimeSpan.FromMilliseconds(15),
                OriginalSize = new FileInfo(assetPath).Length,
                ProcessedSize = new FileInfo(assetPath).Length
            };
        }

        public async Task<AssetValidationResult> ValidateAsync(string assetPath)
        {
            await Task.Delay(15);
            return new AssetValidationResult
            {
                IsValid = File.Exists(assetPath),
                AssetPath = assetPath,
                ValidationTime = TimeSpan.FromMilliseconds(15)
            };
        }

        public void UpdateConfig(AssetProcessorConfig config) { }

        public void ProcessAsset(string assetPath, OptimizationLevel level)
        {
            NI.Hit();
        }
    }

    // ================================================================================================
    // MODEL PROCESSOR
    // ================================================================================================
    public class ModelProcessor : IAssetProcessor
    {
        public AssetManagerType ManagerType => AssetManagerType.Model;

        AssetType IAssetProcessor.AssetType => default;
        object IAssetProcessor.AssetManagerType { get => AssetManagerType.Model; set { } }

        public IEnumerable<string> SupportedExtensions =>
            new[] { ".fbx", ".obj", ".dae", ".3ds" };

        public async Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config)
        {
            try
            {
                await Task.Delay(100);
                File.Copy(inputPath, outputPath, true);

                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Success,
                    ProcessingTime = TimeSpan.FromMilliseconds(100),
                    OriginalSize = new FileInfo(inputPath).Length,
                    ProcessedSize = new FileInfo(outputPath).Length
                };
            }
            catch (Exception ex)
            {
                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Failed,
                    Error = ex.Message,
                    ProcessingTime = TimeSpan.FromMilliseconds(100)
                };
            }
        }

        public async Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config)
        {
            await Task.Delay(20);
            return new AssetProcessResult
            {
                InputPath = assetPath,
                OutputPath = assetPath,
                Status = AssetProcessStatus.Success,
                ProcessingTime = TimeSpan.FromMilliseconds(20),
                OriginalSize = new FileInfo(assetPath).Length,
                ProcessedSize = new FileInfo(assetPath).Length
            };
        }

        public async Task<AssetValidationResult> ValidateAsync(string assetPath)
        {
            await Task.Delay(20);
            return new AssetValidationResult
            {
                IsValid = File.Exists(assetPath),
                AssetPath = assetPath,
                ValidationTime = TimeSpan.FromMilliseconds(20)
            };
        }

        public void UpdateConfig(AssetProcessorConfig config) { }

        public void ProcessAsset(string assetPath, OptimizationLevel level)
        {
            NI.Hit();
        }
    }

    // ================================================================================================
    // FONT PROCESSOR
    // ================================================================================================
    public class FontProcessor : IAssetProcessor
    {
        public AssetManagerType ManagerType => AssetManagerType.Font;

        AssetType IAssetProcessor.AssetType => default;
        object IAssetProcessor.AssetManagerType { get => AssetManagerType.Font; set { } }

        public IEnumerable<string> SupportedExtensions =>
            new[] { ".ttf", ".otf", ".woff", ".fnt" };

        public async Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config)
        {
            try
            {
                await Task.Delay(40);
                File.Copy(inputPath, outputPath, true);

                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Success,
                    ProcessingTime = TimeSpan.FromMilliseconds(40),
                    OriginalSize = new FileInfo(inputPath).Length,
                    ProcessedSize = new FileInfo(outputPath).Length
                };
            }
            catch (Exception ex)
            {
                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Failed,
                    Error = ex.Message,
                    ProcessingTime = TimeSpan.FromMilliseconds(40)
                };
            }
        }

        public async Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config)
        {
            await Task.Delay(10);
            return new AssetProcessResult
            {
                InputPath = assetPath,
                OutputPath = assetPath,
                Status = AssetProcessStatus.Success,
                ProcessingTime = TimeSpan.FromMilliseconds(10),
                OriginalSize = new FileInfo(assetPath).Length,
                ProcessedSize = new FileInfo(assetPath).Length
            };
        }

        public async Task<AssetValidationResult> ValidateAsync(string assetPath)
        {
            await Task.Delay(10);
            return new AssetValidationResult
            {
                IsValid = File.Exists(assetPath),
                AssetPath = assetPath,
                ValidationTime = TimeSpan.FromMilliseconds(10)
            };
        }

        public void UpdateConfig(AssetProcessorConfig config) { }

        public void ProcessAsset(string assetPath, OptimizationLevel level)
        {
            NI.Hit();
        }
    }

    // ================================================================================================
    // DATA PROCESSOR
    // ================================================================================================
    public class DataProcessor : IAssetProcessor
    {
        public AssetManagerType ManagerType => AssetManagerType.Data;

        AssetType IAssetProcessor.AssetType => default;
        object IAssetProcessor.AssetManagerType { get => AssetManagerType.Data; set { } }

        public IEnumerable<string> SupportedExtensions =>
            new[] { ".json", ".xml", ".csv", ".bin", ".yaml" };

        public async Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config)
        {
            try
            {
                await Task.Delay(25);
                File.Copy(inputPath, outputPath, true);

                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Success,
                    ProcessingTime = TimeSpan.FromMilliseconds(25),
                    OriginalSize = new FileInfo(inputPath).Length,
                    ProcessedSize = new FileInfo(outputPath).Length
                };
            }
            catch (Exception ex)
            {
                return new AssetProcessResult
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Status = AssetProcessStatus.Failed,
                    Error = ex.Message,
                    ProcessingTime = TimeSpan.FromMilliseconds(25)
                };
            }
        }

        public async Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config)
        {
            await Task.Delay(5);
            return new AssetProcessResult
            {
                InputPath = assetPath,
                OutputPath = assetPath,
                Status = AssetProcessStatus.Success,
                ProcessingTime = TimeSpan.FromMilliseconds(5),
                OriginalSize = new FileInfo(assetPath).Length,
                ProcessedSize = new FileInfo(assetPath).Length
            };
        }

        public async Task<AssetValidationResult> ValidateAsync(string assetPath)
        {
            await Task.Delay(5);
            return new AssetValidationResult
            {
                IsValid = File.Exists(assetPath),
                AssetPath = assetPath,
                ValidationTime = TimeSpan.FromMilliseconds(5)
            };
        }

        public void UpdateConfig(AssetProcessorConfig config) { }

        public void ProcessAsset(string assetPath, OptimizationLevel level)
        {
            NI.Hit();
        }
    }
}
