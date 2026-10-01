using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// PORT: controls menu, opened from the settings menu (GameStateMenuOptions). Built at runtime as a modal
// accessible panel; like the game's menus, each page starts with its title:
//   main: Keyboard / PlayStation gamepad / Xbox gamepad / Back
//   device: one entry per key of each run action ("Jump: Up arrow"), one "Jump: add key" entry per action,
//           Restore defaults, Back
// Choosing a key entry waits for the new key (or gamepad button) and replaces it; the same key changes nothing
// and Delete removes it. Choosing an "add" entry adds the pressed key. Esc cancels, as does 10 s without input.
// The result is said at once and the focus stays on the same entry.
// Esc (or the gamepad's back button) goes back one page. The keys are kept in the save (PortBindings).
//
// The page's elements are updated in place: the accessibility plugin breaks (exceptions every frame) when the
// focused element is destroyed, so removed entries leave the container and the focus moves before they are
// destroyed.
public class PortControlsMenu : MonoBehaviour
{
	private const float c_CaptureTimeout = 10f;

	private const int c_RemoveKey = (int)KeyCode.Delete;

	private const string c_Title = "title";

	private static PortControlsMenu s_Instance;

	private enum Page
	{
		Main,
		Device
	}

	private Page m_Page;

	private int m_Device;

	private Font m_Font;

	private AccessibleUIGroupRoot m_Group;

	private Transform m_Content;

	// Elements on the page by entry id ("title", "device:1", "key:2:32", "add:2", ...)
	private readonly Dictionary<string, GameObject> m_Elements = new Dictionary<string, GameObject>();

	private struct Entry
	{
		public string Id;

		public string Caption;

		public UnityEngine.Events.UnityAction OnClick;
	}

	private readonly List<Entry> m_Entries = new List<Entry>();

	// Capture: the entry being changed (code -1 = add a key)
	private bool m_Capturing;

	private PortAction m_CaptureAction;

	private int m_CaptureCode;

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
		PortControlsMenu menu = root.AddComponent<PortControlsMenu>();
		s_Instance = menu;
		menu.m_Font = (font != null) ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
		menu.m_ReturnFocus = returnFocus;

