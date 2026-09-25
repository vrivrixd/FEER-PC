using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectThemeInputController : MonoBehaviour
{
	public Animator swipeAnimator;

	public GameObject forestThemeBtn;

	public GameObject factoryThemeBtn;

	public Text buyBtnText;

	public Button buyBtn;

	protected Vector2 m_StartingTouch;

	protected bool m_IsTouching;

	protected int m_SwipeDirection;

	protected int m_FocusedTheme;

	protected bool m_isTap;

	protected const int c_SWIPE_LEFT = 3;

	protected const int c_SWIPE_RIGHT = 4;

	protected const int c_SWIPE_NONE = 0;

	protected bool m_SnapNext;

	protected int m_CurrentFocusedTheme;

	protected const int c_THEME_FOREST = 1;

	protected const int c_THEME_FACTORY = 2;

	protected bool m_ThemeIsExpanded;

	protected int m_SelectedThemeToBuy;

	protected bool m_AccessibleMode;

	protected const int c_FACTORY_PURCHASED = 1;

	protected const int c_FACTORY_WAITING_FOR_APPROVAL = 2;

	protected const int c_FACTORY_BUY = 3;

	protected int m_FactoryStatus;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SnapNext()
	{
	}

	public void ThemeForestFocused()
	{
	}

	public void ThemeFactoryFocused()
	{
	}

	public void ForestClicked()
	{
	}

	private IEnumerator StartLoadingTheme(int themeNumber)
	{
		return null;
	}

	protected void AdjustBuyButtons(int themeNumber)
	{
	}

	public void FactoryClicked()
	{
	}

	protected void ExpandFactoryTheme()
	{
	}

	public void BuyFactoryThemeClicked()
	{
	}

	public void BackToThemesBtnClicked()
	{
	}

	public void OnIAPManagerClosed()
	{
	}
}
