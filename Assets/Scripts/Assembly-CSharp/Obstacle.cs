using UnityEngine;
using UnityEngine.Audio;

public abstract class Obstacle : MonoBehaviour
{
	protected string m_ObjectID;

	[HideInInspector]
	public ObstaclePool pool;

	protected TrackManager m_TrackManager;

	protected int[] m_StateStack;

	protected const int c_STATE_SPAWNED = 0;

	protected const int c_STATE_HEARABLE_IN_FRONT = 1;

	protected const int c_STATE_HEARABLE_BEHIND = 2;

	protected const int c_STATE_EXIT = 3;

	protected const int c_STATE_PAUSE = 4;

	protected const int c_STATE_NONE = 5;

	protected const int c_CURRENT_STATE = 0;

	public SpawnElementType obstacleType;

	public AudioClip killsPlayerClip;

	public AudioMixerGroup killsPlayerOutput;

	public float bloodSecondsToWait;

	public bool hideFog;

	public string objectID => null;

	protected abstract void SwitchStateTo(int state);

	protected abstract void PushState(int state);

	protected abstract void PopState();

	protected abstract void EnterState();

	public abstract void FreeObstacle();

	public abstract void Spawn(TrackManager trackManager);

	public abstract void ShieldEntered();

	public abstract void Impact();

	public abstract bool IsVisible();
}
