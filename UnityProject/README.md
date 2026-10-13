# Crevice Hero — recovered original Unity project

This project was reconstructed from the original `Crevice_Hero_1.1.5.apk` using AssetRipper 2.0.0 and the recovered Mono assembly.

## Verified engine version

The APK contains the Unity version marker **4.5.5f1**. The exported project settings also identify `4.5.5f1`. The user's earlier guess of `4.0.0.1f` was close in era but not the version in this APK.

## Recovered into this project

- Original serialized `SplashScene` and `PlayScene` scene files.
- 56 recovered prefabs, including the original character animation prefabs and skill background.
- Original Resources textures, sprite atlases, audio clips, fonts, materials, shaders, map/character/gem/language data.
- Decompiled gameplay and UI C# scripts from the game's Mono assembly, plus exported plugin sources and assemblies where present.
- Original Unity `ProjectSettings` and build scene list.

## What remains before claiming a faithful Windows build

1. Open with Unity **4.5.5f1** (or first test import compatibility in a newer editor without overwriting the recovered copy).
2. Resolve any compile errors from Android-only SDKs / missing third-party plugins and ensure Windows builds use the original gameplay scripts, not the Python prototype.
3. Build a Windows Standalone player and test both scenes.
4. Compare gameplay, animation, controls, physics, audio and UI side-by-side with the Android original. A recovered scene/project is a major step, not proof the Windows build is already identical.

The current project is a recovery result and may need editor-side repairs. Do not redistribute the original game's assets or code publicly; keep this repository private and respect the rights holder's terms.
