using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateMenuPowerUp : GameState
{
	public GameObject navPanel;

	public Color navSelectedColor;

	public Text mapsBtn;

	public Text powerUpsBtn;

	public RectTransform panelTransform;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject upgradePanel;

	public GameObject mapsPanel;

	public GameObject accessibleMapsPanelRoot;

	public GameObject accessiblePowerUpsPanelRoot;

	public GameObject previewFactoryPanel;

	public Text buyFactoryBtnText;

	public Button buyFactoryBtn;

	public GameObject restorePurchaseBtn;

	public Button themeForestBtn;

	public Button themeFactoryBtn;

	public Color inactiveColor;

	public Color activeColor;

	public AudioSource powerUpProgress;

	public AudioSource powerUpUpgraded;

	public Text boostLevel;

	public Text boostUpgradeText;

	public Text boostUpgradeCost;

	public Image boostBtnImage;

	public Button boostBtn;

	public UAP_BaseElement accessibleBoost;

	protected UAP_BaseElement accessibleBoostBtn;

	public Text shieldLevel;

	public Text shieldUpgradeText;

	public Text shieldUpgradeCost;

	public Image shieldBtnImage;

	public Button shieldBtn;

	public UAP_BaseElement accessibleShield;

	protected UAP_BaseElement accessibleShieldBtn;

	public Text lightDoublerLevel;

	public Text lightDoublerUpgradeText;

	public Text lightDoublerUpgradeCost;

	public Image lightDoublerBtnImage;

	public Button lightDoublerBtn;

	public UAP_BaseElement accessibleLightDoubler;

	protected UAP_BaseElement accessibleLightDoublerBtn;

	public Text weaponLevel;

	public Text weaponUpgradeText;

	public Text weaponUpgradeCost;

	public Image weaponBtnImage;

	public Button weaponBtn;

	public UAP_BaseElement accessibleWeapon;

	protected UAP_BaseElement accessibleWeaponBtn;

	protected string m_ttsOF;

	protected string m_ttsUpgrade;

	protected string m_ttsFor;

	protected string m_ttsLights;

	protected string m_upgradeText;

	protected bool m_Init;

	protected string m_strPanelAnnouncement;

	protected int m_currentPanel = -1;

	protected const int c_PANEL_POWERUPS = 2;

	protected const int c_PANEL_MAPS = 3;

	protected const int c_FACTORY_PURCHASED = 1;

	protected const int c_FACTORY_WAITING_FOR_APPROVAL = 2;

	protected const int c_FACTORY_BUY = 3;

	protected int m_FactoryStatus = -1;

	protected bool m_ThemePreviewActive;

	protected Theme m_SelectedThemeToBuy;

	// Data for each power-up in the store (in the original the four variants are identical duplicated code)
	private class PowerUpUI
	{
		public int index;
		public Func<int> level;
		public Func<bool> increase;
		public Text levelText;
		public Text upgradeText;
		public Text upgradeCost;
		public Image btnImage;
		public Button btn;
		public UAP_BaseElement accessible;
		public UAP_BaseElement accessibleBtn;
		public string nameKey;
		public string descKey;
	}

	private PowerUpUI m_Boost;

	private PowerUpUI m_Shield;

	private PowerUpUI m_LightDoubler;

	private PowerUpUI m_Weapon;

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	public override void Enter(GameState from)
	{
		m_ThemePreviewActive = false;
		gameObject.SetActive(true);
		powerUpProgress.gameObject.SetActive(true);
		powerUpUpgraded.gameObject.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (m_strPanelAnnouncement == null)
			{
				m_strPanelAnnouncement = L("INVENTORY");
			}
			UAP_AccessibilityManager.Say(m_strPanelAnnouncement, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		}
		if (!m_Init)
		{
			InitUI();
		}
		else
		{
			UpdateUI();
		}
		if (from.GetName() == GameStateName.Init || m_currentPanel == -1 || m_currentPanel == 3)
		{
			if (from.GetName() == GameStateName.Init || m_currentPanel == -1)
			{
				m_currentPanel = 3;
			}
			mapsPanel.SetActive(true);
			mapsBtn.color = navSelectedColor;
			powerUpsBtn.color = Color.white;
		}
		else if (m_currentPanel == 2)
		{
			upgradePanel.SetActive(true);
			mapsBtn.color = Color.white;
			powerUpsBtn.color = navSelectedColor;
		}
		navPanel.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, true, GameStateName.None);
	}

	protected void InitUI()
	{
		m_ttsOF = " " + L("tts_of") + " ";
		m_ttsUpgrade = L("tts_upgrade") + " ";
		m_ttsLights = " " + L("lights");
		m_ttsFor = " " + L("tts_skip_quest_2") + " ";
		m_upgradeText = L("UPGRADE");
		accessibleBoostBtn = boostBtn.GetComponent<UAP_BaseElement>();
		accessibleShieldBtn = shieldBtn.GetComponent<UAP_BaseElement>();
		accessibleLightDoublerBtn = lightDoublerBtn.GetComponent<UAP_BaseElement>();
		accessibleWeaponBtn = weaponBtn.GetComponent<UAP_BaseElement>();
		CreatePowerUpUIs();
		InitBoostUI();
		InitShieldUI();
		InitLightDoublerUI();
		InitWeaponUI();
		restorePurchaseBtn.SetActive(false);
		Theme theme = DataManager.Instance.selectedTheme;
		if (theme == Theme.Factory)
		{
			themeFactoryBtn.interactable = false;
		}
		else if (theme == Theme.Forest)
		{
			themeForestBtn.interactable = false;
		}
		else
		{
			return;
		}
		m_Init = true;
	}

	private void CreatePowerUpUIs()
	{
		DataManager dm = DataManager.Instance;
		m_Boost = new PowerUpUI
		{
			index = dm.BOOST,
			level = () => DataManager.Instance.playerData.boostLevel,
			increase = () => DataManager.Instance.IncreaseBoostLevel(),
			levelText = boostLevel,
			upgradeText = boostUpgradeText,
			upgradeCost = boostUpgradeCost,
			btnImage = boostBtnImage,
			btn = boostBtn,
			accessible = accessibleBoost,
			accessibleBtn = accessibleBoostBtn,
			nameKey = "tts_BOOST",
			descKey = "increase your boost time"
		};
		m_Shield = new PowerUpUI
		{
			index = dm.SHIELD,
			level = () => DataManager.Instance.playerData.shieldLevel,
			increase = () => DataManager.Instance.IncreaseShieldLevel(),
			levelText = shieldLevel,
			upgradeText = shieldUpgradeText,
			upgradeCost = shieldUpgradeCost,
			btnImage = shieldBtnImage,
			btn = shieldBtn,
			accessible = accessibleShield,
			accessibleBtn = accessibleShieldBtn,
			nameKey = "SHIELD",
			descKey = "be protected for a longer time"
		};
		m_LightDoubler = new PowerUpUI
		{
			index = dm.COIN_MULTIPLIER,
			level = () => DataManager.Instance.playerData.coinMultiplierLevel,
			increase = () => DataManager.Instance.IncreaseCoinMultiplierLevel(),
			levelText = lightDoublerLevel,
			upgradeText = lightDoublerUpgradeText,
			upgradeCost = lightDoublerUpgradeCost,
			btnImage = lightDoublerBtnImage,
			btn = lightDoublerBtn,
			accessible = accessibleLightDoubler,
			accessibleBtn = accessibleLightDoublerBtn,
			nameKey = "tts_LightDoubler",
			descKey = "get more time for collecting"
		};
		m_Weapon = new PowerUpUI
		{
			index = dm.WEAPON,
			level = () => DataManager.Instance.playerData.weaponLevel,
			increase = () => DataManager.Instance.IncreaseWeaponLevel(),
			levelText = weaponLevel,
			upgradeText = weaponUpgradeText,
			upgradeCost = weaponUpgradeCost,
			btnImage = weaponBtnImage,
			btn = weaponBtn,
			accessible = accessibleWeapon,
			accessibleBtn = accessibleWeaponBtn,
			nameKey = "tts_WEAPON",
			descKey = "shooting power will last longer"
		};
	}

	private static PowerUpLevel[] Levels(PowerUpUI p)
	{
		return DataManager.Instance.powerUps[p.index].powerUpLevels;
	}

	private void SetAffordable(PowerUpUI p, bool affordable)
	{
		Color color = affordable ? activeColor : inactiveColor;
		p.upgradeText.color = color;
		p.upgradeCost.color = color;
	}

	private void InitPowerUpUI(PowerUpUI p)
	{
		int level = p.level();
		int maxLevel = Levels(p).Length;
		p.levelText.text = level.ToString() + "/" + maxLevel.ToString();
		if (level == maxLevel)
		{
			p.accessible.m_Text = L(p.nameKey) + ",\n Level " + level.ToString() + m_ttsOF + maxLevel.ToString();
			p.btnImage.fillAmount = 1f;
			p.upgradeText.text = "";
			p.upgradeCost.text = "";
			p.btn.interactable = false;
			p.accessibleBtn.m_CustomHint = true;
			p.accessibleBtn.m_Text = L("tts_max_level");
			return;
		}
		p.accessible.m_Text = L(p.nameKey) + ",\n Level " + level.ToString() + m_ttsOF + maxLevel.ToString() + ",\n " + L(p.descKey);
		p.btnImage.fillAmount = (float)level / (float)maxLevel;
		int coinsToPay = Levels(p)[level].coinsToPay;
		bool affordable = DataManager.Instance.playerData.coins >= coinsToPay;
		SetAffordable(p, affordable);
		p.btn.interactable = affordable;
		p.accessibleBtn.m_CustomHint = !affordable;
		p.upgradeCost.text = NumberFormatter.FormatToLocale(coinsToPay);
		p.accessibleBtn.m_Text = m_ttsUpgrade + L(p.nameKey) + m_ttsFor + NumberFormatter.FormatToLocale(coinsToPay) + m_ttsLights;
	}

	private void UpdatePowerUpAffordability(PowerUpUI p)
	{
		int level = p.level();
		if (level == Levels(p).Length)
		{
			return;
		}
		bool affordable = DataManager.Instance.playerData.coins >= Levels(p)[level].coinsToPay;
		SetAffordable(p, affordable);
		p.btn.interactable = affordable;
		p.accessibleBtn.m_CustomHint = !affordable;
	}

	// After a purchase: disables the buttons that became too expensive
	private void UpdatePowerUpBtn(PowerUpUI p)
	{
		if (!p.btn.interactable)
		{
			return;
		}
		if (Levels(p)[p.level()].coinsToPay <= DataManager.Instance.playerData.coins)
		{
			return;
		}
		SetAffordable(p, false);
		p.btn.interactable = false;
		p.accessibleBtn.m_CustomHint = true;
	}

	private void UpgradeClicked(PowerUpUI p, PowerUpUI other1, PowerUpUI other2, PowerUpUI other3)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		p.btn.interactable = false;
		p.upgradeText.text = "";
		p.upgradeCost.text = "";
		p.increase();
		scoreCoinsPanel.UpdateCoins(0.5f);
		UpdatePowerUpBtn(other1);
		UpdatePowerUpBtn(other2);
		UpdatePowerUpBtn(other3);
		int minLevel = p.level();
		int maxLevel = Levels(p).Length;
		float finalProgress;
		int upgradePay;
		if (minLevel == maxLevel)
		{
			finalProgress = 1f;
			upgradePay = -1;
		}
		else
		{
			upgradePay = Levels(p)[minLevel].coinsToPay;
			finalProgress = (float)minLevel / (float)maxLevel;
		}
		StartCoroutine(AnimateUpgrade(finalProgress, p.btnImage, minLevel, maxLevel, p.levelText, p.accessible, p.nameKey, p.upgradeText, p.upgradeCost, p.accessibleBtn, p.descKey, upgradePay, p.btn));
	}

	protected void UpdateUI()
	{
		UpdatePowerUpAffordability(m_Boost);
		UpdatePowerUpAffordability(m_Shield);
		UpdatePowerUpAffordability(m_LightDoubler);
		UpdatePowerUpAffordability(m_Weapon);
	}

	public override GameStateName GetName()
	{
		return GameStateName.MenuUpgrades;
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override void Tick()
	{
		if (m_ThemePreviewActive && m_currentPanel == 3)
		{
			AdjustThemeBuyButtons(Theme.Factory);
		}
	}

	protected void AdjustThemeBuyButtons(Theme theme)
	{
		if (theme != Theme.Factory)
		{
			return;
		}
		PlayerThemeData themeData = DataManager.Instance.playerThemeData;
		if (themeData.themeFactoryPurchased)
		{
			if (m_FactoryStatus != 1)
			{
				buyFactoryBtn.interactable = true;
				buyFactoryBtnText.text = L("PLAY");
				m_FactoryStatus = 1;
			}
		}
		else if (themeData.themeFactoryWaitingForApproval)
		{
			if (m_FactoryStatus != 2)
			{
				buyFactoryBtn.interactable = false;
				buyFactoryBtnText.text = L("WAITING FOR APPROVAL");
				m_FactoryStatus = 2;
			}
		}
		else if (m_FactoryStatus != 3)
		{
			buyFactoryBtn.interactable = true;
			buyFactoryBtnText.text = L("BUY NOW");
			m_FactoryStatus = 3;
		}
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
		scoreCoinsPanel.Hide();
		navPanel.SetActive(false);
		upgradePanel.SetActive(false);
		mapsPanel.SetActive(false);
		previewFactoryPanel.SetActive(false);
		powerUpProgress.gameObject.SetActive(false);
		powerUpUpgraded.gameObject.SetActive(false);
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		if (m_currentPanel == 2)
		{
			upgradePanel.GetComponent<Animation>().Play("UpgradePanelSlideOut");
		}
		else if (m_currentPanel == 3)
		{
			mapsPanel.GetComponent<Animation>().Play("SmallPanelSlideOut");
		}
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		CustomGameManager.Instance.SwitchState(toState);
	}

	protected void InitBoostUI()
	{
		InitPowerUpUI(m_Boost);
	}

	protected void InitShieldUI()
	{
		InitPowerUpUI(m_Shield);
	}

	protected void InitLightDoublerUI()
	{
		InitPowerUpUI(m_LightDoubler);
	}

	protected void InitWeaponUI()
	{
		InitPowerUpUI(m_Weapon);
	}

	protected void UpdateBoostBtn()
	{
		UpdatePowerUpBtn(m_Boost);
	}

	protected void UpdateWeaponBtn()
	{
		UpdatePowerUpBtn(m_Weapon);
	}

	protected void UpdateShieldBtn()
	{
		UpdatePowerUpBtn(m_Shield);
	}

	protected void UpdateLightDoublerBtn()
	{
		UpdatePowerUpBtn(m_LightDoubler);
	}

	public void UpgradeBoostBtnClicked()
	{
		UpgradeClicked(m_Boost, m_Shield, m_LightDoubler, m_Weapon);
	}

	private IEnumerator AnimateUpgrade(float finalProgress, Image btnImage, int minLevel, int maxLevel, Text lvlText, UAP_BaseElement accesibleText, string accessiblePowerUp, Text upgradeText, Text upgradeCosts, UAP_BaseElement accessibleBtn, string upgradeDesc, int upgradePay, Button btn)
	{
		float elapsedTime = 0f;
		float startingFillAmount = btnImage.fillAmount;
		float pitchStartValue = 0.8f;
		powerUpProgress.pitch = 0.8f;
		powerUpProgress.Play();
		float finalPitch = pitchStartValue + (finalProgress - startingFillAmount) * (1.2f - pitchStartValue);
		float duration = (finalProgress - startingFillAmount) * 0.8f;
		while (elapsedTime < duration)
		{
			btnImage.fillAmount = Mathf.Lerp(startingFillAmount, finalProgress, elapsedTime / duration);
			powerUpProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		btnImage.fillAmount = finalProgress;
		powerUpProgress.Stop();
		powerUpUpgraded.Play();
		while (powerUpUpgraded.isPlaying)
		{
			yield return null;
		}
		lvlText.text = minLevel.ToString() + "/" + maxLevel.ToString();
		bool activateBtn = false;
		if (minLevel == maxLevel)
		{
			accesibleText.m_Text = L(accessiblePowerUp) + ",\n Level " + minLevel.ToString() + m_ttsOF + maxLevel.ToString();
			accessibleBtn.m_CustomHint = true;
			accessibleBtn.m_Text = L("tts_max_level");
		}
		else
		{
			accesibleText.m_Text = L(accessiblePowerUp) + ",\n Level " + minLevel.ToString() + m_ttsOF + maxLevel.ToString() + ",\n " + L("tts_upgrade_to") + " " + L(upgradeDesc);
			if (DataManager.Instance.playerData.coins < upgradePay)
			{
				upgradeText.color = inactiveColor;
				upgradeCosts.color = inactiveColor;
				accessibleBtn.m_CustomHint = true;
			}
			else
			{
				upgradeText.color = activeColor;
				upgradeCosts.color = activeColor;
				accessibleBtn.m_CustomHint = false;
				activateBtn = true;
			}
			upgradeText.text = m_upgradeText;
			upgradeCosts.text = NumberFormatter.FormatToLocale(upgradePay);
			accessibleBtn.m_Text = m_ttsUpgrade + L(accessiblePowerUp) + m_ttsFor + NumberFormatter.FormatToLocale(upgradePay) + m_ttsLights;
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.Say(L(accessiblePowerUp) + " " + L("tts_upgraded to Level") + " " + minLevel.ToString(), true, true, (UAP_AudioQueue.EInterrupt)0x4f);
			bool startedSpeaking = false;
			float startedSpeakingTimeOut = 0f;
			while (!startedSpeaking)
			{
				if (UAP_AccessibilityManager.IsSpeaking())
				{
					startedSpeaking = true;
				}
				startedSpeakingTimeOut += Time.deltaTime;
				if (1.5f < startedSpeakingTimeOut)
				{
					startedSpeaking = true;
				}
				yield return null;
			}
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
		}
		if (activateBtn)
		{
			btn.interactable = true;
		}
		UAP_AccessibilityManager.BlockInput(false, true);
	}

	public void UpgradeShieldBtnClicked()
	{
		UpgradeClicked(m_Shield, m_Boost, m_LightDoubler, m_Weapon);
	}

	public void UpgradeLightDoublerBtnClicked()
	{
		UpgradeClicked(m_LightDoubler, m_Boost, m_Shield, m_Weapon);
	}

	public void UpgradeWeaponBtnClicked()
	{
		UpgradeClicked(m_Weapon, m_Boost, m_Shield, m_LightDoubler);
	}

	private IEnumerator AnimateUpgradeNoUAP(float finalProgress, Image btnImage, int minLevel, int maxLevel, Text lvlText, string accessiblePowerUp, Text upgradeText, Text upgradeCosts, string upgradeDesc, int upgradePay, Button btn)
	{
		float elapsedTime = 0f;
		float startingFillAmount = btnImage.fillAmount;
		float pitchStartValue = 0.8f;
		powerUpProgress.pitch = 0.8f;
		powerUpProgress.Play();
		float finalPitch = pitchStartValue + (finalProgress - startingFillAmount) * (1.2f - pitchStartValue);
		float duration = (finalProgress - startingFillAmount) * 0.8f;
		while (elapsedTime < duration)
		{
			btnImage.fillAmount = Mathf.Lerp(startingFillAmount, finalProgress, elapsedTime / duration);
			powerUpProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		btnImage.fillAmount = finalProgress;
		powerUpProgress.Stop();
		powerUpUpgraded.Play();
		while (powerUpUpgraded.isPlaying)
		{
			yield return null;
		}
		lvlText.text = minLevel.ToString() + "/" + maxLevel.ToString();
		if (minLevel == maxLevel)
		{
			yield break;
		}
		bool affordable = DataManager.Instance.playerData.coins >= upgradePay;
		upgradeText.color = affordable ? activeColor : inactiveColor;
		upgradeCosts.color = affordable ? activeColor : inactiveColor;
		upgradeText.text = m_upgradeText;
		upgradeCosts.text = NumberFormatter.FormatToLocale(upgradePay);
		if (affordable)
		{
			btn.interactable = true;
		}
	}

	public void ThemeForestBtnClicked()
	{
		if (DataManager.Instance.selectedTheme != Theme.Forest)
		{
			FeerSceneManager.Instance.ChangeTheme(Theme.Forest);
		}
	}

	public void ThemeFactoryBtnClicked()
	{
		if (DataManager.Instance.selectedTheme == Theme.Factory)
		{
			return;
		}
		if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
		{
			AdjustThemeBuyButtons(Theme.Factory);
			// PORT: CustomAnalyticsTracker.StoreItemClicked removed.
			m_ThemePreviewActive = true;
			previewFactoryPanel.SetActive(true);
		}
		else
		{
			FeerSceneManager.Instance.ChangeTheme(Theme.Factory);
		}
	}

	public void RestorePurchaseBtnClicked()
	{
		IAPManager.Instance.StartRestorePurchases(OnRestorePurchaseFinished);
	}

	public void BuyFactoryThemeClicked()
	{
		if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
		{
			m_SelectedThemeToBuy = Theme.Factory;
			IAPManager.Instance.BuyThemeFactory(OnIAPManagerClosed, "store_in_game");
		}
		else
		{
			FeerSceneManager.Instance.ChangeTheme(Theme.Factory);
		}
	}

	public void BackToThemesBtnClicked()
	{
		m_ThemePreviewActive = false;
		previewFactoryPanel.SetActive(false);
	}

	public void OnIAPManagerClosed()
	{
		if (m_SelectedThemeToBuy != Theme.Factory)
		{
			return;
		}
		if (!DataManager.Instance.playerThemeData.themeFactoryPurchased)
		{
			AdjustThemeBuyButtons(Theme.Factory);
			return;
		}
		FeerSceneManager.Instance.ChangeTheme(Theme.Factory);
	}

	public void OnRestorePurchaseFinished()
	{
	}

	public override bool PortBack()
	{
		if (!gameObject.activeSelf)
		{
			return false;
		}
		if (previewFactoryPanel.activeInHierarchy)
		{
			BackToThemesBtnClicked();
		}
		else
		{
			MenuBtnClicked();
		}
		return true;
	}

	public void MenuBtnClicked()
	{
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.Menu));
		}
	}

	public void NavMapsBtnClicked()
	{
		if (m_currentPanel == 3)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessibleMapsPanelRoot, true);
			}
			return;
		}
		StartCoroutine(SwitchPanel(3));
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		if (m_currentPanel == 3)
		{
			mapsPanel.GetComponent<Animation>().Play("SmallPanelSlideOut");
		}
		else if (m_currentPanel == 2)
		{
			upgradePanel.GetComponent<Animation>().Play("UpgradePanelSlideOut");
		}
		if (toPanel == 2)
		{
			upgradePanel.SetActive(true);
			powerUpsBtn.color = navSelectedColor;
		}
		else if (toPanel == 3)
		{
			mapsPanel.SetActive(true);
			mapsBtn.color = navSelectedColor;
		}
		if (m_currentPanel == 2)
		{
			powerUpsBtn.color = Color.white;
		}
		else if (m_currentPanel == 3)
		{
			mapsBtn.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		if (m_currentPanel == 3)
		{
			mapsPanel.SetActive(false);
		}
		else if (m_currentPanel == 2)
		{
			upgradePanel.SetActive(false);
		}
		m_currentPanel = toPanel;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false, true);
		}
	}

	public void NavPowerUpsBtnClicked()
	{
		if (m_currentPanel == 2)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessiblePowerUpsPanelRoot, true);
			}
			return;
		}
		StartCoroutine(SwitchPanel(2));
	}
}
