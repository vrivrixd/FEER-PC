using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

// PORT: le DualShock 4 / DualSense direto pelo HID (USB e Bluetooth).
//
// Pelo Bluetooth o controle passa a mandar o relatorio estendido (0x11 no DS4, 0x31 no DualSense)
// assim que algum programa pede (Steam, a propria vibracao...). O Windows nao entrega esse relatorio
// como joystick, entao o Unity para de ver os botoes. Lendo o HID aqui, os dois modos funcionam igual.
//
// Os botoes usam a mesma numeracao do Unity/DirectInput: 0 Quadrado, 1 Cruz, 2 Circulo, 3 Triangulo,
// 4 L1, 5 R1, 6 L2, 7 R2, 8 Share/Create, 9 Options, 10 L3, 11 R3, 12 PS, 13 Touchpad.
public static class PortSonyInput
{
	private class Reader
	{
		public string Path;

		public bool DualSense;

		public IntPtr Handle;

		public volatile bool Alive = true;
	}

	private static readonly List<Reader> s_Readers = new List<Reader>();

	private static readonly object s_Lock = new object();

	private static int s_Buttons;

	private static int s_PressedAccum;

	private static int s_Hat = 8;

	private static byte s_LX = 128, s_LY = 128, s_RX = 128, s_RY = 128;

	private static bool s_HasData;

	private static float s_NextScan;

	// Estado do frame atual (preenchido por Poll)
	public static bool Active { get; private set; }

	public static int PressedThisFrame { get; private set; }

	public static Vector2 LeftStick { get; private set; }

	public static Vector2 RightStick { get; private set; }

	public static Vector2 Dpad { get; private set; }

	// Chamado uma vez por frame pelo PortInput
	public static void Poll()
	{
		lock (s_Lock)
		{
			s_Readers.RemoveAll(r => !r.Alive);
			if (s_Readers.Count == 0)
			{
				s_HasData = false;
			}
		}
		if (s_Readers.Count == 0 && Time.unscaledTime >= s_NextScan)
		{
			s_NextScan = Time.unscaledTime + 3f;
			Scan();
		}
		lock (s_Lock)
		{
			Active = s_HasData;
			PressedThisFrame = s_PressedAccum;
			s_PressedAccum = 0;
			LeftStick = new Vector2(Norm(s_LX), -Norm(s_LY));
			RightStick = new Vector2(Norm(s_RX), -Norm(s_RY));
			int h = s_Hat;
			float x = (h == 1 || h == 2 || h == 3) ? 1f : ((h == 5 || h == 6 || h == 7) ? -1f : 0f);
			float y = (h == 7 || h == 0 || h == 1) ? 1f : ((h == 3 || h == 4 || h == 5) ? -1f : 0f);
			Dpad = new Vector2(x, y);
		}
	}

	public static bool WasPressed(int button)
	{
		return Active && (PressedThisFrame & (1 << button)) != 0;
	}

	private static float Norm(byte v)
	{
		return Mathf.Clamp((v - 128) / 127f, -1f, 1f);
	}

	private static void Scan()
	{
		try
		{
			foreach (KeyValuePair<string, bool> pad in PortRumble.FindSonyPads())
			{
				IntPtr handle = CreateFileRaw(pad.Key, PortRumble.GENERIC_READ | PortRumble.GENERIC_WRITE, PortRumble.FILE_SHARE_READ | PortRumble.FILE_SHARE_WRITE, IntPtr.Zero, PortRumble.OPEN_EXISTING, 0, IntPtr.Zero);
				if (handle == IntPtr.Zero || handle == new IntPtr(-1))
				{
					continue;
				}
				Reader reader = new Reader { Path = pad.Key, DualSense = pad.Value, Handle = handle };
				lock (s_Lock)
				{
					s_Readers.Add(reader);
				}
				Thread thread = new Thread(() => ReadLoop(reader));
				thread.IsBackground = true;
				thread.Name = "PortSonyInput";
				thread.Start();
				Debug.Log("[PortSonyInput] Lendo " + (pad.Value ? "DualSense" : "DualShock 4") + " pelo HID");
			}
		}
		catch (Exception e)
		{
			Debug.Log("[PortSonyInput] " + e.Message);
		}
	}

	private static void ReadLoop(Reader reader)
	{
		byte[] buffer = new byte[1024];
		try
		{
			while (reader.Alive)
			{
				uint read;
				if (!ReadFile(reader.Handle, buffer, (uint)buffer.Length, out read, IntPtr.Zero) || read == 0)
				{
					break;
				}
				Parse(buffer, (int)read, reader.DualSense);
			}
		}
		catch (Exception)
		{
		}
		reader.Alive = false;
		CloseHandle(reader.Handle);
	}

	private static void Parse(byte[] r, int len, bool dualSense)
	{
		int o;
		bool dsLayout;
		if (r[0] == 0x11 && !dualSense && len >= 12)
		{
			o = 3; // DS4 Bluetooth estendido
			dsLayout = false;
		}
		else if (r[0] == 0x31 && dualSense && len >= 13)
		{
			o = 2; // DualSense Bluetooth estendido
			dsLayout = true;
		}
		else if (r[0] == 0x01 && len >= 10)
		{
			o = 1;
			// DualSense pela USB manda o relatorio completo; pelo Bluetooth simples tem o formato do DS4
			dsLayout = dualSense && len >= 64;
		}
		else
		{
			return;
		}
		int b0 = dsLayout ? r[o + 7] : r[o + 4];
		int b1 = dsLayout ? r[o + 8] : r[o + 5];
		int b2 = dsLayout ? r[o + 9] : r[o + 6];
		int buttons = ((b0 >> 4) & 0x0F) | (b1 << 4) | ((b2 & 0x03) << 12);
		lock (s_Lock)
		{
			s_PressedAccum |= buttons & ~s_Buttons;
			s_Buttons = buttons;
			s_Hat = b0 & 0x0F;
			s_LX = r[o];
			s_LY = r[o + 1];
			s_RX = r[o + 2];
			s_RY = r[o + 3];
			s_HasData = true;
		}
	}

	public static void Shutdown()
	{
		lock (s_Lock)
		{
			foreach (Reader reader in s_Readers)
			{
				reader.Alive = false;
				CancelIoEx(reader.Handle, IntPtr.Zero);
			}
			s_Readers.Clear();
		}
	}

	[DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateFileRaw(string name, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool ReadFile(IntPtr file, byte[] buffer, uint count, out uint read, IntPtr overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CancelIoEx(IntPtr file, IntPtr overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr handle);
}
