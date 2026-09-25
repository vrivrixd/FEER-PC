using System.Collections.Generic;
using UnityEngine;

public class ConsumablePooler : MonoBehaviour
{
	private static Dictionary<string, ConsumablePool> s_ConsumablePool;

	protected const int k_StartingPoolSize = 2;

	public ConsumablePool GetPool(Consumable consumable)
	{
		return null;
	}

	private void CreatePool(Consumable consumable)
	{
	}

	public void ResetPools()
	{
	}

	public void ClearPools()
	{
	}
}
