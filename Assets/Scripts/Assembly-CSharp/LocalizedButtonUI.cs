using UnityEngine;
using UnityEngine.UI;

public class LocalizedButtonUI : MonoBehaviour
{
	public bool useCallback;

	private void Awake()
	{
		if (!useCallback)
		{
			Text text = GetComponentInChildren<Text>();
			text.text = LocalizationManager.Instance.GetLocalizedValue(text.text);
		}
		else
		{
			LocalizationManager.Instance.RegisterOnLocalizationReadyCallback(OnLocalizationReady);
		}
	}

	public void OnLocalizationReady()
	{
		Text text = GetComponentInChildren<Text>();
		text.text = LocalizationManager.Instance.GetLocalizedValue(text.text);
		LocalizationManager.Instance.UnRegisterOnLocalizationReadyCallback(OnLocalizationReady);
	}
}
