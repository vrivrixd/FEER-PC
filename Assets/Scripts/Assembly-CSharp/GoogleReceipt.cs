using System;

[Serializable]
public class GoogleReceipt : StoreSpecificReceipt
{
	public string packageName;

	public string purchaseState;

	public string purchaseToken;
}
