using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCoinsPanelUI : MonoBehaviour
{
	public Animation scoreCoinsAnim;

	public Text scoreText;

	public Text coinsText;

	public Color highscoreColor;

	public Color profileColor;

	public Color defaultColor;

	[HideInInspector]
	public UAP_BaseElement accessibleScoreText;

	[HideInInspector]
	public UAP_BaseElement accessibleCoinsText;

	[HideInInspector]
	public bool m_Init;

	protected RectTransform m_RectTransform;

	protected Transform m_Transform;

	protected AccessibleUIGroupRoot m_AccessibleRoot;

	protected int m_DisplayedCoins;

	protected int m_DisplayedScore;

	protected string m_localizedScore;

	protected string m_localizedLights;

	protected string m_voLocalizedYouHave;

	protected string m_voLocalizedLight;

	protected string m_voLocalizedLights;

	protected string m_voYourHighscoreIs;

	protected string m_voLocalizedPointsMultiplied;

	protected string m_voLocalizedLightsMultiplied;

	protected string m_voLocalizedYouHaveCollected;

	protected string m_voLocalizedYouHaveScored;

	protected string m_voLocalizedLightsThisRun;

	protected string m_voLocalizedPointsThisRun;

	public void SlideIn()
	{
	}

	public void SlideOut()
	{
	}

	public void Show(RectTransform parent, bool animated, bool enableAccessibility, int priority, bool updateScoreCoins = true, GameStateName stateName = GameStateName.None)
	{
	}

	public void Hide()
	{
	}

	public void Init()
	{
	}

	public void UpdateScoreCoins()
	{
	}

	public void UpdateCoins(float duration = 0.5f)
	{
	}

	public void SetCoins(int coins, float duration = 0.5f)
	{
	}

	public void SetScore(int score, float duration = 0.5f)
	{
	}

	public void SetScoreColor(Color c)
	{
	}

	public void SetDefaultColor()
	{
	}

	public void RenderPlayGameScoreUI(bool updateAccessible = false)
	{
	}

	public void RenderPlayGameCoinsUI(bool updateAccessible = false)
	{
	}

	public void RenderScoreUI(int score)
	{
	}

	public void RenderCoinsUI(int coins)
	{
	}

	private IEnumerator AnimateCoins(int finalCoins, float duration)
	{
		return null;
	}

	private IEnumerator AnimateScore(int finalScore, float duration)
	{
		return null;
	}
}
