using UnityEngine;

// PORT: gamepads on macOS, through the Input System package, which is a layer over Apple's GameController
// framework. This is the Mac counterpart of PortSonyInput's Windows HID reader and it answers exactly the
// same questions, so PortPad can hand back either one and the rest of the port does not know the difference.
//
// Why the Input System rather than Unity's legacy Input: the legacy manager reads the pad by axis and button
// *index*, and the port's ProjectSettings/InputManager.asset picks those indices to suit DirectInput's layout
// on Windows. macOS orders its axes differently, so the same indices do not mean the same things there. The
// Input System asks the framework what each control *is* and looks it up by name, which is the part that is
// actually portable.
//
// The codes below are PortBindings' own, not the Input System's indices: buttons are matched by control name
// per layout, so a DualSense reports 0 = Square, 1 = Cross, ... as the PlayStation page of the controls menu
// already says, and an Xbox pad reports 0 = A, 1 = B, ... as the Xbox page says.
public static class PortGamePad
{
	// Control names in PortBindings order, for a pad presenting the Xbox layout.
	private static readonly string[] c_XboxButtons =
	{
		"buttonSouth",      // 0 A / Cross
		"buttonEast",       // 1 B / Circle
		"buttonWest",       // 2 X / Square
		"buttonNorth",      // 3 Y / Triangle
		"leftShoulder",     // 4 LB / L1
		"rightShoulder",    // 5 RB / R1
		"select",           // 6 Back / View
		"start",            // 7 Start / Menu
		"leftStickPress",   // 8 L3
		"rightStickPress"   // 9 R3
	};

	// The same codes for a pad presenting the PlayStation layout. L2 and R2 are separate controls from L1
	// and R1 here, which is why the triggers are read per layout too.
	private static readonly string[] c_SonyButtons =
	{
		"buttonWest",          // 0 Square
		"buttonSouth",         // 1 Cross
		"buttonEast",          // 2 Circle
		"buttonNorth",         // 3 Triangle
		"leftShoulder",        // 4 L1
		"rightShoulder",       // 5 R1
		"leftTriggerButton",   // 6 L2
		"rightTriggerButton",  // 7 R2
		"select",              // 8 Share / Create
		"start",               // 9 Options
		"leftStickPress",      // 10 L3
		"rightStickPress",     // 11 R3
		null,                  // 12 the PS button, which the framework does not expose as a control
		"touchpadButton"       // 13 Touchpad
	};

	// Buttons pressed this frame, as a bitmask over PortBindings codes, filled by Poll
	private static int s_Pressed;

	private static bool s_Active;

	private static bool s_Sony;

	private static bool s_LeftTrigger;

	private static bool s_RightTrigger;

	// A gamepad is connected.
	public static bool Active => s_Active;

	// The connected gamepad presents the PlayStation layout (Cross, Circle, Options...).
	public static bool Sony => s_Sony;

	public static Vector2 LeftStick { get; private set; }

	public static Vector2 RightStick { get; private set; }

	public static Vector2 Dpad { get; private set; }

	// PortBindings' pseudo-codes for the two triggers.
	public static bool LeftTriggerPressed => s_LeftTrigger;

	public static bool RightTriggerPressed => s_RightTrigger;

	// Called once per frame by PortInput
	public static void Poll()
	{
#if ENABLE_INPUT_SYSTEM
		s_Pressed = 0;
		s_LeftTrigger = false;
		s_RightTrigger = false;
		s_Active = false;
		s_Sony = false;
		string[] layout = c_XboxButtons;
		Vector2 left = Vector2.zero;
		Vector2 right = Vector2.zero;
		Vector2 dpad = Vector2.zero;

		var pads = UnityEngine.InputSystem.Gamepad.all;
		for (int i = 0; i < pads.Count; i++)
		{
			UnityEngine.InputSystem.Gamepad pad = pads[i];
			s_Active = true;
			bool sony = PresentsSonyLayout(pad);
			if (sony && !s_Sony)
			{
				s_Sony = true;
				layout = c_SonyButtons;
			}
			for (int b = 0; b < layout.Length; b++)
			{
				if (IsPressed(pad, layout[b]))
				{
					s_Pressed |= 1 << b;
				}
			}
			s_LeftTrigger |= IsPressed(pad, "leftTriggerButton") || IsPressed(pad, "leftTrigger");
			s_RightTrigger |= IsPressed(pad, "rightTriggerButton") || IsPressed(pad, "rightTrigger");
			left = pad.leftStick.ReadValue();
			right = pad.rightStick.ReadValue();
			dpad = pad.dpad.ReadValue();
		}
		LeftStick = left;
		RightStick = right;
		Dpad = dpad;
#else
		// No Input System package: nothing to read, and the legacy Input path stays in charge.
		s_Pressed = 0;
		s_Active = false;
		s_Sony = false;
		s_LeftTrigger = false;
		s_RightTrigger = false;
		LeftStick = Vector2.zero;
		RightStick = Vector2.zero;
		Dpad = Vector2.zero;
#endif
	}

#if ENABLE_INPUT_SYSTEM
	// A control by name, or null. TryGetChildControl is the one that returns null for a control the layout
	// does not have - GetChildControl and the indexer both throw - and a cast rather than the generic form so
	// that a name resolving to some other kind of control is a "no" and not an exception in Update.
	private static bool IsPressed(UnityEngine.InputSystem.Gamepad pad, string controlName)
	{
		if (controlName == null)
		{
			return false;
		}
		var button = pad.TryGetChildControl(controlName) as UnityEngine.InputSystem.Controls.ButtonControl;
		return button != null && button.isPressed;
	}

	// Does this pad present the PlayStation layout? Asked of the pad rather than tested against
	// DualSenseGamepad, because the concrete HID classes have moved around between package versions, and the
	// touchpad is the one control a DualShock or DualSense has that an Xbox pad never has. The name is the
	// second opinion, for a pad that has lost its touchpad to a remapping tool.
	private static bool PresentsSonyLayout(UnityEngine.InputSystem.Gamepad pad)
	{
		if (pad.TryGetChildControl("touchpadButton") != null)
		{
			return true;
		}
		string name = ((pad.displayName ?? "") + " " + (pad.name ?? "")).ToLowerInvariant();
		return name.Contains("dualsense") || name.Contains("dualshock") || name.Contains("sony") || name.Contains("playstation");
	}
#endif

	public static bool WasPressed(int button)
	{
		return s_Active && button >= 0 && button < 32 && (s_Pressed & (1 << button)) != 0;
	}

	// The Input System owns the pads and closes them itself; there is nothing to release.
	public static void Shutdown()
	{
		s_Pressed = 0;
		s_Active = false;
		LeftStick = Vector2.zero;
		RightStick = Vector2.zero;
		Dpad = Vector2.zero;
	}
}