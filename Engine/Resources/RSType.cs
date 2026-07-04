/*
File:    RSType.cs
Author:  BDC
Created: 2026-02-07
Purpose: Defines the categories of resources supported by the engine.
Notes:   Expandable enumeration used by loaders, registries, and metadata.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Represents the general category of a resource.
    ///Used for classification, validation, and loader routing.
    ///</summary>
    public enum RSType
    {
        Unknown = 0,

        //Visual assets
        Texture = 1,
        SpriteSheet = 2,

        //Audio assets
        Sound = 3,
        Music = 4,

        //Data assets
        Json = 5,
        Binary = 6,

        //Future expansion points
        Font = 7,
        Shader = 8
    }
}


