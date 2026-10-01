using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// PORT: keys and gamepad buttons for the run actions, editable in the settings menu (PortControlsMenu) and
// kept in the save (PlayerData_v_1_1_3.portBindings). Menu navigation (arrows, Enter, Esc, sticks, d-pad)
// is fixed.
//
// Codes: keyboard = KeyCode; gamepad = button number as in PortInput (PlayStation 0 Square, 1 Cross,
// 2 Circle, 3 Triangle, 4 L1, 5 R1, 6 L2, 7 R2, 8 Share, 9 Options, 10 L3, 11 R3, 12 PS, 13 Touchpad;
// Xbox 0 A, 1 B, 2 X, 3 Y, 4 LB, 5 RB, 6 Back, 7 Start, 8 left stick click, 9 right stick click,
// c_LT/c_RT the triggers). Stick and d-pad directions: DirectionCode(source, direction), on both gamepads.
public enum PortAction
{
	Left,
	Right,
	Jump,
	Slide,
	Shoot,
	Pause,
	Score,
	Lights,
	Missions
}

public static class PortBindings
{
	public const int Keyboard = 0;

	public const int PlayStation = 1;

	public const int Xbox = 2;

	public const int DeviceCount = 3;

	public const int ActionCount = 9;

	public const int c_LT = 100;

	public const int c_RT = 101;

	// Direction sources and directions; code = 110 + source * 10 + direction
	public const int c_Dpad = 0;

	public const int c_LeftStick = 1;

	public const int c_RightStick = 2;

	public const int c_DirUp = 0;

	public const int c_DirDown = 1;

	public const int c_DirLeft = 2;

	public const int c_DirRight = 3;

	private const int c_DirBase = 110;

	// Saved bindings start with this; older saves had no stick/d-pad directions (they always moved)
	private const string c_Version = "2;";

	public static int DirectionCode(int source, int direction)
	{
		return c_DirBase + source * 10 + direction;
	}

	public static bool IsDirection(int code)
	{
		return code >= c_DirBase && code < c_DirBase + 30 && (code - c_DirBase) % 10 < 4;
	}

	public static int DirectionSource(int code)
	{
		return (code - c_DirBase) / 10;
	}

	public static int DirectionOf(int code)
	{
		return (code - c_DirBase) % 10;
	}

	public static readonly int[] DirectionCodes = BuildDirectionCodes();

	private static int[] BuildDirectionCodes()
	{
		int[] codes = new int[12];
		for (int i = 0; i < 12; i++)
		{
			codes[i] = DirectionCode(i / 4, i % 4);
		}
		return codes;
	}

	// The d-pad and both sticks in one direction, followed by the given buttons
	private static int[] Dir(int direction, params int[] buttons)
	{
		List<int> codes = new List<int> { DirectionCode(c_Dpad, direction), DirectionCode(c_LeftStick, direction), DirectionCode(c_RightStick, direction) };
		codes.AddRange(buttons);
		return codes.ToArray();
	}

	private static List<int>[][] s_Bindings;

	private static List<int>[] Defaults(int device)
	{
		switch (device)
		{
		case Keyboard:
			return Make(
				new[] { (int)KeyCode.LeftArrow },
				new[] { (int)KeyCode.RightArrow },
				new[] { (int)KeyCode.UpArrow },
				new[] { (int)KeyCode.DownArrow },
				new[] { (int)KeyCode.Space, (int)KeyCode.LeftControl, (int)KeyCode.RightControl },
				new[] { (int)KeyCode.Escape },
				new[] { (int)KeyCode.S },
				new[] { (int)KeyCode.L },
				new[] { (int)KeyCode.Q });
		case PlayStation:
			return Make(Dir(c_DirLeft, 4, 6), Dir(c_DirRight, 5, 7), Dir(c_DirUp, 1), Dir(c_DirDown, 2), new[] { 0, 3 }, new[] { 8, 9 }, new[] { 10 }, new[] { 11 }, new[] { 13 });
		default:
			return Make(Dir(c_DirLeft, 4, c_LT), Dir(c_DirRight, 5, c_RT), Dir(c_DirUp, 0), Dir(c_DirDown, 1), new[] { 2, 3 }, new[] { 7 }, new[] { 8 }, new[] { 9 }, new[] { 6 });
		}
	}

	private static List<int>[] Make(params int[][] codes)
	{
		List<int>[] lists = new List<int>[ActionCount];
		for (int i = 0; i < ActionCount; i++)
		{
			lists[i] = new List<int>(codes[i]);
		}
		return lists;
	}

	private static void EnsureLoaded()
	{
		if (s_Bindings != null)
		{
			return;
		}
		s_Bindings = new List<int>[DeviceCount][];
		for (int d = 0; d < DeviceCount; d++)
		{
			s_Bindings[d] = Defaults(d);
		}
		DataManager dm = DataManager.Instance;
		string saved = (dm != null && dm.playerData != null) ? dm.playerData.portBindings : null;
		if (string.IsNullOrEmpty(saved))
		{
			return;
		}
		// "2;device/device/device", device = "action|action|...", action = "code,code"
		bool old = !saved.StartsWith(c_Version);
		if (!old)
		{
			saved = saved.Substring(c_Version.Length);
		}
		try
		{
			string[] devices = saved.Split('/');
			for (int d = 0; d < DeviceCount && d < devices.Length; d++)
			{
				string[] actions = devices[d].Split('|');
				if (actions.Length != ActionCount)
				{
					continue;
				}
				for (int a = 0; a < ActionCount; a++)
				{
					List<int> list = new List<int>();
					foreach (string code in actions[a].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						list.Add(int.Parse(code));
					}
					s_Bindings[d][a] = list;
				}
			}
		}
		catch (Exception e)
		{
			Debug.Log("[PortBindings] Invalid saved bindings: " + e.Message);
		}
		if (old)
		{
			// The sticks and the d-pad used to move, jump and slide always: keep that
			for (int d = PlayStation; d <= Xbox; d++)
			{
				List<int>[] defaults = Defaults(d);
				for (int a = 0; a < ActionCount; a++)
				{
					int index = 0;
					foreach (int code in defaults[a])
					{
						if (IsDirection(code) && !s_Bindings[d][a].Contains(code))
						{
							s_Bindings[d][a].Insert(index++, code);
						}
					}
				}
			}
		}
	}

