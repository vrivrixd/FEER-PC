using System.Collections;
using UnityEngine;

public class ObstacleSimple : Obstacle
{
	public GameObject sprite;

	public float minDistance;

	public float maxDistance;

	public float distanceAfter;

	public float baseValue;

	public bool useLogarithmic;

	public Animator m_ObstacleAnimator;

	public AudioClip walkingClip;

	public AudioClip isShotClip;

	protected Transform m_Transform;

	protected int m_ObstacleLane;

	protected int m_CurrentLane;

	protected AudioSource m_Audio;

	protected AudioLowPassFilter m_LowPassFilter;

	protected int m_TargetPan;

	protected Coroutine m_ChangePan;

	protected float m_Distance;

	protected float m_laneVolume;

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

	protected void SetObstacleLane()
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

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
	}

	protected void DampAudio(float currentPosition)
	{
	}

	protected void StopObstacleSoundAndAnimation()
	{
	}

	private void AdjustVolume(float currentPosition)
	{
	}

	private void HandleLaneChange()
	{
	}

	private void SetLaneVolume()
	{
	}

	private void StartObstacleSound(float currentPosition)
	{
	}

	private void ChangePan()
	{
	}

	private IEnumerator IEChangePan()
	{
		return null;
	}
}
