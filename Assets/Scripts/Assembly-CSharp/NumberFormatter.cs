public static class NumberFormatter
{
	public static string FormatToLocale(int intNumber)
	{
		return string.Format(LocalizationManager.Instance.GetCultureInfo(), "{0:n0}", intNumber);
	}

	public static string FormatToLocale(long longNumber)
	{
		return string.Format(LocalizationManager.Instance.GetCultureInfo(), "{0:n0}", longNumber);
	}
}
