using UnityEngine;

// PORT: entrada do PC convertida nos gestos do jogo original (Android: deslizar e tocar).
//
// Teclado (ja existia no codigo original): setas = deslizar, Espaco = tocar (atirar).
// Esc = gesto de pausa (toque duplo com dois dedos): pausa o jogo ou pula o tutorial.
// Nos menus o plugin de acessibilidade usa as setas, Enter e Esc (padrao do UAP no Windows).
//
// Joystick (Xbox/XInput e PlayStation DualShock 4/DualSense, detectado pelo nome):
//   analogicos e direcional = setas;
//   X/Cruz (A no Xbox) e botao do touchpad = Enter nos menus, Espaco (tocar/atirar) no jogo;
//   Start/Options = pausa/retoma na corrida e no menu de pausa (como o Esc), Enter nos outros menus;
//   Circulo (B no Xbox), Select/Back/Share = Esc (inclusive o gesto de pausa).
// "No jogo" = o plugin de acessibilidade esta pausado (a corrida esta rodando sem menu).
// Vibracao do controle: ver PortRumble.
//
// Mouse: arrastar com o botao esquerdo = deslizar; clique sem arrastar = tocar.
//
// Os valores valem apenas durante o frame em que o gesto aconteceu.
[DefaultExecutionOrder(-1000)]
public class PortInput : MonoBehaviour
{
	public static bool GameSwipeLeft { get; private set; }

	public static bool GameSwipeRight { get; private set; }

	public static bool GameSwipeUp { get; private set; }

	public static bool GameSwipeDown { get; private set; }

	public static bool GameTap { get; private set; }

	// Teclas virtuais geradas pelo joystick neste frame
	private static bool s_Up;

	private static bool s_Down;

	private static bool s_Left;

	private static bool s_Right;

	private static bool s_Submit;

	private static bool s_Cancel;

	private static bool s_PauseToggle;

	private const float c_StickThreshold = 0.6f;

	private const float c_StickRelease = 0.3f;

	// Mesmo limite do jogo original para reconhecer um deslizar (2% da largura da tela)
	private const float c_MouseSwipeThreshold = 0.02f;

	private Vector2 m_StickLast;

	private bool m_MouseDown;

	private bool m_MouseSwiped;

	private Vector2 m_MouseStart;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Create()
	{
		GameObject go = new GameObject("PortInput");
		DontDestroyOnLoad(go);
		go.AddComponent<PortInput>();
	}

	// Substitui Input.GetKeyDown nos pontos do jogo/UAP que leem teclas: inclui o joystick.
	public static bool GetKeyDown(KeyCode key)
	{
		if (Input.GetKeyDown(key))
		{
			return true;
		}
		switch (key)
		{
		case KeyCode.UpArrow:
			return s_Up;
		case KeyCode.DownArrow:
			return s_Down;
		case KeyCode.LeftArrow:
			return s_Left;
		case KeyCode.RightArrow:
			return s_Right;
		case KeyCode.Return:
			return s_Submit;
		case KeyCode.Escape:
			return s_Cancel;
		default:
			return false;
		}
	}

	private static float ReadAxis(string axis)
	{
		try
		{
			return Input.GetAxisRaw(axis);
		}
		catch (System.ArgumentException)
		{
			return 0f;
		}
	}

	private void Update()
	{
		GameSwipeLeft = false;
		GameSwipeRight = false;
		GameSwipeUp = false;
		GameSwipeDown = false;
		GameTap = false;
		s_Up = false;
		s_Down = false;
		s_Left = false;
		s_Right = false;
		s_Submit = false;
		s_Cancel = false;
		s_PauseToggle = false;
		UpdateJoystick();
		UpdateMouse();
		// Esc do teclado ou do controle: gesto de pausa
		if (Input.GetKeyDown(KeyCode.Escape) || s_Cancel || s_PauseToggle)
		{
			UAP_AccessibilityManager.PortTriggerPauseToggle();
		}
	}

	// Numeracao dos botoes no Unity (Input Manager antigo)
	private static readonly KeyCode[] c_XboxGameButtons = { KeyCode.JoystickButton0 };            // A

	private static readonly KeyCode[] c_XboxSubmitButtons = { KeyCode.JoystickButton7 };          // Start

	private static readonly KeyCode[] c_XboxCancelButtons = { KeyCode.JoystickButton1, KeyCode.JoystickButton6 }; // B, Back

	private static readonly KeyCode[] c_SonyGameButtons = { KeyCode.JoystickButton1, KeyCode.JoystickButton13 }; // Cruz, touchpad

	private static readonly KeyCode[] c_SonySubmitButtons = { KeyCode.JoystickButton9 };          // Options

	private static readonly KeyCode[] c_SonyCancelButtons = { KeyCode.JoystickButton2, KeyCode.JoystickButton8 }; // Circulo, Share/Create

	private bool m_Sony;

	private float m_NextNameCheck;

