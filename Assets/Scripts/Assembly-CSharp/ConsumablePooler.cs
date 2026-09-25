using System.Collections.Generic;
using UnityEngine;

public class ConsumablePooler : MonoBehaviour
{
	private static Dictionary<string, ConsumablePool> s_ConsumablePool = new Dictionary<string, ConsumablePool>();

	protected const int k_StartingPoolSize = 2;

	public ConsumablePool GetPool(Consumable consumable)
	{
		if (!s_ConsumablePool.ContainsKey(consumable.objectID))
		{
			CreatePool(consumable);
		}
		return s_ConsumablePool[consumable.objectID];
	}

	private void CreatePool(Consumable consumable)
	{
		ConsumablePool pool = new ConsumablePool(consumable, k_StartingPoolSize, transform);
		s_ConsumablePool.Add(consumable.objectID, pool);
	}

	public void ResetPools()
	{
		foreach (KeyValuePair<string, ConsumablePool> item in s_ConsumablePool)
		{
			item.Value.ResetPool();
		}
		s_ConsumablePool = new Dictionary<string, ConsumablePool>();
	}

	public void ClearPools()
	{
		foreach (KeyValuePair<string, ConsumablePool> item in s_ConsumablePool)
		{
			item.Value.ClearPool();
		}
		s_ConsumablePool = new Dictionary<string, ConsumablePool>();
	}
}
