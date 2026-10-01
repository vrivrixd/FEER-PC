using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// PORT: controls menu, opened from the settings menu (GameStateMenuOptions). Built at runtime as modal
// accessible pages:
//   main: Keyboard / PlayStation gamepad / Xbox gamepad / Close
//   device: one entry per key of each run action ("Jump: Up arrow"), one "Jump: add key" entry per action,
//           Restore defaults, Back
// Choosing a key entry waits for the new key (or gamepad button) and replaces it; the same key changes nothing
// and Delete removes it. Choosing an "add" entry adds the pressed key. Esc cancels, as does 10 s without input.
// Afterwards the same page comes back with the focus on the same entry.
// Esc (or the gamepad's back button) goes back one page. The keys are kept in the save (PortBindings).
//
// Each page is its own popup container. The accessibility plugin breaks (exceptions every frame) when the
// focused element is destroyed while its container is active, so a new page is shown first and the old one
// is destroyed only after the plugin has moved the focus to the new one.
public class PortControlsMenu : MonoBehaviour
{
	private const float c_CaptureTimeout = 10f;

	private const int c_RemoveKey = (int)KeyCode.Delete;

	private static PortControlsMenu s_Instance;

	private enum Page
	{
		Main,
		Device
	}

	private Page m_Page;

	private int m_Device;

	private Font m_Font;

	// Current page and the page being replaced (destroyed once the new one has the focus)
	private GameObject m_PageRoot;

	private GameObject m_OldPage;

	private Transform m_Content;

	private int m_Order;

	private int m_FocusIndex;

	private GameObject m_FocusTarget;

	// Capture: the entry being changed (code -1 = add a key)
	private bool m_Capturing;

	private PortAction m_CaptureAction;

	private int m_CaptureCode;

