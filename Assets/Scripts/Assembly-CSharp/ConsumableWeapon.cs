using System.Collections;
using UnityEngine;

public class ConsumableWeapon : Consumable
{
	public GameObject sprite;

	public float minDistance = 1f;

	public float maxDistance = 15f;

	public float distanceAfter = 2f;

	public Animator m_ObstacleAnimator;

	protected Transform m_Transform;

	protected int m_ObstacleLane;

	protected int m_CurrentLane = 1;

	protected AudioSource m_Audio;

	protected AudioLowPassFilter m_LowPassFilter;

	protected int m_TargetPan;

	protected Coroutine m_ChangePan;

	protected float m_Distance;

	protected float m_laneVolume;

	protected Collider m_Collider;

	protected float m_DefaultCutOffFrequency;

	protected bool m_Init;

	protected bool m_FirstPowerUpTold;

	public override string GetName()
	{
		return "Weapon";
	}

	public override void Collected()
	{
		if (m_ChangePan != null)
		{
			StopCoroutine(m_ChangePan);
		}
		m_Audio.Stop();
		m_Audio.panStereo = 0f;
		m_Collider.enabled = false;
		sprite.gameObject.SetActive(false);
		m_StateStack[c_CURRENT_STATE] = c_STATE_NONE;
	}

	protected override void SwitchStateTo(int state)
	{
		m_StateStack[c_CURRENT_STATE] = state;
		EnterState();
	}

	protected override void PushState(int state)
	{
		int previous = m_StateStack[0];
		m_StateStack[0] = state;
		m_StateStack[1] = previous;
		if (previous == c_STATE_SPAWNED)
		{
			m_ObstacleAnimator.speed = 0f;
		}
		else if (previous == c_STATE_HEARABLE_IN_FRONT || previous == c_STATE_HEARABLE_BEHIND)
		{
			m_ObstacleAnimator.speed = 0f;
			m_Audio.Pause();
		}
	}

	protected override void PopState()
	{
		int previous = m_StateStack[1];
		if (previous == c_STATE_SPAWNED)
		{
			m_ObstacleAnimator.speed = 0f;
		}
		else if (previous == c_STATE_HEARABLE_IN_FRONT || previous == c_STATE_HEARABLE_BEHIND)
		{
			m_ObstacleAnimator.speed = 1f;
			m_Audio.UnPause();
		}
		m_StateStack[0] = m_StateStack[1];
	}

	protected override void EnterState()
	{
		switch (m_StateStack[c_CURRENT_STATE])
		{
		case c_STATE_SPAWNED:
			SetObstacleLane();
			m_ObstacleAnimator.speed = 0f;
			break;
		case c_STATE_HEARABLE_IN_FRONT:
			m_ObstacleAnimator.speed = 1f;
			StartObstacleSound(m_Transform.position.z);
			break;
		case c_STATE_HEARABLE_BEHIND:
			m_LowPassFilter.cutoffFrequency = 2500f;
			break;
		case c_STATE_EXIT:
			StopObstacleSoundAndAnimation();
			break;
		}
	}

	protected void SetObstacleLane()
	{
		float x = gameObject.transform.localPosition.x;
		if (x < -0.3f)
		{
			m_ObstacleLane = 0;
		}
		else
		{
			m_ObstacleLane = (x <= 0.3f) ? 1 : 2;
		}
	}

	public override void Spawn(TrackManager trackManager)
	{
		m_TrackManager = trackManager;
		SwitchStateTo(c_STATE_SPAWNED);
	}

	public override void FreeConsumable()
	{
		if (!m_Init)
		{
			Init();
		}
		if (m_ChangePan != null)
		{
			StopCoroutine(m_ChangePan);
		}
		m_Audio.Stop();
		m_Audio.panStereo = 0f;
		m_Collider.enabled = true;
		sprite.gameObject.SetActive(true);
		m_LowPassFilter.cutoffFrequency = m_DefaultCutOffFrequency;
		m_FirstPowerUpTold = false;
		m_FirstPowerUpTold = !DataManager.Instance.m_PlayerData.isFirstAccessiblePowerUp;
		m_ObstacleAnimator.speed = 0f;
		pool.Free(this);
		m_StateStack[c_CURRENT_STATE] = c_STATE_NONE;
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		m_Audio = GetComponent<AudioSource>();
		m_LowPassFilter = GetComponent<AudioLowPassFilter>();
		m_Distance = maxDistance - minDistance;
		distanceAfter = 0f - distanceAfter;
		m_Transform = transform;
		m_Collider = GetComponent<BoxCollider>();
		m_DefaultCutOffFrequency = m_LowPassFilter.cutoffFrequency;
		m_Init = true;
		m_FirstPowerUpTold = !DataManager.Instance.m_PlayerData.isFirstAccessiblePowerUp;
	}

