# FEER for Windows (unofficial preservation port)

An unofficial Windows port of **FEER – The Game of Running Blind**, the accessible audio endless runner by [MENTAL HOME](https://www.mentalhome.eu/), made so that the game can still be played and remembered.

## Ownership and purpose

**FEER, its name, design, audio, voice recordings, artwork, music and all other game content belong to MENTAL HOME (Vienna, Austria).** This repository is not affiliated with, endorsed by or supported by MENTAL HOME. No rights to the game are claimed here.

FEER won Best Game 2018 from AppleVis and the Futurezone Award. It was one of the first endless runners designed for blind and visually impaired players and sighted players alike. MENTAL HOME still presents the game on its [website](https://www.mentalhome.eu/feer/). The Android version linked from that page is no longer on Google Play (the store link returns "not found"). Without that download, the game would slowly disappear for the Android players who loved it.

This project exists to **keep the memory of the game alive**, to preserve it and keep it playable on a PC. Please support the original creators.

## What this is

The project was rebuilt from the Android release (`eu.mentalhome.feer` 1.1.10, Unity 2020.3.15f2, IL2CPP) into a Unity project that builds for Windows:

- The scenes and assets were recovered with AssetRipper.
- The game code was rewritten in C# from the decompiled IL2CPP binary. It follows the original logic as closely as possible.
- Every change made for the port is marked with a `PORT:` comment in the code.
- Port-only code lives in `Assets/Scripts/Port` and `Assets/Editor/Port`.

### Removed

These features depended on services that no longer exist or have no meaning offline:

- Online leaderboards and friends
- In-app purchases (the Factory theme is unlocked)
- Analytics
- Sharing
- Google TTS

### Added for PC

- **Screen readers:**
  - Speech goes through Tolk, so it works with NVDA, JAWS and other screen readers.
  - SAPI is used when no screen reader is running.
- **Keyboard, gamepad and mouse** input.
- **Gamepad rumble** when the player dies or stumbles.
  - Xbox/XInput pads use XInput.
  - DualShock 4 and DualSense use HID, over USB or Bluetooth.
- **Esc goes back** to the previous menu. In the pause menu it resumes the game.
- **Run status keys:**
  - S or L3 says the current score.
  - L or R3 says the lights collected in this run and in total.
- **Power-up countdown:**
  - Uses the clock sounds from the start-of-game countdown when the active power-up is about to end.
  - Can be turned off in the settings.
- **Key and button instructions** instead of touch instructions ("tap", "swipe") in all five languages. They name the gamepad buttons when a gamepad is connected.
- **Bug fix:** an obstacle could get stuck behind the player after pausing the tutorial twice.

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
| Menus | Arrows, Enter, Esc | Stick or d-pad, Cross, Circle | Stick or d-pad, A, B |

Mouse: drag to swipe, click to tap.

## Building

1. Install Unity **2020.3.15f2**.
2. Open this folder as a project.
3. Build with the editor method `PortBuild.BuildWindows`, which writes `../Build/Feer.exe`:

```
Unity.exe -quit -projectPath <this folder> -executeMethod PortBuild.BuildWindows -logFile build.log
```

Saves are kept in `%USERPROFILE%\AppData\LocalLow\Hansjoerg Mikesch\Feer`.

## Credits

- **FEER:** © MENTAL HOME. All rights to the original game belong to them.
- **Windows port:** made for personal use and preservation.
- **Tolk:** screen reader abstraction library by Davy Kager (LGPL).
