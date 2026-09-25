using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
	protected string m_ObjectID = System.Guid.NewGuid().ToString();

	[HideInInspector]
	public CollectiblePool pool;

	public CollectibleType collectibleType;

	protected TrackManager m_TrackManager;

	protected int[] m_StateStack = new int[2];

	protected const int c_STATE_SPAWNED = 0;

	protected const int c_STATE_HEARABLE_IN_FRONT = 1;

	protected const int c_STATE_HEARABLE_BEHIND = 2;

	protected const int c_STATE_EXIT = 3;

	protected const int c_STATE_PAUSE = 4;

	protected const int c_STATE_NONE = 5;

	protected const int c_CURRENT_STATE = 0;

	public string objectID => m_ObjectID;

	protected abstract void SwitchStateTo(int state);

	protected abstract void PushState(int state);

	protected abstract void PopState();

	protected abstract void EnterState();

	public abstract void FreeCollectible();

	public abstract void Spawn(TrackManager trackManager);
}
