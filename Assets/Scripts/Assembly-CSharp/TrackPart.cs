using UnityEngine;

public abstract class TrackPart : MonoBehaviour
{
	protected string m_ObjectID = System.Guid.NewGuid().ToString();

	public TrackPartType trackPartType;

	public Transform entryPoint;

	public Transform exitPoint;

	public Transform collectiblesTransform;

	public Transform obstaclesTransform;

	public Transform consumablesTransform;

	[HideInInspector]
	public TrackPartPool pool;

	public string objectID => m_ObjectID;

	public float trackLength => exitPoint.localPosition.z - entryPoint.localPosition.z;

	public void TranslatePart(Vector3 translation)
	{
		transform.Translate(translation);
	}

	public void CleanUp()
	{
		Obstacle[] obstacles = obstaclesTransform.GetComponentsInChildren<Obstacle>();
		for (int i = 0; i < obstacles.Length; i++)
		{
			obstacles[i].FreeObstacle();
		}
		Collectible[] collectibles = collectiblesTransform.GetComponentsInChildren<Collectible>();
		for (int j = 0; j < collectibles.Length; j++)
		{
			collectibles[j].FreeCollectible();
		}
		Consumable[] consumables = consumablesTransform.GetComponentsInChildren<Consumable>();
		for (int k = 0; k < consumables.Length; k++)
		{
			consumables[k].FreeConsumable();
		}
		pool.Free(this);
	}

	public abstract void SpawnElements();
}
