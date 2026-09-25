using UnityEngine;

public class PlayerStumbleCollider : MonoBehaviour
{
	public AudioSource stumbleAudio;

	public PlayerCollider playerCollider;

	public MainCamera mainCamera;

	protected BoxCollider m_Collider;

	protected float m_StartingColliderHeight;

	protected readonly Vector3 k_SlidingColliderScale;

	protected readonly Vector3 k_JumpingColliderScale;

	protected readonly Vector3 k_DefaultColliderScale;

	protected const int k_MonsterLayerIndex = 8;

	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	protected int m_MonsterEntered;

	protected int m_AirEntered;

	protected int m_GroundEntered;

	private void Start()
	{
	}

	public void ResetVariables()
	{
	}

	public void Slide(bool sliding)
	{
	}

	public void Jump(bool jumping)
	{
	}

	protected void OnTriggerExit(Collider c)
	{
	}

	protected void OnTriggerEnter(Collider c)
	{
	}
}
