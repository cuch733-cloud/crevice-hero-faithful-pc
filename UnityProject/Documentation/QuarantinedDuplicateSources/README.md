# Duplicate decompilation copies quarantined

These 16 root-level C# files duplicated classes already present in `Assets/Scripts/Assembly-CSharp/` but had no `.meta` files / GUIDs. Unity compiles ordinary C# files under `Assets` into the same default assembly unless assembly definitions split them, so leaving both copies would cause duplicate-type compilation errors. The `Assembly-CSharp` copies were retained because their `.meta` GUIDs are the ones referenced by the recovered serialized scenes/prefabs. The extra copies are preserved here for audit, not imported into Unity's `Assets` tree.
