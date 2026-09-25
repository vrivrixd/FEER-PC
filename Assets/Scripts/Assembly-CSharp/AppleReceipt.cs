using System;

[Serializable]
public class AppleReceipt : StoreSpecificReceipt
{
	public DateTime originalPurchaseDate;

	public string originalTransactionIdentifier;

	public int productType;

	public int quantity;
}
