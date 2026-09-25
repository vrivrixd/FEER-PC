using UnityEngine;

// PORT: entrada adicional do PC (joystick e mouse) convertida nos gestos do jogo original.
// Teclado: as setas ja sao lidas pelo codigo original do jogo.
// Os valores valem apenas durante o frame em que o gesto aconteceu.
[DefaultExecutionOrder(-1000)]
public class PortInput : MonoBehaviour
{
	public static bool GameSwipeLeft { get; private set; }

	public static bool GameSwipeRight { get; private set; }

	public static bool GameSwipeUp { get; private set; }

	public static bool GameSwipeDown { get; private set; }

	public static bool GameTap { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Create()
	{
		GameObject go = new GameObject("PortInput");
		DontDestroyOnLoad(go);
		go.AddComponent<PortInput>();
	}

	private void Update()
	{
		GameSwipeLeft = false;
		GameSwipeRight = false;
		GameSwipeUp = false;
		GameSwipeDown = false;
		GameTap = false;
	}
}
