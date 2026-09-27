using UnityEngine;
using UnityEngine.UI;

public class InputFieldUI : MonoBehaviour
{
	public float screenHeightReference = 1080f;

	public RectTransform containerToMove;

	public float heightToMove = 200f;

	protected InputField m_InputField;

	protected bool m_Init;

	protected bool m_ContainerMoved;

	private void Start()
	{
		m_Init = false;
		Init();
	}

	protected void Init()
	{
		m_ContainerMoved = false;
		m_InputField = GetComponent<InputField>();
		m_Init = true;
	}

	public void Tick()
	{
		if (!m_Init)
		{
			Init();
		}
		if (!m_ContainerMoved)
		{
			if (m_InputField.isFocused && TouchScreenKeyboard.visible)
			{
				if (!TouchScreenKeyboard.hideInput)
				{
					TouchScreenKeyboard.hideInput = true;
				}
				SetRect(containerToMove, 0f, 0f - heightToMove, 0f, heightToMove);
				m_ContainerMoved = true;
			}
		}
		else if (!TouchScreenKeyboard.visible)
		{
			SetRect(containerToMove, 0f, 0f, 0f, 0f);
			m_ContainerMoved = false;
		}
	}

	public void Reset()
	{
		if (m_ContainerMoved)
		{
			SetRect(containerToMove, 0f, 0f, 0f, 0f);
			m_ContainerMoved = false;
		}
	}

	protected void SetRect(RectTransform trs, float left, float top, float right, float bottom)
	{
		trs.offsetMin = new Vector2(left, bottom);
		trs.offsetMax = new Vector2(0f - right, 0f - top);
	}

	protected int GetAndroidKeyboardSize()
	{
		// PORT: uses AndroidJavaObject (Android on-screen keyboard); no equivalent on Windows.
		return 0;
	}
}
