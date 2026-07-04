//============================================================================
//File:        RSHandle.cs
//Program:     RSHandle
//Author:      BDC
//Created:     2026-02-07
//
//Purpose:     
//     Canonical handle type for the RS (Resource System) subsystem. This
//     program provides a deterministic wrapper around an RSKey and the
//     loaded asset instance resolved by the RS loaders. RSHandle is used
//     throughout the RS subsystem, including RSManager, RS loaders,
//     StaticLayoutLoader, and gameplay systems that depend on RS asset
//     resolution.
//
//Responsibilities:
//     • Maintain a stable association between an RSKey and its resolved
//       runtime instance.
//     • Provide deterministic type‑safe access to the underlying instance.
//     • Expose diagnostic information for debugging and subsystem tracing.
//     • Remain isolated from the future Asset subsystem and its types.
//
//Notes:
//     • RSHandle does not implement lifecycle management, reference
//       counting, or pipeline semantics. Those responsibilities belong to
//       RSManager and the RS loaders.
//     • This program reflects the current RS subsystem architecture and
//       must not be expanded beyond the capabilities of the RS pipeline.
//     • When the new Asset subsystem is introduced, RSHandle will be
//       retired in a controlled, non‑compounding migration.
//============================================================================

using System;
using SASZombieAssaultTD.Engine.Assets;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Represents a deterministic reference to a loaded RS asset instance.
    ///Provides type‑safe access and diagnostic information for RS subsystem
    ///consumers.
    ///</summary>
    public sealed class RSHandle
    {
        ///<summary>
        ///The unique RSKey identifying this asset within the RS subsystem.
        ///</summary>
        public RSKey Key { get; }

        ///<summary>
        ///The resolved runtime instance associated with this RSKey.
        ///Guaranteed non‑null after successful RS loading.
        ///</summary>
        public object Instance { get; }

        ///<summary>
        ///The concrete runtime type of the loaded instance.
        ///</summary>
        public Type InstanceType => Instance.GetType();

        ///<summary>
        ///Constructs a new RSHandle bound to the specified RSKey and instance.
        ///</summary>
        public RSHandle(RSKey key, object instance)
        {
            Key = key;
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        ///<summary>
        ///Returns the loaded instance cast to the specified type. Throws an
        ///InvalidCastException if the underlying instance is not compatible.
        ///</summary>
        public T As<T>() where T : class
        {
            return Instance as T
                ?? throw new InvalidCastException(
                    $"RS asset '{Key}' is not of type {typeof(T).Name}.");
        }

        ///<summary>
        ///Returns a diagnostic string describing this RSHandle.
        ///</summary>
        public override string ToString()
        {
            return $"RSHandle(Key={Key}, Type={InstanceType.Name})";
        }
    }
}