	// DualShock 4 e DualSense sem emulador aparecem como "Wireless Controller" (DirectInput);
	// qualquer outro controle usa o layout do Xbox (XInput), inclusive DS4Windows/Steam.
	private void DetectControllerType()
	{
		if (Time.unscaledTime < m_NextNameCheck)
		{
			return;
		}
		m_NextNameCheck = Time.unscaledTime + 2f;
		m_Sony = false;
		foreach (string name in Input.GetJoystickNames())
		{
			if (string.IsNullOrEmpty(name))
			{
				continue;
			}
			string n = name.ToLowerInvariant();
			m_Sony = !n.Contains("xbox") && !n.Contains("xinput") && (n.Contains("wireless controller") || n.Contains("dualsense") || n.Contains("dualshock") || n.Contains("sony") || n.Contains("playstation"));
			break;
		}
	}

	private static bool IsRunningOrPaused()
	{
		if (UAP_AccessibilityManager.IsEnabled() && !UAP_AccessibilityManager.IsActive())
		{
			return true;
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		GameState top = (cgm != null) ? cgm.topState : null;
		return top != null && (top.GetName() == GameStateName.Pause || (!UAP_AccessibilityManager.IsEnabled() && top.GetName() == GameStateName.PlayGame));
	}

	private static bool AnyDown(KeyCode[] keys)
	{
		foreach (KeyCode k in keys)
		{
			if (Input.GetKeyDown(k))
			{
				return true;
			}
		}
		return false;
	}

	private static Vector2 Strongest(Vector2 a, Vector2 b)
	{
		return (b.sqrMagnitude > a.sqrMagnitude) ? b : a;
	}

	private void UpdateJoystick()
	{
		DetectControllerType();
		// Unity: Y dos analogicos e positivo para baixo (PortJoyY ja vem invertido); Y do direcional e positivo para cima.
		Vector2 value = new Vector2(ReadAxis("PortJoyX"), ReadAxis("PortJoyY"));
		if (m_Sony)
		{
			value = Strongest(value, new Vector2(ReadAxis("PortAxis2"), -ReadAxis("PortAxis5")));
			value = Strongest(value, new Vector2(ReadAxis("PortAxis6"), ReadAxis("PortAxis7")));
		}
		else
		{
			value = Strongest(value, new Vector2(ReadAxis("PortAxis3"), -ReadAxis("PortAxis4")));
			value = Strongest(value, new Vector2(ReadAxis("PortAxis5"), ReadAxis("PortAxis6")));
		}
		// Gera um "toque" de tecla so quando a direcao passa do limite (sem repetir enquanto segurado)
		if (Mathf.Abs(m_StickLast.x) < c_StickRelease && Mathf.Abs(m_StickLast.y) < c_StickRelease)
		{
			if (Mathf.Abs(value.x) >= c_StickThreshold || Mathf.Abs(value.y) >= c_StickThreshold)
			{
				if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
				{
					if (value.x < 0f)
					{
						s_Left = true;
					}
					else
					{
						s_Right = true;
					}
				}
				else if (value.y > 0f)
				{
					s_Up = true;
				}
				else
				{
					s_Down = true;
				}
				m_StickLast = value;
			}
		}
		else if (Mathf.Abs(value.x) < c_StickRelease && Mathf.Abs(value.y) < c_StickRelease)
		{
			m_StickLast = Vector2.zero;
		}
		if (AnyDown(m_Sony ? c_SonyGameButtons : c_XboxGameButtons))
		{
			// Com o plugin de acessibilidade pausado a corrida esta rodando: vira Espaco; senao, Enter.
			// Sem o plugin ligado nao ha como distinguir: faz os dois (como o clique do mouse).
			if (!UAP_AccessibilityManager.IsEnabled())
			{
				GameTap = true;
				s_Submit = true;
			}
			else if (!UAP_AccessibilityManager.IsActive())
			{
				GameTap = true;
			}
			else
			{
				s_Submit = true;
			}
		}
		if (AnyDown(m_Sony ? c_SonySubmitButtons : c_XboxSubmitButtons))
		{
			// Start: pausa/retoma na corrida e no menu de pausa (como o Esc); Enter nos outros menus.
			if (IsRunningOrPaused())
			{
				s_PauseToggle = true;
			}
			else
			{
				s_Submit = true;
			}
		}
		if (AnyDown(m_Sony ? c_SonyCancelButtons : c_XboxCancelButtons))
		{
			s_Cancel = true;
		}
	}

	private void UpdateMouse()
	{
		if (Input.touchCount > 0)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			m_MouseDown = true;
			m_MouseSwiped = false;
			m_MouseStart = Input.mousePosition;
		}
		else if (m_MouseDown && Input.GetMouseButton(0) && !m_MouseSwiped)
		{
			Vector2 diff = (Vector2)Input.mousePosition - m_MouseStart;
			diff = new Vector2(diff.x / (float)Screen.width, diff.y / (float)Screen.width);
			if (diff.magnitude > c_MouseSwipeThreshold)
			{
				if (Mathf.Abs(diff.y) <= Mathf.Abs(diff.x))
				{
					if (diff.x < 0f)
					{
						GameSwipeLeft = true;
					}
					else
					{
						GameSwipeRight = true;
					}
				}
				else if (diff.y < 0f)
				{
					GameSwipeDown = true;
				}
				else
				{
					GameSwipeUp = true;
				}
				m_MouseSwiped = true;
			}
		}
		if (Input.GetMouseButtonUp(0) && m_MouseDown)
		{
			m_MouseDown = false;
			if (!m_MouseSwiped)
			{
				GameTap = true;
			}
		}
	}
}
