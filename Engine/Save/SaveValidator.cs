/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\SaveValidator.cs
Purpose: Comprehensive save data validation system.
Features: Version compatibility, data range validation, required field validation, P120/P100 data validation.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;

//
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
namespace SASZombieAssaultTD.Engine.Save
{
    ///<summary>
    ///Comprehensive save data validator.
    ///P140-05: Implements validation for save data including P120/P100 integration fields.
    ///</summary>
    public class SaveValidator
    {
        private readonly int _currentSaveVersion;
        private readonly List<ValidationRule> _validationRules;

        ///<summary>
        ///Gets the current save version.
        ///</summary>
        public int CurrentSaveVersion => _currentSaveVersion;

        ///<summary>
        ///Initializes a new save validator.
        ///</summary>
        ///<param name="currentSaveVersion">The current save version.</param>
        public SaveValidator(int currentSaveVersion = 2)
        {
            _currentSaveVersion = currentSaveVersion;
            _validationRules = new List<ValidationRule>();
            InitializeValidationRules();
        }

        ///<summary>
        ///Initializes validation rules.
        ///</summary>
        private void InitializeValidationRules()
        {
            //Basic SaveData validation rules
            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.RequiredField,
                Description = "Player name cannot be empty",
                Validate = (data) => !string.IsNullOrEmpty(data.PlayerName)
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "High score cannot be negative",
                Validate = (data) => data.HighScore >= 0
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Current level must be at least 1",
                Validate = (data) => data.CurrentLevel >= 1
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Total kills cannot be negative",
                Validate = (data) => data.TotalKills >= 0
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Total waves completed cannot be negative",
                Validate = (data) => data.TotalWavesCompleted >= 0
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Total play time cannot be negative",
                Validate = (data) => data.TotalPlayTime >= 0
            });

