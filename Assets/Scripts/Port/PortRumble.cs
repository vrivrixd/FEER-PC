using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

// PORT: vibracao do controle no PC (o original usava Handheld.Vibrate no celular).
//
// - Controles XInput (Xbox, ou PlayStation via DS4Windows/Steam): XInputSetState.
// - DualShock 4 e DualSense ligados direto (USB ou Bluetooth): relatorio de saida HID.
//
// Tudo e best effort: se o controle nao existir ou estiver em uso exclusivo, nada acontece.
public class PortRumble : MonoBehaviour
{
	private static PortRumble s_Instance;

	private float m_StopTime = -1f;

	private readonly List<SonyPad> m_SonyPads = new List<SonyPad>();

	private float m_NextSonyScan;

	private byte m_BtSequence;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Create()
	{
		GameObject go = new GameObject("PortRumble");
		DontDestroyOnLoad(go);
		s_Instance = go.AddComponent<PortRumble>();
	}

	// Teste: Feer.exe -porttestrumble vibra 3 segundos depois de abrir
	private void Start()
	{
		if (Array.IndexOf(Environment.GetCommandLineArgs(), "-porttestrumble") >= 0)
		{
			Invoke("TestRumble", 3f);
		}
	}

	private void TestRumble()
	{
		Debug.Log("[PortRumble] teste");
		Death();
	}

	// Vibra os controles conectados. strong = motor grande, weak = motor pequeno (0..1).
	public static void Rumble(float strong, float weak, float seconds)
	{
		if (s_Instance != null)
		{
			s_Instance.Play(strong, weak, seconds);
		}
	}

	// Vibracao quando o personagem morre
	public static void Death()
	{
		Rumble(1f, 1f, 0.6f);
	}

	// Vibracao quando o personagem tropeca (substitui Handheld.Vibrate do original)
	public static void Stumble()
	{
		Rumble(0.6f, 0.4f, 0.25f);
	}

	private void Play(float strong, float weak, float seconds)
	{
		SetMotors(Mathf.Clamp01(strong), Mathf.Clamp01(weak));
		m_StopTime = Time.unscaledTime + seconds;
	}

	private void Update()
	{
		if (m_StopTime >= 0f && Time.unscaledTime >= m_StopTime)
		{
			m_StopTime = -1f;
			SetMotors(0f, 0f);
		}
	}

	private void OnApplicationQuit()
	{
		SetMotors(0f, 0f);
		foreach (SonyPad pad in m_SonyPads)
		{
			pad.Dispose();
		}
		m_SonyPads.Clear();
	}

	private void SetMotors(float strong, float weak)
	{
		try
		{
			SetXInput(strong, weak);
		}
		catch (Exception e)
		{
			Debug.Log("[PortRumble] XInput: " + e.Message);
		}
		try
		{
			SetSony(strong, weak);
		}
		catch (Exception e)
		{
			Debug.Log("[PortRumble] HID: " + e.Message);
		}
	}

	// ------------------------------------------------------------------ XInput

	[StructLayout(LayoutKind.Sequential)]
	private struct XInputVibration
	{
		public ushort LeftMotorSpeed;

		public ushort RightMotorSpeed;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct XInputState
	{
		public uint PacketNumber;

		public ushort Buttons;

		public byte LeftTrigger;

		public byte RightTrigger;

		public short ThumbLX;

		public short ThumbLY;

		public short ThumbRX;

		public short ThumbRY;
	}

