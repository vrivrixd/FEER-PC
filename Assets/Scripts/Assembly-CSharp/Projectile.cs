using UnityEngine;

public class Projectile : MonoBehaviour
{
	public AudioSource shotSound;

	public Animator projectileAnimator;

	public Transform movingProjectile;

	public float flightLength;

	protected string m_ObjectID;

	[HideInInspector]
	public ProjectilePool pool;

	protected bool m_isResumed;

	protected GameStateName m_CurrentGameState;

	public string objectID => null;

	public void FreeProjectile()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	protected void UnpauseProjectile()
	{
	}

	protected void PauseProjectile()
	{
	}
}
