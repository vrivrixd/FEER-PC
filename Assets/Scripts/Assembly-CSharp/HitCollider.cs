using UnityEngine;

public class HitCollider : MonoBehaviour
{
	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	protected const int k_CollectiblesLayerIndex = 13;

	protected const int k_ConsumablesLayerIndex = 14;

	protected void OnTriggerEnter(Collider c)
	{
		switch (c.gameObject.layer)
		{
		case k_CollectiblesLayerIndex:
			CustomGameManager.Instance.CollectedGhost();
			break;
		case k_AirLayerIndex:
			CustomGameManager.Instance.SlidedUnderRavens();
			break;
		case k_GroundLayerIndex:
			CustomGameManager.Instance.JumpedOverHands();
			break;
		}
	}

	public void ConsumableCollected(ConsumableType consumable)
	{
		switch ((int)consumable)
		{
		case 1:
			CustomGameManager.Instance.BoostCollected();
			break;
		case 2:
			CustomGameManager.Instance.CoinDoublerCollected();
			break;
		case 3:
			CustomGameManager.Instance.ShieldCollected();
			break;
		case 4:
			CustomGameManager.Instance.WeaponCollected();
			break;
		}
	}
}
