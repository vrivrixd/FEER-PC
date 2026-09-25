using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool
{
	protected Stack<Projectile> m_FreeInstances = new Stack<Projectile>();

	protected Projectile m_Original;

	protected Transform m_PoolTransform;

	public ProjectilePool(Projectile original, int initialSize, Transform parent)
	{
		m_Original = original;
		m_PoolTransform = parent;
		m_FreeInstances = new Stack<Projectile>(initialSize);
		for (int i = 0; i < initialSize; i++)
		{
			Projectile obj = Object.Instantiate(original);
			obj.transform.SetParent(m_PoolTransform);
			obj.gameObject.SetActive(false);
			m_FreeInstances.Push(obj);
		}
	}

	public Projectile Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

	public Projectile Get(Vector3 pos, Quaternion quat)
	{
		Projectile obj = (m_FreeInstances.Count > 0) ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);
		obj.gameObject.SetActive(true);
		obj.transform.position = pos;
		obj.transform.rotation = quat;
		return obj;
	}

	public void Free(Projectile obj)
	{
		obj.transform.SetParent(m_PoolTransform);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Push(obj);
	}

	public void ResetPool()
	{
		m_FreeInstances = new Stack<Projectile>();
	}

	public void ClearPool()
	{
		while (m_FreeInstances.Count > 0)
		{
			Object.DestroyImmediate(m_FreeInstances.Pop().gameObject);
		}
		m_FreeInstances = new Stack<Projectile>();
	}
}
