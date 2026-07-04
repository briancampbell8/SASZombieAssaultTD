/*
File:    SystemRegistration.cs
Path:    Engine/GameRoot/SystemRegistration.cs
Purpose: P11-09-01 - Centralizes all system and manager registration.
         Ensures every subsystem is connected to engine in clean order.

Role:     System registration and dependency injection specialist.
         - ECS system registration functions
         - Manager registration functions
         - Controller registration functions
         - Subsystem wiring functions
         - Dependency resolution coordination

Notes:    Contains all registration logic extracted from GameRoot.
         Works with the ISystemRegistry for clean dependency injection.
         Registration order is maintained for proper dependency resolution.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Scenes;
namespace SASZombieAssaultTD.Engine
//
{
    ///<summary>
    ///Partial class containing system registration logic for GameRoot.
    ///</summary>
    public partial class GameRoot
    {
        ///<summary>
        ///Registers all core systems with the system registry.
        ///</summary>
        public void RegisterCoreSystems()
        {
            try
            {
                DLogger.Log("Starting core system registration...");

                //Register scene manager
                RegisterSceneManager();

                //Register other core systems as needed
                RegisterAdditionalSystems();

                DLogger.Log("Core system registration completed successfully");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Error, $"Core system registration failed: {ex.Message}");
                DLogger.Log(ex.ToString(), "Core system registration");
                throw;
            }
        }

        ///<summary>
        ///Registers the scene manager with the system registry.
        ///</summary>
        private void RegisterSceneManager()
        {
            var sceneManager = new SceneManager();
            _systemRegistry.RegisterService<SceneManager>(sceneManager);
            DLogger.Log("SceneManager registered with system registry");
        }

        ///<summary>
        ///Registers additional systems as needed.
        ///</summary>
        private void RegisterAdditionalSystems()
        {
            //Register other systems here as they are added
            //This is a placeholder for future system registration
            DLogger.Log("Additional systems registration completed");
        }

        ///<summary>
        ///Gets a registered service from the system registry.
        ///</summary>
        ///<typeparam name="T">The type of service to retrieve.</typeparam>
        ///<returns>The registered service instance.</returns>
        public T GetService<T>() where T : class
        {
            return _systemRegistry.GetService<T>() ?? throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }

        ///<summary>
        ///Gets a registered service from the system registry (nullable).
        ///</summary>
        ///<typeparam name="T">The type of service to retrieve.</typeparam>
        ///<returns>The registered service instance, or null if not found.</returns>
        public T? GetServiceOrNull<T>() where T : class
        {
            return _systemRegistry.GetService<T>();
        }

        ///<summary>
        ///Registers a service with the system registry.
        ///</summary>
        ///<typeparam name="T">The type of service to register.</typeparam>
        ///<param name="service">The service instance to register.</param>
        public void RegisterService<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _systemRegistry.RegisterService(service);
            DLogger.Log($"Service of type {typeof(T).Name} registered with system registry");
        }
    }
}
