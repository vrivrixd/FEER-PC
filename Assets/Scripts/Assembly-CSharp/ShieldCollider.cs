using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
	public PlayerCollider playerCollider;

	protected const int k_MonsterLayerIndex = 8;

	protected const int k_GroundLayerIndex = 9;

	protected const int k_AirLayerIndex = 10;

	private void OnTriggerEnter(Collider c)
	{
		int layer = c.gameObject.layer;
		if ((uint)(layer - 8) > 2u)
		{
			return;
		}
		Obstacle obstacle = c.gameObject.GetComponent<Obstacle>();
		if (obstacle == null)
		{
			obstacle = c.GetComponentInParent<Obstacle>();
		}
		obstacle.ShieldEntered();
		playerCollider.ShieldActivated(true);
	}
}
