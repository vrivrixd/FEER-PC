using System;
using System.IO;
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

	public bool initFinished => m_Init;

	public static LocalizationManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public void Init()
	{
		m_Init = false;
		LoadLocalizedText();
	}

	public void RegisterOnLocalizationReadyCallback(OnLocalizationReadyCallbackFunc func)
	{
		m_OnLocalizationReadyCallbacks = (OnLocalizationReadyCallbackFunc)Delegate.Combine(m_OnLocalizationReadyCallbacks, func);
		if (m_OnLocalizationReadyCallbacks != null && m_Init)
		{
			m_OnLocalizationReadyCallbacks();
		}
	}

	public void UnRegisterOnLocalizationReadyCallback(OnLocalizationReadyCallbackFunc func)
	{
		m_OnLocalizationReadyCallbacks = (OnLocalizationReadyCallbackFunc)Delegate.Remove(m_OnLocalizationReadyCallbacks, func);
	}

	public void LocalizedTextLoaded(bool success, string fileContent)
	{
		if (success)
		{
			m_LocalizedText = new Dictionary<string, string>();
			LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(fileContent);
			for (int i = 0; i < localizationData.items.Length; i++)
			{
				m_LocalizedText.Add(localizationData.items[i].key, localizationData.items[i].value);
			}
		}
		else
		{
			Debug.LogError("Localization Manager: Cannot find localization file!");
		}
		if (!m_Init)
		{
			m_Init = true;
		}
		if (m_OnLocalizationReadyCallbacks != null)
		{
			m_OnLocalizationReadyCallbacks();
		}
	}

	public void SetUserLanguage(SystemLanguage language)
	{
		m_LanguageCodeLong = GetLongLanguageCodeForSystemLanguage(language);
		m_LanguageCodeShort = GetShortLanguageCodeForSystemLanguage(language);
		m_macOSTTSVoiceParam = "";
		for (int i = 0; i < macOSTTSVoices.Length; i++)
		{
			if (macOSTTSVoices[i].language == language)
			{
				m_macOSTTSVoiceParam = "-v " + macOSTTSVoices[i].voiceName + " ";
				break;
			}
		}
		m_CultureInfo = new CultureInfo(m_LanguageCodeLong);
		for (int j = 0; j < supportedLanguages.Length; j++)
		{
			if (supportedLanguages[j] == language)
			{
				m_UserLanguageIndex = j;
				break;
			}
		}
	}

	public void LoadLocalizedText()
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, "localization_" + m_LanguageCodeShort + ".json");
		DataManager.Instance.LoadFile(filePath, LocalizedTextLoaded);
	}

	public string GetLocalizedValue(string key)
	{
		if (key.Length > 0 && m_LocalizedText.ContainsKey(key) && m_LocalizedText[key] != null)
		{
			return m_LocalizedText[key];
		}
		return key;
	}

	public string GetLongLanguageCode()
	{
		return m_LanguageCodeLong;
	}

	public string GetShortLanguageCode()
	{
		return m_LanguageCodeShort;
	}

	public string GetShortLanguageCodeForSystemLanguage(SystemLanguage language)
	{
		switch (language)
		{
		case SystemLanguage.French:
			return "fr";
		case SystemLanguage.German:
			return "de";
		case SystemLanguage.Italian:
			return "it";
		case SystemLanguage.Spanish:
			return "es";
		default:
			return "en";
		}
	}

	public string GetLongLanguageCodeForSystemLanguage(SystemLanguage language)
	{
		switch (language)
		{
		case SystemLanguage.French:
			return "fr-FR";
		case SystemLanguage.German:
			return "de-DE";
		case SystemLanguage.Italian:
			return "it-IT";
		case SystemLanguage.Spanish:
			return "es-ES";
		default:
			return "en-US";
		}
	}

	public SystemLanguage GetUserLanguage()
	{
		return GetSystemLanguageForShortLangCode(m_LanguageCodeShort);
	}

	public CultureInfo GetCultureInfo()
	{
		return m_CultureInfo;
	}

	public string GetMacOSTTSVoiceParam()
	{
		return m_macOSTTSVoiceParam;
	}

	public bool IsShortCodeLanguageSupported(string shortLanguageCode)
	{
		return IsSystemLanguageSupported(GetSystemLanguageForShortLangCode(shortLanguageCode));
	}

	public bool IsSystemLanguageSupported(SystemLanguage language)
	{
		for (int i = 0; i < supportedLanguages.Length; i++)
		{
			if (supportedLanguages[i] == language)
			{
				return true;
			}
		}
		return false;
	}

	public SystemLanguage GetSystemLanguageForShortLangCode(string shortLangCode)
	{
		switch (shortLangCode)
		{
		case "en":
		case "EN":
			return SystemLanguage.English;
		case "de":
		case "DE":
			return SystemLanguage.German;
		case "fr":
		case "FR":
			return SystemLanguage.French;
		case "it":
		case "IT":
			return SystemLanguage.Italian;
		case "es":
		case "ES":
			return SystemLanguage.Spanish;
		default:
			return SystemLanguage.Unknown;
		}
	}

	public void ChangeLanguage(SystemLanguage toLanguage)
	{
		m_Init = false;
		SetUserLanguage(toLanguage);
		// PORT: AndroidTTS.ChangeLanguage removido (TTS do Android/Google).
		LoadLocalizedText();
	}

	public List<Dropdown.OptionData> GetLanguageDropdownList()
	{
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
		for (int i = 0; i < supportedLanguages.Length; i++)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			optionData.text = GetLocalizedValue(supportedLanguages[i].ToString());
			for (int j = 0; j < languageFlags.Length; j++)
			{
				if (languageFlags[j].language == supportedLanguages[i])
				{
					optionData.image = languageFlags[j].flag;
					break;
				}
			}
			list.Add(optionData);
		}
		return list;
	}

	public int GetUserLanguageIndex()
	{
		return m_UserLanguageIndex;
	}

	public SystemLanguage GetSystemLanguageForIndex(int index)
	{
		return supportedLanguages[index];
	}
}
