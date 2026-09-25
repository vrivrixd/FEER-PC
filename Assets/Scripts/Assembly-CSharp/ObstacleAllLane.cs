using UnityEngine;

public class ObstacleAllLane : Obstacle
{
	public GameObject sprite;

	public Animator m_ObstacleAnimator;

	public float minDistance;

	public float maxDistance;

	public float distanceAfter;

	protected AudioSource m_Audio;

	protected AudioLowPassFilter m_LowPassFilter;

	protected Transform m_Transform;

	protected float m_Distance;

	protected Collider m_Collider;

	protected float m_DefaultCutOffFrequency;

	protected bool m_ShieldSet;

	protected bool m_Visible;

	protected bool m_Init;

	public override bool IsVisible()
	{
		return false;
	}

	public override void Impact()
	{
	}

	public override void ShieldEntered()
	{
	}

	protected override void SwitchStateTo(int state)
	{
	}

	protected override void PushState(int state)
	{
	}

	protected override void PopState()
	{
	}

	protected override void EnterState()
	{
	}

	protected void StopObstacleSoundAndAnimation()
	{
	}

	protected void StartObstacleSound()
	{
	}

	public override void Spawn(TrackManager trackManager)
	{
	}

	public override void FreeObstacle()
	{
	}

	private void Start()
	{
	}

	private void Init()
	{
	}

	protected void UpdateHearableInFront(float currentPosition)
	{
	}

	protected void DampAudio(float currentPosition)
	{
	}

	private void Update()
	{
	}
}
