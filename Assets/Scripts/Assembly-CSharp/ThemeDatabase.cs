using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class ThemeDatabase : ScriptableObject
{
	public AudioClip backgroundSound;

	public AudioMixerSnapshot defaultSnapshot;

	public AudioMixerSnapshot mutedSnapshot;

	public AudioClip runClip;

	public MissionLevels[] themeLevels;

	public Material cameraBackground;

	public TrackPart[] trackParts;

	public TrackPart[] trackPartsSpecial;

	public Obstacle[] zombiePrefabs;

	public Obstacle[] airPrefabs;

	public Obstacle[] groundPrefabs;

	public AudioClip tutorialSound;

	public MissionLevels tutorialLevel;

	public ThemeTutorial tutorialScript;
}
