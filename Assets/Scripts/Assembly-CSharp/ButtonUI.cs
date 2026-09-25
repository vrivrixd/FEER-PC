using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
	public bool buttonEnabled;

	public Color enabledBtnColor;

	public Color enabledTextColor;

	public Color disabledBtnColor;

	public Color disabledTextColor;

	protected Button m_Button;

	protected Image m_ButtonImg;

	protected Text m_ButtonText;

	protected bool m_Init;

	private void Start()
	{
		InitUI();
	}

	protected void InitUI()
	{
		m_Button = GetComponent<Button>();
		m_ButtonImg = GetComponent<Image>();
		m_ButtonText = GetComponentInChildren<Text>();
		if (buttonEnabled)
		{
			SetButtonEnabled();
		}
		else
		{
			SetButtonDisabled();
		}
		m_Init = true;
	}

	protected void SetButtonEnabled()
	{
		m_Button.interactable = true;
		m_ButtonImg.color = enabledBtnColor;
		if (m_ButtonText != null)
		{
			m_ButtonText.color = enabledTextColor;
		}
	}

	protected void SetButtonDisabled()
	{
		m_Button.interactable = false;
		m_ButtonImg.color = disabledBtnColor;
		if (m_ButtonText != null)
		{
			m_ButtonText.color = disabledTextColor;
		}
	}

	public void EnableButton(bool enabled)
	{
		if (buttonEnabled == enabled)
		{
			return;
		}
		buttonEnabled = enabled;
		if (!m_Init)
		{
			InitUI();
		}
		else if (enabled)
		{
			SetButtonEnabled();
		}
		else
		{
			SetButtonDisabled();
		}
	}
}
