/*
File:    RenderInitValidator.cs
Author:  BDC
Created: 2026-02-17

Purpose:
Validates that all render components initialize correctly.

Notes:
Provides comprehensive validation for rendering system startup.
Ensures deterministic initialization and early error detection.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Validates rendering component initialization and provides deterministic startup checks.
    ///</summary>
    public sealed class RenderInitValidator
    {
        private readonly List<ValidationResult> _validationResults = new();

        ///<summary>
        ///Gets all validation results from the last validation run.
        ///</summary>
        public IReadOnlyList<ValidationResult> ValidationResults => _validationResults;

        ///<summary>
        ///Gets whether all validations passed.
        ///</summary>
        public bool AllValid => _validationResults.All(r => r.IsValid);

        ///<summary>
        ///Gets the number of critical errors found.
        ///</summary>
        public int CriticalErrorCount => _validationResults.Count(r => r.Severity == ValidationSeverity.Critical);

        ///<summary>
        ///Validates the complete rendering system initialization.
        ///</summary>
        public bool ValidateInitialization(IRenderContext renderContext, RenderPipelineConfig config)
        {
            _validationResults.Clear();

            //Core components validation
            ValidateRenderContext(renderContext);
            ValidateConfiguration(config);

            //System integration validation
            ValidateSystemDependencies();

            //Resource validation
            ValidateResourceAvailability();

            //Performance validation
            ValidatePerformanceSettings(config);

            return AllValid;
        }

        ///<summary>
        ///Validates a specific render component.
        ///</summary>
        public bool ValidateComponent(object component, string componentName)
        {
            var result = new ValidationResult
            {
                ComponentName = componentName,
                IsValid = true,
                Severity = ValidationSeverity.Info
            };

            try
            {
                //Check for null references
                if (component == null)
                {
                    result.IsValid = false;
                    result.Severity = ValidationSeverity.Critical;
                    result.Message = "Component is null";
                    _validationResults.Add(result);
                    return false;
                }

                //Check for required properties using reflection
                var componentType = component.GetType();
                var requiredProperties = GetRequiredProperties(componentType);

                foreach (var property in requiredProperties)
                {
                    var value = property.GetValue(component);
                    if (value == null)
                    {
                        result.IsValid = false;
                        result.Severity = ValidationSeverity.Error;
                        result.Message = $"Required property '{property.Name}' is null";
                        break;
                    }
                }

                result.Message = result.IsValid ?
                "Component validation passed" :
                result.Message;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Severity = ValidationSeverity.Critical;
                result.Message = $"Validation exception: {ex.Message}";
            }

            _validationResults.Add(result);
            return result.IsValid;
        }

        ///<summary>
        ///Gets a detailed validation report.
        ///</summary>
        public string GetValidationReport()
        {
            if (_validationResults.Count == 0)
                return "No validation results available.";

            var report = new System.Text.StringBuilder();
            report.AppendLine("=== Rendering System Validation Report ===");
            report.AppendLine($"Total Checks: {_validationResults.Count}");
            report.AppendLine($"Passed: {_validationResults.Count(r => r.IsValid)}");
            report.AppendLine($"Failed: {_validationResults.Count(r => !r.IsValid)}");
            report.AppendLine($"Critical Errors: {CriticalErrorCount}");
            report.AppendLine();

            //Group by severity
            var groupedResults = _validationResults.GroupBy(r => r.Severity);

            foreach (var group in groupedResults.OrderByDescending(g => g.Key))
            {
                report.AppendLine($"{group.Key} ({group.Count()}):");

                foreach (var result in group)
                {
                    var status = result.IsValid ? "✓" : "✗";
                    report.AppendLine($"  {status} {result.ComponentName}: {result.Message}");
                }
                report.AppendLine();
            }

            return report.ToString();
        }

        private void ValidateRenderContext(IRenderContext renderContext)
        {
            var result = new ValidationResult
            {
                ComponentName = "IRenderContext",
                Severity = ValidationSeverity.Critical
            };

            if (renderContext == null)
            {
                result.IsValid = false;
                result.Message = "IRenderContext is null";
            }
            else if (renderContext.Width <= 0 || renderContext.Height <= 0)
            {
                result.IsValid = false;
                result.Message = $"Invalid render context dimensions: {renderContext.Width}x{renderContext.Height}";
            }
            else
            {
                result.IsValid = true;
                result.Message = $"Render context initialized: {renderContext.Width}x{renderContext.Height}";
                result.Severity = ValidationSeverity.Info;
            }

            _validationResults.Add(result);
        }

        private void ValidateConfiguration(RenderPipelineConfig config)
        {
            var result = new ValidationResult
            {
                ComponentName = "RenderPipelineConfig",
                Severity = ValidationSeverity.Error
            };

            if (config == null)
            {
                result.IsValid = false;
                result.Message = "RenderPipelineConfig is null";
            }
            else if (!config.ValidateSettings(out var errorMessage))
            {
                result.IsValid = false;
                result.Message = $"Configuration validation failed: {errorMessage}";
            }
            else
            {
                result.IsValid = true;
                result.Message = "Configuration is valid";
                result.Severity = ValidationSeverity.Info;
            }

            _validationResults.Add(result);
        }

        private void ValidateSystemDependencies()
        {
            //Check for required system dependencies
            var requiredSystems = new[]
            {
                "SASZombieAssaultTD.Engine.Diagnostics.FrameStats",
                "SASZombieAssaultTD.Engine.Rendering.RenderDiagnostics"
            };

            foreach (var systemName in requiredSystems)
            {
                var result = new ValidationResult
                {
                    ComponentName = systemName,
                    IsValid = true,
                    Severity = ValidationSeverity.Info
                };

                try
                {
                    var type = Type.GetType(systemName);
                    if (type == null)
                    {
                        result.IsValid = false;
                        result.Severity = ValidationSeverity.Warning;
                        result.Message = "System type not found";
                    }
                    else
                    {
                        result.Message = "System dependency available";
                    }
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.Severity = ValidationSeverity.Error;
                    result.Message = $"Dependency check failed: {ex.Message}";
                }

                _validationResults.Add(result);
            }
        }

        private void ValidateResourceAvailability()
        {
            //Check for critical resource availability
            var criticalResources = new[]
            {
                "DefaultFont",
                "MissingTexture",
                "WhitePixel"
            };

            foreach (var resource in criticalResources)
            {
                var result = new ValidationResult
                {
                    ComponentName = $"Resource:{resource}",
                    IsValid = true,
                    Severity = ValidationSeverity.Info
                };

                //Placeholder for actual resource checking
                result.Message = "Resource check placeholder - implement actual resource validation";

                _validationResults.Add(result);
            }
        }

        private void ValidatePerformanceSettings(RenderPipelineConfig config)
        {
            if (config == null) return;

            var result = new ValidationResult
            {
                ComponentName = "PerformanceSettings",
                IsValid = true,
                Severity = ValidationSeverity.Info
            };

            //Validate performance-critical settings
            if (config.MaxDrawCallsPerFrame <= 0)
            {
                result.IsValid = false;
                result.Severity = ValidationSeverity.Warning;
                result.Message = "MaxDrawCallsPerFrame should be positive";
            }
            else if (config.MaxDrawCallsPerFrame > 50000)
            {
                result.IsValid = false;
                result.Severity = ValidationSeverity.Warning;
                result.Message = "MaxDrawCallsPerFrame is very high, may impact performance";
            }
            else
            {
                result.Message = $"Performance settings validated: {config.MaxDrawCallsPerFrame} max draws/frame";
            }

            _validationResults.Add(result);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(Type componentType)
        {
            //Return properties that should not be null for render components
            return componentType.GetProperties()
            .Where(p => p.Name.Contains("Texture") ||
            p.Name.Contains("Context") ||
            p.Name.Contains("Renderer"));
        }
    }

    ///<summary>
    ///Represents the result of a validation check.
    ///</summary>
    public sealed class ValidationResult
    {
        public string ComponentName { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public ValidationSeverity Severity { get; set; }
    }

    ///<summary>
    ///Severity levels for validation results.
    ///</summary>
    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }
}




