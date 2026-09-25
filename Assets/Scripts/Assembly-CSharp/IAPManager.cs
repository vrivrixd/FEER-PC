using System;
using UnityEngine;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
	public GameObject iapCanvas;

	public GameObject loadingWheel;

	public GameObject infoPanel;

	public Text infoPanelTitleTxt;

	public Text infoPanelMessageTxt;

	protected const string c_PRODUCT_ID_THEME_FACTORY = "eu.mentalhome.feer.theme_factory";

	protected Action m_CallbackPurchaseFunction;

	protected bool m_RestoringPurchases;

	protected int m_PurchasesRestored;

	protected bool m_Initialized;

	protected IAPPurchaser m_IAPPurchaser;

	protected IAPSimulatedPurchaser m_IAPSimulatedPurchaser;

	protected bool m_UseSimulatedPurchaser;

	protected bool m_InitializationFailed;

	protected bool m_IAPDisabledByUser;

	private static IAPManager instance;

	public bool initializationFailed
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool iapDisabledByUser
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public static IAPManager Instance => null;

	private void Awake()
	{
	}

	public void Init()
	{
	}

	public void OnPurchaseSuccess()
	{
	}

	public void OnPurchasedProductUnkown()
	{
	}

	public void OnPurchasedProductVerificationFailed()
	{
	}

	public void OnRestorePurchasesStarted()
	{
	}

	public void OnRestorePurchaseSuccess()
	{
	}

	public void OnRestorePurchaseFailed()
	{
	}

	public void OnRestorePurchaseNotSupported()
	{
	}

	public void BuyThemeFactory(Action callbackFunction, string transactionContext)
	{
	}

	public void StartRestorePurchases(Action callbackFunction)
	{
	}

	public string GetThemeFactoryProductId()
	{
		return null;
	}

	public void InfoPanelBtnClicked()
	{
	}

	protected void CloseIAPManager()
	{
	}

	public void ShowIAPDisabledError()
	{
	}

	public void ShowConnectionError()
	{
	}

	public void ShowProductNotAvailableError()
	{
	}

	public void ShowPurchaseFailedError(bool informUser, string errorMessage)
	{
	}

	public void ShowOnProductDeferredMessageWhenActive()
	{
	}

	public void ShowRestorePurchaseIAPDisabledError()
	{
	}

	private void ShowErrorMessage(string errorMessage)
	{
	}

	private void ShowRestoreFailedMessage()
	{
	}

	private void ShowRestoreSuccessMessage(bool restoredPurchases)
	{
	}

	private void ShowRestoreNotSupportedMessage()
	{
	}
}
