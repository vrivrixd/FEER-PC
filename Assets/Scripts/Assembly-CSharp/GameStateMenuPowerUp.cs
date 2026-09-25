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

	protected int m_currentPanel;

	protected const int c_PANEL_POWERUPS = 2;

	protected const int c_PANEL_MAPS = 3;

	protected const int c_FACTORY_PURCHASED = 1;

	protected const int c_FACTORY_WAITING_FOR_APPROVAL = 2;

	protected const int c_FACTORY_BUY = 3;

	protected int m_FactoryStatus;

	protected bool m_ThemePreviewActive;

	protected Theme m_SelectedThemeToBuy;

	public override void Enter(GameState from)
	{
	}

	public override GameStateName GetName()
	{
		return GameStateName.None;
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
	}

	public override void Exit(GameState to)
	{
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	protected void InitUI()
	{
	}

	protected void UpdateUI()
	{
	}

	protected void InitBoostUI()
	{
	}

	protected void InitWeaponUI()
	{
	}

	protected void InitShieldUI()
	{
	}

	protected void InitLightDoublerUI()
	{
	}

	protected void UpdateBoostBtn()
	{
	}

	protected void UpdateWeaponBtn()
	{
	}

	protected void UpdateShieldBtn()
	{
	}

	protected void UpdateLightDoublerBtn()
	{
	}

	public void UpgradeBoostBtnClicked()
	{
	}

	public void UpgradeShieldBtnClicked()
	{
	}

	public void UpgradeLightDoublerBtnClicked()
	{
	}

	public void UpgradeWeaponBtnClicked()
	{
	}

	private IEnumerator AnimateUpgrade(float finalProgress, Image btnImage, int minLevel, int maxLevel, Text lvlText, UAP_BaseElement accesibleText, string accessiblePowerUp, Text upgradeText, Text upgradeCosts, UAP_BaseElement accessibleBtn, string upgradeDesc, int upgradePay, Button btn)
	{
		return null;
	}

	private IEnumerator AnimateUpgradeNoUAP(float finalProgress, Image btnImage, int minLevel, int maxLevel, Text lvlText, string accessiblePowerUp, Text upgradeText, Text upgradeCosts, string upgradeDesc, int upgradePay, Button btn)
	{
		return null;
	}

	public void ThemeForestBtnClicked()
	{
	}

	public void ThemeFactoryBtnClicked()
	{
	}

	public void RestorePurchaseBtnClicked()
	{
	}

	protected void AdjustThemeBuyButtons(Theme theme)
	{
	}

	public void BuyFactoryThemeClicked()
	{
	}

	public void BackToThemesBtnClicked()
	{
	}

	public void OnIAPManagerClosed()
	{
	}

	public void OnRestorePurchaseFinished()
	{
	}

	public void MenuBtnClicked()
	{
	}

	public void NavMapsBtnClicked()
	{
	}

	public void NavPowerUpsBtnClicked()
	{
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		return null;
	}
}
