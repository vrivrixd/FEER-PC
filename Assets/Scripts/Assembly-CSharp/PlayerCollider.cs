using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerCollider : MonoBehaviour
{
	public MainCamera mainCamera;

	public BloodOnLens bloodOnLens;

	public AudioSource atmoBassBeat;

	public PlayerInputController inputController;

	public GameObject shieldObj;

	public PlayerStumbleCollider stumbleCollider;

	public AudioSource audioCoinCollected;

	public AudioSource audioPowerUpBoost;

	public AudioSource audioPowerUpCoinDoubler;

	public AudioSource audioPowerUpShield;

	public AudioSource audioPowerUpShieldImpact;

	public AudioSource audioDeath;

	protected BoxCollider m_Collider;

	protected float m_StartingColliderHeight;

	protected readonly Vector3 k_SlidingColliderScale = new Vector3(1f, 0.5f, 1f);

	protected readonly Vector3 k_JumpingColliderScale = new Vector3(1f, 0.5f, 1f);

	protected readonly Vector3 k_DefaultColliderScale = new Vector3(1f, 2f, 1f);

	protected const int k_MonsterLayerIndex = 8;

	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	protected const int k_CollectiblesLayerIndex = 13;

	protected const int k_ConsumablesLayerIndex = 14;

	protected const int k_HoleLayerIndex = 16;

	protected const int k_FootstepSoundLayerIndex = 17;

	protected const int k_HeightChangeLayerIndex = 18;

	protected bool m_IsBoost;

	protected bool m_IsCoinMultiplier;

	protected bool m_IsShield;

	protected bool m_IsInvincible;

	protected bool m_isPaused;

	protected int m_ShieldActivationCount;

	protected bool m_ShieldIncreasing;

	protected bool m_ShieldDecreasing;

	protected Coroutine m_ShieldRoutine;

	protected int m_SpecialFootstepAreaCounter;

	public bool isBoost => m_IsBoost;

	public bool isShield => m_IsShield;

	private void Start()
	{
		m_Collider = GetComponent<BoxCollider>();
		m_StartingColliderHeight = m_Collider.bounds.size.y;
		shieldObj.SetActive(false);
	}

	public void StopPlaying()
	{
		PauseAllAudio(true);
		m_isPaused = true;
	}

	protected void PauseAllAudio(bool paused)
	{
		if (paused)
		{
			atmoBassBeat.Pause();
			audioCoinCollected.Pause();
			audioPowerUpBoost.Pause();
			audioPowerUpCoinDoubler.Pause();
			audioPowerUpShield.Pause();
			audioPowerUpShieldImpact.Pause();
			audioDeath.Pause();
		}
		else
		{
			atmoBassBeat.UnPause();
			audioCoinCollected.UnPause();
			audioPowerUpBoost.UnPause();
			audioPowerUpCoinDoubler.UnPause();
			audioPowerUpShield.UnPause();
			audioPowerUpShieldImpact.UnPause();
			audioDeath.UnPause();
		}
	}

	protected void StopAllAudio()
	{
		atmoBassBeat.Stop();
		audioCoinCollected.Stop();
		audioPowerUpBoost.Stop();
		audioPowerUpCoinDoubler.Stop();
		audioPowerUpShield.Stop();
		audioPowerUpShieldImpact.Stop();
		audioDeath.Stop();
	}

	public void ResumePlaying()
	{
		PauseAllAudio(false);
		m_isPaused = false;
	}

	public void ReviveGame()
	{
		m_SpecialFootstepAreaCounter = 0;
		PauseAllAudio(false);
		m_isPaused = false;
	}

	public void QuitPlaying()
	{
		StopAllCoroutines();
		StopAllAudio();
		m_IsBoost = false;
		m_IsCoinMultiplier = false;
		m_IsInvincible = false;
		shieldObj.SetActive(false);
		m_isPaused = false;
		stumbleCollider.ResetVariables();
		m_SpecialFootstepAreaCounter = 0;
	}

	public void StopInvincible()
	{
		m_IsInvincible = false;
	}

	public void StartPowerUpSound(ConsumableType consumableType)
	{
		switch (consumableType)
		{
		case ConsumableType.Shield:
			if (!shieldObj.activeSelf)
			{
				shieldObj.SetActive(true);
			}
			audioPowerUpShield.pitch = 0.2f;
			audioPowerUpShield.Play();
			break;
		case ConsumableType.CoinMultiplier:
			audioPowerUpCoinDoubler.pitch = 1f;
			audioPowerUpCoinDoubler.Play();
			break;
		case ConsumableType.Boost:
			atmoBassBeat.volume = 0.5f;
			atmoBassBeat.pitch = 1f;
			atmoBassBeat.Play();
			audioPowerUpBoost.pitch = 0.7f;
			audioPowerUpBoost.Play();
			break;
		}
	}

	public void SetBoost(bool boost)
	{
		if (m_IsBoost == boost)
		{
			return;
		}
		m_IsBoost = boost;
		if (!boost)
		{
			if (audioPowerUpBoost.isPlaying)
			{
				StartCoroutine(FadeOutAudio(audioPowerUpBoost, 1.2f));
			}
			if (atmoBassBeat.isPlaying)
			{
				StartCoroutine(FadeOutAudio(atmoBassBeat, 1.2f));
			}
			return;
		}
		m_IsInvincible = true;
		if (!atmoBassBeat.isPlaying)
		{
			atmoBassBeat.volume = 0.5f;
			atmoBassBeat.pitch = 1f;
			atmoBassBeat.Play();
		}
		StartCoroutine(IncreaseAudioVolume(atmoBassBeat, 1f, 10f));
		StartCoroutine(IncreaseAudioPitch(atmoBassBeat, 1.5f, 10f));
		if (!audioPowerUpBoost.isPlaying)
		{
			audioPowerUpBoost.pitch = 0.7f;
			audioPowerUpBoost.Play();
		}
		StartCoroutine(IncreaseAudioPitch(audioPowerUpBoost, 1.3f, 10f));
	}

	private IEnumerator IncreaseAudioVolume(AudioSource audioSource, float toVolume, float increaseTime)
	{
		float startVolume = audioSource.volume;
		while (audioSource.volume < toVolume - 0.01f)
		{
			if (!m_isPaused)
			{
				audioSource.volume = audioSource.volume + startVolume * Time.deltaTime / increaseTime;
			}
			yield return null;
		}
		audioSource.volume = toVolume;
	}

	private IEnumerator IncreaseAudioPitch(AudioSource audioSource, float toPitch, float increaseTime)
	{
		float startPitch = audioSource.pitch;
		while (audioSource.pitch < toPitch - 0.01f)
		{
			if (!m_isPaused)
			{
				audioSource.pitch = audioSource.pitch + startPitch * Time.deltaTime / increaseTime;
			}
			yield return null;
		}
		audioSource.pitch = toPitch;
	}

	private IEnumerator DecreaseAudioPitch(AudioSource audioSource, float toPitch, float decreaseTime)
	{
		float startPitch = audioSource.pitch;
		while (toPitch + 0.01f < audioSource.pitch)
		{
			if (!m_isPaused)
			{
				audioSource.pitch = audioSource.pitch - startPitch * Time.deltaTime / decreaseTime;
			}
			yield return null;
		}
		audioSource.pitch = toPitch;
	}

	private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeOutTime)
	{
		float startVolume = audioSource.volume;
		while (0.01f < audioSource.volume)
		{
			if (!m_isPaused)
			{
				audioSource.volume = audioSource.volume - startVolume * Time.deltaTime / fadeOutTime;
			}
			yield return null;
		}
		audioSource.volume = 0f;
		audioSource.Stop();
		audioSource.volume = 1f;
	}

	public void ShieldActivated(bool activated)
	{
		m_ShieldActivationCount += activated ? 1 : (-1);
		if (m_ShieldActivationCount < 1)
		{
			if (m_ShieldIncreasing)
			{
				StopCoroutine(m_ShieldRoutine);
				m_ShieldIncreasing = false;
			}
			if (!m_ShieldDecreasing && !(audioPowerUpShield.pitch <= 0.2f))
			{
				m_ShieldDecreasing = true;
				m_ShieldRoutine = StartCoroutine(DecreaseAudioPitch(audioPowerUpShield, 0.2f, 0.4f));
			}
		}
		else
		{
			if (m_ShieldDecreasing)
			{
				StopCoroutine(m_ShieldRoutine);
				m_ShieldDecreasing = false;
			}
			if (!m_ShieldIncreasing && !(0.6f <= audioPowerUpShield.pitch))
			{
				m_ShieldIncreasing = true;
				m_ShieldRoutine = StartCoroutine(IncreaseAudioPitch(audioPowerUpShield, 0.6f, 0.2f));
			}
		}
	}

	public void SetShield(bool useShield)
	{
		if (m_IsShield == useShield)
		{
			return;
		}
		m_IsShield = useShield;
		if (useShield)
		{
			if (!shieldObj.activeSelf)
			{
				shieldObj.SetActive(true);
			}
			m_ShieldActivationCount = 0;
			m_ShieldIncreasing = false;
			m_ShieldDecreasing = false;
			if (!audioPowerUpShield.isPlaying)
			{
				audioPowerUpShield.pitch = 0.2f;
				audioPowerUpShield.Play();
			}
		}
		else
		{
			if (audioPowerUpShield.isPlaying)
			{
				audioPowerUpShield.Stop();
			}
			if (m_ShieldRoutine != null)
			{
				StopCoroutine(m_ShieldRoutine);
			}
			shieldObj.SetActive(false);
		}
	}

	public void SetCoinMultiplier(bool coinMultiplier)
	{
		if (m_IsCoinMultiplier == coinMultiplier)
		{
			return;
		}
		m_IsCoinMultiplier = coinMultiplier;
		if (coinMultiplier)
		{
			CustomGameManager.Instance.DoubleCoinMultiplier(true);
			if (!audioPowerUpCoinDoubler.isPlaying)
			{
				audioPowerUpCoinDoubler.pitch = 1f;
				audioPowerUpCoinDoubler.Play();
			}
		}
		else
		{
			CustomGameManager.Instance.DoubleCoinMultiplier(false);
			if (audioPowerUpCoinDoubler.isPlaying)
			{
				audioPowerUpCoinDoubler.Stop();
			}
		}
	}

	public void Slide(bool sliding)
	{
		if (sliding)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_SlidingColliderScale);
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * -0.5f, 0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * 0.5f, 0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_DefaultColliderScale);
		}
		stumbleCollider.Slide(sliding);
	}

	public void Jump(bool jumping)
	{
		if (jumping)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_JumpingColliderScale);
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * 0.5f, 0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * -0.5f, 0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_DefaultColliderScale);
		}
		stumbleCollider.Jump(jumping);
	}

	private Obstacle GetObstacle(Collider c)
	{
		Obstacle obstacle = c.GetComponent<Obstacle>();
		if (obstacle == null)
		{
			obstacle = c.GetComponentInParent<Obstacle>();
		}
		return obstacle;
	}

	protected void OnTriggerEnter(Collider c)
	{
		int layer = c.gameObject.layer;
		switch (layer)
		{
		case k_MonsterLayerIndex:
		case k_GroundLayerIndex:
		case k_AirLayerIndex:
		{
			Obstacle obstacle = GetObstacle(c);
			if (!m_IsInvincible)
			{
				if (m_IsShield)
				{
					ShieldImpact(obstacle);
					break;
				}
				if (!CustomGameManager.Instance.invincible && obstacle.IsVisible())
				{
					if (layer == k_MonsterLayerIndex)
					{
						CustomGameManager.Instance.IsKilledByZombie();
						c.enabled = false;
						KilledByZombie(c.gameObject.GetComponentInChildren<Animator>(), obstacle.killsPlayerClip, obstacle.killsPlayerOutput, obstacle.bloodSecondsToWait, obstacle.hideFog);
					}
					else if (layer == k_GroundLayerIndex)
					{
						CustomGameManager.Instance.IsKilledByGround();
						KilledByHands(obstacle.killsPlayerClip, obstacle.killsPlayerOutput, obstacle.bloodSecondsToWait, obstacle.hideFog);
					}
					else
					{
						CustomGameManager.Instance.IsKilledByAir();
						KilledByRavens(obstacle.killsPlayerClip, obstacle.killsPlayerOutput, obstacle.bloodSecondsToWait, obstacle.hideFog);
					}
					if (CustomGameManager.Instance.isTutorial)
					{
						obstacle.Impact();
					}
					break;
				}
			}
			if (m_IsShield)
			{
				ShieldImpact(obstacle);
			}
			break;
		}
		case k_CollectiblesLayerIndex:
			audioCoinCollected.Play();
			if (m_IsCoinMultiplier)
			{
				Invoke("PlayOneShotCoinsCollectedSound", 0.15f);
			}
			break;
		case k_ConsumablesLayerIndex:
		{
			audioCoinCollected.Play();
			Consumable consumable = c.GetComponent<Consumable>();
			consumable.Collected();
			CustomGameManager.Instance.PowerUpCollected(consumable);
			break;
		}
		case k_HoleLayerIndex:
			if (!m_IsInvincible && !CustomGameManager.Instance.invincible)
			{
				CustomGameManager.Instance.IsKilledByHole();
				KilledByHole(c.gameObject.GetComponent<DeathSettings>());
			}
			break;
		case k_FootstepSoundLayerIndex:
			m_SpecialFootstepAreaCounter++;
			inputController.PlaySpecialFootstepSound(c.GetComponent<FootstepSound>().soundClip);
			break;
		case k_HeightChangeLayerIndex:
			inputController.ChangeHeight(c.gameObject.GetComponent<HeightChange>());
			break;
		}
	}

	protected void OnTriggerExit(Collider c)
	{
		if (c.gameObject.layer == k_FootstepSoundLayerIndex)
		{
			m_SpecialFootstepAreaCounter--;
			if (m_SpecialFootstepAreaCounter == 0)
			{
				inputController.StopSpecialFootstepSound();
			}
		}
	}

	protected void PlayOneShotCoinsCollectedSound()
	{
		audioCoinCollected.PlayOneShot(audioCoinCollected.clip);
	}

	protected void KilledByHole(DeathSettings deathSettings)
	{
		mainCamera.FallInHole(true);
		StartPlayerDeath(deathSettings.deathSound, deathSettings.deathOutput, deathSettings.bloodSecondsToWait, deathSettings.hideFog);
	}

	protected void StartPlayerDeath(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
		PlayDeadSound(deathClip, deathOutput);
		if (hideFog)
		{
			CustomGameManager.Instance.trackManager.HideFog();
		}
		StartCoroutine(StartBloodOnLens(bloodTime));
		float time = bloodOnLens.bloodFreezingTime + bloodTime;
		if (time < deathClip.length)
		{
			time = deathClip.length;
		}
		StartCoroutine(PlayerIsDead(time + 0.5f));
	}

	private IEnumerator StartBloodOnLens(float secondsToWait)
	{
		yield return new WaitForSeconds(secondsToWait);
		bloodOnLens.StartBloodOnLens();
	}

	protected void KilledByHands(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
		mainCamera.FallBack(true);
		StartPlayerDeath(deathClip, deathOutput, bloodTime, hideFog);
	}

	protected void KilledByRavens(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
		mainCamera.FallBack(true);
		StartPlayerDeath(deathClip, deathOutput, bloodTime, hideFog);
	}

	protected void KilledByZombie(Animator zombieAnim, AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
		zombieAnim.SetBool("isKillingYou", true);
		mainCamera.FallBack(true);
		StartPlayerDeath(deathClip, deathOutput, bloodTime, hideFog);
	}

	protected void PlayDeadSound(AudioClip deathClip, AudioMixerGroup deathOutput)
	{
		audioDeath.outputAudioMixerGroup = deathOutput;
		audioDeath.clip = deathClip;
		audioDeath.Play();
	}

	private IEnumerator PlayerIsDead(float time)
	{
		yield return new WaitForSeconds(time);
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.IsDeadAnimationFinished, null);
		audioDeath.clip = null;
	}

	protected void ShieldImpact(Obstacle obstacle)
	{
		audioPowerUpShieldImpact.Play();
		obstacle.Impact();
		SetShield(false);
		CustomGameManager.Instance.StopPowerUp();
	}
}
