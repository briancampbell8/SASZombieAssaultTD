Scene System
============

Overview
--------

The scene system organizes game flow into discrete scenes (main menu, game,
pause, etc.) and coordinates transitions between them.

Architecture
------------

- Scene: Base class with Initialize, Update, Render.
- SceneManager: Owns the active scene stack.
- SceneStack / SceneTransition: Push/pop/replace semantics for navigation.
