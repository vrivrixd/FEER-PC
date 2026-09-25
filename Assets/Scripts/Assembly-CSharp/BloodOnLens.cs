using System.Collections;
using UnityEngine;

public class BloodOnLens : MonoBehaviour
{
	public ParticleSystem bloodParticles;

	protected float m_zPosition;

	protected Vector3 m_TargetPosition;

	protected float m_BloodFreezingTime;

	public float bloodFreezingTime => 0f;

	private void Start()
	{
	}

	public void StopBloodOnLens()
	{
	}

	public void StartBloodOnLens()
	{
	}

	public void FreezeBloodOnLens()
	{
	}

	public void MoveBloodOnLens()
	{
	}

	private IEnumerator IEMoveBlood()
	{
		return null;
	}
}