	private static void Save()
	{
		StringBuilder sb = new StringBuilder(c_Version);
		for (int d = 0; d < DeviceCount; d++)
		{
			if (d > 0)
			{
				sb.Append('/');
			}
			for (int a = 0; a < ActionCount; a++)
			{
				if (a > 0)
				{
					sb.Append('|');
				}
				sb.Append(string.Join(",", s_Bindings[d][a]));
			}
		}
		DataManager.Instance.SavePortBindings(sb.ToString());
	}

	public static List<int> Get(int device, PortAction action)
	{
		EnsureLoaded();
		return s_Bindings[device][(int)action];
	}

	// Adds the code to the action; a code belongs to one action only. Returns the action it was taken from, or -1.
	public static int Add(int device, PortAction action, int code)
	{
		EnsureLoaded();
		int from = -1;
		for (int a = 0; a < ActionCount; a++)
		{
			if (a != (int)action && s_Bindings[device][a].Remove(code))
			{
				from = a;
			}
		}
		if (!s_Bindings[device][(int)action].Contains(code))
		{
			s_Bindings[device][(int)action].Add(code);
		}
		Save();
		return from;
	}

	public static void Remove(int device, PortAction action, int code)
	{
		EnsureLoaded();
		s_Bindings[device][(int)action].Remove(code);
		Save();
	}

	public static void RestoreDefaults(int device)
	{
		EnsureLoaded();
		s_Bindings[device] = Defaults(device);
		Save();
	}

	// Forget the loaded bindings (save reset or reloaded)
	public static void Reload()
	{
		s_Bindings = null;
	}

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	public static string ActionName(PortAction action)
	{
		return L("port_action_" + action.ToString().ToLowerInvariant());
	}

	public static string DeviceName(int device)
	{
		return L(device == Keyboard ? "port_controls_keyboard" : (device == PlayStation ? "port_controls_playstation" : "port_controls_xbox"));
	}

	public static string CodeName(int device, int code)
	{
		if (device == Keyboard)
		{
			return KeyName((KeyCode)code);
		}
		if (IsDirection(code))
		{
			int source = DirectionSource(code);
			string name = L(source == c_Dpad ? "port_pad_dpad" : (source == c_LeftStick ? "port_pad_left_stick" : "port_pad_right_stick"));
			return name + " " + L("port_dir_" + new[] { "up", "down", "left", "right" }[DirectionOf(code)]);
		}
		if (device == PlayStation)
		{
			switch (code)
			{
			case 0: return L("port_pad_square");
			case 1: return L("port_pad_cross");
			case 2: return L("port_pad_circle");
			case 3: return L("port_pad_triangle");
			case 4: return "L1";
			case 5: return "R1";
			case 6: return "L2";
			case 7: return "R2";
			case 8: return "Share";
			case 9: return "Options";
			case 10: return "L3";
			case 11: return "R3";
			case 12: return "PS";
			case 13: return L("port_pad_touchpad");
			}
		}
		else
		{
			switch (code)
			{
			case 0: return "A";
			case 1: return "B";
			case 2: return "X";
			case 3: return "Y";
			case 4: return "LB";
			case 5: return "RB";
			case 6: return "Back";
			case 7: return "Start";
			case 8: return L("port_pad_left_stick_click");
			case 9: return L("port_pad_right_stick_click");
			case c_LT: return "LT";
			case c_RT: return "RT";
			}
		}
		return L("port_pad_button") + " " + code;
	}

	public static string KeyName(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.UpArrow: return L("port_key_up");
		case KeyCode.DownArrow: return L("port_key_down");
		case KeyCode.LeftArrow: return L("port_key_left");
		case KeyCode.RightArrow: return L("port_key_right");
		case KeyCode.Space: return L("port_key_space");
		case KeyCode.LeftControl: return L("port_key_left_ctrl");
		case KeyCode.RightControl: return L("port_key_right_ctrl");
		case KeyCode.LeftShift: return L("port_key_left_shift");
		case KeyCode.RightShift: return L("port_key_right_shift");
		case KeyCode.LeftAlt: return L("port_key_left_alt");
		case KeyCode.RightAlt: return "AltGr";
		case KeyCode.Return: return L("port_key_enter");
		case KeyCode.KeypadEnter: return L("port_key_num") + " " + L("port_key_enter");
		case KeyCode.Escape: return L("port_key_escape");
		case KeyCode.Tab: return "Tab";
		case KeyCode.Backspace: return L("port_key_backspace");
		}
		if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
		{
			return ((int)(key - KeyCode.Alpha0)).ToString();
		}
		if (key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
		{
			return L("port_key_num") + " " + (int)(key - KeyCode.Keypad0);
		}
		return key.ToString();
	}

	public static string Names(int device, PortAction action)
	{
		List<int> codes = Get(device, action);
		if (codes.Count == 0)
		{
			return L("port_controls_none");
		}
		StringBuilder sb = new StringBuilder();
		foreach (int code in codes)
		{
			if (sb.Length > 0)
			{
				sb.Append(", ");
			}
			sb.Append(CodeName(device, code));
		}
		return sb.ToString();
	}
}
