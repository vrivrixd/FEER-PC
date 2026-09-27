using UnityEngine;

// PORT: entrada do PC convertida nos gestos do jogo original (Android: deslizar e tocar).
//
// Teclado (ja existia no codigo original): setas = deslizar, Espaco = tocar (atirar).
// Esc = gesto de pausa (toque duplo com dois dedos): pausa o jogo ou pula o tutorial.
// PORT: nos menus o Esc volta a tela anterior (GameState.PortBack); no menu de pausa, continua o jogo.
// Nos menus o plugin de acessibilidade usa as setas, Enter e Esc (padrao do UAP no Windows).
//
// Joystick (Xbox/XInput e PlayStation DualShock 4/DualSense, detectado pelo nome):
//   analogicos e direcional = setas (menus e corrida);
//   Na corrida: Cruz (A) pula, Circulo (B) agacha, Triangulo/Quadrado/touchpad (X/Y) atiram,
//     L1/L2 (LB/LT) = faixa esquerda, R1/R2 (RB/RT) = faixa direita, Options/Share (Start/Back) pausam.
//   Nos menus: Cruz/Options/touchpad (A/Start) = Enter; Circulo/Share (B/Back) = Esc.
// "Na corrida" = o plugin de acessibilidade esta pausado (a corrida esta rodando sem menu).
// Controles PlayStation sao lidos direto pelo HID quando possivel (PortSonyInput): funciona
// tambem pelo Bluetooth, onde o Windows nao entrega os botoes ao Unity.
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

	private const float c_EscRepeatDelay = 0.35f;

	private float m_NextEscTime;

	private static bool TryBack()
	{
		// Editando um campo: o Esc so cancela a edicao (tratado pelo plugin de acessibilidade)
		if (UAP_AccessibilityManager.PortIsInteracting())
		{
			return true;
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		GameState top = (cgm != null) ? cgm.topState : null;
		return top != null && top.PortBack();
	}

	private void OnApplicationQuit()
	{
		PortSonyInput.Shutdown();
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
		// Esc do teclado ou do controle: nos menus volta a tela anterior; senao, gesto de pausa.
		// Intervalo minimo entre dois Esc: o mesmo aperto pode chegar duas vezes (controle + teclado
		// simulado pelo Steam), o que fazia o jogo retomar e pausar de novo.
		bool esc = Input.GetKeyDown(KeyCode.Escape) || s_Cancel;
		if ((esc || s_PauseToggle) && Time.unscaledTime >= m_NextEscTime)
		{
			m_NextEscTime = Time.unscaledTime + c_EscRepeatDelay;
			if (!(esc && !IsRunning() && TryBack()))
			{
				UAP_AccessibilityManager.PortTriggerPauseToggle();
			}
		}
	}

	// Numeracao dos botoes no Unity (Input Manager antigo)
	// Numeracao dos botoes no Unity (Input Manager antigo).
	// Xbox: 0 A, 1 B, 2 X, 3 Y, 4 LB, 5 RB, 6 Back, 7 Start (LT/RT sao eixos 9 e 10).
	// PlayStation: 0 Quadrado, 1 Cruz, 2 Circulo, 3 Triangulo, 4 L1, 5 R1, 6 L2, 7 R2, 8 Share, 9 Options, 13 Touchpad.
	private class Layout
	{
		public KeyCode[] Jump;      // corrida: pular (= direcional para cima)

		public KeyCode[] Slide;     // corrida: agachar (= direcional para baixo)

		public KeyCode[] Shoot;     // corrida: atirar (= Espaco)

		public KeyCode[] Left;      // corrida: faixa da esquerda

		public KeyCode[] Right;     // corrida: faixa da direita

		public KeyCode[] Pause;     // corrida: pausa (= Esc)

		public KeyCode[] Submit;    // menus: Enter

		public KeyCode[] Cancel;    // menus: Esc
	}

	private static readonly Layout c_Xbox = new Layout
	{
		Jump = new[] { KeyCode.JoystickButton0 },
		Slide = new[] { KeyCode.JoystickButton1 },
		Shoot = new[] { KeyCode.JoystickButton2, KeyCode.JoystickButton3 },
		Left = new[] { KeyCode.JoystickButton4 },
		Right = new[] { KeyCode.JoystickButton5 },
		Pause = new[] { KeyCode.JoystickButton6, KeyCode.JoystickButton7 },
		Submit = new[] { KeyCode.JoystickButton0, KeyCode.JoystickButton7 },
		Cancel = new[] { KeyCode.JoystickButton1, KeyCode.JoystickButton6 }
	};

	private static readonly Layout c_Sony = new Layout
	{
		Jump = new[] { KeyCode.JoystickButton1 },
		Slide = new[] { KeyCode.JoystickButton2 },
		Shoot = new[] { KeyCode.JoystickButton0, KeyCode.JoystickButton3, KeyCode.JoystickButton13 },
		Left = new[] { KeyCode.JoystickButton4, KeyCode.JoystickButton6 },
		Right = new[] { KeyCode.JoystickButton5, KeyCode.JoystickButton7 },
		Pause = new[] { KeyCode.JoystickButton8, KeyCode.JoystickButton9 },
		Submit = new[] { KeyCode.JoystickButton1, KeyCode.JoystickButton9, KeyCode.JoystickButton13 },
		Cancel = new[] { KeyCode.JoystickButton2, KeyCode.JoystickButton8 }
	};

	private bool m_LeftTriggerDown;

	private bool m_RightTriggerDown;

	private string m_LastJoystickName;

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
			if (name != m_LastJoystickName)
			{
				m_LastJoystickName = name;
				Debug.Log("[PortInput] Controle: " + name);
			}
			string n = name.ToLowerInvariant();
			m_Sony = !n.Contains("xbox") && !n.Contains("xinput") && (n.Contains("wireless controller") || n.Contains("dualsense") || n.Contains("dualshock") || n.Contains("sony") || n.Contains("playstation"));
			break;
		}
	}

	// Corrida rodando (plugin de acessibilidade pausado); sem o plugin, usa o estado do jogo.
	private static bool IsRunning()
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			return !UAP_AccessibilityManager.IsActive();
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		GameState top = (cgm != null) ? cgm.topState : null;
		return top != null && top.GetName() == GameStateName.PlayGame;
	}

	// true = controle PlayStation lido direto pelo HID (PortSonyInput); o Unity e ignorado para ele.
	private static bool s_UseHid;

	private static bool AnyDown(KeyCode[] keys)
	{
		foreach (KeyCode k in keys)
		{
			if (s_UseHid ? PortSonyInput.WasPressed(k - KeyCode.JoystickButton0) : Input.GetKeyDown(k))
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
		PortSonyInput.Poll();
		s_UseHid = PortSonyInput.Active;
		if (s_UseHid)
		{
			m_Sony = true;
		}
		// Unity: Y dos analogicos e positivo para baixo (PortJoyY ja vem invertido); Y do direcional e positivo para cima.
		Vector2 value = new Vector2(ReadAxis("PortJoyX"), ReadAxis("PortJoyY"));
		if (s_UseHid)
		{
			value = Strongest(Strongest(PortSonyInput.LeftStick, PortSonyInput.RightStick), PortSonyInput.Dpad);
		}
		else if (m_Sony)
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
		Layout layout = m_Sony ? c_Sony : c_Xbox;
		// Gatilhos do Xbox sao eixos: gera um toque quando passam da metade
		bool leftTrigger = false;
		bool rightTrigger = false;
		if (!m_Sony)
		{
			bool lt = ReadAxis("PortAxis8") > 0.5f;
			bool rt = ReadAxis("PortAxis9") > 0.5f;
			leftTrigger = lt && !m_LeftTriggerDown;
			rightTrigger = rt && !m_RightTriggerDown;
			m_LeftTriggerDown = lt;
			m_RightTriggerDown = rt;
		}
		if (IsRunning())
		{
			// Corrida: botoes viram os gestos do jogo; o direcional continua valendo (acima).
			if (AnyDown(layout.Jump))
			{
				GameSwipeUp = true;
			}
			if (AnyDown(layout.Slide))
			{
				GameSwipeDown = true;
			}
			if (AnyDown(layout.Left) || leftTrigger)
			{
				GameSwipeLeft = true;
			}
			if (AnyDown(layout.Right) || rightTrigger)
			{
				GameSwipeRight = true;
			}
			if (AnyDown(layout.Shoot))
			{
				GameTap = true;
			}
			if (AnyDown(layout.Pause))
			{
				s_PauseToggle = true;
			}
		}
		else
		{
			if (AnyDown(layout.Submit))
			{
				s_Submit = true;
			}
			if (AnyDown(layout.Cancel))
			{
				s_Cancel = true;
			}
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
