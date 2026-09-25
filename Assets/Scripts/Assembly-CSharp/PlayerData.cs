using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
	public bool showAppRateDialog;

	public int updateNewVersionInfoMessageCount;

	public string userId;

	public int timesSubmitHighscoreAsked;

	public string highscoreNickname;

	public SystemLanguage userLanguage;

	public bool useReverseLeftRight;

	public bool useReverseUpDown;

	public bool useCenterLaneOrientation;

	public bool useVibration;

	public DateTime installationTime;

	public DateTime sessionStartTime;

	public long sessionCount;

	public long sessionId;

	public bool playTutorial;

	public bool isFirstGame;

	public bool isFirstAccesibleGameEver;

	public bool isFirstAccessiblePowerUp;

	public bool isFirstAccessiblePowerUpCoinDoubler;

	public bool isFirstAccessiblePowerUpBoost;

	public bool isFirstAccessiblePowerUpShield;

	public bool isFirstAccessiblePowerUpWeapon;

	public int highscore;

	public int scoreMultiplier;

	public int coins;

	public int boostDistance;

	public int boostLevel;

	public int coinMultiplierLevel;

	public int weaponLevel;

	public int coinMultiplierDistance;

	public int shieldLevel;

	public int shieldDistance;

	public int weaponDistance;

	public int lastRewardedMissionNumber;
}
