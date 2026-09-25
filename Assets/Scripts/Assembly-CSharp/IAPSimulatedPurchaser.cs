using System.Collections;
using UnityEngine;

public class IAPSimulatedPurchaser : MonoBehaviour
{
	public bool isAndroidDevice;

	public bool initializationSuccess;

	public bool initalizationFailed;

	public bool userDisabledIAP;

	public bool listenForOnDeferred;

	public bool productAvailableForPurchase;

	public bool receiptIsValid;

	public bool purchaseFailed;

	public SimulatedPurchaseFailedReasons failedReasons;

	public bool purchaseSuccess;

	public bool purchaseDeferred;

	public bool restorePurchaseSuccess;

	public bool purchasesRestored;

	public bool restorationSupported;

	protected bool m_IsInitialized;

	protected bool m_InitializationFailed;

	protected const string c_PRODUCT_ID_THEME_FACTORY = "eu.mentalhome.feer.theme_factory";

	public void Init()
	{
	}

	private void Update()
	{
	}

	private IEnumerator InitializePurchasing()
	{
		return null;
	}

	private void RetryInitialization()
	{
	}

	public void BuyProductID(string productId)
	{
	}

	private IEnumerator Purchasing(string productId)
	{
		return null;
	}

	public void OnDeferred(string productId)
	{
	}

	public void OnPurchaseFailed(string productId)
	{
	}

	public void ProcessPurchase(string productId)
	{
	}

	public void RestorePurchases()
	{
	}

	private IEnumerator WaitToBuy(string productId)
	{
		return null;
	}

	private IEnumerator WaitToRestorePurchases()
	{
		return null;
	}

	private IEnumerator InformToEnableTalkback(string productId)
	{
		return null;
	}

	public void BuyThemeFactory()
	{
	}

	public void StartRestorePurchases()
	{
	}
}
