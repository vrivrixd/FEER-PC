using System.Collections.Generic;
using UnityEngine;

public class CollectiblePooler : MonoBehaviour
{
	private static Dictionary<string, CollectiblePool> s_CollectiblePool = new Dictionary<string, CollectiblePool>();

	protected const int k_StartingPoolSize = 2;

	public CollectiblePool GetPool(Collectible collectible)
	{
		if (!s_CollectiblePool.ContainsKey(collectible.objectID))
		{
			CreatePool(collectible);
		}
		return s_CollectiblePool[collectible.objectID];
	}

	private void CreatePool(Collectible collectible)
	{
		CollectiblePool pool = new CollectiblePool(collectible, k_StartingPoolSize, transform);
		s_CollectiblePool.Add(collectible.objectID, pool);
	}

	public void ResetPools()
	{
		foreach (KeyValuePair<string, CollectiblePool> item in s_CollectiblePool)
		{
			item.Value.ResetPool();
		}
		s_CollectiblePool = new Dictionary<string, CollectiblePool>();
	}

	public void ClearPools()
	{
		foreach (KeyValuePair<string, CollectiblePool> item in s_CollectiblePool)
		{
			item.Value.ClearPool();
		}
		s_CollectiblePool = new Dictionary<string, CollectiblePool>();
	}
}
