# Crevice Hero — original Unity-engine reconstruction

This is the Unity-engine reconstruction track. The Python/Pygame prototype is not the target for exact fidelity.

## Verified original engine

The original Android package `Crevice_Hero_1.1.5.apk` contains the Unity version marker **4.5.5f1**. It is not Unity 4.0.0.1f. The recovered managed assembly is `Assembly-CSharp.dll`; the gameplay scripts in `Assets/Scripts` were decompiled from that assembly and are intended as source references for reconstructing the original project.

## Current state / important limitation

This is a source-and-data migration scaffold, not yet an openable or faithful Unity project. The original serialized scenes/prefabs, tk2d sprite collection/animation assets, plugin dependencies, and audio must still be reconstructed or recovered from the APK's Unity asset files. The copied scripts reference third-party packages and Unity scene objects and cannot compile as a complete game without those dependencies and scene wiring.

The exact route is to reconstruct the Unity 4.5.5f1 project and its original scene/prefab hierarchy, preserve the original gameplay scripts, then add a Windows Standalone target. Do not treat the existing Pygame executable as the faithful build.

## Source/data provenance

- `Assets/Scripts`: recovered/decompiled original C# gameplay scripts.
- `Assets/OriginalData`: extracted character, map, gem, and language data.

Keep this repository private. Original assets/code may be subject to the original rights holder's terms.
