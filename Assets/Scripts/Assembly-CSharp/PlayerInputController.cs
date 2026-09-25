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
		projectilePooler.ResetPools();
	}

	public void SetPlayerSound(AudioClip runClip)
	{
		m_RunClip = runClip;
		audioRun.clip = runClip;
	}

	public void SetWeapon(bool enabled)
	{
		m_WeaponActive = enabled;
		if (enabled)
		{
			powerUpWeaponSound.Play();
		}
		else
		{
			powerUpWeaponSound.Stop();
		}
	}

	public void NewGameStarted()
	{
		m_Tutorial = false;
		SwitchActionState(c_RUNNING);
	}

	public void TutorialEnd()
	{
		m_Tutorial = false;
	}

	public void TutorialStarted()
	{
		m_TutorialRunStopped = false;
		m_Tutorial = true;
		m_TutotalState = c_TUTORIAL_NO_INPUT;
		SwitchActionState(c_RUNNING);
	}

	public void TutorialStopRunning(bool allowChangeLane, bool allowJumpSlide)
	{
		m_TutorialRunStopped = true;
		audioRun.Pause();
		TutorialAllowInput(allowChangeLane, allowJumpSlide);
	}

	public void TutorialResumeRunning(bool allowChangeLane, bool allowJumpSlide)
	{
		m_TutorialRunStopped = false;
		audioRun.UnPause();
		TutorialAllowInput(allowChangeLane, allowJumpSlide);
	}

	public void TutorialAllowInput(bool allowChangeLane, bool allowJumpSlide)
	{
		if (allowChangeLane && allowJumpSlide)
		{
			m_TutotalState = c_TUTORIAL_INPUT;
			return;
		}
		m_TutotalState = allowChangeLane ? c_TUTORIAL_CHANGE_LANES : c_TUTORIAL_NO_INPUT;
		if (m_ActionState != c_RUNNING)
		{
			SwitchActionState(c_RUNNING);
		}
	}

	public void TutorialPaused(bool paused)
	{
		if (paused)
		{
			m_IsPlaying = false;
			m_IsSwiping = false;
			PauseAllAudio(true);
			return;
		}
		if (m_GameStopped)
		{
			return;
		}
		m_IsPlaying = true;
		audioSlide.UnPause();
		if (!m_TutorialRunStopped)
		{
			audioRun.UnPause();
		}
		audioPlayerBreathing.UnPause();
		audioPlayerHeartbeat.UnPause();
		audioJumpEnd.UnPause();
		audioJumpStart.UnPause();
		audioChangeLane.UnPause();
		powerUpWeaponSound.UnPause();
	}

	public void GameStopped()
	{
		m_IsPlaying = false;
		m_TutorialRunStopped = true;
		m_GameStopped = true;
		PauseAllAudio(true);
	}

	protected void PauseAllAudio(bool paused)
	{
		if (paused)
		{
			audioSlide.Pause();
			audioRun.Pause();
			audioPlayerBreathing.Pause();
			audioPlayerHeartbeat.Pause();
			audioJumpEnd.Pause();
			audioJumpStart.Pause();
			audioChangeLane.Pause();
			powerUpWeaponSound.Pause();
		}
		else
		{
			audioSlide.UnPause();
			audioRun.UnPause();
			audioPlayerBreathing.UnPause();
			audioPlayerHeartbeat.UnPause();
			audioJumpEnd.UnPause();
			audioJumpStart.UnPause();
			audioChangeLane.UnPause();
			powerUpWeaponSound.UnPause();
		}
	}

	protected void StopAllAudio()
	{
		audioSlide.Stop();
		audioRun.Stop();
		audioPlayerBreathing.Stop();
		audioPlayerHeartbeat.Stop();
		audioJumpEnd.Stop();
		audioJumpStart.Stop();
		audioChangeLane.Stop();
		powerUpWeaponSound.Stop();
	}

	public void PlaySpecialFootstepSound(AudioClip specialClip)
	{
		if (!m_IsSpecialFootstepSound)
		{
			m_SpecialRunClip = specialClip;
			ChangeAudioClip(audioRun, mixerGroupSpecialRun, specialClip);
			m_IsSpecialFootstepSound = true;
		}
		else if (m_SpecialRunClip != specialClip)
		{
			m_SpecialRunClip = specialClip;
			ChangeAudioClip(audioRun, mixerGroupSpecialRun, specialClip);
		}
	}

	public void StopSpecialFootstepSound()
	{
		if (m_IsSpecialFootstepSound)
		{
			ChangeAudioClip(audioRun, mixerGroupRun, m_RunClip);
			m_IsSpecialFootstepSound = false;
		}
	}

	protected void ChangeAudioClip(AudioSource audioSource, AudioMixerGroup mixerGroup, AudioClip audioClip)
	{
		if (audioSource.isPlaying)
		{
			audioSource.volume = 0f;
			audioSource.Stop();
			audioSource.clip = audioClip;
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = 1f;
			audioSource.Play();
		}
		else
		{
			audioSource.clip = audioClip;
			audioSource.outputAudioMixerGroup = mixerGroup;
		}
	}

	public void GameRevived()
	{
		GameQuit(false);
		if (m_WeaponActive)
		{
			powerUpWeaponSound.Play();
		}
		m_TutorialRunStopped = false;
		m_HeightChange = null;
		playerHeightTransform.localPosition = Vector3.zero;
		SwitchActionState(c_RUNNING);
	}

	public void GameQuit(bool reset = true)
	{
		StopAllCoroutines();
		StopAllAudio();
		if (reset)
		{
			m_WeaponActive = false;
			m_AudioRunPitch = 0.867f;
			audioRun.pitch = 0.867f;
			m_AudioHeartbeatPitch = 0.85f;
			audioPlayerHeartbeat.pitch = 0.85f;
		}
		m_PanValue = 0f;
		audioRun.panStereo = 0f;
		m_GameStopped = false;
		m_UserControl = true;
		m_IsPlaying = false;
		StopSpecialFootstepSound();
		transform.localPosition = Vector3.zero;
		m_TargetPosition = Vector3.zero;
		m_HeightChange = null;
		playerHeightTransform.localPosition = Vector3.zero;
		m_CurrentLane = 1;
		m_IsSwiping = false;
		if (m_ActionState == c_JUMPING)
		{
			playerCollider.Jump(false);
			CustomGameManager.Instance.m_isJumping = false;
		}
		if (m_ActionState == c_SLIDING)
		{
			playerCollider.Slide(false);
			CustomGameManager.Instance.m_isSliding = false;
		}
		m_ActionState = 0;
		if (projectileParent != null)
		{
			Projectile[] projectiles = projectileParent.GetComponentsInChildren<Projectile>();
			for (int i = 0; i < projectiles.Length; i++)
			{
				projectiles[i].gameObject.SetActive(false);
				projectiles[i].FreeProjectile();
			}
		}
	}

	public void GameResumed()
	{
		PauseAllAudio(false);
		m_GameStopped = false;
		m_IsPlaying = true;
	}

	protected void SwitchActionState(int toState)
	{
		int fromState = m_ActionState;
		m_ActionState = toState;
		switch (toState)
		{
		case c_RUNNING:
			EnterRunning(fromState);
			break;
		case c_JUMPING:
			EnterJumping(fromState);
			break;
		case c_SLIDING:
			EnterSliding(fromState);
			break;
		}
	}

	protected void EnterRunning(int fromState)
	{
		switch (fromState)
		{
		case 0:
			m_IsPlaying = true;
			break;
		case c_JUMPING:
			mainCamera.Jump(false);
			playerCollider.Jump(false);
			CustomGameManager.Instance.m_isJumping = false;
			audioJumpEnd.Play();
			break;
		case c_SLIDING:
			mainCamera.Slide(false);
			playerCollider.Slide(false);
			CustomGameManager.Instance.m_isSliding = false;
			audioSlide.Stop();
			break;
		}
		audioRun.Play();
		if (!audioPlayerHeartbeat.isPlaying)
		{
			audioPlayerHeartbeat.Play();
		}
		if (!audioPlayerBreathing.isPlaying)
		{
			audioPlayerBreathing.Play();
		}
		if (!m_UserControl)
		{
			StartCoroutine(IncreaseAudioPitch(audioRun, 1.3f, 10f));
			StartCoroutine(IncreaseAudioPitch(audioPlayerHeartbeat, 1.5f, 10f));
		}
	}

	protected void EnterJumping(int fromState)
	{
		if (audioRun.isPlaying)
		{
			audioRun.Stop();
		}
		if (audioSlide.isPlaying)
		{
			audioSlide.Stop();
		}
		if (fromState == c_SLIDING)
		{
			mainCamera.Slide(false);
			playerCollider.Slide(false);
			CustomGameManager.Instance.m_isSliding = false;
		}
		mainCamera.Jump(true);
		m_JumpStart = CustomGameManager.Instance.m_TotalWorldDistance;
		playerCollider.Jump(true);
		CustomGameManager.Instance.m_isJumping = true;
		CustomGameManager.Instance.Jumped();
		audioJumpStart.Play();
		if (!audioPlayerHeartbeat.isPlaying)
		{
			audioPlayerHeartbeat.Play();
		}
		if (!audioPlayerBreathing.isPlaying)
		{
			audioPlayerBreathing.Play();
		}
	}

	protected void EnterSliding(int fromState)
	{
		if (audioRun.isPlaying)
		{
			audioRun.Stop();
		}
		if (fromState == c_JUMPING)
		{
			mainCamera.Jump(false);
			playerCollider.Jump(false);
			CustomGameManager.Instance.m_isJumping = false;
			if (audioJumpStart.isPlaying)
			{
				audioJumpStart.Stop();
			}
			if (audioJumpEnd.isPlaying)
			{
				audioJumpEnd.Stop();
			}
		}
		mainCamera.Slide(true);
		m_SlideStart = CustomGameManager.Instance.m_TotalWorldDistance;
		playerCollider.Slide(true);
		CustomGameManager.Instance.m_isSliding = true;
		CustomGameManager.Instance.Slided();
		audioSlide.Play();
		if (!audioPlayerHeartbeat.isPlaying)
		{
			audioPlayerHeartbeat.Play();
		}
		if (!audioPlayerBreathing.isPlaying)
		{
			audioPlayerBreathing.Play();
		}
	}

	public void GiveControlAway()
	{
		m_UserControl = false;
		if (m_ActionState == c_RUNNING)
		{
			StartCoroutine(IncreaseAudioPitch(audioRun, 1.3f, 10f));
			StartCoroutine(IncreaseAudioPitch(audioPlayerHeartbeat, 1.5f, 10f));
		}
	}

	public void GiveControlBack()
	{
		m_UserControl = true;
		audioRun.pitch = m_AudioRunPitch;
		audioPlayerHeartbeat.pitch = m_AudioHeartbeatPitch;
	}

	private IEnumerator IncreaseAudioPitch(AudioSource audioSource, float toPitch, float increaseTime)
	{
		float startPitch = audioSource.pitch;
		while (audioSource.pitch < toPitch - 0.01f)
		{
			if (m_IsPlaying)
			{
				audioSource.pitch = audioSource.pitch + startPitch * Time.deltaTime / increaseTime;
			}
			yield return null;
		}
		audioSource.pitch = toPitch;
	}

	public void SetRunSpeed(float speed)
	{
		if (!(speed < 12f))
		{
			return;
		}
		m_AudioRunPitch = speed / 15f + 0.5f;
		audioRun.pitch = m_AudioRunPitch;
		m_AudioHeartbeatPitch = speed / 10f + 0.3f;
		audioPlayerHeartbeat.pitch = m_AudioHeartbeatPitch;
	}

	private bool CanJumpOrSlide(int toState)
	{
		return m_ActionState != toState && m_UserControl && (!m_Tutorial || m_TutotalState != c_TUTORIAL_CHANGE_LANES);
	}

	private void TryJump()
	{
		if (CanJumpOrSlide(c_JUMPING))
		{
			SwitchActionState(c_JUMPING);
		}
	}

	private void TrySlide()
	{
		if (CanJumpOrSlide(c_SLIDING))
		{
			SwitchActionState(c_SLIDING);
		}
	}

	private void Update()
	{
		if (!m_IsPlaying)
		{
			return;
		}
		if (m_Tutorial && m_TutotalState == c_TUTORIAL_NO_INPUT)
		{
			return;
		}
		PlayerData_v_1_1_3 playerData = DataManager.Instance.m_PlayerData;
		// PORT: além das setas originais, aceita os gestos virtuais do PortInput (joystick/mouse).
		bool keyLeft = Input.GetKeyDown(KeyCode.LeftArrow) || PortInput.GameSwipeLeft;
		bool keyRight = Input.GetKeyDown(KeyCode.RightArrow) || PortInput.GameSwipeRight;
		bool keyUp = Input.GetKeyDown(KeyCode.UpArrow) || PortInput.GameSwipeUp;
		bool keyDown = Input.GetKeyDown(KeyCode.DownArrow) || PortInput.GameSwipeDown;
		if (keyLeft)
		{
			ChangeLane(playerData.useReverseLeftRight ? 1 : -1);
		}
		else if (keyRight)
		{
			ChangeLane(playerData.useReverseLeftRight ? -1 : 1);
		}
		else if ((keyUp && !playerData.useReverseUpDown) || (keyDown && playerData.useReverseUpDown))
		{
			TryJump();
		}
		else if ((keyDown && !playerData.useReverseUpDown) || (keyUp && playerData.useReverseUpDown))
		{
			TrySlide();
		}
		else if (m_WeaponActive && (Input.GetKeyDown(KeyCode.Space) || PortInput.GameTap))
		{
			ShootProjectile();
		}
		if (Input.touchCount == 1)
		{
			if (m_IsSwiping)
			{
				Vector2 diff = Input.GetTouch(0).position - m_StartingTouch;
				diff = new Vector2(diff.x / (float)Screen.width, diff.y / (float)Screen.width);
				if (diff.magnitude > 0.02f)
				{
					if (Mathf.Abs(diff.y) <= Mathf.Abs(diff.x))
					{
						if (diff.x < 0f)
						{
							ChangeLane(playerData.useReverseLeftRight ? 1 : -1);
						}
						else
						{
							ChangeLane(playerData.useReverseLeftRight ? -1 : 1);
						}
					}
					else if (diff.y < 0f)
					{
						if (playerData.useReverseUpDown)
						{
							TryJump();
						}
						else
						{
							TrySlide();
						}
					}
					else if (playerData.useReverseUpDown)
					{
						TrySlide();
					}
					else
					{
						TryJump();
					}
					m_IsSwiping = false;
					m_isTap = false;
				}
				else
				{
					m_isTap = true;
				}
			}
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				m_StartingTouch = Input.GetTouch(0).position;
				m_IsSwiping = true;
				m_isTap = false;
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				m_IsSwiping = false;
				if (m_isTap)
				{
					m_isTap = false;
					if (m_WeaponActive)
					{
						ShootProjectile();
					}
				}
			}
		}
		Vector3 targetPosition = m_TargetPosition;
		if (m_ActionState == c_JUMPING)
		{
			float length = m_Tutorial ? 7f : jumpLength;
			if ((CustomGameManager.Instance.m_TotalWorldDistance - m_JumpStart) / length >= 1f)
			{
				SwitchActionState(c_RUNNING);
			}
		}
		else if (m_ActionState == c_SLIDING)
		{
			float length = m_Tutorial ? 7f : slideLength;
			if ((CustomGameManager.Instance.m_TotalWorldDistance - m_SlideStart) / length >= 1f)
			{
				SwitchActionState(c_RUNNING);
			}
		}
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, laneChangeSpeed * Time.deltaTime);
		AdjustSoundPan();
		if (m_HeightChange != null)
		{
			float targetHeight = m_HeightChange.targetHeight;
			if (Mathf.Abs(targetHeight - playerHeightTransform.localPosition.y) <= 0.1f)
			{
				playerHeightTransform.localPosition = new Vector3(0f, targetHeight, 0f);
				m_HeightChange = null;
			}
			else
			{
				playerHeightTransform.localPosition = Vector3.MoveTowards(playerHeightTransform.localPosition, new Vector3(0f, targetHeight, 0f), m_HeightChange.speed * Time.deltaTime);
			}
		}
	}

	private void ChangeLane(int direction)
	{
		int lane = m_CurrentLane + direction;
		if ((uint)lane < 3u)
		{
			m_CurrentLane = lane;
			CustomGameManager.Instance.LaneChangedTo(m_CurrentLane);
			m_TargetPosition = new Vector3(laneWidth * (float)(m_CurrentLane - 1), 0f, 0f);
			audioChangeLane.Play();
		}
		else if (UAP_AccessibilityManager.IsEnabled())
		{
			stumbleAudio.panStereo = (lane < 3) ? -1f : 1f;
			stumbleAudio.Play();
		}
	}

	public void ChangeHeight(HeightChange heightChange)
	{
		m_HeightChange = heightChange;
	}

	protected void AdjustSoundPan()
	{
		float value = transform.localPosition.x / laneWidth * 0.75f;
		if (Mathf.Abs(value - m_PanValue) < 0.01f)
		{
			return;
		}
		if (DataManager.Instance.m_PlayerData.useCenterLaneOrientation)
		{
			value = 0f - value;
		}
		m_PanValue = value;
		audioRun.panStereo = value;
	}

	protected void ShootProjectile()
	{
		ProjectilePool pool = projectilePooler.GetPool(projectilePrefab);
		Projectile projectile = pool.Get(Vector3.zero, Quaternion.identity);
		projectile.transform.position = new Vector3((float)m_CurrentLane - 1f, 0f, 0f);
		projectile.pool = pool;
		projectile.transform.SetParent(projectileParent);
	}
}
