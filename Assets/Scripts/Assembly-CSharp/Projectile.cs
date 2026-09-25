using UnityEngine;

public class Projectile : MonoBehaviour
{
	public AudioSource shotSound;

	public Animator projectileAnimator;

	public Transform movingProjectile;

	public float flightLength = 10f;

	protected string m_ObjectID = System.Guid.NewGuid().ToString();

	[HideInInspector]
	public ProjectilePool pool;

	protected bool m_isResumed;

	protected GameStateName m_CurrentGameState;

	public string objectID => m_ObjectID;

	public void FreeProjectile()
	{
		m_isResumed = false;
		projectileAnimator.speed = 1f;
		shotSound.Stop();
		m_CurrentGameState = GameStateName.None;
		pool.Free(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
		GameStateName name = CustomGameManager.Instance.topState.GetName();
		if (name != m_CurrentGameState)
		{
			if (name == GameStateName.Pause)
			{
				if (m_CurrentGameState == GameStateName.PlayGame)
				{
					PauseProjectile();
				}
			}
			else if (name == GameStateName.PlayGame)
			{
				if (m_CurrentGameState == GameStateName.Pause && CustomGameManager.Instance.topState.GetStatus() == GameStateStatus.Resume)
				{
					m_isResumed = true;
				}
			}
			else
			{
				gameObject.SetActive(false);
				FreeProjectile();
			}
			m_CurrentGameState = name;
		}
		if (name != GameStateName.PlayGame)
		{
			return;
		}
		if (m_isResumed)
		{
			if (CustomGameManager.Instance.topState.GetStatus() == GameStateStatus.Running)
			{
				m_isResumed = false;
				UnpauseProjectile();
			}
		}
		else if (!(movingProjectile.position.z < flightLength))
		{
			gameObject.SetActive(false);
			FreeProjectile();
		}
	}

	protected void UnpauseProjectile()
	{
		projectileAnimator.speed = 1f;
		shotSound.UnPause();
	}

	protected void PauseProjectile()
	{
		projectileAnimator.speed = 0f;
		shotSound.Pause();
	}
}
