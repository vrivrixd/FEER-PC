using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool
{
	protected Stack<Projectile> m_FreeInstances;

	protected Projectile m_Original;

	protected Transform m_PoolTransform;

	public ProjectilePool(Projectile original, int initialSize, Transform parent)
	{
	}

	public Projectile Get()
	{
		return null;
	}

	public Projectile Get(Vector3 pos, Quaternion quat)
	{
		return null;
	}

	public void Free(Projectile obj)
	{
	}

	public void ResetPool()
	{
	}

	public void ClearPool()
	{
	}
}
