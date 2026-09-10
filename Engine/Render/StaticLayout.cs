// ====================================================================================================
//  FILE: StaticLayout.cs
//  PATH: ./Engine/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;// using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;  namespace SASZombieAssaultTD.Engine.Render;  ///<summary> ///Data model for static UI and map layout definitions. ///Loaded from JSON configuration files. ///</summary> public sealed class StaticLayout {     public List<StaticImageEntry> Images { get; set; } = new(); }  ///<summary> ///Individual static image entry with position, size, and layer information. ///</summary> public sealed class StaticImageEntry {     public string Id { get; set; } = string.Empty;     public string Path { get; set; } = string.Empty;     public int X { get; set; }     public int Y { get; set; }     public int Width { get; set; }     public int Height { get; set; }     public int Layer { get; set; }     public bool Visible { get; set; } = true; }

