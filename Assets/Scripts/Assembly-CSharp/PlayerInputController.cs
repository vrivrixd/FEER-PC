using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerInputController : MonoBehaviour
{
	public Transform playerHeightTransform;

	public AudioSource audioPlayerBreathing;

	public AudioSource audioPlayerHeartbeat;

	public MainCamera mainCamera;

	public PlayerCollider playerCollider;

	public AudioSource audioSlide;

	public float slideLength;

	public AudioSource audioJumpStart;

	public AudioSource audioJumpEnd;

	public float jumpLength;

	public float jumpHeight;

	public AudioSource audioRun;

	public AudioMixerGroup mixerGroupRun;

	public AudioMixerGroup mixerGroupSpecialRun;

	public AudioSource audioChangeLane;

	public float laneWidth;

	public float laneChangeSpeed;

	public AudioSource stumbleAudio;

	public ProjectilePooler projectilePooler;

	public Projectile projectilePrefab;

	public Transform projectileParent;

	public AudioSource powerUpWeaponSound;

	protected int m_CurrentLane;

	protected Vector3 m_TargetPosition;

	protected float m_JumpStart;

	protected float m_SlideStart;

	protected Vector2 m_StartingTouch;

	protected bool m_IsSwiping;

	protected bool m_UserControl;

	protected bool m_IsPlaying;

	protected bool m_Tutorial;

	protected int m_TutotalState;

	protected const int c_TUTORIAL_NO_INPUT = 0;

	protected const int c_TUTORIAL_INPUT = 1;

	protected const int c_TUTORIAL_CHANGE_LANES = 2;

	protected int m_ActionState;

	protected const int c_RUNNING = 1;

	protected const int c_JUMPING = 2;

	protected const int c_SLIDING = 3;

	protected bool m_IsSpecialFootstepSound;

	protected float m_AudioRunPitch;

	protected float m_AudioHeartbeatPitch;

	protected float m_PanValue;

	protected bool m_WeaponActive;

	protected bool m_isTap;

	protected AudioClip m_RunClip;

	protected AudioClip m_SpecialRunClip;

	protected HeightChange m_HeightChange;

	protected bool m_TutorialRunStopped;

	protected bool m_GameStopped;

	private void Awake()
	{
	}

	public void SetPlayerSound(AudioClip runClip)
	{
	}

	public void SetWeapon(bool enabled)
	{
	}

	public void NewGameStarted()
	{
	}

	public void TutorialEnd()
	{
	}

	public void TutorialStarted()
	{
	}

	public void TutorialStopRunning(bool allowChangeLane, bool allowJumpSlide)
	{
	}

	public void TutorialResumeRunning(bool allowChangeLane, bool allowJumpSlide)
	{
	}

	public void TutorialAllowInput(bool allowChangeLane, bool allowJumpSlide)
	{
	}

	public void TutorialPaused(bool paused)
	{
	}

	public void GameStopped()
	{
	}

	protected void PauseAllAudio(bool paused)
	{
	}

	protected void StopAllAudio()
	{
	}

	public void PlaySpecialFootstepSound(AudioClip specialClip)
	{
	}

	public void StopSpecialFootstepSound()
	{
	}

	protected void ChangeAudioClip(AudioSource audioSource, AudioMixerGroup mixerGroup, AudioClip audioClip)
	{
	}

	public void GameRevived()
	{
	}

	public void GameQuit(bool reset = true)
	{
	}

	public void GameResumed()
	{
	}

	protected void SwitchActionState(int toState)
	{
	}

	protected void EnterRunning(int fromState)
	{
	}

	protected void EnterJumping(int fromState)
	{
	}

	protected void EnterSliding(int fromState)
	{
	}

	public void GiveControlAway()
	{
	}

	public void GiveControlBack()
	{
	}

	private IEnumerator IncreaseAudioPitch(AudioSource audioSource, float toPitch, float increaseTime)
	{
		return null;
	}

	public void SetRunSpeed(float speed)
	{
	}

	private void Update()
	{
	}

	private void ChangeLane(int direction)
	{
	}

	public void ChangeHeight(HeightChange heightChange)
	{
	}

	protected void AdjustSoundPan()
	{
	}

	protected void ShootProjectile()
	{
	}
}
