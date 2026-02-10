# SAS: Zombie Assault TD - Modern Rebuild


## Table of Contents


### ✦ The Story Behind the Rebuild  
A look at why SAS: Zombie Assault TD needed a modern resurrection and what this project sets out to preserve.

- [Why This Rebuild Was Necessary](#why-this-rebuild-was-necessary)


### ✦ The Architecture of a Modern Classic  
How the engine is structured, why it’s modular, and how it stays future-proof.
- Timing Controller

- [Engine Architecture Overview](#engine-architecture-overview)
- Input Router
- Render Queue


### ✦ The Foundation: Assets & Data  
Where the game’s content lives, how it loads, and how the pipeline keeps everything stable.

- [Asset Pipeline and Data Flow](#asset-pipeline-and-data-flow)


### ✦ The Heartbeat of the Game  
Rendering, timing, and the systems that make the world feel alive and authentic.

- [Rendering and Timing Deep Dive](#rendering-and-timing-deep-dive)


### ✦ The Core Gameplay Experience  
Towers, enemies, waves, upgrades, and the logic that defines the battlefield.

- [Gameplay Systems Overview](#gameplay-systems-overview)


### ✦ The Interface Players Remember  
Menus, HUD, overlays, and the visual language that makes the game feel like SAS TD.

- [User Interface Systems Overview](#user-interface-systems-overview)
This project is a modern, standalone reconstruction of SAS: Zombie Assault TD, created to preserve the original experience after the shutdown of Adobe Flash rendered the
classic version unplayable for most players. The goal is to deliver a faithful, offline, future-proof edition of the game while replacing outdated dependencies with a clean,
modular, and framework-agnostic engine that can evolve for years to come.

The original Flash release depended entirely on technologies that no longer exist in modern browsers or operating systems. Once Flash reached its end-of-life in 2020, the
game's core functionality - loading assets, saving progress, and handling UI elements - became inaccessible without workarounds that were fragile, inconsistent, or technically
unsafe. What had once been a widely enjoyed tower-defense experience effectively vanished overnight, preserved only in scattered SWF files and incomplete archival attempts.

This rebuild began as a preservation effort, driven by the belief that classic games deserve to remain playable without relying on abandoned software or proprietary systems
that can disappear at any moment. The project aims to restore the original gameplay as accurately as possible while ensuring that the underlying engine is modern, maintainable,
and independent of any single framework or platform. Every component is being rebuilt with clarity and longevity in mind, allowing the game to survive technological shifts
without requiring another full rewrite.

This is not a remaster, reinterpretation, or reimagining. It is a preservation-first reconstruction that respects the original design, mechanics, pacing, and feel of SAS:
Zombie Assault TD. The objective is simple: keep the game alive, accessible, and enjoyable for players today and for players decades from now, without the fragility and
limitations that defined the Flash era.


## Why This Rebuild Was Necessary

When Adobe Flash reached its end-of-life in 2020, the original SAS: Zombie Assault TD became unplayable for most players. The game depended entirely on the Flash runtime and
on Ninja Kiwi's servers for loading assets, saving progress, and handling UI elements. Once Flash support was removed from modern browsers and operating systems, the classic
version effectively disappeared, surviving only as archived SWF files that could no longer run in a standard environment.

The first step in preserving the game was extracting and restoring the original SWF. This required removing network calls, rebuilding missing functionality, and ensuring the
game could launch and operate without relying on external services. The goal was not to modify the original design but to reconstruct the environment it needed in order to
function independently, safely, and predictably on modern systems.

The rebuild initially used MonoGame as a temporary bridge to stand up a playable version. MonoGame provided a rendering layer, input handling, and a basic game loop, allowing
the project to progress quickly at a time when the priority was simply restoring functionality. However, as development continued, it became clear that MonoGame introduced
long-term maintenance issues. Its content pipeline, asset builder, and framework assumptions created friction with the project's preservation-first philosophy.

To ensure the game could evolve without being tied to any specific framework, the decision was made to remove MonoGame entirely and replace it with a custom, framework-
agnostic engine. This new architecture emphasizes clarity, modularity, and independence. Every system - rendering, input, timing, assets, UI, and gameplay - is being rebuilt
in a way that can adapt to future technologies without requiring another full rewrite or migration.

This rebuild exists because the original game deserves to be preserved with accuracy and respect. Players should be able to experience SAS: Zombie Assault TD without relying
on abandoned software, fragile emulation layers, or outdated dependencies. The modern engine ensures the game can survive long-term, remain maintainable, and continue to
evolve while staying faithful to the original design and intent of the Flash-era release.


## Engine Architecture Overview

The modern engine for SAS: Zombie Assault TD is designed around clarity, modularity, and long-term independence. Its purpose is to replace the fragile, framework-dependent
environment of the original Flash release with a clean, predictable architecture that can survive future platform changes without requiring another full rewrite. Every system
is built to be self-contained, easy to reason about, and capable of evolving as technology shifts.

At the highest level, the engine is composed of several core subsystems: rendering, input, timing, assets, UI, and gameplay. Each subsystem has a clearly defined role and
communicates with others through simple, well-structured boundaries. This separation ensures that changes in one area do not cascade unpredictably into others, preserving the
stability and maintainability of the entire project.

The rendering system is responsible for drawing the game world, UI elements, and effects. It is intentionally framework-agnostic, relying on a minimal abstraction layer that
can be adapted to different graphics backends in the future. By avoiding assumptions about specific libraries or APIs, the engine remains flexible and portable across a wide
range of platforms and environments.

Input handling is similarly isolated. The system captures player actions, normalizes them into a consistent format, and forwards them to the gameplay layer. This approach
ensures that input logic remains stable even if the underlying hardware or operating system changes. Whether the game is played with a mouse, keyboard, or future input
devices, the engine can adapt without requiring major structural changes.

The timing subsystem manages the game loop, frame updates, and time scaling. It provides a reliable foundation for animation, movement, and simulation, ensuring that gameplay
remains consistent regardless of hardware performance. By centralizing timing logic, the engine avoids the drift and inconsistency that often affected Flash-era games and
ensures that the pacing of the original experience is preserved with accuracy.

Asset management is another critical component. The system loads, organizes, and provides access to images, data files, and other resources. It is designed to be simple,
predictable, and free from the pipeline constraints that made MonoGame difficult to maintain. Assets are handled in a way that supports both the original game's structure and
future expansions or modifications without introducing unnecessary complexity.

The UI subsystem manages menus, buttons, overlays, and in-game displays. It is built to be lightweight and modular, allowing new elements to be added or existing ones to be
modified without disrupting the rest of the engine. This flexibility is essential for long-term preservation, as UI requirements often evolve while the core experience must
remain familiar to returning players.

A critical part of this architecture is its commitment to preserving the original look and feel of SAS: Zombie Assault TD. Every system is designed not only for technical
clarity and long-term stability but also to ensure that the game continues to play, sound, and present itself exactly as traditional fans remember. Visual timing, animation
behavior, UI layout, enemy pacing, and tower responsiveness are all reconstructed with accuracy as the guiding principle. The goal is not to modernize the experience but to
faithfully preserve it, allowing returning players to feel immediately at home while ensuring the game remains accessible for decades to come.

Finally, the gameplay subsystem contains the logic that defines towers, enemies, waves, upgrades, and interactions. It is structured to mirror the original Flash design as
closely as possible while benefiting from modern clarity and organization. By keeping gameplay logic separate from rendering, input, and UI, the engine ensures that the core
experience remains faithful to the original game.

Together, these subsystems form a cohesive architecture that supports both preservation and future development. The engine is not tied to any specific framework, library, or
platform. Instead, it is built on principles of modularity, transparency, and long-term maintainability. This design ensures that SAS: Zombie Assault TD can continue to exist,
evolve, and remain playable for decades to come.


## Asset Pipeline and Data Flow

The asset pipeline for SAS: Zombie Assault TD is built to be clear, predictable, and long-lasting. Its purpose is to provide a stable flow of data from disk to engine, ensuring
that assets load consistently across platforms while remaining easy to maintain, extend, and understand. Every part of the pipeline is designed with preservation in mind so
the game can continue to function reliably as technology evolves.

At its core, the pipeline manages images, metadata, configuration files, and other resources required by the engine. Assets are stored in straightforward, human-readable
formats whenever possible, avoiding unnecessary complexity and ensuring that future developers, archivists, or modders can work with the project without specialized tools or
proprietary build steps. This approach keeps the project accessible and resilient over time.

The loading process begins with a lightweight discovery system that scans the asset directory and identifies available resources. Each file is validated, categorized, and
mapped to an internal representation that the engine can use without ambiguity. By performing this work at runtime, the engine maintains flexibility and avoids the fragility
that often comes from rigid or tool-dependent pipelines.

Once assets are discovered, they are passed to specialized loaders responsible for interpreting each file type. Image loaders decode textures into a format suitable for
rendering, while data loaders parse configuration files that define enemy stats, tower properties, wave structures, and UI layout information. These loaders are modular and
replaceable, allowing the engine to support new formats or updated standards without requiring structural changes.

Transparency is a guiding principle of the pipeline. Every step—from file discovery to final in-engine representation—is designed to be observable and debuggable. If an asset
is missing, malformed, or incompatible, the engine reports the issue clearly and continues running whenever possible. This behavior ensures that problems are easy to diagnose
and prevents the unpredictable failures that once plagued older game environments.

The data flow within the engine is structured for clarity and consistency. Once assets are loaded, they are stored in centralized registries that provide fast, reliable
access to textures, metadata, and configuration values. Subsystems such as rendering, gameplay, and UI retrieve the resources they need through these registries, ensuring
that asset usage remains consistent across the entire engine. This design prevents duplication, reduces memory overhead, and eliminates the risk of mismatched data.

A central goal of the pipeline is to preserve the original look and feel of SAS: Zombie Assault TD. Textures are loaded at their authentic resolution unless explicitly
scaled, color profiles are preserved, and animation timing is derived from the same conceptual structures used in the Flash version. The modern engine restores the pure
experience fans remember, delivering the same gameplay, pacing, and enjoyment while removing the old service layers that once stood between players and the game.

The pipeline is also built for growth. New towers, enemies, maps, or UI elements can be added simply by placing new files in the appropriate directory and defining their
properties in a configuration file. No rebuild steps or external tools are required. This flexibility ensures that the project can evolve naturally while remaining faithful
to its preservation-first philosophy.

Together, these systems form a robust, transparent, and future-proof asset pipeline that supports both the authenticity of the original game and the long-term maintainability
of the modern engine. By focusing on clarity, accessibility, and preservation, the pipeline ensures that SAS: Zombie Assault TD remains adaptable and easy to sustain for
generations of players and developers to come.


## Rendering and Timing Deep Dive

The rendering and timing systems form the heartbeat of the modern SAS: Zombie Assault TD engine. Together, they determine how often the world is updated, how frames are drawn,
and how closely the experience matches the pacing and responsiveness of the original game. Their design focuses on consistency, clarity, and long-term stability so that the
game feels the same every time it is played, regardless of hardware or platform.

At the core of the rendering system is a simple, predictable loop that draws the game world, user interface, and visual effects in a well-defined order. The engine maintains
a clear separation between what is drawn and how it is drawn. Game logic decides what should appear on screen, while the rendering layer is responsible for turning that
information into pixels. This separation keeps the visuals flexible and allows the engine to adapt to different graphics backends without changing gameplay behavior.

The rendering process is organized into distinct stages. First, the engine prepares the scene by gathering the current state of towers, enemies, projectiles, and UI elements.
Next, it issues draw calls in a consistent sequence so that background elements, gameplay objects, and overlays appear in the correct order. Finally, it presents the completed
frame as a coherent image. This structured approach ensures that visual layering remains stable and that the game always looks intentional and readable.

Timing is managed by a dedicated subsystem that controls how often the game updates and how those updates relate to rendering. The engine tracks elapsed time between frames
and uses that information to advance animations, move enemies, process tower actions, and handle wave progression. By basing these behaviors on measured time rather than raw
frame counts, the engine maintains consistent gameplay even when frame rates vary.

A key responsibility of the timing system is to preserve the pacing of the original SAS: Zombie Assault TD. Enemy movement, attack intervals, tower firing rates, and wave
timing are all tuned to match the feel of the Flash-era release. The engine uses stable time steps and carefully controlled interpolation so that motion appears smooth while
gameplay remains predictable. This balance allows the game to feel responsive without drifting away from the rhythm that long-time players remember.

The relationship between rendering and timing is carefully managed to avoid visual or gameplay inconsistencies. Game logic updates are processed in a controlled sequence, and
rendering reflects the most recent stable state of the world. This prevents situations where visuals appear to get ahead of or fall behind the underlying simulation. The
result is a game that feels cohesive, where what the player sees always matches what the game is actually doing.

Animations are driven by the same timing foundation. Each animation uses time-based progression so that it plays at the correct speed regardless of frame rate. This applies
to enemy movement cycles, tower firing effects, UI transitions, and environmental details. By grounding animations in consistent timing data, the engine preserves the visual
identity of the original game while ensuring that motion remains smooth on modern systems.

The rendering and timing systems also support future growth. New visual effects, UI elements, or gameplay features can be added without disrupting the core loop. As long as
they respect the established timing and rendering boundaries, they will integrate cleanly into the existing structure. This makes it possible to expand the game while keeping
the experience familiar and stable for returning players.

Together, the rendering and timing subsystems provide a reliable foundation for the entire engine. They ensure that SAS: Zombie Assault TD looks and feels like the game fans
remember, while operating on a modern, maintainable, and clearly defined technical base. By focusing on consistency, separation of concerns, and preservation of pacing, this
design keeps the experience authentic today and sustainable for the future.


## Gameplay Systems Overview

The gameplay systems of SAS: Zombie Assault TD define how the world behaves, how enemies advance, how towers respond, and how players interact with the battlefield. These
systems form the core experience of the game, shaping its pacing, strategy, and moment-to-moment tension. The modern engine rebuilds these systems with clarity and structure
while preserving the authentic feel that long-time players remember.

At the center of gameplay is the tower system. Each tower represents a distinct tactical role, from rapid-fire suppression to high-damage precision to area control. Towers
track enemies, evaluate firing opportunities, and execute attacks based on well-defined behaviors. Their logic is organized into modular components that handle targeting,
firing, cooldowns, upgrades, and visual effects. This structure ensures that each tower behaves consistently while remaining easy to extend or refine.

Enemy behavior is driven by a dedicated subsystem that manages movement, pathfinding, health, resistances, and attack patterns. Enemies advance along predetermined routes,
react to tower fire, and apply pressure to the player through escalating waves. Their attributes are defined in data files that mirror the structure of the original game,
allowing the modern engine to reproduce enemy pacing, durability, and threat levels with high accuracy.

Waves form the backbone of the game’s progression. Each wave defines which enemies appear, when they spawn, and how they challenge the player. The wave system coordinates
timing, escalation, and difficulty curves so that the game unfolds with the same rhythm as the original release. The engine processes wave data through a clear sequence of
stages—initialization, spawning, advancement, and completion—ensuring that each wave feels intentional and balanced.

Upgrades allow players to strengthen their defenses and adapt to increasing threats. Each tower supports multiple upgrade paths that enhance damage, range, firing rate, or
special abilities. The upgrade system is data-driven, enabling new upgrade tiers or entirely new tower types to be added without altering core gameplay logic. This approach
keeps the system flexible while preserving the strategic depth that defined the original game.

Interactions between towers and enemies are governed by a consistent set of rules. Damage calculations, status effects, projectile behavior, and collision detection all
operate on predictable principles. These rules ensure that gameplay outcomes are fair, readable, and aligned with player expectations. By grounding interactions in clear
mechanics, the engine maintains the tactical clarity that made the original game engaging.

Resource management plays a key role in shaping player decisions. Players earn currency by defeating enemies and completing waves, and they spend it to place towers, upgrade
existing defenses, or prepare for future threats. The resource system is tuned to match the pacing of the original game, ensuring that players experience the same sense of
progression and strategic tension.

The modern engine restores the pure experience fans remember, delivering the same gameplay, pacing, and enjoyment while removing the old service layers that once stood
between players and the game. Every gameplay system—towers, enemies, waves, upgrades, and interactions—is designed to feel familiar while benefiting from modern clarity and
structure.

Together, these gameplay systems create a cohesive, responsive, and strategically rich experience. They preserve the identity of SAS: Zombie Assault TD while providing a
clean, maintainable foundation for future enhancements. By organizing gameplay logic into clear subsystems, the engine ensures that the game remains faithful to its roots and
sustainable for years to come.


## User Interface Systems Overview

The user interface system defines how players interact with SAS: Zombie Assault TD, how information is presented, and how the game communicates moment-to-moment decisions.
The modern UI framework is built to be lightweight, modular, and faithful to the original layout and behavior while benefiting from the clarity and structure of a modern
engine. Its purpose is to deliver the same intuitive experience returning players remember while ensuring the system remains easy to maintain and extend.

At its foundation, the UI system is composed of discrete elements such as buttons, panels, overlays, icons, and text displays. Each element is represented by a self-contained
component that defines its position, appearance, behavior, and interaction rules. These components can be combined to form menus, HUD elements, and in-game notifications,
allowing the UI to be assembled from predictable building blocks.

The UI layout is driven by a data-first approach. Each screen—main menu, map selection, upgrade interface, and in-game HUD—is defined through structured configuration files
that specify element placement, sizing, and behavior. This approach mirrors the structure of the original game while giving the modern engine the flexibility to adjust layouts
for different resolutions or future enhancements without rewriting core logic.

Interaction handling is managed by a dedicated subsystem that processes input events and routes them to the appropriate UI components. Buttons respond to clicks, sliders
adjust values, and overlays react to game state changes. By separating interaction logic from rendering, the UI remains responsive and consistent even as the underlying
graphics backend evolves.

The in-game HUD is designed to preserve the clarity and pacing of the original experience. Health bars, wave counters, tower information, and resource displays update in
real time based on gameplay events. Each element is refreshed through a stable update loop that ensures information is always accurate and synchronized with the game state.
This consistency helps players make informed decisions without distraction or delay.

Menus and overlays follow the same principles. Whether the player is selecting a map, upgrading a tower, or reviewing mission results, each screen is built from modular
components that can be rearranged or extended without disrupting the rest of the system. This modularity ensures that future additions—new menus, expanded settings, or
enhanced accessibility options—can be integrated cleanly.

A key goal of the UI system is to preserve the visual identity of SAS: Zombie Assault TD. Fonts, iconography, spacing, and animation timing are tuned to match the original
presentation while benefiting from modern rendering clarity. The modern engine restores the pure experience fans remember, delivering the same readability and responsiveness
without the service layers or constraints that once shaped the Flash-era interface.

The UI system also supports dynamic updates. Notifications, warnings, and contextual prompts appear and disappear based on gameplay conditions, ensuring that players receive
the right information at the right time. These behaviors are driven by clear rules that keep the interface predictable and unobtrusive.

Together, the UI systems create a cohesive, readable, and responsive interface that supports both new players and long-time fans. By organizing UI logic into modular
components and data-driven layouts, the engine ensures that the interface remains faithful to its roots while providing a clean foundation for future enhancements. This
design keeps the experience familiar, accessible, and sustainable for years to come.



---

## Development Philosophy: Build Upward

The engine for SAS: Zombie Assault TD is built on a strict, sequential workflow designed to prevent drift, ambiguity, and partial states. Every change follows a build-upward model: foundations are stabilized before new layers are added, and no step is skipped or reordered once defined.

Milestones are treated as hard boundaries. A phase is not considered complete until its structure, behavior, and automation are fully verified. This approach ensures that debugging happens at the correct layer, that regressions are easy to trace, and that the engine remains predictable as it grows.

Automation plays a central role in this philosophy. PowerShell tools are used to maintain structure, enforce naming and layout rules, and keep the project audit-friendly. Logs, appendixes, and structural snapshots exist so that future developers—and future versions of this project—can understand exactly how and why the engine evolved.

In short: the engine is not built through ad-hoc changes. It is built upward, one verified layer at a time.

## Future Platform Direction

The modern engine for SAS: Zombie Assault TD is intentionally designed to be platform-agnostic. While the current focus is on stabilizing the desktop version and ensuring the engine runs cleanly at the grassroots level, the long-term goal is to support WebAssembly (WASM) as a primary deployment target.

WASM allows the entire engine—gameplay logic, systems, timing, scenes, and core architecture—to run directly inside a web browser without rewriting the underlying code. Only the platform layer (rendering, input, and asset loading paths) needs to adapt, while the engine’s scaffolding, namespaces, and modular subsystems remain unchanged.

This approach ensures that once the engine is fully operational on desktop, it can be brought to the web smoothly and predictably. The project is being built with this transition in mind so that future changes, enhancements, and platform shifts can be made without requiring another full rewrite.

In short: get the plane flying on desktop first, then land it safely on the web—without rebuilding the plane.

## Current Engine Status

The engine is currently in the foundational stabilization phase, with a focus on getting the core systems running cleanly at the grassroots level.

- Phase 1 structural verification: **Complete**
  - Engine directory structure established
  - Core, systems, UI, and gameplay stubs populated
  - Legacy Flash-derived scaffolding removed in favor of the modern /Engine layout
- Namespace alignment: **Complete**
- Empty file population for required engine components: **Complete**
- PowerShell automation pipeline: **Operational**
  - ProjectStructure and PowerShellLog appendixes maintained
  - Engine scaffolding updates tracked and auditable

The next major milestone is DebugSession-0001, focused on:
- Launching the engine under F5
- Verifying GameLoop and initial scene initialization
- Observing runtime behavior and capturing the first baseline

Once the engine is running reliably at this level, subsequent sessions will target rendering, asset loading, and eventually the platform abstraction required for WASM.

## Milestone History

This section records major structural and architectural milestones for the modern SAS: Zombie Assault TD engine. Entries are added intentionally to reflect completed phases, not in-progress work.

- [2026-01-21] Engine scaffolding established for core, systems, UI, gameplay, and tools components.
- [2026-01-21] Event Dispatcher subsystem stub created and integrated into the engine structure.

# Appendixes
## Appendix A — ProjectStructure.md  
A complete, auto-generated snapshot of the project’s directory layout.  
This appendix serves as the structural map of the engine, allowing developers and archivists to understand how the project is organized at any point in time.

This file is maintained by the automation pipeline and reflects the most recent structure after cleanup, refactoring, or expansion.


## Appendix B — PowerShellLog.md  
A chronological audit trail of every automated action performed by the project’s PowerShell tools.  
This appendix preserves the historical lineage of changes, ensuring that every modification is traceable, reviewable, and reproducible.

It functions as the project’s black box recorder, documenting the evolution of the engine and its supporting files.


## Appendix C — Storytelling_Engine_Workflow.md  
The master workflow document that defines the philosophy, sequencing, and operational logic behind the storytelling engine.  
This appendix acts as the high-level guide for how the engine is built, maintained, and extended.

It is the source-of-truth reference for restoration, modernization, and future development.

### Engine Scaffolding Update (2026-01-21 09:54:48)
- Engine directories and stub files have been created for all core, systems, UI, gameplay, and tools components.
- Automation build scripts exist for each subsystem, ready for population.

### Event Dispatcher Subsystem Update (2026-01-21 10:00:11)
- Event Dispatcher subsystem stub created and integrated into the engine structure.


## Appendix D — Legacy SAS TD Structure (Historical Reference)

Before the modern engine rebuild, the project contained a large number of legacy files extracted from the original Flash-based SAS: Zombie Assault TD. These files represented the structure of the SWF decompilation and included placeholder .cs files for enemies, towers, UI, scenes, and systems that existed in the Flash version but were not part of the new engine.

These legacy files were preserved temporarily to document the shape of the original game and to guide subsystem reconstruction. They served as an archaeological layer during the rebuild, helping to map old concepts to the new modular architecture.

### Original Legacy Structure (Summarized)
The Flash-derived dump included directories such as:

- /Core
- /Entities
- /Systems
- /Scenes
- /UI
- /Graphics
- /Projectiles
- /Towers
- /Zombies

These folders contained empty or partial .cs files that mirrored the naming conventions of the original SWF but did not contain usable logic.

### Old → New Subsystem Mapping
| Legacy Concept | Modern Engine Subsystem |
|----------------|-------------------------|
| Enemy logic | Engine/Entities + Engine/Managers/EnemyManager.cs |
| Tower logic | Engine/Systems/Gameplay/TowerBase.cs |
| Projectile behavior | Engine/Systems/Gameplay/ProjectileSystem.cs |
| Wave progression | Engine/Systems/Gameplay/WaveController.cs |
| UI elements | Engine/Systems/UI |
| Rendering | Engine/Systems/RenderQueue.cs |
| Input | Engine/Systems/InputRouter.cs |
| Timing | Engine/Core/TimingController.cs |
| Asset loading | Engine/Systems/Assets |

### Removal of Legacy Files
Once the modern engine structure was fully established, all legacy .cs files were removed to prevent confusion, reduce noise, and maintain a clean, audit-friendly codebase. The modern engine now lives exclusively under:

/Engine

This appendix preserves the historical context so the codebase can remain clean without losing the lineage of the original game.

## Development Philosophy: Build Upward

The engine for SAS: Zombie Assault TD is built on a strict, sequential workflow designed to prevent drift, ambiguity, and partial states. Every change follows a build-upward model: foundations are stabilized before new layers are added, and no step is skipped or reordered once defined.

Milestones are treated as hard boundaries. A phase is not considered complete until its structure, behavior, and automation are fully verified. This approach ensures that debugging happens at the correct layer, that regressions are easy to trace, and that the engine remains predictable as it grows.

Automation plays a central role in this philosophy. PowerShell tools are used to maintain structure, enforce naming and layout rules, and keep the project audit-friendly. Logs, appendixes, and structural snapshots exist so that future developers—and future versions of this project—can understand exactly how and why the engine evolved.

In short: the engine is not built through ad-hoc changes. It is built upward, one verified layer at a time.

## Future Platform Direction

The modern engine for SAS: Zombie Assault TD is intentionally designed to be platform-agnostic. While the current focus is on stabilizing the desktop version and ensuring the engine runs cleanly at the grassroots level, the long-term goal is to support WebAssembly (WASM) as a primary deployment target.

WASM allows the entire engine—gameplay logic, systems, timing, scenes, and core architecture—to run directly inside a web browser without rewriting the underlying code. Only the platform layer (rendering, input, and asset loading paths) needs to adapt, while the engine’s scaffolding, namespaces, and modular subsystems remain unchanged.

This approach ensures that once the engine is fully operational on desktop, it can be brought to the web smoothly and predictably. The project is being built with this transition in mind so that future changes, enhancements, and platform shifts can be made without requiring another full rewrite.

In short: get the plane flying on desktop first, then land it safely on the web—without rebuilding the plane.

## Current Engine Status

The engine is currently in the foundational stabilization phase, with a focus on getting the core systems running cleanly at the grassroots level.

- Phase 1 structural verification: **Complete**
  - Engine directory structure established
  - Core, systems, UI, and gameplay stubs populated
  - Legacy Flash-derived scaffolding removed in favor of the modern /Engine layout
- Namespace alignment: **Complete**
- Empty file population for required engine components: **Complete**
- PowerShell automation pipeline: **Operational**
  - ProjectStructure and PowerShellLog appendixes maintained
  - Engine scaffolding updates tracked and auditable

The next major milestone is DebugSession-0001, focused on:
- Launching the engine under F5
- Verifying GameLoop and initial scene initialization
- Observing runtime behavior and capturing the first baseline

Once the engine is running reliably at this level, subsequent sessions will target rendering, asset loading, and eventually the platform abstraction required for WASM.

## Milestone History

This section records major structural and architectural milestones for the modern SAS: Zombie Assault TD engine. Entries are added intentionally to reflect completed phases, not in-progress work.

- [2026-01-21] Engine scaffolding established for core, systems, UI, gameplay, and tools components.
- [2026-01-21] Event Dispatcher subsystem stub created and integrated into the engine structure.
