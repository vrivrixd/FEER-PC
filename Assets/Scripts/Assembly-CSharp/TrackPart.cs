using UnityEngine;

public abstract class TrackPart : MonoBehaviour
{
	protected string m_ObjectID;

	public TrackPartType trackPartType;

	public Transform entryPoint;

	public Transform exitPoint;

	public Transform collectiblesTransform;

	public Transform obstaclesTransform;

	public Transform consumablesTransform;

	[HideInInspector]
	public TrackPartPool pool;

	public string objectID => null;

	public float trackLength => 0f;

	public void TranslatePart(Vector3 translation)
	{
	}

	public void CleanUp()
	{
	}

	public abstract void SpawnElements();
}
