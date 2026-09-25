using System;

[Serializable]
public class PlayerThemeData
{
	public bool themeFactoryPurchased;

	public bool themeFactoryWaitingForApproval;

	public DateTime themeFactoryApprovalStartDate;

	public bool themeFactoryTutorialPlayed;

	public string themeFactoryOriginalReceipt;

	public IAPReceiptData themeFactoryReceiptData;
}
