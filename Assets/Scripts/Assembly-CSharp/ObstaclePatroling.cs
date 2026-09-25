using System.Collections;
using UnityEngine;

public class ObstaclePatroling : Obstacle
{
	public GameObject sprite;

	public Collider objectCollider;

	protected Transform m_PatrolingTransform;

	public float maxXPatroling;

	public Animator m_ObstacleAnimator;

	public float minDistance = 1f;

	public float maxDistance = 12f;

	public float distanceAfter = 2f;

	protected AudioSource m_Audio;

	protected AudioLowPassFilter m_LowPassFilter;

	protected Transform m_Transform;

	protected float m_Distance;

	protected float m_DefaultCutOffFrequency;

	protected bool m_ShieldSet;

	protected bool m_Visible = true;

	protected bool m_Init;

	protected float m_playerPositionCorrVal;

	protected int m_CurrentLane = 1;

	protected Coroutine m_CorrPosRoutine;

	protected Vector3 m_TargetPosition;

	public float patrolingSpeed = 1f;

	public override bool IsVisible()
	{
		return m_Visible;
	}

	public override void Impact()
	{
		objectCollider.enabled = false;
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
		{
			float random = Random.Range(-1f, 1f);
			Vector3 localPosition = m_PatrolingTransform.localPosition;
			m_TargetPosition = new Vector3((random <= 0f) ? (0f - maxXPatroling) : maxXPatroling, localPosition.y, m_PatrolingTransform.localPosition.z);
			m_ObstacleAnimator.speed = 1f;
			break;
		}
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
		m_CurrentLane = CustomGameManager.Instance.m_CurrentLane;
		m_Audio.panStereo = GetPan();
		float z = m_Transform.position.z;
		m_Audio.volume = (z <= minDistance) ? 1f : (1f - (m_Transform.position.z - minDistance) / m_Distance);
		m_Audio.Play();
	}

	public override void Spawn(TrackManager trackManager)
	{
		m_TrackManager = trackManager;
		if (!m_Init)
		{
			Init();
		}
		float random = Random.Range(-1f, 1f);
		Vector3 localPosition = m_PatrolingTransform.localPosition;
		localPosition.x = random * maxXPatroling;
		m_PatrolingTransform.localPosition = localPosition;
		SwitchStateTo(c_STATE_SPAWNED);
	}

	public override void FreeObstacle()
	{
		if (!m_Init)
		{
			Init();
		}
		if (m_CorrPosRoutine != null)
		{
			StopCoroutine(m_CorrPosRoutine);
		}
		m_Audio.Stop();
		m_Audio.panStereo = 0f;
		m_Visible = true;
		objectCollider.enabled = true;
		sprite.gameObject.SetActive(true);
		m_LowPassFilter.cutoffFrequency = m_DefaultCutOffFrequency;
		m_ShieldSet = false;
		m_playerPositionCorrVal = 0f;
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
		m_DefaultCutOffFrequency = m_LowPassFilter.cutoffFrequency;
		m_PatrolingTransform = objectCollider.gameObject.GetComponent<Transform>();
		m_Init = true;
	}

	private float GetPan()
	{
		float pan = (m_PatrolingTransform.localPosition.x + m_playerPositionCorrVal) / maxXPatroling;
		if (pan >= 1f)
		{
			return 1f;
		}
		if (pan <= -1f)
		{
			return -1f;
		}
		return pan;
	}

	protected void HandleLaneChange()
	{
		int currentLane = CustomGameManager.Instance.m_CurrentLane;
		if (currentLane != m_CurrentLane)
		{
			m_CurrentLane = currentLane;
			if (m_CorrPosRoutine != null)
			{
				StopCoroutine(m_CorrPosRoutine);
			}
			m_CorrPosRoutine = StartCoroutine(ChangeXCorrValue());
		}
	}

	private IEnumerator ChangeXCorrValue()
	{
		float targetVal = 0f;
		if (m_CurrentLane == 2)
		{
			targetVal = 0f - maxXPatroling;
		}
		else if (m_CurrentLane == 0)
		{
			targetVal = maxXPatroling;
		}
		for (float t = 0f; t < 1f; t += Time.deltaTime)
		{
			m_playerPositionCorrVal = Mathf.Lerp(m_playerPositionCorrVal, targetVal, t);
			yield return null;
		}
		m_playerPositionCorrVal = targetVal;
	}

	protected void UpdateHearableInFront(float currentZPosition)
	{
		float currentPosition = currentZPosition;
		Patrole();
		m_Audio.volume = (currentPosition <= minDistance) ? 1f : (1f - (currentPosition - minDistance) / m_Distance);
		HandleLaneChange();
		m_Audio.panStereo = GetPan();
		if (!m_Visible)
		{
			return;
		}
		if (currentPosition >= 1f)
		{
			if ((CustomGameManager.Instance.m_isSliding && obstacleType == SpawnElementType.PatrolingObstacleAir) || (CustomGameManager.Instance.m_isJumping && obstacleType == SpawnElementType.PatrolingObstacleGround))
			{
				transform.Translate(0f, 0f, Time.deltaTime * CustomGameManager.Instance.m_Speed * -1.3f);
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
			Patrole();
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

	protected void Patrole()
	{
		m_PatrolingTransform.localPosition = Vector3.MoveTowards(m_PatrolingTransform.localPosition, m_TargetPosition, Time.deltaTime * (patrolingSpeed + CustomGameManager.Instance.m_Speed * 0.25f));
		float x = m_PatrolingTransform.localPosition.x;
		if (x <= 0f - (maxXPatroling - 0.01f))
		{
			m_TargetPosition = new Vector3(maxXPatroling, m_PatrolingTransform.localPosition.y, m_PatrolingTransform.localPosition.z);
		}
		else if (!(m_PatrolingTransform.localPosition.x < maxXPatroling - 0.01f))
		{
			m_TargetPosition = new Vector3(0f - maxXPatroling, m_PatrolingTransform.localPosition.y, m_PatrolingTransform.localPosition.z);
		}
	}
}