	[DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
	private static extern uint XInputGetState(uint index, out XInputState state);

	[DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
	private static extern uint XInputSetState(uint index, ref XInputVibration vibration);

	private static void SetXInput(float strong, float weak)
	{
		XInputVibration v = new XInputVibration
		{
			LeftMotorSpeed = (ushort)(strong * 65535f),
			RightMotorSpeed = (ushort)(weak * 65535f)
		};
		for (uint i = 0; i < 4; i++)
		{
			XInputState state;
			if (XInputGetState(i, out state) == 0)
			{
				XInputSetState(i, ref v);
			}
		}
	}

	// ------------------------------------------------------------------ DualShock 4 / DualSense (HID)

	private enum SonyKind
	{
		DS4Usb,
		DS4Bluetooth,
		DualSenseUsb,
		DualSenseBluetooth
	}

	private class SonyPad : IDisposable
	{
		public SafeFileHandle Handle;

		public SonyKind Kind;

		public int ReportLength;

		public void Dispose()
		{
			try
			{
				Handle?.Dispose();
			}
			catch
			{
			}
		}
	}

	private const ushort c_SonyVendor = 0x054C;

	private static readonly ushort[] c_DS4Products = { 0x05C4, 0x09CC, 0x0BA0 };

	private static readonly ushort[] c_DualSenseProducts = { 0x0CE6, 0x0DF2 };

	private void SetSony(float strong, float weak)
	{
		if (Time.unscaledTime >= m_NextSonyScan)
		{
			m_NextSonyScan = Time.unscaledTime + 5f;
			ScanSonyPads();
		}
		byte big = (byte)(strong * 255f);
		byte small = (byte)(weak * 255f);
		for (int i = m_SonyPads.Count - 1; i >= 0; i--)
		{
			SonyPad pad = m_SonyPads[i];
			try
			{
				byte[] report = BuildReport(pad, big, small);
				// PORT: FileStream do Mono recusa handles de HID ("Invalid handle"); grava direto.
				uint written;
				if (!WriteFile(pad.Handle, report, (uint)report.Length, out written, IntPtr.Zero))
				{
					throw new IOException("WriteFile falhou: " + Marshal.GetLastWin32Error());
				}
			}
			catch (Exception e)
			{
				Debug.Log("[PortRumble] " + pad.Kind + ": " + e.Message);
				// Desconectado: tenta achar de novo na proxima vibracao
				pad.Dispose();
				m_SonyPads.RemoveAt(i);
				m_NextSonyScan = 0f;
			}
		}
	}

	private byte[] BuildReport(SonyPad pad, byte big, byte small)
	{
		byte[] r = new byte[pad.ReportLength];
		switch (pad.Kind)
		{
		case SonyKind.DS4Usb:
			r[0] = 0x05;
			r[1] = 0x01; // so o motor (nao mexe na luz)
			r[4] = small;
			r[5] = big;
			break;
		case SonyKind.DS4Bluetooth:
			EnsureLength(ref r, 78);
			r[0] = 0x11;
			r[1] = 0xC0; // HID + CRC
			r[3] = 0x01;
			r[6] = small;
			r[7] = big;
			WriteCrc(r, 74);
			break;
		case SonyKind.DualSenseUsb:
			r[0] = 0x02;
			r[1] = 0x03; // vibracao compativel + selecao de haptico
			r[3] = small;
			r[4] = big;
			if (r.Length > 39)
			{
				r[39] = 0x04; // vibracao compativel v2 (firmwares novos)
			}
			break;
		case SonyKind.DualSenseBluetooth:
			EnsureLength(ref r, 78);
			r[0] = 0x31;
			r[1] = (byte)(m_BtSequence << 4);
			m_BtSequence = (byte)((m_BtSequence + 1) & 0x0F);
			r[2] = 0x10;
			r[3] = 0x03;
			r[5] = small;
			r[6] = big;
			r[41] = 0x04;
			WriteCrc(r, 74);
			break;
		}
		return r;
	}

	private static void EnsureLength(ref byte[] r, int length)
	{
		if (r.Length < length)
		{
			r = new byte[length];
		}
	}

	// CRC32 dos relatorios Bluetooth: semente 0xA2 seguida dos bytes do relatorio
	private static void WriteCrc(byte[] r, int crcOffset)
	{
		uint crc = 0xFFFFFFFFu;
		crc = CrcByte(crc, 0xA2);
		for (int i = 0; i < crcOffset; i++)
		{
			crc = CrcByte(crc, r[i]);
		}
		crc = ~crc;
		r[crcOffset] = (byte)crc;
		r[crcOffset + 1] = (byte)(crc >> 8);
		r[crcOffset + 2] = (byte)(crc >> 16);
		r[crcOffset + 3] = (byte)(crc >> 24);
	}

	private static uint CrcByte(uint crc, byte b)
	{
		crc ^= b;
		for (int k = 0; k < 8; k++)
		{
			crc = ((crc & 1) != 0) ? ((crc >> 1) ^ 0xEDB88320u) : (crc >> 1);
		}
		return crc;
	}

	private void ScanSonyPads()
	{
		foreach (SonyPad pad in m_SonyPads)
		{
			pad.Dispose();
		}
		m_SonyPads.Clear();
		Guid hidGuid;
		HidD_GetHidGuid(out hidGuid);
		IntPtr set = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
		if (set == new IntPtr(-1))
		{
			return;
		}
		try
		{
			SP_DEVICE_INTERFACE_DATA data = new SP_DEVICE_INTERFACE_DATA();
			data.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
			for (uint index = 0; SetupDiEnumDeviceInterfaces(set, IntPtr.Zero, ref hidGuid, index, ref data); index++)
			{
				string path = GetDevicePath(set, ref data);
				if (path != null)
				{
					SonyPad pad = TryOpen(path);
					if (pad != null)
					{
						m_SonyPads.Add(pad);
					}
				}
			}
		}
		finally
		{
			SetupDiDestroyDeviceInfoList(set);
		}
	}

	private static string GetDevicePath(IntPtr set, ref SP_DEVICE_INTERFACE_DATA data)
	{
		int size;
		SetupDiGetDeviceInterfaceDetail(set, ref data, IntPtr.Zero, 0, out size, IntPtr.Zero);
		if (size <= 0)
		{
			return null;
		}
		IntPtr buffer = Marshal.AllocHGlobal(size);
		try
		{
			// cbSize da estrutura SP_DEVICE_INTERFACE_DETAIL_DATA_W: 8 em 64 bits
			Marshal.WriteInt32(buffer, (IntPtr.Size == 8) ? 8 : 6);
			if (!SetupDiGetDeviceInterfaceDetail(set, ref data, buffer, size, out size, IntPtr.Zero))
			{
				return null;
			}
			return Marshal.PtrToStringUni(new IntPtr(buffer.ToInt64() + 4));
		}
		finally
		{
			Marshal.FreeHGlobal(buffer);
		}
	}

	private static SonyPad TryOpen(string path)
	{
		// Primeiro abre sem acesso para ler o VID/PID (nao atrapalha outros programas)
		using (SafeFileHandle probe = CreateFile(path, 0, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero))
		{
			if (probe.IsInvalid)
			{
				return null;
			}
			HIDD_ATTRIBUTES attr = new HIDD_ATTRIBUTES();
			attr.Size = Marshal.SizeOf(typeof(HIDD_ATTRIBUTES));
			if (!HidD_GetAttributes(probe, ref attr) || attr.VendorID != c_SonyVendor)
			{
				return null;
			}
			bool ds4 = Array.IndexOf(c_DS4Products, attr.ProductID) >= 0;
			bool dualSense = Array.IndexOf(c_DualSenseProducts, attr.ProductID) >= 0;
			if (!ds4 && !dualSense)
			{
				return null;
			}
		}
		SafeFileHandle handle = CreateFile(path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
		if (handle.IsInvalid)
		{
			return null;
		}
		HIDD_ATTRIBUTES a = new HIDD_ATTRIBUTES();
		a.Size = Marshal.SizeOf(typeof(HIDD_ATTRIBUTES));
		HidD_GetAttributes(handle, ref a);
		int outputLength = 0;
		IntPtr preparsed;
		if (HidD_GetPreparsedData(handle, out preparsed))
		{
			HIDP_CAPS caps;
			if (HidP_GetCaps(preparsed, out caps) == HIDP_STATUS_SUCCESS)
			{
				outputLength = caps.OutputReportByteLength;
			}
			HidD_FreePreparsedData(preparsed);
		}
		if (outputLength <= 0)
		{
			handle.Dispose();
			return null;
		}
		bool isDs4 = Array.IndexOf(c_DS4Products, a.ProductID) >= 0;
		Debug.Log("[PortRumble] HID aberto: " + a.ProductID.ToString("X4") + " saida " + outputLength);
		SonyPad pad = new SonyPad();
		pad.Handle = handle;
		pad.ReportLength = outputLength;
		// Pela USB o maior relatorio de saida e pequeno (32 no DS4, 48 no DualSense); pelo Bluetooth e bem maior.
		if (isDs4)
		{
			pad.Kind = (outputLength <= 32) ? SonyKind.DS4Usb : SonyKind.DS4Bluetooth;
		}
		else
		{
			pad.Kind = (outputLength <= 64) ? SonyKind.DualSenseUsb : SonyKind.DualSenseBluetooth;
		}
		Debug.Log("[PortRumble] Controle PlayStation encontrado: " + pad.Kind + " (relatorio " + outputLength + ")");
		return pad;
	}

	private const uint GENERIC_READ = 0x80000000u;

	private const uint GENERIC_WRITE = 0x40000000u;

	private const uint FILE_SHARE_READ = 1;

	private const uint FILE_SHARE_WRITE = 2;

	private const uint OPEN_EXISTING = 3;

	private const uint DIGCF_PRESENT = 0x2;

	private const uint DIGCF_DEVICEINTERFACE = 0x10;

	private const int HIDP_STATUS_SUCCESS = 0x00110000;

	[StructLayout(LayoutKind.Sequential)]
	private struct SP_DEVICE_INTERFACE_DATA
	{
		public int cbSize;

		public Guid InterfaceClassGuid;

		public int Flags;

		public IntPtr Reserved;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct HIDD_ATTRIBUTES
	{
		public int Size;

		public ushort VendorID;

		public ushort ProductID;

		public ushort VersionNumber;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct HIDP_CAPS
	{
		public ushort Usage;

		public ushort UsagePage;

		public ushort InputReportByteLength;

		public ushort OutputReportByteLength;

		public ushort FeatureReportByteLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
		public ushort[] Reserved;

		public ushort NumberLinkCollectionNodes;

		public ushort NumberInputButtonCaps;

		public ushort NumberInputValueCaps;

		public ushort NumberInputDataIndices;

		public ushort NumberOutputButtonCaps;

		public ushort NumberOutputValueCaps;

		public ushort NumberOutputDataIndices;

		public ushort NumberFeatureButtonCaps;

		public ushort NumberFeatureValueCaps;

		public ushort NumberFeatureDataIndices;
	}

	[DllImport("hid.dll")]
	private static extern void HidD_GetHidGuid(out Guid guid);

	[DllImport("hid.dll")]
	private static extern bool HidD_GetAttributes(SafeFileHandle device, ref HIDD_ATTRIBUTES attributes);

	[DllImport("hid.dll")]
	private static extern bool HidD_GetPreparsedData(SafeFileHandle device, out IntPtr preparsed);

	[DllImport("hid.dll")]
	private static extern bool HidD_FreePreparsedData(IntPtr preparsed);

	[DllImport("hid.dll")]
	private static extern int HidP_GetCaps(IntPtr preparsed, out HIDP_CAPS caps);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
	private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr parent, uint flags);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiEnumDeviceInterfaces(IntPtr set, IntPtr devInfo, ref Guid classGuid, uint index, ref SP_DEVICE_INTERFACE_DATA data);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
	private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr set, ref SP_DEVICE_INTERFACE_DATA data, IntPtr detail, int detailSize, out int requiredSize, IntPtr devInfo);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiDestroyDeviceInfoList(IntPtr set);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteFile(SafeFileHandle file, byte[] buffer, uint count, out uint written, IntPtr overlapped);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern SafeFileHandle CreateFile(string name, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);
}