	private int m_CaptureEntry;

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
		s_Instance.ShowMain(0, true);
	}

	public static void Back()
	{
		if (s_Instance == null)
		{
			return;
		}
		if (s_Instance.m_Capturing)
		{
			s_Instance.StopCapture();
			s_Instance.Say(L("port_controls_cancelled"));
			s_Instance.ShowDevice(s_Instance.m_Device, s_Instance.m_CaptureEntry, false);
			return;
		}
		// A page is still being replaced
		if (s_Instance.m_OldPage != null)
		{
			return;
		}
		if (s_Instance.m_Page == Page.Device)
		{
			s_Instance.ShowMain(s_Instance.m_Device, true);
		}
		else
		{
			Close();
		}
	}

	public static void Close()
	{
		if (s_Instance == null)
		{
			return;
		}
		PortControlsMenu menu = s_Instance;
		s_Instance = null;
		if (menu.m_Capturing)
		{
			menu.StopCapture();
		}
		// Disabling the pages first deactivates their containers, so the settings menu gets the focus back
		// before any element is destroyed
		if (menu.m_OldPage != null)
		{
			menu.m_OldPage.SetActive(false);
		}
		if (menu.m_PageRoot != null)
		{
			menu.m_PageRoot.SetActive(false);
		}
		Destroy(menu.gameObject);
		if (menu.m_ReturnFocus != null)
		{
			UAP_AccessibilityManager.SelectElement(menu.m_ReturnFocus, true);
		}
	}

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	private void Say(string text)
	{
		UAP_AccessibilityManager.Say(text, false, true, UAP_AudioQueue.EInterrupt.All);
	}

	// Pages ----------------------------------------------------------------------------------------------

	private void ShowMain(int focus, bool sayTitle)
	{
		m_Page = Page.Main;
		NewPage(L("port_controls_title"), sayTitle, focus);
		AddButton(PortBindings.DeviceName(PortBindings.Keyboard), () => ShowDevice(PortBindings.Keyboard, 0, true));
		AddButton(PortBindings.DeviceName(PortBindings.PlayStation), () => ShowDevice(PortBindings.PlayStation, 0, true));
		AddButton(PortBindings.DeviceName(PortBindings.Xbox), () => ShowDevice(PortBindings.Xbox, 0, true));
		AddButton(L("CLOSE"), Close);
		FinishPage();
	}

	private void ShowDevice(int device, int focus, bool sayTitle)
	{
		m_Page = Page.Device;
		m_Device = device;
		NewPage(PortBindings.DeviceName(device), sayTitle, focus);
		string add = L(device == PortBindings.Keyboard ? "port_controls_add_key_entry" : "port_controls_add_button_entry");
		for (int a = 0; a < PortBindings.ActionCount; a++)
		{
			PortAction action = (PortAction)a;
			string name = PortBindings.ActionName(action);
			foreach (int code in PortBindings.Get(device, action))
			{
				int c = code;
				int entry = m_Order;
				AddButton(name + ": " + PortBindings.CodeName(device, c), () => StartCapture(action, c, entry));
			}
			int addEntry = m_Order;
			AddButton(name + ": " + add, () => StartCapture(action, -1, addEntry));
		}
		int restoreEntry = m_Order;
		AddButton(L("port_controls_restore"), () => RestoreDefaults(restoreEntry));
		AddButton(L("port_controls_back"), () => ShowMain(m_Device, true));
		FinishPage();
	}

	private void RestoreDefaults(int entry)
	{
		if (m_OldPage != null)
		{
			return;
		}
		PortBindings.RestoreDefaults(m_Device);
		Say(L("port_controls_restored"));
		ShowDevice(m_Device, entry, false);
	}

	// Capture ------------------------------------------------------------------------------------------

	private void StartCapture(PortAction action, int code, int entry)
	{
		if (m_Capturing || m_OldPage != null)
		{
			return;
		}
		string name = PortBindings.ActionName(action);
		string text;
		if (m_Device == PortBindings.Keyboard)
		{
			text = string.Format(L(code < 0 ? "port_controls_press_key" : "port_controls_press_new_key"), name, code < 0 ? "" : PortBindings.CodeName(m_Device, code));
		}
		else
		{
			if (!PortInput.GamepadConnected || PortInput.PadDevice != m_Device)
			{
				Say(string.Format(L("port_controls_connect_pad"), PortBindings.DeviceName(m_Device)));
				return;
			}
			text = string.Format(L(code < 0 ? "port_controls_press_button" : "port_controls_press_new_button"), name, code < 0 ? "" : PortBindings.CodeName(m_Device, code));
		}
		Say(text);
		m_Capturing = true;
		m_CaptureAction = action;
		m_CaptureCode = code;
		m_CaptureEntry = entry;
		// The Enter or button press that chose the entry must not be captured
		m_CaptureArmFrame = Time.frameCount + 2;
		m_CaptureDeadline = Time.unscaledTime + c_CaptureTimeout;
		UAP_AccessibilityManager.BlockInput(true, false);
	}

	private void StopCapture()
	{
		m_Capturing = false;
		PortInput.Capturing = false;
		UAP_AccessibilityManager.BlockInput(false, false);
	}

	private void Update()
	{
		if (m_OldPage != null)
		{
			GameObject focus = UAP_AccessibilityManager.GetCurrentFocusObject();
			if ((focus != null && m_PageRoot != null && focus.transform.IsChildOf(m_PageRoot.transform)) || Time.unscaledTime > m_OldPageDeadline)
			{
				Destroy(m_OldPage);
				m_OldPage = null;
			}
		}
		if (!m_Capturing || Time.frameCount < m_CaptureArmFrame)
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
			Back();
			return;
		}
		int code = (m_Device == PortBindings.Keyboard || PortInput.CapturedKey == c_RemoveKey) ? PortInput.CapturedKey : PortInput.CapturedPad;
		if (code < 0)
		{
			return;
		}
		StopCapture();
		Say(Apply(code));
		ShowDevice(m_Device, m_CaptureEntry, false);
	}

	// Applies the captured key to the entry being changed; returns what to say
	private string Apply(int code)
	{
		string action = PortBindings.ActionName(m_CaptureAction);
		// Delete is reserved for removing keys
		if (code == c_RemoveKey)
		{
			if (m_CaptureCode < 0)
			{
				return L("port_controls_unchanged");
			}
			PortBindings.Remove(m_Device, m_CaptureAction, m_CaptureCode);
			return string.Format(L("port_controls_removed"), PortBindings.CodeName(m_Device, m_CaptureCode), action);
		}
		if (code == m_CaptureCode)
		{
			return L("port_controls_unchanged");
		}
		string name = PortBindings.CodeName(m_Device, code);
		int from = PortBindings.Add(m_Device, m_CaptureAction, code);
		if (m_CaptureCode >= 0)
		{
			PortBindings.Remove(m_Device, m_CaptureAction, m_CaptureCode);
		}
		string text = string.Format(L("port_controls_assigned"), name, action);
		if (from >= 0)
		{
			text += " " + string.Format(L("port_controls_removed"), name, PortBindings.ActionName((PortAction)from));
		}
		return text;
	}

	// UI helpers ---------------------------------------------------------------------------------------

	private float m_OldPageDeadline;

	private void NewPage(string title, bool sayTitle, int focus)
	{
		if (m_OldPage != null)
		{
			Destroy(m_OldPage);
		}
		m_OldPage = m_PageRoot;
		if (m_OldPage != null)
		{
			// Hidden at once, destroyed in Update once the new page has the focus
			CanvasGroup hide = m_OldPage.AddComponent<CanvasGroup>();
			hide.alpha = 0f;
			hide.blocksRaycasts = false;
			m_OldPageDeadline = Time.unscaledTime + 2f;
		}
		m_FocusIndex = focus;
		m_FocusTarget = null;
		m_Order = 0;

		GameObject page = CreateRect("Page", transform, new Vector2(0.1f, 0.05f), new Vector2(0.9f, 0.95f));
		page.SetActive(false);
		page.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.95f);
		AccessibleUIGroupRoot group = page.AddComponent<AccessibleUIGroupRoot>();
		group.m_PopUp = true;
		// The page name is read when the page opens (not when it is rebuilt after a change)
		group.m_ContainerName = sayTitle ? title : "";
		m_PageRoot = page;

		GameObject label = CreateRect("Title", page.transform, new Vector2(0.05f, 0.9f), new Vector2(0.95f, 0.99f));
		SetupText(label.AddComponent<Text>(), title, 30);

		GameObject scroll = CreateRect("Content", page.transform, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.89f));
		VerticalLayoutGroup layout = scroll.AddComponent<VerticalLayoutGroup>();
		layout.spacing = 2f;
		layout.childControlHeight = true;
		layout.childControlWidth = true;
		layout.childForceExpandHeight = true;
		layout.childForceExpandWidth = true;
		m_Content = scroll.transform;
	}

	private void FinishPage()
	{
		// The focused entry starts the page: the plugin reads it when the page opens
		if (m_FocusTarget == null && m_Content.childCount > 0)
		{
			m_FocusTarget = m_Content.GetChild(m_Content.childCount - 1).gameObject;
		}
		if (m_FocusTarget != null)
		{
			m_FocusTarget.GetComponent<AccessibleButton>().m_ForceStartHere = true;
		}
		m_PageRoot.SetActive(true);
	}

	private void AddButton(string caption, UnityEngine.Events.UnityAction onClick)
	{
		GameObject go = CreateRect("Button", m_Content, Vector2.zero, Vector2.one);
		go.AddComponent<LayoutElement>().preferredHeight = 30f;
		Image image = go.AddComponent<Image>();
		image.color = new Color(0.45f, 0f, 0f, 1f);
		Button button = go.AddComponent<Button>();
		button.targetGraphic = image;
		button.onClick.AddListener(onClick);
		GameObject label = CreateRect("Text", go.transform, Vector2.zero, Vector2.one);
		Text text = label.AddComponent<Text>();
		SetupText(text, caption, 20);
		text.resizeTextMaxSize = 20;
		text.resizeTextMinSize = 8;
		text.resizeTextForBestFit = true;
		AccessibleButton accessible = go.AddComponent<AccessibleButton>();
		accessible.m_NameLabel = label;
		accessible.m_ManualPositionOrder = m_Order;
		if (m_Order == m_FocusIndex)
		{
			m_FocusTarget = go;
		}
		m_Order++;
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
