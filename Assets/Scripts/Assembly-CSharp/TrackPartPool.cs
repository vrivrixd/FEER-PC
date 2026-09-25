using System.Collections.Generic;
using UnityEngine;

public class TrackPartPool
{
	protected Stack<TrackPart> m_FreeInstances = new Stack<TrackPart>();

	protected TrackPart m_Original;

	protected Transform m_PoolTransform;

	public TrackPartPool(TrackPart original, int initialSize, Transform parent)
	{
		m_Original = original;
		m_PoolTransform = parent;
		m_FreeInstances = new Stack<TrackPart>(initialSize);
		for (int i = 0; i < initialSize; i++)
		{
			TrackPart obj = Object.Instantiate(original);
			obj.transform.SetParent(m_PoolTransform);
			obj.gameObject.SetActive(false);
			m_FreeInstances.Push(obj);
		}
	}

	public TrackPart Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

	public TrackPart Get(Vector3 pos, Quaternion quat)
	{
		TrackPart obj = (m_FreeInstances.Count > 0) ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);
		obj.gameObject.SetActive(true);
		obj.transform.position = pos;
		obj.transform.rotation = quat;
		return obj;
	}

	public void Free(TrackPart obj)
	{
		obj.transform.SetParent(m_PoolTransform);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Push(obj);
	}

	public void ResetPool()
	{
		m_FreeInstances = new Stack<TrackPart>();
	}

	public void ClearPool()
	{
		while (m_FreeInstances.Count > 0)
		{
			Object.DestroyImmediate(m_FreeInstances.Pop().gameObject);
		}
		m_FreeInstances = new Stack<TrackPart>();
	}
}
