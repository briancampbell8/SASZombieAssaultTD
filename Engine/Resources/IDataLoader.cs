// ====================================================================================================
//  FILE: IDataLoader.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the IDataLoader module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    DataLoader.cs
Author:  BDC
Created: 2026-02-10

Purpose:
Loads data files from disk and parses JSON content when applicable.

Notes:
LoadAll was removed � AssetPipeline.LoadAll is the canonical bulk loader.

*/
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Assets
{
    public interface IDataLoader
    {
        static abstract object Load(string path);
        //  static abstract object Load(string path);
        //Already defined in DataLoader.cs as a static method,
        //but interfaces can't have static methods until C# 12.0, and even then they can't be abstract.   
    }
}

