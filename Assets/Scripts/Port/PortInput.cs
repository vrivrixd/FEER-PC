using UnityEngine;

// PORT: entrada do PC convertida nos gestos do jogo original (Android: deslizar e tocar).
//
// Teclado (ja existia no codigo original): setas = deslizar, Espaco = tocar (atirar).
// Esc = gesto de pausa (toque duplo com dois dedos): pausa o jogo ou pula o tutorial.
// Nos menus o plugin de acessibilidade usa as setas, Enter e Esc (padrao do UAP no Windows).
//
// Joystick: direcional ou analogico esquerdo = setas; A = Enter/tocar; B = Esc;
// Start = gesto de pausa. Tambem funciona nos menus (vira as teclas do UAP).
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
		UpdateJoystick();
		UpdateMouse();
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
		{
			UAP_AccessibilityManager.PortTriggerPauseToggle();
		}
	}

	private void UpdateJoystick()
	{
		Vector2 stick = new Vector2(ReadAxis("PortJoyX"), ReadAxis("PortJoyY"));
		Vector2 dpad = new Vector2(ReadAxis("PortDpadX"), ReadAxis("PortDpadY"));
		Vector2 value = (dpad.sqrMagnitude > stick.sqrMagnitude) ? dpad : stick;
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
		if (Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			s_Submit = true;
			GameTap = true;
		}
		if (Input.GetKeyDown(KeyCode.JoystickButton1))
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
