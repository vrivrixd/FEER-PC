using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// PORT: controls menu, opened from the settings menu (GameStateMenuOptions). Built at runtime as a modal
// accessible panel with three pages:
//   main: Keyboard / PlayStation gamepad / Xbox gamepad / Close
//   device: one button per run action ("Jump: Up arrow, W"), Restore defaults, Back
//   action: Add key (or button), one "Remove <key>" button per key, Back
// Adding waits for the next key or gamepad button (PortInput.Capturing); Esc cancels, as does 10 s without input.
// Esc (or the gamepad's back button) goes back one page. The keys are kept in the save (PortBindings).
public class PortControlsMenu : MonoBehaviour
{
	private const float c_CaptureTimeout = 10f;

	// The accessibility plugin registers new elements about 0.5 s after they appear
	private const float c_RegisterDelay = 0.6f;

	private static PortControlsMenu s_Instance;

	private enum Page
	{
		Main,
		Device,
		Action
	}

	private Page m_Page;

	private int m_Device;

	private PortAction m_Action;

	private Font m_Font;

	private Transform m_Content;

	private bool m_Capturing;

	private float m_CaptureDeadline;

	private int m_CaptureArmFrame;

	// Element that gets the focus back when the menu closes (the settings menu's Controls button)
	private GameObject m_ReturnFocus;

	public static bool IsOpen => s_Instance != null;

	public static void Open(Font font, GameObject returnFocus)
	{
		if (s_Instance != null)
		{
			return;
		}
		GameObject root = new GameObject("PortControlsMenu");
		root.SetActive(false);
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 900;
		CanvasScaler scaler = root.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1280f, 720f);
		root.AddComponent<GraphicRaycaster>();
		s_Instance = root.AddComponent<PortControlsMenu>();
		s_Instance.m_Font = (font != null) ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
		s_Instance.m_ReturnFocus = returnFocus;

