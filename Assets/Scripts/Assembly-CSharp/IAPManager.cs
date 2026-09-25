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

	// PORT: compras removidas. O tema Factory ja vem liberado (DataManager.LoadPlayerThemeData);
	// o gerenciador fica inicializado mas sem loja.
	public bool initializationFailed
	{
		get
		{
			return m_InitializationFailed;
		}
		set
		{
			m_InitializationFailed = value;
		}
	}

	public bool iapDisabledByUser
	{
		get
		{
			return m_IAPDisabledByUser;
		}
		set
		{
			m_IAPDisabledByUser = value;
		}
	}

	public static IAPManager Instance => instance;

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
		if (DataManager.Instance.gameMode == GameMode.Presentation || m_Initialized)
		{
			return;
		}
		m_UseSimulatedPurchaser = false;
		m_Initialized = true;
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
		callbackFunction?.Invoke();
	}

	public void StartRestorePurchases(Action callbackFunction)
	{
		callbackFunction?.Invoke();
	}

	public string GetThemeFactoryProductId()
	{
		return "eu.mentalhome.feer.theme_factory";
	}

	public void InfoPanelBtnClicked()
	{
		if (m_CallbackPurchaseFunction != null)
		{
			m_CallbackPurchaseFunction();
			m_CallbackPurchaseFunction = null;
		}
		if (loadingWheel.activeSelf)
		{
			loadingWheel.SetActive(false);
		}
		if (infoPanel.activeSelf)
		{
			infoPanel.SetActive(false);
		}
		if (iapCanvas.activeSelf)
		{
			iapCanvas.SetActive(false);
		}
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
}
