// ====================================================================================================
//  FILE: ICompositeCreator.cs
//  PATH: Engine/Interfaces/ICompositeCreator.cs
//  PROGRAM: ICompositeCreator.cs
//  MODULE: Resource Management Framework
//
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering,
//      validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
// ====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Unified, deterministic resource creation and resolution interface.
    /// </summary>
    public interface ICompositeCreator
    {
        /// <summary>
        /// Resolves a resource of the specified type using a deterministic key.
        /// </summary>
        /// <typeparam name="TResource">The resource type (texture, font, data, etc.).</typeparam>
        /// <param name="key">Deterministic resource key or identifier.</param>
        /// <returns>The resolved resource instance.</returns>
        TResource Resolve<TResource>(string key);

        /// <summary>
        /// Attempts to resolve a resource of the specified type using a deterministic key.
        /// </summary>
        /// <typeparam name="TResource">The resource type (texture, font, data, etc.).</typeparam>
        /// <param name="key">Deterministic resource key or identifier.</param>
        /// <param name="resource">The resolved resource instance if found.</param>
        /// <returns>True if the resource was found and validated; otherwise false.</returns>
        bool TryResolve<TResource>(string key, out TResource resource);

        /// <summary>
        /// Registers a resource factory for a given type and key space. Factories must be pure and deterministic for a
        /// given key.
        /// </summary>
        /// <typeparam name="TResource">The resource type produced by the factory.</typeparam>
        /// <param name="factory">Factory function that creates or loads the resource.</param>
        void RegisterFactory<TResource>(Func<string, TResource> factory);
    }
}
