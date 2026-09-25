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

	protected readonly Vector3 k_SlidingColliderScale;

	protected readonly Vector3 k_JumpingColliderScale;

	protected readonly Vector3 k_DefaultColliderScale;

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

	public bool isBoost => false;

	public bool isShield => false;

	private void Start()
	{
	}

	public void StopPlaying()
	{
	}

	protected void PauseAllAudio(bool paused)
	{
	}

	protected void StopAllAudio()
	{
	}

	public void ResumePlaying()
	{
	}

	public void ReviveGame()
	{
	}

	public void QuitPlaying()
	{
	}

	public void StopInvincible()
	{
	}

	public void StartPowerUpSound(ConsumableType consumableType)
	{
	}

	public void SetBoost(bool boost)
	{
	}

	private IEnumerator IncreaseAudioVolume(AudioSource audioSource, float toVolume, float increaseTime)
	{
		return null;
	}

	private IEnumerator IncreaseAudioPitch(AudioSource audioSource, float toPitch, float increaseTime)
	{
		return null;
	}

	private IEnumerator DecreaseAudioPitch(AudioSource audioSource, float toPitch, float decreaseTime)
	{
		return null;
	}

	private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeOutTime)
	{
		return null;
	}

	public void ShieldActivated(bool activated)
	{
	}

	public void SetShield(bool useShield)
	{
	}

	public void SetCoinMultiplier(bool coinMultiplier)
	{
	}

	public void Slide(bool sliding)
	{
	}

	public void Jump(bool jumping)
	{
	}

	protected void OnTriggerEnter(Collider c)
	{
	}

	protected void OnTriggerExit(Collider c)
	{
	}

	protected void PlayOneShotCoinsCollectedSound()
	{
	}

	protected void KilledByHole(DeathSettings deathSettings)
	{
	}

	protected void StartPlayerDeath(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
	}

	private IEnumerator StartBloodOnLens(float secondsToWait)
	{
		return null;
	}

	protected void KilledByHands(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
	}

	protected void KilledByRavens(AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
	}

	protected void KilledByZombie(Animator zombieAnim, AudioClip deathClip, AudioMixerGroup deathOutput, float bloodTime, bool hideFog)
	{
	}

	protected void PlayDeadSound(AudioClip deathClip, AudioMixerGroup deathOutput)
	{
	}

	private IEnumerator PlayerIsDead(float time)
	{
		return null;
	}

	protected void ShieldImpact(Obstacle obstacle)
	{
	}
}
