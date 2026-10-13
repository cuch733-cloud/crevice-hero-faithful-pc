# Original Unity project recovery status

- [x] Inspected the original APK's embedded Unity version marker: `4.5.5f1`.
- [x] Located the recovered `Assembly-CSharp.dll` and decompiled gameplay C# sources.
- [x] Added original gameplay source and decoded configuration data to a Unity-oriented project tree.
- [ ] Export and restore original serialized scenes and prefab hierarchy.
- [ ] Restore tk2d sprite collections, animation clips, fonts, and UI layout.
- [ ] Restore original audio and required plugin assemblies or replace platform-only service adapters.
- [ ] Open and validate the project with the matching Unity editor.
- [ ] Build a Windows Standalone player and compare screenshots/behaviour against the Android original.

No claim of exact matching should be made until the remaining items are completed and tested.
