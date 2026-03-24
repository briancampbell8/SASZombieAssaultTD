Asset Pipeline Initialization
=============================

Overview
--------

The asset pipeline is responsible for discovering, loading, and registering
all textures and JSON data required by the engine.

Flow
----

1. AssetDiscovery scans the configured asset root.
2. TextureLoader loads all supported texture files.
3. DataLoader parses JSON configuration and gameplay data.
4. AssetRegistry exposes a unified lookup surface to the rest of the engine.