            //Settings validation rules
            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Master volume must be between 0 and 1",
                Validate = (data) => data.Settings.MasterVolume >= 0f && data.Settings.MasterVolume <= 1f
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Music volume must be between 0 and 1",
                Validate = (data) => data.Settings.MusicVolume >= 0f && data.Settings.MusicVolume <= 1f
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "SFX volume must be between 0 and 1",
                Validate = (data) => data.Settings.SfxVolume >= 0f && data.Settings.SfxVolume <= 1f
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Screen width must be at least 640",
                Validate = (data) => data.Settings.ScreenWidth >= 640
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Screen height must be at least 480",
                Validate = (data) => data.Settings.ScreenHeight >= 480
            });

            _validationRules.Add(new ValidationRule
            {
                Category = ValidationCategory.DataRange,
                Description = "Quality level must be between 0 and 3",
                Validate = (data) => data.Settings.QualityLevel >= 0 && data.Settings.QualityLevel <= 3
            });

            Dlogger.Log(LogSubsystems.Save, LogLevel.Info, $"SaveValidator: Initialized {_validationRules.Count} validation rules");
        }

        ///<summary>
        ///Validates save data.
        ///</summary>
        ///<param name="saveData">The save data to validate.</param>
        ///<returns>Validation result.</returns>
        public ValidationResult Validate(SaveData saveData)
        {
            if (saveData == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "Save data is null" }
                };
            }

            var result = new ValidationResult { IsValid = true };

            //Check version compatibility
            if (!IsVersionCompatible(saveData.Version))
            {
                result.IsValid = false;
                result.AddError($"Save version {saveData.Version} is not compatible with current version {_currentSaveVersion}");
            }

            //Run validation rules
            foreach (var rule in _validationRules)
            {
                try
                {
                    if (!rule.Validate(saveData))
                    {
                        result.IsValid = false;
                        result.AddError($"[{rule.Category}] {rule.Description}");
                    }
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.AddError($"Validation error for rule '{rule.Description}': {ex.Message}");
                }
            }

            //Validate extended data if present
            if (saveData is SaveDataExtended extendedData)
            {
                ValidateExtendedData(extendedData, result);
            }

            Dlogger.Log(LogSubsystems.Save, LogLevel.Info,
                $"SaveValidator: Validation {(result.IsValid ? "passed" : "failed")} - Errors: {result.Errors.Count}, Warnings: {result.Warnings.Count}");

            return result;
        }

        ///<summary>
        ///Validates extended save data (P120/P100 integration).
        ///</summary>
        ///<param name="extendedData">The extended save data.</param>
        ///<param name="result">The validation result to update.</param>
        private void ValidateExtendedData(SaveDataExtended extendedData, ValidationResult result)
        {
            //Validate battlefield progress
            foreach (var kvp in extendedData.BattlefieldProgress)
            {
                var battlefield = kvp.Key;
                var progress = kvp.Value;

                if (progress.Battlefield != battlefield)
                {
                    result.AddWarning($"Battlefield progress mismatch: key={battlefield}, value={progress.Battlefield}");
                }

                if (progress.HighestWave < 0)
                {
                    result.AddError($"Battlefield {battlefield.GetDisplayName()} has negative highest wave");
                }

                if (progress.HighScore < 0)
                {
                    result.AddError($"Battlefield {battlefield.GetDisplayName()} has negative high score");
                }

                if (progress.PlayTime < 0)
                {
                    result.AddError($"Battlefield {battlefield.GetDisplayName()} has negative play time");
                }
            }

            //Validate wave progress
            foreach (var kvp in extendedData.WaveProgress)
            {
                var waveNumber = kvp.Key;
                var progress = kvp.Value;

                if (progress.WaveNumber != waveNumber)
                {
                    result.AddWarning($"Wave progress mismatch: key={waveNumber}, value={progress.WaveNumber}");
                }

                if (progress.EnemiesKilled < 0)
                {
                    result.AddError($"Wave {waveNumber} has negative enemies killed");
                }

                if (progress.ChampionsDefeated < 0)
                {
                    result.AddError($"Wave {waveNumber} has negative champions defeated");
                }

                if (progress.TimeTaken < 0)
                {
                    result.AddError($"Wave {waveNumber} has negative time taken");
                }
            }

            //Validate current wave
            if (extendedData.CurrentWave < 0)
            {
                result.AddError("Current wave is negative");
            }

            //Validate unlocked battlefields
            foreach (var kvp in extendedData.UnlockedBattlefields)
            {
                if (!Enum.IsDefined(typeof(BattlefieldType), kvp.Key))
                {
                    result.AddError($"Invalid battlefield type in unlocked battlefields: {kvp.Key}");
                }
            }
        }

        ///<summary>
        ///Checks if a save version is compatible.
        ///</summary>
        ///<param name="version">The save version.</param>
        ///<returns>True if compatible.</returns>
        private bool IsVersionCompatible(int version)
        {
            //Support versions 1 and 2
            return version >= 1 && version <= _currentSaveVersion;
        }

        ///<summary>
        ///Validates all saves in a directory.
        ///</summary>
        ///<param name="saveDirectory">The save directory.</param>
        ///<returns>Validation result for all saves.</returns>
        public ValidationResult ValidateAllSaves(string saveDirectory)
        {
            var result = new ValidationResult { IsValid = true };

            if (!System.IO.Directory.Exists(saveDirectory))
            {
                result.IsValid = false;
                result.AddError($"Save directory does not exist: {saveDirectory}");
                return result;
            }

            var files = System.IO.Directory.GetFiles(saveDirectory, "*.json");
            var saveManager = SaveManager.Instance;

            foreach (var filePath in files)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(filePath);
                    var saveData = System.Text.Json.JsonSerializer.Deserialize<SaveData>(json);

                    if (saveData != null)
                    {
                        var saveResult = Validate(saveData);
                        if (!saveResult.IsValid)
                        {
                            result.IsValid = false;
                            result.AddError($"Invalid save file: {System.IO.Path.GetFileName(filePath)}");
                            result.Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.AddError($"Failed to validate {System.IO.Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            Dlogger.Log(LogSubsystems.Save, LogLevel.Info,
                $"SaveValidator: Validated {files.Length} saves - {(result.IsValid ? "All valid" : "Some invalid")}");

            return result;
        }
    }

    ///<summary>
    ///Validation rule definition.
    ///</summary>
    public class ValidationRule
    {
        public ValidationCategory Category { get; set; }
        public string Description { get; set; }
        public Func<SaveData, bool> Validate { get; set; }
    }

    ///<summary>
    ///Validation category.
    ///</summary>
    public enum ValidationCategory
    {
        RequiredField,
        DataRange,
        Consistency,
        P120Integration,
        P100Integration
    }

    ///<summary>
    ///Validation result.
    ///</summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public override string ToString()
        {
            return $"ValidationResult: IsValid={IsValid}, Errors={Errors.Count}, Warnings={Warnings.Count}";
        }
    }
}