		GameObject panel = CreateRect("Panel", root.transform, new Vector2(0.1f, 0.03f), new Vector2(0.9f, 0.97f));
		panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.95f);
		menu.m_Group = panel.AddComponent<AccessibleUIGroupRoot>();
		menu.m_Group.m_PopUp = true;
		GameObject content = CreateRect("Content", panel.transform, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.98f));
		VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
		layout.spacing = 2f;
		layout.childControlHeight = true;
		layout.childControlWidth = true;
		layout.childForceExpandHeight = false;
		layout.childForceExpandWidth = true;
		menu.m_Content = content.transform;
		// The first page is read from its title when the panel opens
		menu.ShowMain(c_Title);
		root.SetActive(true);
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
			return;
		}
		if (s_Instance.m_Page == Page.Device)
		{
			s_Instance.ShowMain(c_Title);
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
		// Disabling the panel first deactivates its container, so the settings menu gets the focus back
		// before any element is destroyed
		menu.m_Group.gameObject.SetActive(false);
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

	// Interrupts what is being read; the next key press interrupts it in turn
	private void Say(string text)
	{
		UAP_AccessibilityManager.Say(text, true, true, UAP_AudioQueue.EInterrupt.All);
	}

	// Pages ----------------------------------------------------------------------------------------------

	private void ShowMain(string focus)
	{
		m_Page = Page.Main;
		m_Entries.Clear();
		AddEntry(c_Title, L("port_controls_title"), null);
		for (int d = 0; d < PortBindings.DeviceCount; d++)
		{
			int device = d;
			AddEntry("device:" + d, PortBindings.DeviceName(d), () => ShowDevice(device, c_Title));
		}
		AddEntry("close", L("port_controls_back"), Close);
		Sync(focus, null);
	}

	private void ShowDevice(int device, string focus, string say = null)
	{
		m_Page = Page.Device;
		m_Device = device;
		m_Entries.Clear();
		AddEntry(c_Title, PortBindings.DeviceName(device), null);
		string add = L(device == PortBindings.Keyboard ? "port_controls_add_key_entry" : "port_controls_add_button_entry");
		for (int a = 0; a < PortBindings.ActionCount; a++)
		{
			PortAction action = (PortAction)a;
			string name = PortBindings.ActionName(action);
			foreach (int code in PortBindings.Get(device, action))
			{
				int c = code;
				AddEntry(KeyId(action, c), name + ": " + PortBindings.CodeName(device, c), () => StartCapture(action, c));
			}
			AddEntry(AddId(action), name + ": " + add, () => StartCapture(action, -1));
		}
		AddEntry("restore", L("port_controls_restore"), RestoreDefaults);
		AddEntry("back", L("port_controls_back"), () => ShowMain(c_Title));
		Sync(focus, say);
	}

	private string KeyId(PortAction action, int code)
	{
		return "key:" + m_Device + ":" + (int)action + ":" + code;
	}

	private string AddId(PortAction action)
	{
		return "add:" + m_Device + ":" + (int)action;
	}

	private void RestoreDefaults()
	{
		if (m_Capturing)
		{
			return;
		}
		PortBindings.RestoreDefaults(m_Device);
		ShowDevice(m_Device, "restore", L("port_controls_restored"));
	}

	// Capture ------------------------------------------------------------------------------------------

	private void StartCapture(PortAction action, int code)
	{
		if (m_Capturing)
		{
			return;
		}
		string name = PortBindings.ActionName(action);
		string current = code < 0 ? "" : PortBindings.CodeName(m_Device, code);
		string text;
		if (m_Device == PortBindings.Keyboard)
		{
			text = string.Format(L(code < 0 ? "port_controls_press_key" : "port_controls_press_new_key"), name, current);
		}
		else
		{
			if (!PortInput.GamepadConnected || PortInput.PadDevice != m_Device)
			{
				Say(string.Format(L("port_controls_connect_pad"), PortBindings.DeviceName(m_Device)));
				return;
			}
			text = string.Format(L(code < 0 ? "port_controls_press_button" : "port_controls_press_new_button"), name, current);
		}
		Say(text);
		m_Capturing = true;
		m_CaptureAction = action;
		m_CaptureCode = code;
		// The Enter or button press that chose the entry must not be captured
		m_CaptureArmFrame = Time.frameCount + 1;
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
		if (!m_Capturing || Time.frameCount < m_CaptureArmFrame)
		{
			return;
		}
		if (!PortInput.Capturing)
		{
			// From the next frame PortInput reports the first key or button pressed
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
		Apply(code);
	}

	// Applies the captured key to the entry being changed, says the result and keeps the focus on the entry
	private void Apply(int code)
	{
		string action = PortBindings.ActionName(m_CaptureAction);
		string focus;
		string text;
		// Delete is reserved for removing keys
		if (code == c_RemoveKey)
		{
			if (m_CaptureCode < 0)
			{
				Say(L("port_controls_unchanged"));
				return;
			}
			PortBindings.Remove(m_Device, m_CaptureAction, m_CaptureCode);
			text = string.Format(L("port_controls_removed"), PortBindings.CodeName(m_Device, m_CaptureCode), action);
			// The focus goes to the next entry of the action (another key, or "add key")
			focus = AddId(m_CaptureAction);
			List<int> codes = PortBindings.Get(m_Device, m_CaptureAction);
			string removedId = KeyId(m_CaptureAction, m_CaptureCode);
			for (int i = 0; i < m_Entries.Count - 1; i++)
			{
				if (m_Entries[i].Id == removedId && m_Entries[i + 1].Id.StartsWith("key:"))
				{
					focus = m_Entries[i + 1].Id;
				}
			}
			ShowDevice(m_Device, focus, text);
			return;
		}
		if (code == m_CaptureCode || (m_CaptureCode < 0 && PortBindings.Get(m_Device, m_CaptureAction).Contains(code)))
		{
			Say(L("port_controls_unchanged"));
			return;
		}
		string name = PortBindings.CodeName(m_Device, code);
		int from = PortBindings.Add(m_Device, m_CaptureAction, code);
		if (m_CaptureCode >= 0)
		{
			PortBindings.Remove(m_Device, m_CaptureAction, m_CaptureCode);
		}
		text = string.Format(L("port_controls_assigned"), name, action);
		if (from >= 0)
		{
			text += " " + string.Format(L("port_controls_removed"), name, PortBindings.ActionName((PortAction)from));
		}
		// The new key takes the place of the replaced one; an added key keeps the focus on "add key"
		focus = m_CaptureCode >= 0 ? KeyId(m_CaptureAction, code) : AddId(m_CaptureAction);
		ShowDevice(m_Device, focus, text);
	}

	// UI ---------------------------------------------------------------------------------------------

	private void AddEntry(string id, string caption, UnityEngine.Events.UnityAction onClick)
	{
		m_Entries.Add(new Entry { Id = id, Caption = caption, OnClick = onClick });
	}

	// Makes the page show m_Entries: existing elements are kept (and renamed), the others are created or
	// removed. Then the focus goes to the entry "focus", and "say" is said instead of reading it.
	private void Sync(string focus, string say)
	{
		HashSet<string> ids = new HashSet<string>();
		for (int i = 0; i < m_Entries.Count; i++)
		{
			Entry entry = m_Entries[i];
			// The title keeps its id but belongs to each page
			string id = entry.Id == c_Title ? c_Title + ":" + m_Page + ":" + m_Device : entry.Id;
			if (entry.Id == c_Title && focus == c_Title)
			{
				focus = id;
			}
			ids.Add(id);
			if (!m_Elements.TryGetValue(id, out GameObject go))
			{
				go = (entry.OnClick == null) ? CreateLabel() : CreateButton();
				m_Elements[id] = go;
			}
			go.transform.SetSiblingIndex(i);
			go.GetComponentInChildren<Text>().text = entry.Caption;
			go.GetComponent<UAP_BaseElement>().m_ManualPositionOrder = i;
			Button button = go.GetComponent<Button>();
			if (button != null)
			{
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(entry.OnClick);
			}
		}
		List<GameObject> removed = new List<GameObject>();
		foreach (KeyValuePair<string, GameObject> pair in m_Elements)
		{
			if (!ids.Contains(pair.Key))
			{
				removed.Add(pair.Value);
			}
		}
		foreach (GameObject go in removed)
		{
			foreach (KeyValuePair<string, GameObject> pair in new List<KeyValuePair<string, GameObject>>(m_Elements))
			{
				if (pair.Value == go)
				{
					m_Elements.Remove(pair.Key);
				}
			}
			// Out of the container (and hidden) before the focus moves; destroyed afterwards
			go.SetActive(false);
			go.transform.SetParent(transform, false);
		}
		m_Group.RefreshContainer();
		if (m_Elements.TryGetValue(focus, out GameObject target))
		{
			UAP_AccessibilityManager.SelectElement(target, true);
		}
		if (say != null)
		{
			Say(say);
		}
		foreach (GameObject go in removed)
		{
			Destroy(go);
		}
	}

	private GameObject CreateLabel()
	{
		GameObject go = CreateRect("Title", m_Content, Vector2.zero, Vector2.one);
		go.AddComponent<LayoutElement>().preferredHeight = 40f;
		Text text = go.AddComponent<Text>();
		SetupText(text, 28);
		AccessibleLabel accessible = go.AddComponent<AccessibleLabel>();
		accessible.m_NameLabel = go;
		return go;
	}

	private GameObject CreateButton()
	{
		GameObject go = CreateRect("Button", m_Content, Vector2.zero, Vector2.one);
		go.AddComponent<LayoutElement>().preferredHeight = 26f;
		Image image = go.AddComponent<Image>();
		image.color = new Color(0.45f, 0f, 0f, 1f);
		Button button = go.AddComponent<Button>();
		button.targetGraphic = image;
		GameObject label = CreateRect("Text", go.transform, Vector2.zero, Vector2.one);
		Text text = label.AddComponent<Text>();
		SetupText(text, 18);
		text.resizeTextMaxSize = 18;
		text.resizeTextMinSize = 8;
		text.resizeTextForBestFit = true;
		AccessibleButton accessible = go.AddComponent<AccessibleButton>();
		accessible.m_NameLabel = label;
		return go;
	}

	private void SetupText(Text text, int size)
	{
		text.font = m_Font;
		text.fontSize = size;
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.white;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Truncate;
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
