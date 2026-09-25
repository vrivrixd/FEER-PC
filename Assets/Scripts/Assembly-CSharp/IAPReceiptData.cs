using System;

[Serializable]
public class IAPReceiptData
{
	public string purchasedProductID;

	public string purchasedProductTransactionID;

	public string receiptProductID;

	public string receiptTransactionID;

	public DateTime receiptPurchaseDate;

	public StoreSpecificReceipt storeSpecificReceiptData;
}
