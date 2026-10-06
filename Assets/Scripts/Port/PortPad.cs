using UnityEngine;

// PORT: the one gamepad the rest of the port talks to.
//
// Windows: PortSonyInput, which reads DualShock 4 and DualSense over HID directly, because over Bluetooth
// the extended report stops arriving as a joystick and Windows drops the buttons.
// Everywhere else: PortGamePad, which goes through the Input System and Apple's GameController framework.
//
// The decision is made at compile time, not at runtime, because only one of the two exists in a given build:
// PortSonyInput's kernel32 HID code is not compiled into a Mac build at all. Everything else in the port asks
// this class and never has to know which platform it is on.
public static class PortPad
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	public static bool Active => PortSonyInput.Active;

	public static bool Sony => PortSonyInput.Active;

	public static Vector2 LeftStick => PortSonyInput.LeftStick;

	public static Vector2 RightStick => PortSonyInput.RightStick;

	public static Vector2 Dpad => PortSonyInput.Dpad;

	// The HID reports carry L2 and R2 as ordinary buttons, so there is no trigger axis to read; the
	// Xbox-axis path in PortInput stays in charge of the triggers on Windows, as it always has.
	public static bool LeftTriggerPressed => false;

	public static bool RightTriggerPressed => false;

	public static bool WasPressed(int button) => PortSonyInput.WasPressed(button);

	public static void Poll() => PortSonyInput.Poll();

	public static void Shutdown() => PortSonyInput.Shutdown();
#else
	public static bool Active => PortGamePad.Active;

	public static bool Sony => PortGamePad.Sony;

	public static Vector2 LeftStick => PortGamePad.LeftStick;

	public static Vector2 RightStick => PortGamePad.RightStick;

	public static Vector2 Dpad => PortGamePad.Dpad;

	public static bool LeftTriggerPressed => PortGamePad.LeftTriggerPressed;

	public static bool RightTriggerPressed => PortGamePad.RightTriggerPressed;

	public static bool WasPressed(int button) => PortGamePad.WasPressed(button);

	public static void Poll() => PortGamePad.Poll();

	public static void Shutdown() => PortGamePad.Shutdown();
#endif
}