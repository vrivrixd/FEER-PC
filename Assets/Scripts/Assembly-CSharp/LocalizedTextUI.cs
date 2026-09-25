using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextUI : MonoBehaviour
{
	public bool useCallback;

	private void Awake()
	{
		if (!useCallback)
		{
			Text text = GetComponent<Text>();
			text.text = LocalizationManager.Instance.GetLocalizedValue(text.text);
		}
		else
		{
			LocalizationManager.Instance.RegisterOnLocalizationReadyCallback(OnLocalizationReady);
		}
	}

	public void OnLocalizationReady()
	{
		Text text = GetComponent<Text>();
		text.text = LocalizationManager.Instance.GetLocalizedValue(text.text);
		LocalizationManager.Instance.UnRegisterOnLocalizationReadyCallback(OnLocalizationReady);
	}
}
