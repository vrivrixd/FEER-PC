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

	protected bool m_isTap = true;

	protected const int c_SWIPE_LEFT = 3;

	protected const int c_SWIPE_RIGHT = 4;

	protected const int c_SWIPE_NONE = 0;

	protected bool m_SnapNext;

	protected int m_CurrentFocusedTheme = 1;

	protected const int c_THEME_FOREST = 1;

	protected const int c_THEME_FACTORY = 2;

	protected bool m_ThemeIsExpanded;

	protected int m_SelectedThemeToBuy;

	protected bool m_AccessibleMode;

	protected const int c_FACTORY_PURCHASED = 1;

	protected const int c_FACTORY_WAITING_FOR_APPROVAL = 2;

	protected const int c_FACTORY_BUY = 3;

	protected int m_FactoryStatus = -1;

	private void Start()
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			m_AccessibleMode = true;
		}
	}

	private void Update()
	{
		if (m_ThemeIsExpanded)
		{
			if (m_CurrentFocusedTheme == 2)
			{
				AdjustBuyButtons(2);
			}
			return;
		}
		if (!m_AccessibleMode)
		{
			if (Input.touchCount != 1)
			{
				return;
			}
			Touch touch = Input.GetTouch(0);
			TouchPhase phase = touch.phase;
			if (m_IsTouching)
			{
				Vector2 diff = Input.GetTouch(0).position - m_StartingTouch;
				Vector2 normalized = new Vector2(diff.x / (float)Screen.width, diff.y / (float)Screen.width);
				if (normalized.magnitude <= 0.01f || Mathf.Abs(normalized.x) <= Mathf.Abs(normalized.y))
				{
					swipeAnimator.SetFloat("speedValue", 0f);
				}
				else
				{
					int direction = (0f <= normalized.x) ? 4 : 3;
					if (m_SwipeDirection != direction)
					{
						swipeAnimator.SetTrigger((0f <= normalized.x) ? "swipeRight" : "swipeLeft");
						m_SwipeDirection = direction;
					}
					swipeAnimator.SetFloat("speedValue", 3f);
					m_isTap = false;
				}
				m_StartingTouch = touch.position;
			}
			if (phase == TouchPhase.Ended)
			{
				m_IsTouching = false;
				if (m_SwipeDirection != 0 && !m_isTap)
				{
					if (m_SnapNext)
					{
						m_SnapNext = false;
						swipeAnimator.SetTrigger("snapNext");
						swipeAnimator.SetFloat("speedValue", 3f);
					}
					else
					{
						swipeAnimator.SetFloat("speedValue", -3f);
					}
				}
			}
			else
			{
				if (phase != TouchPhase.Began)
				{
					return;
				}
				m_StartingTouch = touch.position;
				m_IsTouching = true;
				m_isTap = true;
			}
			m_SwipeDirection = 0;
			return;
		}
		if (m_CurrentFocusedTheme == 2)
		{
			if (UAP_AccessibilityManager.GetCurrentFocusObject() != forestThemeBtn)
			{
				return;
			}
			m_CurrentFocusedTheme = 1;
			swipeAnimator.SetTrigger("snapNext");
			swipeAnimator.SetTrigger("swipeRight");
		}
		else
		{
			if (m_CurrentFocusedTheme != 1)
			{
				return;
			}
			if (UAP_AccessibilityManager.GetCurrentFocusObject() != factoryThemeBtn)
			{
				return;
			}
			m_CurrentFocusedTheme = 2;
			swipeAnimator.SetTrigger("snapNext");
			swipeAnimator.SetTrigger("swipeLeft");
		}
		swipeAnimator.SetFloat("speedValue", 3f);
	}

	public void SnapNext()
	{
		m_SnapNext = !m_SnapNext;
	}

	public void ThemeForestFocused()
	{
		m_FocusedTheme = 1;
	}

	public void ThemeFactoryFocused()
	{
		m_FocusedTheme = 2;
	}

	public void ForestClicked()
	{
		if (m_AccessibleMode)
		{
			swipeAnimator.Play("FirstThemeLoading");
			StartCoroutine(StartLoadingTheme(1));
		}
		else if (m_isTap)
		{
			if (m_FocusedTheme == 1)
			{
				swipeAnimator.Play("FirstThemeLoading");
				StartCoroutine(StartLoadingTheme(1));
			}
			else
			{
				swipeAnimator.SetTrigger("swipeRight");
				swipeAnimator.SetFloat("speedValue", 3f);
				swipeAnimator.SetTrigger("snapNext");
			}
			m_IsTouching = false;
			m_SwipeDirection = 0;
		}
	}

	private IEnumerator StartLoadingTheme(int themeNumber)
	{
		float elapsedTime = 0f;
		while (elapsedTime < 0.5f)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		if (themeNumber == 2)
		{
			FeerSceneManager.Instance.ThemeFactorySelected();
		}
		else if (themeNumber == 1)
		{
			FeerSceneManager.Instance.ThemeForestSelected();
		}
	}

	protected void AdjustBuyButtons(int themeNumber)
	{
		if (themeNumber != 2)
		{
			return;
		}
		PlayerThemeData themeData = DataManager.Instance.playerThemeData;
		if (themeData.themeFactoryPurchased)
		{
			if (m_FactoryStatus != 1)
			{
				buyBtn.interactable = true;
				buyBtnText.text = LocalizationManager.Instance.GetLocalizedValue("PLAY");
				m_FactoryStatus = 1;
			}
		}
		else if (themeData.themeFactoryWaitingForApproval)
		{
			if (m_FactoryStatus != 2)
			{
				buyBtn.interactable = false;
				buyBtnText.text = LocalizationManager.Instance.GetLocalizedValue("WAITING FOR APPROVAL");
				m_FactoryStatus = 2;
			}
		}
		else if (m_FactoryStatus != 3)
		{
			buyBtn.interactable = true;
			buyBtnText.text = LocalizationManager.Instance.GetLocalizedValue("BUY NOW");
			m_FactoryStatus = 3;
		}
	}

	public void FactoryClicked()
	{
		if (m_AccessibleMode)
		{
			if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
			{
				AdjustBuyButtons(2);
				ExpandFactoryTheme();
			}
			else
			{
				swipeAnimator.Play("SecondThemeLoading");
				StartCoroutine(StartLoadingTheme(2));
			}
		}
		else if (m_isTap)
		{
			if (m_FocusedTheme == 2)
			{
				if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
				{
					AdjustBuyButtons(2);
					ExpandFactoryTheme();
				}
				else
				{
					swipeAnimator.Play("SecondThemeLoading");
					StartCoroutine(StartLoadingTheme(2));
				}
			}
			else
			{
				swipeAnimator.SetTrigger("swipeLeft");
				swipeAnimator.SetFloat("speedValue", 3f);
				swipeAnimator.SetTrigger("snapNext");
			}
			m_IsTouching = false;
			m_SwipeDirection = 0;
		}
	}

	protected void ExpandFactoryTheme()
	{
		// PORT: CustomAnalyticsTracker.StoreItemClicked removed (analytics).
		m_ThemeIsExpanded = true;
		swipeAnimator.Play("SecondThemeExpand");
	}

	public void BuyFactoryThemeClicked()
	{
		if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
		{
			m_SelectedThemeToBuy = 2;
			IAPManager.Instance.BuyThemeFactory(OnIAPManagerClosed, "store_launch");
		}
		else
		{
			swipeAnimator.Play("SecondThemePurchased");
			StartCoroutine(StartLoadingTheme(2));
		}
	}

	public void BackToThemesBtnClicked()
	{
		m_ThemeIsExpanded = false;
		swipeAnimator.Play("SecondThemeShrink");
	}

	public void OnIAPManagerClosed()
	{
		if (m_SelectedThemeToBuy != 2)
		{
			return;
		}
		if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
		{
			AdjustBuyButtons(2);
			return;
		}
		swipeAnimator.Play("SecondThemePurchased");
		StartCoroutine(StartLoadingTheme(2));
	}
}
