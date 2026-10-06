# FEER for Windows and macOS (unofficial preservation port)

An unofficial Windows and macOS port of **FEER – The Game of Running Blind**, the accessible audio endless runner by [MENTAL HOME](https://www.mentalhome.eu/), made so that the game can still be played and remembered.

## Ownership and purpose

**FEER, its name, design, audio, voice recordings, artwork, music and all other game content belong to MENTAL HOME (Vienna, Austria).** This repository is not affiliated with, endorsed by or supported by MENTAL HOME. No rights to the game are claimed here.

FEER won Best Game 2018 from AppleVis and the Futurezone Award. It was one of the first endless runners designed for blind and visually impaired players and sighted players alike. MENTAL HOME still presents the game on their [website](https://www.mentalhome.eu/feer/). The Android version linked from that page is no longer on Google Play (the store link returns "not found"). Without that download, the game would slowly disappear for the Android players who loved it.

This project exists to **keep the memory of the game alive**, to preserve it and keep it playable on a desktop. Please support the original creators.

## What this is

The project was rebuilt from the Android release (`eu.mentalhome.feer` 1.1.10, Unity 2020.3.15f2, IL2CPP) into a Unity project that builds for Windows and macOS:

- The scenes and assets were recovered with AssetRipper.
- The game code was rewritten in C# from the decompiled IL2CPP binary. It follows the original logic as closely as possible.
- Every change made for the port is marked with a `PORT:` comment in the code.
- Port-only code lives in `Assets/Scripts/Port` and `Assets/Editor/Port`.

The Windows build is the one that came first. The macOS build was added on the `mac` branch on top of it and changes nothing about how the Windows build behaves.

### Removed

These features depended on services that no longer exist or have no meaning offline:

- Online leaderboards and friends
- In-app purchases (the Factory theme is unlocked)
- Analytics
- Sharing
- Google TTS

### Added for desktop

- **Screen readers:**
  - Windows: speech goes through Tolk, so it works with NVDA, JAWS and other screen readers. SAPI is used when no screen reader is running.
  - macOS: speech goes through **VoiceOver**, via `/usr/bin/osascript`. When VoiceOver is not running — or has refused the Apple Event — the system voice (`say`) is used instead, which is what the plugin already did.
- **Keyboard, gamepad and mouse** input.
- **Gamepad rumble** when the player dies or stumbles.
  - Windows: Xbox/XInput pads use XInput; DualShock 4 and DualSense use HID, over USB or Bluetooth.
  - macOS: every pad the GameController framework knows, through the Input System package. There is no vendor table to keep up to date.
- **Esc goes back** to the previous menu. In the pause menu it resumes the game.
- **Run status keys:**
  - S or L3 says the current score.
  - L or R3 says the lights collected in this run and in total.
  - Q says the current missions.
- **Power-up countdown:**
  - Uses the clock sounds from the start-of-game countdown when the active power-up is about to end.
  - Can be turned off in the settings.
- **Key and button instructions** instead of touch instructions ("tap", "swipe") in all five languages. They name the gamepad buttons when a gamepad is connected.
- **Bug fix:** an obstacle could get stuck behind the player after pausing the tutorial twice.
- **Auto-updater** from the GitHub releases, one zip per platform (`FEER-PC-win64.zip`, `FEER-PC-osx.zip`).

The recorded tutorial voice is original audio and still talks about swiping.

## Controls

| Action | Keyboard | PlayStation | Xbox |
|---|---|---|---|
| Change lane | Left / Right arrows | Stick or d-pad, L1/L2, R1/R2 | Stick or d-pad, LB/LT, RB/RT |
| Jump | Up arrow | Cross or up | A or up |
| Slide | Down arrow | Circle or down | B or down |
| Shoot (weapon) | Space | Square, Triangle, touchpad | X, Y |
| Pause / skip tutorial | Esc | Options, Share | Start, Back |
| Say score | S | L3 | Left stick click |
| Say lights | L | R3 | Right stick click |
| Say missions | Q | Touchpad | Back |
| Menus | Arrows, Enter, Esc | Stick or d-pad, Cross, Circle | Stick or d-pad, A, B |

Mouse: drag to swipe, click to tap.

Every key and button is rebindable in Options → Controls.

**Quit:** nothing in the game quits it. That is Alt+F4 on Windows and Cmd+Q on the Mac, or Quit in a menu.

## Building

1. Install Unity **2022.3.62f3** (the Apple Silicon editor, not the Intel one).
2. Open this folder as a project.
3. Build with one of the two editor methods.

### From a Mac, the easy way

Four `.command` files at the top of the repository, double-clickable in Finder:

| Double-click | Does |
|---|---|
| `Build Mac.command` | Builds `../Build/Feer.app`, then checks every binary in it with `lipo` |
| `Build Windows.command` | Builds `../Build/Feer.exe` (needs the Windows Build Support module) |
| `Make Mac Release.command` | Builds and packages `../Build/FEER-PC-osx.zip` for the in-game updater |
| `Open in Unity.command` | Opens the project in the editor the project asks for |

The same things from a terminal, which is what the `.command` files call:

```
./tools/build_macos.sh            # build and verify the Mac player
./tools/build_windows.sh          # build the Windows player
./tools/make_release_macos.sh      # build and zip, as the updater expects
./tools/make_release_macos.sh 1.3  # ...and set the bundled version to 1.3
```

None of them need you to know where Unity is installed. `tools/unity.sh` finds an editor, checks it against
`ProjectSettings/ProjectVersion.txt`, and says so loudly if it finds the wrong one — which matters here,
because 2020.3 and 2022.3 produce different players. Set `UNITY_PATH` to choose a particular editor.

The Mac build is verified after every build rather than trusted: the scripts grep the log for `Succeeded`
and then run `lipo -archs` over the bundle, because a thinning step that silently matched no files would
otherwise report success over a universal binary.

### By hand

Both are `-executeMethod` targets, so the game can be built without opening the editor at all:

**Windows** — `PortBuild.BuildWindows`, which writes `../Build/Feer.exe`:

```
Unity.exe -quit -projectPath <this folder> -executeMethod PortBuild.BuildWindows -logFile build.log
```

**macOS** — `PortBuild.BuildMacOS`, which writes `../Build/Feer.app`:

```
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -buildTarget StandaloneOSX \
    -projectPath <this folder> -executeMethod PortBuild.BuildMacOS -logFile build.log
```

`-buildTarget StandaloneOSX` is not optional: it switches the active target *before* the method runs, which
the architecture setting depends on.

### Why Unity 2022.3 and not 2020.3

The project was recovered with AssetRipper against Unity 2020.3.15f2, and that is the version it was first
ported and built on. It cannot produce a Mac game worth shipping, though, on two counts:

- **2020.3 has no arm64 macOS player.** Native Apple Silicon standalone players arrived in Unity 2021.2. Every
  Mac build from 2020.3 is an x86_64 binary that only runs through Rosetta 2, and Rosetta is being withdrawn —
  a machine on macOS 28 will not run it at all.
- **2020.3 has no arm64 editor on macOS.** Unity's release API lists only `MAC_OS X86_64` for that version.
  There is nothing to install that is not itself a Rosetta binary.

So the editor moved to **2022.3 LTS**, the oldest stream with both an arm64 editor and an arm64 Mac player and
still in support. Nothing in the recovered game had to change: the built-in render pipeline, uGUI, the legacy
Input Manager and the `UnityEngine` API surface the game uses are all unchanged across 2021 and 2022, and no
API that Unity removed in that span is called anywhere in the project.

The one visible consequence is that **the Windows build needs 2022.3 as well.** Both platforms are built from
this one project folder, so there is a single minimum editor version, and it is recorded in
`ProjectSettings/ProjectVersion.txt`.

### Known interaction to watch on Windows

The Input System package is present on *both* platforms, because package dependencies are per project and
cannot be per build target. `activeInputHandler` is set to **Both**, which is what keeps the legacy Input
Manager — the one the Windows gamepad code reads, by DirectInput axis index — working exactly as before.

The thing to check on a Windows machine with a gamepad attached is that the pad still behaves through the
legacy manager while the Input System is also enabled and enumerating the same device. The Mac path does not
touch the legacy joystick input at all (`PortGamePad` takes over completely), but Unity's two backends both
being live on Windows is the one part of this change the code cannot fully control. If Windows gamepad input
misbehaves, the fix is to drop `com.unity.inputsystem` from `Packages/manifest.json`, losing Mac rumble and Mac
pad input and keeping the keyboard and mouse paths intact.

### Notes on the macOS build

- **It is arm64 and native, with no Rosetta anywhere.** This needed a workaround worth knowing about.
  Unity 2022.3 cannot build a single-architecture macOS player: its own manual says the Architecture setting
  "only applies to Build And Run", and that is literal — `platformArchitecture` 0, 1 and 2 and
  `EditorUserBuildSettings`'s `Architecture` string all produce a byte-identical **universal** binary, because
  the setting only chooses which slice Build And Run launches. Unity 6 added a real single-architecture
  choice; getting there means 2022.3 → 6000.x on a project AssetRipper recovered from a 2020.3 Android build,
  which is a much larger risk than it is worth here.

  So `PortBuild.BuildMacOS` thins the finished bundle to arm64 with `lipo` and then **verifies it**, reading
  the architecture back off every binary in the built `.app` with `lipo -archs` and logging the result. A
  universal result is treated as a build failure, not a variation. If you ever build on a Unity that honours
  the setting, the thinning step finds every binary already arm64 and does nothing.
- **It is not signed or notarised.** macOS will refuse to launch a quarantined, unsigned bundle that was moved by
  another program. The in-game updater clears the quarantine flag itself (`xattr -dr com.apple.quarantine`)
  before relaunching; a build moved to another Mac by hand needs the same, or a right click → Open.
- **VoiceOver needs one permission.** The first time the game speaks, macOS asks whether `Feer` may control
  `VoiceOver` — System Preferences → Privacy & Security → Automation. Until that is granted the game logs the
  -1743 refusal once and speaks through the system voice instead. Everything else works either way.

### Saves

Kept in `Application.persistentDataPath`, which Unity resolves per platform:

| Platform | Path |
|---|---|
| Windows | `%USERPROFILE%\AppData\LocalLow\Hansjoerg Mikesch\Feer` |
| macOS | `~/Library/Application Support/Hansjoerg Mikesch/Feer` |

Saves do not move between platforms.

## Credits

- **FEER:** © MENTAL HOME. All rights to the original game belong to them.
- **Windows port:** made for personal use and preservation.
- **macOS port:** the same engine and the same game, with VoiceOver for speech.
- **Tolk:** screen reader abstraction library by Davy Kager (LGPL).
- **Unity Input System** (1.7.0, MIT): used on macOS only, for gamepad input and rumble. `activeInputHandler`
  is set to *Both*, so the legacy Input Manager that the Windows build uses is untouched.