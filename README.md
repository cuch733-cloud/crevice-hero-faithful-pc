# Crevice Hero — Faithful PC Reconstruction

A private-use PC reconstruction of Crevice Hero, using the prior analysis work rather than the earlier endless-runner placeholder.

## What this project reuses

- The original GameController, Hero, Map, Rock, Gem, and related C# gameplay scripts decompiled from the supplied Unity/Mono build.
- The game's embedded char, map, and gem data, extracted from the original Unity assets and decoded into reference/original_data/.
- Original sprite atlases, 16 cropped original gem sprites, all recovered theme 0–4 stage textures, 12 original parallax backgrounds, character artwork, and the title logo from the extracted assets.
- The original game's 16-character roster, character speed/size/skill metadata, map-phase timings, and ceiling-slam cycle as implementation references.

## Current PC build

- Title screen, character select (16 original names and character stats), options, pause, result screen, and persistent best-distance/gem save.
- Side-scrolling cave movement using left/right controls, jumping, collectible gems, variable rock heights, paired crevices, a warning phase, moving ceiling slam/recovery phases, character-specific skill/cooldown behaviour, and rising map difficulty.
- Animated sprite frames cropped from the recovered original character atlases, using each character’s original collider width/height and configured walk speed.
- Windows x64 EXE built by GitHub Actions on every push to main.

## Controls

- A/D or Left/Right — move
- Space / W / Up — jump
- E / Shift — use the selected character's skill
- Esc — pause / return
- C — character select from title screen

## Run from source

1. Install Python 3.12.
2. Run: python -m pip install -r requirements.txt
3. Run: python crevice_hero.py

On Windows, run build_windows.bat, or download the workflow artifact from the Actions tab.

## Accuracy notes

This is a substantially closer reconstruction than the previous prototype, but it is not yet a bit-for-bit or pixel-perfect port of the original Unity game. The original gameplay code and configuration data are available as reference, and the recovered background/stage art is now used in-game. Unity scene/prefab wiring, the exact 16 tk2d animation-library mappings, exact prefab colliders, some special-skill effects, and original audio still need further reconstruction and side-by-side validation. Android-only billing, ads, Google Play, and social services are intentionally omitted.

This repository is intended to remain private. Original game assets and decompiled code remain subject to their rights holders' terms; do not publish or redistribute them without permission.
