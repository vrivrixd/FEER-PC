using UnityEngine;

public class ObstacleAllLane : Obstacle
{
	public GameObject sprite;

	public Animator m_ObstacleAnimator;

	public float minDistance = 1f;

	public float maxDistance = 12f;

	public float distanceAfter = 2f;

	protected AudioSource m_Audio;

	protected AudioLowPassFilter m_LowPassFilter;

	protected Transform m_Transform;

	protected float m_Distance;

	protected Collider m_Collider;

	protected float m_DefaultCutOffFrequency;

	protected bool m_ShieldSet;

	protected bool m_Visible = true;

	protected bool m_Init;

	public override bool IsVisible()
	{
		return m_Visible;
	}

	public override void Impact()
	{
		m_Collider.enabled = false;
		sprite.gameObject.SetActive(false);
		m_Visible = false;
		StopObstacleSoundAndAnimation();
	}

	public override void ShieldEntered()
	{
		m_ShieldSet = true;
		m_ObstacleAnimator.SetBool("isShield", true);
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
			m_ObstacleAnimator.speed = 1f;
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
			m_ObstacleAnimator.speed = 1f;
			break;
		case c_STATE_HEARABLE_IN_FRONT:
			m_ObstacleAnimator.speed = 1f;
			StartObstacleSound();
			break;
		case c_STATE_HEARABLE_BEHIND:
			m_LowPassFilter.cutoffFrequency = 2500f;
			break;
		case c_STATE_EXIT:
			StopObstacleSoundAndAnimation();
			break;
		}
	}

	protected void StopObstacleSoundAndAnimation()
	{
		m_ObstacleAnimator.speed = 0f;
		m_Audio.Stop();
	}

	protected void StartObstacleSound()
	{
		float z = m_Transform.position.z;
		m_Audio.volume = (z <= minDistance) ? 1f : (1f - (m_Transform.position.z - minDistance) / m_Distance);
		m_Audio.Play();
	}

	public override void Spawn(TrackManager trackManager)
	{
		m_TrackManager = trackManager;
		SwitchStateTo(c_STATE_SPAWNED);
	}

	public override void FreeObstacle()
	{
		if (!m_Init)
		{
			Init();
		}
		m_Audio.Stop();
		m_Audio.panStereo = 0f;
		m_Visible = true;
		m_Collider.enabled = true;
		sprite.gameObject.SetActive(true);
		m_LowPassFilter.cutoffFrequency = m_DefaultCutOffFrequency;
		m_ShieldSet = false;
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
		m_Transform = transform;
		m_Audio = GetComponent<AudioSource>();
		m_LowPassFilter = GetComponent<AudioLowPassFilter>();
		m_Distance = maxDistance - minDistance;
		distanceAfter = 0f - distanceAfter;
		m_Collider = GetComponent<BoxCollider>();
		m_DefaultCutOffFrequency = m_LowPassFilter.cutoffFrequency;
		m_Init = true;
	}

	protected void UpdateHearableInFront(float currentPosition)
	{
		m_Audio.volume = (currentPosition <= minDistance) ? 1f : (1f - (currentPosition - minDistance) / m_Distance);
		if (!m_Visible)
		{
			return;
		}
		if (currentPosition >= 1f)
		{
			if ((CustomGameManager.Instance.isSliding && obstacleType == SpawnElementType.AllLaneAirObstacle) || (CustomGameManager.Instance.isJumping && obstacleType == SpawnElementType.AllLaneGroundObstacle))
			{
				transform.Translate(0f, 0f, Time.deltaTime * CustomGameManager.Instance.speed * -1.3f);
			}
			return;
		}
		sprite.gameObject.SetActive(false);
		m_Visible = false;
		if (m_ShieldSet)
		{
			m_ShieldSet = false;
			CustomGameManager.Instance.playerCollider.ShieldActivated(false);
		}
	}

	protected void DampAudio(float currentPosition)
	{
		m_Audio.volume = 1f - currentPosition / distanceAfter;
	}

	private void Update()
	{
		if (m_StateStack[c_CURRENT_STATE] != c_STATE_PAUSE)
		{
			GameState topState = CustomGameManager.Instance.topState;
			GameStateName name = topState.GetName();
			if (name == GameStateName.IsDead || (name == GameStateName.PlayGame && topState.GetStatus() == GameStateStatus.TutorialPaused) || name == GameStateName.Pause)
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
		{
			GameState topState2 = CustomGameManager.Instance.topState;
			if (topState2.GetName() == GameStateName.PlayGame && topState2.GetStatus() != GameStateStatus.TutorialPaused)
			{
				PopState();
			}
			break;
		}
		}
	}
}
