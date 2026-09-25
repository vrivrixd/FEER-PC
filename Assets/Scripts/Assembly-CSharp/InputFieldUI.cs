using UnityEngine;
using UnityEngine.UI;

public class InputFieldUI : MonoBehaviour
{
	public float screenHeightReference;

	public RectTransform containerToMove;

	public float heightToMove;

	protected InputField m_InputField;

	protected bool m_Init;

	protected bool m_ContainerMoved;

	private void Start()
	{
	}

	protected void Init()
	{
	}

	public void Tick()
	{
	}

	public void Reset()
	{
	}

	protected void SetRect(RectTransform trs, float left, float top, float right, float bottom)
	{
	}

	protected int GetAndroidKeyboardSize()
	{
		return 0;
	}
}
