using System.Collections;
using UnityEngine;

public class ObstacleSimple : Obstacle
{
	public GameObject sprite;

	public float minDistance = 1f;

	public float maxDistance = 15f;

	public float distanceAfter = 2f;

	public float baseValue = 10f;

	public bool useLogarithmic;

	public Animator m_ObstacleAnimator;

	public AudioClip walkingClip;

	public AudioClip isShotClip;

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
			SetObstacleLane();
			m_ObstacleAnimator.speed = 1f;
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

	public override void FreeObstacle()
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
		m_Audio.clip = walkingClip;
		m_Audio.loop = true;
		m_Visible = true;
		m_Collider.enabled = true;
		sprite.gameObject.SetActive(true);
		m_LowPassFilter.cutoffFrequency = m_DefaultCutOffFrequency;
		m_ShieldSet = false;
		m_ObstacleAnimator.SetBool("isShield", false);
		m_ObstacleAnimator.SetBool("isKillingYou", false);
		m_ObstacleAnimator.SetBool("isShot", false);
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
	}

	protected void UpdateHearableInFront(float currentPosition)
	{
		if (m_Visible && currentPosition < 1f && (int)obstacleType == 1 && m_ObstacleLane != m_CurrentLane)
		{
			sprite.gameObject.SetActive(false);
			m_Visible = false;
			if (m_ShieldSet)
			{
				m_ShieldSet = false;
				CustomGameManager.Instance.playerCollider.ShieldActivated(false);
			}
		}
		HandleLaneChange();
		AdjustVolume(currentPosition);
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

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 15)
		{
			CustomGameManager.Instance.IsZombieKilled();
			m_ObstacleAnimator.SetBool("isShot", true);
			m_Collider.enabled = false;
			m_Audio.Stop();
			m_Audio.loop = false;
			m_Audio.clip = isShotClip;
			m_Audio.Play();
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
		float t = (currentPosition - minDistance) / m_Distance;
		if (useLogarithmic)
		{
			t = Mathf.Pow(t, baseValue);
		}
		return 1f - t - m_laneVolume;
	}

	private void AdjustVolume(float currentPosition)
	{
		m_Audio.volume = GetDistanceVolume(currentPosition);
	}

	private void HandleLaneChange()
	{
		int currentLane = CustomGameManager.Instance.currentLane;
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
		m_CurrentLane = CustomGameManager.Instance.currentLane;
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
