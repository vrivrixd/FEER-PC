using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
	public PlayerCollider playerCollider;

	protected const int k_MonsterLayerIndex = 8;

	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	private void OnTriggerEnter(Collider c)
	{
	}
}