		GameObject panel = CreateRect("Panel", root.transform, new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.95f));
		panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.95f);
		AccessibleUIGroupRoot group = panel.AddComponent<AccessibleUIGroupRoot>();
		group.m_PopUp = true;
		GameObject content = CreateRect("Content", panel.transform, new Vector2(0.05f, 0.03f), new Vector2(0.95f, 0.97f));
		VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
		layout.spacing = 6f;
		layout.childControlHeight = true;
		layout.childControlWidth = true;
		layout.childForceExpandHeight = false;
		layout.childForceExpandWidth = true;
		s_Instance.m_Content = content.transform;
		root.SetActive(true);
		s_Instance.ShowMain(0);
	}

	public static void Back()
	{
		if (s_Instance == null)
		{
			return;
		}
		if (s_Instance.m_Capturing)
		{
			s_Instance.StopCapture(true);
			return;
		}
		switch (s_Instance.m_Page)
		{
		case Page.Action:
			s_Instance.ShowDevice(s_Instance.m_Device, (int)s_Instance.m_Action);
			break;
		case Page.Device:
			s_Instance.ShowMain(s_Instance.m_Device);
			break;
		default:
			Close();
			break;
		}
	}

	public static void Close()
	{
		if (s_Instance == null)
		{
			return;
		}
		if (s_Instance.m_Capturing)
		{
			s_Instance.StopCapture(false);
		}
		GameObject returnFocus = s_Instance.m_ReturnFocus;
		Destroy(s_Instance.gameObject);
		s_Instance = null;
		if (returnFocus != null)
		{
			UAP_AccessibilityManager.SelectElement(returnFocus, true);
		}
	}

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	private static void Say(string text)
	{
		UAP_AccessibilityManager.Say(text, false, true, UAP_AudioQueue.EInterrupt.All);
	}

	// Pages ----------------------------------------------------------------------------------------------

	private void ShowMain(int focus)
	{
		m_Page = Page.Main;
		List<GameObject> buttons = new List<GameObject>();
		Clear(L("port_controls_title"));
		buttons.Add(AddButton(PortBindings.DeviceName(PortBindings.Keyboard), () => ShowDevice(PortBindings.Keyboard, 0)));
		buttons.Add(AddButton(PortBindings.DeviceName(PortBindings.PlayStation), () => ShowDevice(PortBindings.PlayStation, 0)));
		buttons.Add(AddButton(PortBindings.DeviceName(PortBindings.Xbox), () => ShowDevice(PortBindings.Xbox, 0)));
		AddButton(L("CLOSE"), Close);
		Focus(buttons[Mathf.Clamp(focus, 0, buttons.Count - 1)]);
	}

	private void ShowDevice(int device, int focus)
	{
		m_Page = Page.Device;
		m_Device = device;
		List<GameObject> buttons = new List<GameObject>();
		Clear(PortBindings.DeviceName(device));
		for (int a = 0; a < PortBindings.ActionCount; a++)
		{
			PortAction action = (PortAction)a;
			buttons.Add(AddButton(PortBindings.ActionName(action) + ": " + PortBindings.Names(device, action), () => ShowAction(action, 0)));
		}
		buttons.Add(AddButton(L("port_controls_restore"), RestoreDefaults));
		AddButton(L("port_controls_back"), () => ShowMain(m_Device));
		Focus(buttons[Mathf.Clamp(focus, 0, buttons.Count - 1)]);
	}

	private void ShowAction(PortAction action, int focus)
	{
		m_Page = Page.Action;
		m_Action = action;
		List<GameObject> buttons = new List<GameObject>();
		Clear(PortBindings.ActionName(action) + " (" + PortBindings.DeviceName(m_Device) + ")");
		buttons.Add(AddButton(L(m_Device == PortBindings.Keyboard ? "port_controls_add_key" : "port_controls_add_button"), StartCapture));
		foreach (int code in new List<int>(PortBindings.Get(m_Device, action)))
		{
			int c = code;
			buttons.Add(AddButton(string.Format(L("port_controls_remove"), PortBindings.CodeName(m_Device, c)), () => RemoveCode(c)));
		}
		AddButton(L("port_controls_back"), () => ShowDevice(m_Device, (int)m_Action));
		Focus(buttons[Mathf.Clamp(focus, 0, buttons.Count - 1)]);
	}

	private void RestoreDefaults()
	{
		PortBindings.RestoreDefaults(m_Device);
		Say(L("port_controls_restored"));
		ShowDevice(m_Device, PortBindings.ActionCount);
	}

	private void RemoveCode(int code)
	{
		string name = PortBindings.CodeName(m_Device, code);
		PortBindings.Remove(m_Device, m_Action, code);
		Say(string.Format(L("port_controls_removed"), name, PortBindings.ActionName(m_Action)));
		ShowAction(m_Action, 0);
	}

	// Capture ------------------------------------------------------------------------------------------

	private void StartCapture()
	{
		string action = PortBindings.ActionName(m_Action);
		if (m_Device == PortBindings.Keyboard)
		{
			Say(string.Format(L("port_controls_press_key"), action));
		}
		else
		{
			if (!PortInput.GamepadConnected || PortInput.PadDevice != m_Device)
			{
				Say(string.Format(L("port_controls_connect_pad"), PortBindings.DeviceName(m_Device)));
				return;
			}
			Say(string.Format(L("port_controls_press_button"), action));
		}
		m_Capturing = true;
		// The Enter or button press that chose "Add" must not be captured
		m_CaptureArmFrame = Time.frameCount + 2;
		m_CaptureDeadline = Time.unscaledTime + c_CaptureTimeout;
		UAP_AccessibilityManager.BlockInput(true, false);
	}

	private void StopCapture(bool announce)
	{
		m_Capturing = false;
		PortInput.Capturing = false;
		UAP_AccessibilityManager.BlockInput(false, false);
		if (announce)
		{
			Say(L("port_controls_cancelled"));
			ShowAction(m_Action, 0);
		}
	}

	private void Update()
	{
		if (!m_Capturing)
		{
			return;
		}
		if (Time.frameCount < m_CaptureArmFrame)
		{
			return;
		}
		if (!PortInput.Capturing)
		{
			PortInput.Capturing = true;
			return;
		}
		if (Time.unscaledTime > m_CaptureDeadline || PortInput.CapturedKey == (int)KeyCode.Escape)
		{
			StopCapture(true);
			return;
		}
		int code = (m_Device == PortBindings.Keyboard) ? PortInput.CapturedKey : PortInput.CapturedPad;
		if (code < 0)
		{
			return;
		}
		StopCapture(false);
		int from = PortBindings.Add(m_Device, m_Action, code);
		string name = PortBindings.CodeName(m_Device, code);
		string text = string.Format(L("port_controls_assigned"), name, PortBindings.ActionName(m_Action));
		if (from >= 0)
		{
			text += " " + string.Format(L("port_controls_removed"), name, PortBindings.ActionName((PortAction)from));
		}
		Say(text);
		ShowAction(m_Action, 0);
	}

	// UI helpers ---------------------------------------------------------------------------------------

	private int m_Order;

	private void Clear(string title)
	{
		StopAllCoroutines();
		foreach (Transform child in m_Content)
		{
			Destroy(child.gameObject);
		}
		m_Order = 0;
		GameObject label = CreateRect("Title", m_Content, Vector2.zero, Vector2.one);
		label.AddComponent<LayoutElement>().preferredHeight = 56f;
		Text text = label.AddComponent<Text>();
		SetupText(text, title, 30);
		AccessibleLabel accessible = label.AddComponent<AccessibleLabel>();
		accessible.m_NameLabel = label;
		accessible.m_ManualPositionOrder = m_Order++;
	}

	private GameObject AddButton(string caption, UnityEngine.Events.UnityAction onClick)
	{
		GameObject go = CreateRect("Button", m_Content, Vector2.zero, Vector2.one);
		go.AddComponent<LayoutElement>().preferredHeight = 44f;
		Image image = go.AddComponent<Image>();
		image.color = new Color(0.45f, 0f, 0f, 1f);
		Button button = go.AddComponent<Button>();
		button.targetGraphic = image;
		button.onClick.AddListener(onClick);
		GameObject label = CreateRect("Text", go.transform, Vector2.zero, Vector2.one);
		SetupText(label.AddComponent<Text>(), caption, 24);
		AccessibleButton accessible = go.AddComponent<AccessibleButton>();
		accessible.m_NameLabel = label;
		accessible.m_ManualPositionOrder = m_Order++;
		return go;
	}

	private void SetupText(Text text, string value, int size)
	{
		text.font = m_Font;
		text.fontSize = size;
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.white;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Truncate;
		text.text = value;
	}

	private void Focus(GameObject element)
	{
		StartCoroutine(FocusLater(element));
	}

	private IEnumerator FocusLater(GameObject element)
	{
		yield return new WaitForSecondsRealtime(c_RegisterDelay);
		if (element != null)
		{
			UAP_AccessibilityManager.SelectElement(element, true);
		}
	}

	private static GameObject CreateRect(string name, Transform parent, Vector2 min, Vector2 max)
	{
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.transform.SetParent(parent, false);
		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = min;
		rect.anchorMax = max;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		return go;
	}

	private void OnDestroy()
	{
		if (s_Instance == this)
		{
			s_Instance = null;
		}
		PortInput.Capturing = false;
	}
}
