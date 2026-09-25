using UnityEngine;

public class HitCollider : MonoBehaviour
{
	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	protected const int k_CollectiblesLayerIndex = 13;

	protected const int k_ConsumablesLayerIndex = 14;

	protected void OnTriggerEnter(Collider c)
	{
	}

	public void ConsumableCollected(ConsumableType consumable)
	{
	}
}
