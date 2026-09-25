using UnityEngine;
using UnityEngine.Audio;

public class DeathSettings : MonoBehaviour
{
	public AudioClip deathSound;

	public AudioMixerGroup deathOutput;

	public float bloodSecondsToWait;

	public bool hideFog;
}
