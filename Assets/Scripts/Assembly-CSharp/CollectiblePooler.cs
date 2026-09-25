using System.Collections.Generic;
using UnityEngine;

public class CollectiblePooler : MonoBehaviour
{
	private static Dictionary<string, CollectiblePool> s_CollectiblePool;

	protected const int k_StartingPoolSize = 2;

	public CollectiblePool GetPool(Collectible collectible)
	{
		return null;
	}

	private void CreatePool(Collectible collectible)
	{
	}

	public void ResetPools()
	{
	}

	public void ClearPools()
	{
	}
}
