using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
	private static Dictionary<string, ProjectilePool> s_ProjectilePool = new Dictionary<string, ProjectilePool>();

	protected const int k_StartingPoolSize = 2;

	public ProjectilePool GetPool(Projectile projectile)
	{
		if (!s_ProjectilePool.ContainsKey(projectile.m_ObjectID))
		{
			CreatePool(projectile);
		}
		return s_ProjectilePool[projectile.m_ObjectID];
	}

	private void CreatePool(Projectile projectile)
	{
		ProjectilePool pool = new ProjectilePool(projectile, k_StartingPoolSize, transform);
		s_ProjectilePool.Add(projectile.m_ObjectID, pool);
	}

	public void ResetPools()
	{
		foreach (KeyValuePair<string, ProjectilePool> item in s_ProjectilePool)
		{
			item.Value.ResetPool();
		}
		s_ProjectilePool = new Dictionary<string, ProjectilePool>();
	}

	public void ClearPools()
	{
		foreach (KeyValuePair<string, ProjectilePool> item in s_ProjectilePool)
		{
			item.Value.ClearPool();
		}
		s_ProjectilePool = new Dictionary<string, ProjectilePool>();
	}
}
