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
		scoreCoinsAnim.Play("ScoreCoinsSlideIn");
	}

	public void SlideOut()
	{
		scoreCoinsAnim.Play("ScoreCoinsSlideOut");
	}

	public void Show(RectTransform parent, bool animated, bool enableAccessibility, int priority, bool updateScoreCoins = true, GameStateName stateName = GameStateName.None)
	{
		if (!m_Init)
		{
			Init();
		}
		if (updateScoreCoins)
		{
			UpdateScoreCoins();
		}
		m_Transform.SetParent(parent, false);
		m_RectTransform.offsetMax = Vector2.zero;
		m_RectTransform.offsetMin = Vector2.zero;
		if (animated)
		{
			m_RectTransform.anchorMin = new Vector2(1f, 0.8f);
			m_RectTransform.anchorMax = new Vector2(1.5f, 1f);
		}
		else
		{
			m_RectTransform.anchorMin = new Vector2(0.5f, 0.8f);
			m_RectTransform.anchorMax = new Vector2(1f, 1f);
		}
		if (enableAccessibility)
		{
			m_AccessibleRoot.m_Priority = priority;
			m_AccessibleRoot.enabled = true;
		}
		else
		{
			m_AccessibleRoot.enabled = false;
		}
		// GameOver (1) ou MenuLeaderboard (5): cor de recorde; Perfil (12): cor do perfil
		if (stateName == GameStateName.GameOver || stateName == GameStateName.MenuLeaderboard)
		{
			scoreText.color = highscoreColor;
		}
		else if (stateName == GameStateName.MenuProfile)
		{
			scoreText.color = profileColor;
		}
		else
		{
			scoreText.color = defaultColor;
		}
		gameObject.SetActive(true);
		if (animated)
		{
			scoreCoinsAnim.Play("ScoreCoinsSlideIn");
		}
	}

	public void Hide()
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
	}

	public void Init()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_Transform = gameObject.transform;
		m_AccessibleRoot = GetComponent<AccessibleUIGroupRoot>();
		accessibleScoreText = scoreText.GetComponent<UAP_BaseElement>();
		accessibleCoinsText = coinsText.GetComponent<UAP_BaseElement>();
		LocalizationManager lm = LocalizationManager.Instance;
		m_localizedScore = lm.GetLocalizedValue("SCORE") + " ";
		m_localizedLights = lm.GetLocalizedValue("LIGHTS") + " ";
		m_voLocalizedYouHave = lm.GetLocalizedValue("You have") + " ";
		m_voLocalizedLight = " " + lm.GetLocalizedValue("LIGHT");
		m_voLocalizedLights = " " + lm.GetLocalizedValue("LIGHTS");
		m_voYourHighscoreIs = lm.GetLocalizedValue("tts_your_highscore_is") + " ";
		m_voLocalizedPointsMultiplied = lm.GetLocalizedValue("tts_points_multiplied") + " ";
		m_voLocalizedLightsMultiplied = lm.GetLocalizedValue("tts_lights_multiplied") + " ";
		m_voLocalizedYouHaveScored = lm.GetLocalizedValue("tts_you_have_scored") + " ";
		m_voLocalizedYouHaveCollected = lm.GetLocalizedValue("tts_you_have_collected") + " ";
		m_voLocalizedLightsThisRun = " " + lm.GetLocalizedValue("tts_lights_this_run");
		m_voLocalizedPointsThisRun = " " + lm.GetLocalizedValue("tts_points_this_run");
		m_Init = true;
	}

	public void UpdateScoreCoins()
	{
		RenderScoreUI(DataManager.Instance.playerData.highscore);
		RenderCoinsUI(DataManager.Instance.playerData.coins);
	}

	private string CoinsAccessibleText(string formattedCoins)
	{
		return m_voLocalizedYouHave + formattedCoins + ((DataManager.Instance.playerData.coins == 1) ? m_voLocalizedLight : m_voLocalizedLights);
	}

	public void UpdateCoins(float duration = 0.5f)
	{
		int coins = DataManager.Instance.playerData.coins;
		if (accessibleCoinsText != null)
		{
			accessibleCoinsText.m_Text = CoinsAccessibleText(NumberFormatter.FormatToLocale(coins));
		}
		if (duration <= 0f || !gameObject.activeSelf)
		{
			m_DisplayedCoins = coins;
			coinsText.text = m_localizedLights + NumberFormatter.FormatToLocale(coins);
		}
		else
		{
			StartCoroutine(AnimateCoins(coins, duration));
		}
	}

	public void SetCoins(int coins, float duration = 0.5f)
	{
		if (accessibleCoinsText != null)
		{
			accessibleCoinsText.m_Text = CoinsAccessibleText(NumberFormatter.FormatToLocale(coins));
		}
		if (gameObject.activeSelf)
		{
			StartCoroutine(AnimateCoins(coins, duration));
		}
	}

	public void SetScore(int score, float duration = 0.5f)
	{
		if (accessibleScoreText != null)
		{
			accessibleScoreText.m_Text = m_voYourHighscoreIs + NumberFormatter.FormatToLocale(score);
		}
		if (gameObject.activeSelf)
		{
			StartCoroutine(AnimateScore(score, duration));
		}
	}

	public void SetScoreColor(Color c)
	{
		scoreText.color = c;
	}

	public void SetDefaultColor()
	{
		scoreText.color = defaultColor;
	}

	public void RenderPlayGameScoreUI(bool updateAccessible = false)
	{
		if (!m_Init)
		{
			Init();
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		if (cgm.scoreMultiplier < 2)
		{
			scoreText.text = m_localizedScore + NumberFormatter.FormatToLocale(cgm.score);
			if (updateAccessible)
			{
				accessibleScoreText.m_Text = m_voLocalizedYouHaveScored + NumberFormatter.FormatToLocale(cgm.score) + m_voLocalizedPointsThisRun;
			}
		}
		else
		{
			scoreText.text = "x" + cgm.scoreMultiplier.ToString() + "  " + m_localizedScore + NumberFormatter.FormatToLocale(cgm.score);
			if (updateAccessible)
			{
				accessibleScoreText.m_Text = m_voLocalizedPointsMultiplied + cgm.scoreMultiplier.ToString() + "\n\n " + m_voLocalizedYouHaveScored + NumberFormatter.FormatToLocale(cgm.score) + m_voLocalizedPointsThisRun;
			}
		}
	}

	public void RenderPlayGameCoinsUI(bool updateAccessible = false)
	{
		if (!m_Init)
		{
			Init();
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		if (cgm.coinMultiplier < 2)
		{
			coinsText.text = m_localizedLights + NumberFormatter.FormatToLocale(cgm.sumCollectedGhosts);
			if (updateAccessible)
			{
				accessibleCoinsText.m_Text = m_voLocalizedYouHaveCollected + NumberFormatter.FormatToLocale(cgm.sumCollectedGhosts) + m_voLocalizedLightsThisRun;
			}
		}
		else
		{
			coinsText.text = "x" + cgm.coinMultiplier.ToString() + "  " + m_localizedLights + NumberFormatter.FormatToLocale(cgm.sumCollectedGhosts);
			if (updateAccessible)
			{
				accessibleCoinsText.m_Text = m_voLocalizedLightsMultiplied + cgm.coinMultiplier.ToString() + "\n\n " + m_voLocalizedYouHaveCollected + NumberFormatter.FormatToLocale(cgm.sumCollectedGhosts) + m_voLocalizedLightsThisRun;
			}
		}
	}

	public void RenderScoreUI(int score)
	{
		string formatted = NumberFormatter.FormatToLocale(score);
		scoreText.text = m_localizedScore + formatted;
		if (accessibleScoreText != null)
		{
			accessibleScoreText.m_Text = m_voYourHighscoreIs + formatted;
		}
		m_DisplayedScore = score;
	}

	public void RenderCoinsUI(int coins)
	{
		string formatted = NumberFormatter.FormatToLocale(coins);
		coinsText.text = m_localizedLights + formatted;
		if (accessibleCoinsText != null)
		{
			accessibleCoinsText.m_Text = CoinsAccessibleText(formatted);
		}
		m_DisplayedCoins = coins;
	}

	private IEnumerator AnimateCoins(int finalCoins, float duration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			coinsText.text = m_localizedLights + NumberFormatter.FormatToLocale((int)Mathf.Lerp(m_DisplayedCoins, finalCoins, elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		m_DisplayedCoins = finalCoins;
		coinsText.text = m_localizedLights + NumberFormatter.FormatToLocale(finalCoins);
	}

	private IEnumerator AnimateScore(int finalScore, float duration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			scoreText.text = m_localizedScore + NumberFormatter.FormatToLocale((int)Mathf.Lerp(m_DisplayedScore, finalScore, elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		m_DisplayedScore = finalScore;
		scoreText.text = m_localizedScore + NumberFormatter.FormatToLocale(finalScore);
	}
}
