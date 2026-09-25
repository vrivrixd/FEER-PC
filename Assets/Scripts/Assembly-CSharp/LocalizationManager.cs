using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
	public delegate void OnLocalizationReadyCallbackFunc();

	protected OnLocalizationReadyCallbackFunc m_OnLocalizationReadyCallbacks;

	public SystemLanguage[] supportedLanguages;

	public MacOSTTSVoices[] macOSTTSVoices;

	public LanguageFlags[] languageFlags;

	protected Dictionary<string, string> m_LocalizedText;

	protected bool m_Init;

	protected string m_LanguageCodeLong;

	protected string m_LanguageCodeShort;

	protected CultureInfo m_CultureInfo;

	protected string m_macOSTTSVoiceParam;

	protected int m_UserLanguageIndex;

	private static LocalizationManager instance;

	public bool initFinished => false;

	public static LocalizationManager Instance => null;

	private void Awake()
	{
	}

	public void Init()
	{
	}

	public void RegisterOnLocalizationReadyCallback(OnLocalizationReadyCallbackFunc func)
	{
	}

	public void UnRegisterOnLocalizationReadyCallback(OnLocalizationReadyCallbackFunc func)
	{
	}

	public void LocalizedTextLoaded(bool success, string fileContent)
	{
	}

	public void SetUserLanguage(SystemLanguage language)
	{
	}

	public void LoadLocalizedText()
	{
	}

	public string GetLocalizedValue(string key)
	{
		return null;
	}

	public string GetLongLanguageCode()
	{
		return null;
	}

	public string GetShortLanguageCode()
	{
		return null;
	}

	public string GetShortLanguageCodeForSystemLanguage(SystemLanguage language)
	{
		return null;
	}

	public string GetLongLanguageCodeForSystemLanguage(SystemLanguage language)
	{
		return null;
	}

	public SystemLanguage GetUserLanguage()
	{
		return SystemLanguage.Afrikaans;
	}

	public CultureInfo GetCultureInfo()
	{
		return null;
	}

	public string GetMacOSTTSVoiceParam()
	{
		return null;
	}

	public bool IsShortCodeLanguageSupported(string shortLanguageCode)
	{
		return false;
	}

	public bool IsSystemLanguageSupported(SystemLanguage language)
	{
		return false;
	}

	public SystemLanguage GetSystemLanguageForShortLangCode(string shortLangCode)
	{
		return SystemLanguage.Afrikaans;
	}

	public void ChangeLanguage(SystemLanguage toLanguage)
	{
	}

	public List<Dropdown.OptionData> GetLanguageDropdownList()
	{
		return null;
	}

	public int GetUserLanguageIndex()
	{
		return 0;
	}

	public SystemLanguage GetSystemLanguageForIndex(int index)
	{
		return SystemLanguage.Afrikaans;
	}
}
