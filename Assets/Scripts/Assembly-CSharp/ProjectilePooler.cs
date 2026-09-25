using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
	private static Dictionary<string, ProjectilePool> s_ProjectilePool;

	protected const int k_StartingPoolSize = 2;

	public ProjectilePool GetPool(Projectile projectile)
	{
		return null;
	}

	private void CreatePool(Projectile projectile)
	{
	}

	public void ResetPools()
	{
	}

	public void ClearPools()
	{
	}
}
