/*
File:    DataLoader.cs
Author:  BDC
Created: 2026-02-10

Purpose:
Loads data files from disk and parses JSON content when applicable.

Notes:
LoadAll was removed � AssetPipeline.LoadAll is the canonical bulk loader.

*/
using SASZombieAssaultTD.Engine.Diagnostics;

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
