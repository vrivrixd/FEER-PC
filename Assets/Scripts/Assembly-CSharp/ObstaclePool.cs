using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool
{
	protected Stack<Obstacle> m_FreeInstances = new Stack<Obstacle>();

	protected Obstacle m_Original;

	protected Transform m_PoolTransform;

	public ObstaclePool(Obstacle original, int initialSize, Transform parent)
	{
		m_Original = original;
		m_PoolTransform = parent;
		m_FreeInstances = new Stack<Obstacle>(initialSize);
		for (int i = 0; i < initialSize; i++)
		{
			Obstacle obj = Object.Instantiate(original);
			obj.transform.SetParent(m_PoolTransform);
			obj.gameObject.SetActive(false);
			m_FreeInstances.Push(obj);
		}
	}

	public Obstacle Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

	public Obstacle Get(Vector3 pos, Quaternion quat)
	{
		Obstacle obj = (m_FreeInstances.Count > 0) ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);
		obj.gameObject.SetActive(true);
		obj.transform.position = pos;
		obj.transform.rotation = quat;
		return obj;
	}

	public void Free(Obstacle obj)
	{
		obj.transform.SetParent(m_PoolTransform);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Push(obj);
	}

	public void ResetPool()
	{
		m_FreeInstances = new Stack<Obstacle>();
	}

	public void ClearPool()
	{
		while (m_FreeInstances.Count > 0)
		{
			Object.DestroyImmediate(m_FreeInstances.Pop().gameObject);
		}
		m_FreeInstances = new Stack<Obstacle>();
	}
}
