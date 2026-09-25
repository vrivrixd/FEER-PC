using System.Collections.Generic;
using UnityEngine;

public class TrackPartPool
{
	protected Stack<TrackPart> m_FreeInstances;

	protected TrackPart m_Original;

	protected Transform m_PoolTransform;

	public TrackPartPool(TrackPart original, int initialSize, Transform parent)
	{
	}

	public TrackPart Get()
	{
		return null;
	}

	public TrackPart Get(Vector3 pos, Quaternion quat)
	{
		return null;
	}

	public void Free(TrackPart obj)
	{
	}

	public void ResetPool()
	{
	}

	public void ClearPool()
	{
	}
}
