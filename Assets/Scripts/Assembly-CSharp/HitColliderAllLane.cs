using UnityEngine;

public class HitColliderAllLane : MonoBehaviour
{
	protected const int k_MonsterLayerIndex = 8;

	protected const int k_CollectiblesLayerIndex = 13;

	protected const int k_ConsumablesLayerIndex = 14;

	protected const int k_HoleLayerIndex = 16;

	protected void OnTriggerEnter(Collider c)
	{
		switch (c.gameObject.layer)
		{
		case 8:
			CustomGameManager.Instance.IsZombieDodged();
			break;
		case 13:
			CustomGameManager.Instance.IncreaseGhostSpawned();
			break;
		case 14:
			switch (c.GetComponent<Consumable>().consumableType)
			{
			case ConsumableType.Boost:
				CustomGameManager.Instance.IncreasePowerUpBoostSpawned();
				break;
			case ConsumableType.CoinMultiplier:
				CustomGameManager.Instance.IncreasePowerUpCoinDoublerSpawned();
				break;
			case ConsumableType.Shield:
				CustomGameManager.Instance.IncreasePowerUpShieldSpawned();
				break;
			case ConsumableType.Weapon:
				CustomGameManager.Instance.IncreasePowerUpWeaponSpawned();
				break;
			}
			break;
		case 16:
			CustomGameManager.Instance.IsHoleSurvived();
			break;
		}
	}
}