	protected void UpdateHearableInFront(float currentPosition)
	{
		if (!m_FirstPowerUpTold && currentPosition <= 8f && DataManager.Instance.m_PlayerData.isFirstAccessiblePowerUp)
		{
			m_FirstPowerUpTold = true;
			CustomGameManager.Instance.FirstTimeAccessiblePowerUp(ConsumableType.None);
		}
		if (currentPosition < 1f && m_ObstacleLane != m_CurrentLane)
		{
			m_Collider.enabled = false;
			sprite.gameObject.SetActive(false);
		}
		HandleLaneChange();
		AdjustVolume(currentPosition);
	}

	private void Update()
	{
		if (m_StateStack[c_CURRENT_STATE] != c_STATE_PAUSE)
		{
			GameStateName name = CustomGameManager.Instance.topState.GetName();
			if (name == GameStateName.Pause || name == GameStateName.IsDead)
			{
				PushState(c_STATE_PAUSE);
				return;
			}
		}
		switch (m_StateStack[c_CURRENT_STATE])
		{
		case c_STATE_SPAWNED:
			if (!(maxDistance < m_Transform.position.z))
			{
				SwitchStateTo(c_STATE_HEARABLE_IN_FRONT);
			}
			break;
		case c_STATE_HEARABLE_IN_FRONT:
			if (m_Transform.position.z >= 0f)
			{
				UpdateHearableInFront(m_Transform.position.z);
			}
			else
			{
				SwitchStateTo(c_STATE_HEARABLE_BEHIND);
			}
			break;
		case c_STATE_HEARABLE_BEHIND:
			if (distanceAfter <= m_Transform.position.z)
			{
				DampAudio(m_Transform.position.z);
			}
			else
			{
				SwitchStateTo(c_STATE_EXIT);
			}
			break;
		case c_STATE_PAUSE:
			if (CustomGameManager.Instance.topState.GetName() == GameStateName.PlayGame)
			{
				PopState();
			}
			break;
		}
	}

	protected void DampAudio(float currentPosition)
	{
		m_Audio.volume = 1f - currentPosition / distanceAfter - m_laneVolume;
	}

	protected void StopObstacleSoundAndAnimation()
	{
		m_Audio.Stop();
		m_ObstacleAnimator.speed = 0f;
	}

	private float GetDistanceVolume(float currentPosition)
	{
		if (currentPosition <= minDistance)
		{
			return 1f - m_laneVolume;
		}
		return 1f - (currentPosition - minDistance) / m_Distance - m_laneVolume;
	}

	private void AdjustVolume(float currentPosition)
	{
		m_Audio.volume = GetDistanceVolume(currentPosition);
	}

	private void HandleLaneChange()
	{
		int currentLane = CustomGameManager.Instance.m_CurrentLane;
		if (currentLane != m_CurrentLane)
		{
			m_CurrentLane = currentLane;
			ChangePan();
		}
	}

	private void SetLaneVolume()
	{
	}

	private void StartObstacleSound(float currentPosition)
	{
		m_CurrentLane = CustomGameManager.Instance.m_CurrentLane;
		if (m_CurrentLane < m_ObstacleLane)
		{
			m_Audio.panStereo = 1f;
		}
		else
		{
			m_Audio.panStereo = (m_ObstacleLane < m_CurrentLane) ? (-1f) : 0f;
		}
		m_Audio.volume = GetDistanceVolume(currentPosition);
		m_Audio.Play();
	}

	private void ChangePan()
	{
		if (m_CurrentLane < m_ObstacleLane)
		{
			m_TargetPan = 1;
		}
		else if (m_ObstacleLane < m_CurrentLane)
		{
			m_TargetPan = -1;
		}
		else
		{
			m_TargetPan = 0;
		}
		if (m_ChangePan != null)
		{
			StopCoroutine(m_ChangePan);
		}
		m_ChangePan = StartCoroutine(IEChangePan());
	}

	private IEnumerator IEChangePan()
	{
		for (float t = 0f; t < 1f; t += Time.deltaTime)
		{
			m_Audio.panStereo = Mathf.Lerp(m_Audio.panStereo, m_TargetPan, t);
			yield return null;
		}
		m_Audio.panStereo = m_TargetPan;
	}
}
