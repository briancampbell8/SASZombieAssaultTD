# Codebase map: Locate and run Engine/UI/HUDPanel_Finalizer.cs

Purpose
- Quick reference to find, open, build, and run the file: Engine/UI/HUDPanel_Finalizer.cs

Paths
- Repository root (absolute): E:\BDC\Projects\SASZombieAssaultTD\
- Solution file: E:\BDC\Projects\SASZombieAssaultTD\SASZombieAssaultTD.slnx
- Target file (absolute): E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\HUDPanel_Finalizer.cs
- Target file (relative): Engine/UI/HUDPanel_Finalizer.cs

Which project contains the file
- The file lives under the Engine folder. Look for an "Engine" project (Engine/*.csproj) in the solution. In Solution Explorer expand the Engine project to find the UI folder and HUDPanel_Finalizer.cs.

Open the file in Visual Studio (recommended)
1. Launch Visual Studio and open the solution: File -> Open -> Project/Solution -> select SASZombieAssaultTD.slnx
2. In Solution Explorer expand the Engine project.
3. Expand the UI folder and double-click HUDPanel_Finalizer.cs to open it.
4. Alternative quick-open: Press Ctrl+; (Go to) and type "HUDPanel_Finalizer.cs".

Run and debug the code that uses HUDPanel_Finalizer.cs
- HUDPanel_Finalizer.cs is a source file; to execute its logic you must run the project that hosts the game/app.
- Typical steps:
  1. Identify the startup project in Solution Explorer (the main game executable project). Right-click the intended startup project and choose "Set as Startup Project".
  2. Build the solution: Debug -> Build Solution (or press Ctrl+Shift+B).
  3. Set breakpoints inside HUDPanel_Finalizer.cs where you want to inspect behavior.
  4. Start debugging: Debug -> Start Debugging (F5). Execution will hit breakpoints when the code path runs.

Command-line (PowerShell) operations
- From repository root (pwsh.exe):
  cd E:\BDC\Projects\SASZombieAssaultTD\
  dotnet build SASZombieAssaultTD.slnx -c Debug

- To locate the project that contains the file by searching for the file in the workspace (PowerShell):
  Get-ChildItem -Path . -Filter HUDPanel_Finalizer.cs -Recurse | Select-Object FullName

- If you know the Engine project file name (for example Engine/Engine.csproj), you can build or run it directly:
  dotnet build Engine/Engine.csproj -c Debug
  dotnet run --project Engine/Engine.csproj

Notes & troubleshooting
- If the file does not compile or causes errors when building, open the Error List in Visual Studio and inspect the first errors. Fix missing references or incorrect namespaces in the Engine project.
- If HUDPanel_Finalizer.cs is a partial class or referenced from other files, use Solution Explorer or "Find All References" (right-click symbol -> Find All References) to see callsites and the expected runtime flow.
- If you need to step into engine-specific systems (rendering, UI loop), ensure the correct startup scene/entry point is used so HUD code is executed.

Quick checklist
- [ ] Open SASZombieAssaultTD.slnx in Visual Studio
- [ ] Locate Engine -> UI -> HUDPanel_Finalizer.cs
- [ ] Set startup project to the main game project
- [ ] Build solution (Ctrl+Shift+B)
- [ ] Set breakpoints in HUDPanel_Finalizer.cs
- [ ] Start debugging (F5)

If you want, I can create a small debugging checklist tailored to the Engine project after scanning the solution and project files to identify the exact startup project and csproj names.