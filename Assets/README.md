# Procedural Level Toolkit (Unity)

This package provides:
- An Editor Window to generate procedural **Dungeons** and **Terrains** inside the Unity Editor.
- Runtime generators with adjustable settings (seeded/repeatable).
- Optional object placement for props/NPCs.
- An example scene and example ScriptableObjects (created at runtime by you).

## Tested With
- Unity 2021.3+ (should work up to 2023/2024 LTS).

## Install
1. Unzip this folder and copy the `Assets/ProceduralLevelToolkit` directory into your Unity project's `Assets` folder.
2. Open Unity. Unity will compile the scripts.
3. Open the example scene: `Assets/ProceduralLevelToolkit/Examples/ProceduralDemo.unity` (or any scene).
4. Open the tool via menu: **Tools ▶ Procedural Level Toolkit**.

## Quick Start (Dungeon)
1. Open the Editor Window (**Tools ▶ Procedural Level Toolkit**).
2. Select **Generator Type = Dungeon**.
3. Adjust Width/Height, Fill %, Smoothing Steps, and Seed (optional).
4. Click **Generate Dungeon**. A GameObject named `Dungeon_Generated` is created.
5. If no prefabs are assigned, primitive cubes/planes are used.

## Quick Start (Terrain)
1. In the Editor Window select **Generator Type = Terrain**.
2. Adjust Width/Height, Scale, Height Multiplier and Seed (optional).
3. Click **Generate Terrain**. A GameObject named `Terrain_Generated` is created.

## Notes
- All output is parented under a single top-level GameObject for easy deletion.
- A **seed** ensures repeatability. Leave seed blank for random.
- Performance: Very large sizes can take time—start small and scale up.
- Feel free to extend with different algorithms (BSP, Drunkard Walk, Voronoi biomes, etc.).

## Folder Structure
- `Runtime/Dungeon/` — Dungeon generation.
- `Runtime/Terrain/` — Terrain mesh generation.
- `Runtime/Placement/` — Simple object placer.
- `Runtime/Settings/` — ScriptableObject settings (optional).
- `Runtime/Utils/` — Helpers (Noise, RNG).
- `Editor/` — EditorWindow and menu integration.

Enjoy!