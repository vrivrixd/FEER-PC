using UnityEngine;

public class PlayerStumbleCollider : MonoBehaviour
{
	public AudioSource stumbleAudio;

	public PlayerCollider playerCollider;

	public MainCamera mainCamera;

	protected BoxCollider m_Collider;

	protected float m_StartingColliderHeight;

	protected readonly Vector3 k_SlidingColliderScale = new Vector3(1f, 0.5f, 1f);

	protected readonly Vector3 k_JumpingColliderScale = new Vector3(1f, 0.5f, 1f);

	protected readonly Vector3 k_DefaultColliderScale = new Vector3(1f, 2f, 1f);

	protected const int k_MonsterLayerIndex = 8;

	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	protected int m_MonsterEntered;

	protected int m_AirEntered;

	protected int m_GroundEntered;

	private void Start()
	{
		m_Collider = GetComponent<BoxCollider>();
		m_StartingColliderHeight = m_Collider.bounds.size.y;
	}

	public void ResetVariables()
	{
		m_MonsterEntered = 0;
		m_AirEntered = 0;
		m_GroundEntered = 0;
	}

	public void Slide(bool sliding)
	{
		if (sliding)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_SlidingColliderScale);
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * -0.5f, 0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * 0.5f, 0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_DefaultColliderScale);
		}
	}

	public void Jump(bool jumping)
	{
		if (jumping)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_JumpingColliderScale);
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * 0.5f, 0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0f, m_Collider.size.y * -0.5f, 0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_DefaultColliderScale);
		}
	}

	protected void OnTriggerExit(Collider c)
	{
		switch (c.gameObject.layer)
		{
		case k_AirLayerIndex:
			if (m_AirEntered < 1)
			{
				return;
			}
			m_AirEntered--;
			break;
		case k_GroundLayerIndex:
			if (m_GroundEntered < 1)
			{
				return;
			}
			m_GroundEntered--;
			break;
		case k_MonsterLayerIndex:
			if (m_MonsterEntered < 1)
			{
				return;
			}
			m_MonsterEntered--;
			break;
		default:
			return;
		}
		mainCamera.Stumble();
		stumbleAudio.panStereo = 0f;
		stumbleAudio.Play();
		if (DataManager.Instance.playerData.useVibration)
		{
			// PORT: no PC vibra o controle.
#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#else
			PortRumble.Stumble();
#endif
		}
	}

	private bool IsVisibleObstacle(Collider c)
	{
		Obstacle obstacle = c.gameObject.GetComponent<Obstacle>();
		if (obstacle == null)
		{
			obstacle = c.gameObject.GetComponentInParent<Obstacle>();
		}
		return obstacle.IsVisible();
	}

	protected void OnTriggerEnter(Collider c)
	{
		if (playerCollider.isShield || playerCollider.isBoost)
		{
			return;
		}
		switch (c.gameObject.layer)
		{
		case k_AirLayerIndex:
			if (IsVisibleObstacle(c))
			{
				m_AirEntered++;
			}
			break;
		case k_GroundLayerIndex:
			if (IsVisibleObstacle(c))
			{
				m_GroundEntered++;
			}
			break;
		case k_MonsterLayerIndex:
			if (IsVisibleObstacle(c))
			{
				m_MonsterEntered++;
			}
			break;
		}
	}
}
