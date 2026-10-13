# Open and build the recovered Unity project on Windows

1. Download or clone this private repository.
2. In Unity Hub, install **Unity 4.5.5f1** from the archive if it is still offered for your account/platform. The APK's version marker and exported `ProjectVersion.txt` both identify this exact version.
3. In Unity Hub, choose **Add project from disk** and select the `UnityProject` folder (the folder containing `Assets`, `ProjectSettings`, and `Packages`).
4. Let Unity import the project. Record the first compiler errors before changing anything.
5. If it compiles, open `Assets/Scenes/SplashScene.unity`, then use Build Settings to confirm `SplashScene` and `PlayScene` are enabled.
6. Select **PC, Mac & Linux Standalone → Windows**, build to a new output folder, and test the resulting player.
7. Compare against the original Android game. Any compile errors or missing plugin behavior should be repaired with the original scene wiring and gameplay scripts preserved.

If Unity 4.5.5f1 cannot be installed on the current Windows version, duplicate the project and test opening that copy in a later Unity version. Do not upgrade the only recovered copy in place: newer Unity versions may rewrite legacy scene, physics, and plugin data.

The GitHub Actions workflow currently validates the recovered Unity project structure only. It intentionally no longer builds the Python/Pygame prototype and does not claim a Windows executable exists from the Unity project yet.
