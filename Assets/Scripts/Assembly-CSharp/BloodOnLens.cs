using System.Collections;
using UnityEngine;

public class BloodOnLens : MonoBehaviour
{
	public ParticleSystem bloodParticles;

	protected float m_zPosition;

	protected Vector3 m_TargetPosition;

	protected float m_BloodFreezingTime = 0.8f;

	public float bloodFreezingTime => m_BloodFreezingTime;

	private void Start()
	{
		bloodParticles.Stop();
		bloodParticles.Clear();
	}

	public void StopBloodOnLens()
	{
		bloodParticles.Stop();
		bloodParticles.Clear();
		Vector3 localPosition = bloodParticles.transform.localPosition;
		localPosition.z = m_zPosition;
		bloodParticles.transform.localPosition = localPosition;
	}

	public void StartBloodOnLens()
	{
		if (!bloodParticles.isPlaying)
		{
			bloodParticles.Play();
		}
		Invoke("FreezeBloodOnLens", m_BloodFreezingTime);
	}

	public void FreezeBloodOnLens()
	{
		if (bloodParticles.isPlaying && !bloodParticles.isPaused)
		{
			bloodParticles.Pause();
		}
	}

	public void MoveBloodOnLens()
	{
		bloodParticles.Pause();
		m_zPosition = bloodParticles.transform.localPosition.z;
		m_TargetPosition = new Vector3(bloodParticles.transform.localPosition.x, bloodParticles.transform.localPosition.y, 0f);
		StartCoroutine(IEMoveBlood());
	}

	private IEnumerator IEMoveBlood()
	{
		while (0.1f < bloodParticles.transform.localPosition.z)
		{
			bloodParticles.transform.localPosition = Vector3.MoveTowards(bloodParticles.transform.localPosition, m_TargetPosition, Time.deltaTime * 8f);
			yield return null;
		}
		bloodParticles.Stop();
		bloodParticles.Clear();
		Vector3 localPosition = bloodParticles.transform.localPosition;
		localPosition.z = m_zPosition;
		bloodParticles.transform.localPosition = localPosition;
	}
}
