// =====================================================================================================
//  FILE: ValidatePerformanceSettings.cs
//  PATH: Engine/Render/Validation/ValidatePerformanceSettings.cs
//  SUBSYSTEM: Render Pipeline Performance Validation
//
//  ROLE:
//      Provides deterministic validation of RenderPipelineConfig settings to ensure the engine’s
//      rendering subsystem operates within safe, stable, and performant limits.
//
//  RESPONSIBILITIES:
//      - Validate performance‑critical configuration values
//      - Validate stability‑critical configuration values
//      - Produce structured ValidationResult objects for debugging and diagnostics
//      - Guarantee deterministic behavior with no external dependencies
//
//  NON‑RESPONSIBILITIES:
//      - Rendering execution
//      - ECS lifecycle management
//      - GPU resource allocation
//      - Shader compilation
//
//  ARCHITECTURAL NOTES:
//      - Stateless: accepts a config and returns validation results
//      - Deterministic: no randomness, no external calls
//      - Unified: all render‑performance validation logic lives here
// =====================================================================================================

using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Render.Validation
{
    public sealed class PerformanceSettingsValidator
    {
        public List<ValidationResult> Validate(RenderPipelineConfig config)
        {
            var results = new List<ValidationResult>();

            if (config == null)
            {
                var nullResult = new ValidationResult
                {
                    ComponentName = "RenderPipelineConfig",
                    IsValid = false,
                    Message = "RenderPipelineConfig is null",
                    Severity = ValidationEnums.ValidationSeverity.Error
                };

                nullResult.AddError("RenderPipelineConfig cannot be null");
                results.Add(nullResult);
                return results;
            }

            ValidateDrawCalls(config, results);
            ValidateBatching(config, results);
            ValidateShaderModel(config, results);
            ValidateThreading(config, results);
            ValidateFrameRate(config, results);
            ValidateAdapter(config, results);

            return results;
        }

        // ---------------------------------------------------------------------------------------------
        //  VALIDATION RULES
        // ---------------------------------------------------------------------------------------------

        private void ValidateDrawCalls(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "MaxDrawCallsPerFrame",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            if (config.MaxDrawCallsPerFrame <= 0)
                result.AddError("MaxDrawCallsPerFrame must be positive");

            else if (config.MaxDrawCallsPerFrame > 50000)
                result.AddWarning("MaxDrawCallsPerFrame is extremely high and may impact performance");

            else
                result.Message = $"MaxDrawCallsPerFrame = {config.MaxDrawCallsPerFrame}";

            results.Add(result);
        }

        private void ValidateBatching(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "BatchSize",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            if (config.BatchSize <= 0)
                result.AddError("BatchSize must be positive");

            else if (config.BatchSize > 8192)
                result.AddWarning("BatchSize is unusually large and may reduce batching efficiency");

            else
                result.Message = $"BatchSize = {config.BatchSize}";

            results.Add(result);
        }

        private void ValidateShaderModel(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "ShaderModel",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            var supportedModels = new[] { "SM4.0", "SM5.0", "SM6.0" };

            if (string.IsNullOrEmpty(config.ShaderModel))
                result.AddError("ShaderModel cannot be empty");

            else if (!supportedModels.Contains(config.ShaderModel))
                result.AddError($"Unsupported ShaderModel: {config.ShaderModel}");

            else
                result.Message = $"ShaderModel = {config.ShaderModel}";

            results.Add(result);
        }

        private void ValidateThreading(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "RenderThreadCount",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            if (config.RenderThreadCount <= 0)
                result.AddError("RenderThreadCount must be positive");

            else if (config.RenderThreadCount > 32)
                result.AddWarning("RenderThreadCount is unusually high and may cause CPU contention");

            else
                result.Message = $"RenderThreadCount = {config.RenderThreadCount}";

            results.Add(result);
        }

        private void ValidateFrameRate(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "TargetFrameRate",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            if (config.TargetFrameRate <= 0)
                result.AddError("TargetFrameRate must be positive");

            else if (config.TargetFrameRate > 1000)
                result.AddWarning("TargetFrameRate is excessively high and may cause instability");

            else
                result.Message = $"TargetFrameRate = {config.TargetFrameRate}";

            results.Add(result);
        }

        private void ValidateAdapter(RenderPipelineConfig config, List<ValidationResult> results)
        {
            var result = new ValidationResult
            {
                ComponentName = "GPUAdapter",
                IsValid = true,
                Severity = ValidationEnums.ValidationSeverity.Info
            };

            if (string.IsNullOrEmpty(config.GPUAdapterName))
                result.AddError("GPUAdapterName cannot be empty");

            else
                result.Message = $"GPUAdapter = {config.GPUAdapterName}";

            results.Add(result);
        }
    }
}
