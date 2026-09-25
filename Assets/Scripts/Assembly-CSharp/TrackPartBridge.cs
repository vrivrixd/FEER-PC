using UnityEngine;

public class TrackPartBridge : TrackPart
{
	public Transform bridgeTransform;

	public Transform hole1Transform;

	public Transform hole2Transform;

	public override void SpawnElements()
	{
		switch (Random.Range(0, 3))
		{
		case 0:
			bridgeTransform.localPosition = new Vector3(-1f, 0f, 0f);
			hole1Transform.localPosition = Vector3.zero;
			hole2Transform.localPosition = new Vector3(1f, 0f, 0f);
			break;
		case 1:
			hole1Transform.localPosition = new Vector3(-1f, 0f, 0f);
			bridgeTransform.localPosition = Vector3.zero;
			hole2Transform.localPosition = new Vector3(1f, 0f, 0f);
			break;
		case 2:
			hole1Transform.localPosition = new Vector3(-1f, 0f, 0f);
			hole2Transform.localPosition = Vector3.zero;
			bridgeTransform.localPosition = new Vector3(1f, 0f, 0f);
			break;
		}
	}
}
