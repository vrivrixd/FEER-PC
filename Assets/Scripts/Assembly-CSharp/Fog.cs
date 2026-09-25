using UnityEngine;

public class Fog : MonoBehaviour
{
	public Transform parentTransform;

	public ParticleSystem staticBackgroundFog;

	public Transform[] fogElements;

	public ParticleSystem[] fogParticleSystem;

	protected bool m_IsMoving;

	protected float m_SpeedScaleFactor;

	protected float m_SpeedMoveFactor;

	private void Update()
	{
	}

	public void MoveFog()
	{
	}

	public void PauseFog()
	{
	}

	public void UnpauseFog(bool resetFog = true)
	{
	}

	public void SlowDownFog()
	{
	}

	public void SpeedUpFog()
	{
	}

	public void ResetFog()
	{
	}

	public void ColorizeFog(ConsumableType consumable)
	{
	}

	public void HideFog()
	{
	}

	public void ShowFog()
	{
	}
}
