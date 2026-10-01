using UnityEngine;

// PORT: menu click sound (KlickSingle, played by the scene's UISingleClick AudioSource).
// Most of the original buttons play it through their onClick, but not all of them, and the port's own menus
// did not; UAP_BaseElement.Interact plays it for every option chosen through the accessibility plugin.
// A button that also plays it just restarts the same AudioSource, so it is heard once.
public static class PortClickSound
{
	private static AudioSource s_Source;

	public static void Play()
	{
		if (s_Source == null)
		{
			foreach (AudioSource source in Resources.FindObjectsOfTypeAll<AudioSource>())
			{
				if (source.gameObject.name == "UISingleClick" && source.gameObject.scene.IsValid())
				{
					s_Source = source;
					break;
				}
			}
			if (s_Source == null)
			{
				return;
			}
		}
		s_Source.Play();
	}
}
