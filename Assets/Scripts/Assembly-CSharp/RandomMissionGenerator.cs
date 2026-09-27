using System.Collections.Generic;
using UnityEngine;

public static class RandomMissionGenerator
{
	// Goal tables from the original (static arrays in <PrivateImplementationDetails>, extracted from global-metadata)
	private static float[] Seq(float start, float step, int count)
	{
		float[] array = new float[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = start + step * (float)i;
		}
		return array;
	}

	private static float Pick(float[] values)
	{
		return values[Random.Range(0, values.Length)];
	}

	private static void RemoveTypes(List<MissionType> list, params MissionType[] types)
	{
		for (int i = 0; i < types.Length; i++)
		{
			if (list.Contains(types[i]))
			{
				list.Remove(types[i]);
			}
		}
	}

	public static Mission[] GetRandomMissionSet(int scoreMultiplier)
	{
		List<MissionType> list = new List<MissionType>();
		list.Add(MissionType.ChangeLane);
		list.Add(MissionType.CollectGhosts);
		list.Add(MissionType.CollectGhostsWithoutPowerUps);
		list.Add(MissionType.DodgeZombies);
		list.Add(MissionType.KillZombies);
		list.Add(MissionType.Jump);
		list.Add(MissionType.JumpCenter);
		list.Add(MissionType.JumpLeft);
		list.Add(MissionType.JumpRight);
		list.Add(MissionType.JumpOver);
		list.Add(MissionType.PlayGame);
		list.Add(MissionType.PowerUp);
		list.Add(MissionType.PowerUpBoost);
		list.Add(MissionType.PowerUpShield);
		list.Add(MissionType.PowerUpCoinDoubler);
		list.Add(MissionType.PowerUpWeapon);
		list.Add(MissionType.SaveMe);
		list.Add(MissionType.Score);
		list.Add(MissionType.ScoreWithoutPowerUps);
		list.Add(MissionType.ScoreWithoutCollectingGhosts);
		list.Add(MissionType.Slide);
		list.Add(MissionType.SlideLeft);
		list.Add(MissionType.SlideRight);
		list.Add(MissionType.SlideCenter);
		list.Add(MissionType.SlideUnder);
		Mission[] array = new Mission[3];
		float multiplier = scoreMultiplier;
		for (int i = 0; i < array.Length; i++)
		{
			MissionType item = list[Random.Range(0, list.Count)];
			int multiRun = Random.Range(0, 2);
			MissionScope scope = (multiRun != 0) ? MissionScope.MultiRun : MissionScope.SingleRun;
			bool single = multiRun == 0;
			switch (item)
			{
			case MissionType.JumpOver:
				array[i] = new MissionJumpOver(scope, Pick(single ? Seq(20f, 1f, 16) : Seq(250f, 10f, 16)), 0f, false);
				break;
			case MissionType.SlideUnder:
				array[i] = new MissionSlideUnder(scope, Pick(single ? Seq(20f, 1f, 16) : Seq(250f, 10f, 16)), 0f, false);
				break;
			case MissionType.DodgeZombies:
				array[i] = new MissionDodgeZombies(scope, Pick(single ? Seq(100f, 10f, 21) : Seq(1500f, 100f, 26)), 0f, false);
				break;
			case MissionType.PlayGame:
				array[i] = new MissionPlayGame(Pick(Seq(10f, 5f, 9)), 0f, false);
				break;
			case MissionType.ChangeLane:
				array[i] = new MissionChangeLane(scope, Pick(single ? Seq(200f, 10f, 21) : Seq(2000f, 100f, 21)), 0f, false);
				break;
			case MissionType.CollectGhosts:
				array[i] = new MissionCollectGhosts(scope, Pick(single ? Seq(400f, 10f, 21) : Seq(7500f, 100f, 26)), 0f, false);
				RemoveTypes(list, MissionType.CollectGhostsWithoutPowerUps, MissionType.ScoreWithoutCollectingGhosts);
				break;
			case MissionType.Slide:
				array[i] = new MissionSlide(scope, Pick(single ? Seq(60f, 1f, 21) : Seq(500f, 10f, 21)), 0f, false);
				RemoveTypes(list, MissionType.SlideLeft, MissionType.SlideRight, MissionType.SlideCenter);
				break;
			case MissionType.Jump:
				array[i] = new MissionJump(scope, Pick(single ? Seq(60f, 1f, 21) : Seq(500f, 10f, 21)), 0f, false);
				RemoveTypes(list, MissionType.JumpLeft, MissionType.JumpRight, MissionType.JumpCenter);
				break;
			case MissionType.SaveMe:
				array[i] = new MissionSaveMe(scope, Pick(single ? new float[2] { 2f, 3f } : Seq(5f, 1f, 11)), 0f, false);
				break;
			case MissionType.Score:
				array[i] = new MissionScore(scope, Pick(single ? Seq(5000f, 250f, 21) : Seq(50000f, 2500f, 21)) * multiplier, 0f, false);
				RemoveTypes(list, MissionType.ScoreWithoutPowerUps, MissionType.ScoreWithoutCollectingGhosts);
				break;
			case MissionType.ScoreWithoutCollectingGhosts:
				array[i] = new MissionScoreWithoutCollectingGhosts(Pick(Seq(2500f, 100f, 16)) * multiplier, 0f, false);
				RemoveTypes(list, MissionType.ScoreWithoutPowerUps, MissionType.Score, MissionType.CollectGhostsWithoutPowerUps, MissionType.CollectGhosts);
				break;
			case MissionType.ScoreWithoutPowerUps:
				array[i] = new MissionScoreWithoutPowerUps(Pick(Seq(2500f, 100f, 16)) * multiplier, 0f, false);
				RemoveTypes(list, MissionType.Score, MissionType.ScoreWithoutCollectingGhosts, MissionType.PowerUpBoost, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUp, MissionType.PowerUpCoinDoubler);
				break;
			case MissionType.SlideLeft:
				array[i] = new MissionSlideLeft(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.Slide, MissionType.SlideRight, MissionType.SlideCenter);
				break;
			case MissionType.SlideCenter:
				array[i] = new MissionSlideCenter(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.SlideLeft, MissionType.SlideRight, MissionType.Slide);
				break;
			case MissionType.SlideRight:
				array[i] = new MissionSlideRight(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.SlideLeft, MissionType.Slide, MissionType.SlideCenter);
				break;
			case MissionType.JumpLeft:
				array[i] = new MissionJumpLeft(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.Jump, MissionType.JumpRight, MissionType.JumpCenter);
				break;
			case MissionType.JumpCenter:
				array[i] = new MissionJumpCenter(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.JumpLeft, MissionType.JumpRight, MissionType.Jump);
				break;
			case MissionType.JumpRight:
				array[i] = new MissionJumpRight(scope, Pick(single ? Seq(50f, 1f, 21) : Seq(250f, 10f, 16)), 0f, false);
				RemoveTypes(list, MissionType.JumpLeft, MissionType.Jump, MissionType.JumpCenter);
				break;
			case MissionType.PowerUp:
				array[i] = new MissionPowerUp(scope, Pick(single ? Seq(5f, 1f, 6) : Seq(100f, 5f, 11)), 0f, false);
				RemoveTypes(list, MissionType.PowerUpBoost, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUpCoinDoubler, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps, MissionType.KillZombies);
				break;
			case MissionType.PowerUpShield:
				array[i] = new MissionPowerUpShield(scope, Pick(single ? Seq(2f, 1f, 3) : Seq(25f, 1f, 26)), 0f, false);
				RemoveTypes(list, MissionType.PowerUpBoost, MissionType.PowerUpWeapon, MissionType.PowerUp, MissionType.PowerUpCoinDoubler, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps, MissionType.KillZombies);
				break;
			case MissionType.PowerUpBoost:
				array[i] = new MissionPowerUpBoost(scope, Pick(single ? Seq(2f, 1f, 3) : Seq(25f, 1f, 26)), 0f, false);
				RemoveTypes(list, MissionType.PowerUp, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUpCoinDoubler, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps, MissionType.KillZombies);
				break;
			case MissionType.PowerUpCoinDoubler:
				array[i] = new MissionPowerUpCoinDoubler(scope, Pick(single ? Seq(2f, 1f, 3) : Seq(25f, 1f, 26)), 0f, false);
				RemoveTypes(list, MissionType.PowerUpBoost, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUp, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps, MissionType.KillZombies);
				break;
			case MissionType.PowerUpWeapon:
				array[i] = new MissionPowerUpWeapon(scope, Pick(single ? Seq(2f, 1f, 3) : Seq(25f, 1f, 26)), 0f, false);
				RemoveTypes(list, MissionType.PowerUp, MissionType.PowerUpShield, MissionType.PowerUpBoost, MissionType.PowerUpCoinDoubler, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps, MissionType.KillZombies);
				break;
			case MissionType.CollectGhostsWithoutPowerUps:
				array[i] = new MissionCollectGhostsWithoutPowerUps(Pick(Seq(300f, 10f, 21)), 0f, false);
				RemoveTypes(list, MissionType.PowerUp, MissionType.PowerUpBoost, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUpCoinDoubler, MissionType.CollectGhosts, MissionType.ScoreWithoutCollectingGhosts);
				break;
			case MissionType.KillZombies:
				array[i] = new MissionKillZombies(Pick(Seq(10f, 1f, 16)), 0f, false);
				RemoveTypes(list, MissionType.PowerUp, MissionType.PowerUpWeapon, MissionType.PowerUpShield, MissionType.PowerUpBoost, MissionType.PowerUpCoinDoubler, MissionType.ScoreWithoutPowerUps, MissionType.CollectGhostsWithoutPowerUps);
				break;
			}
			list.Remove(item);
		}
		return array;
	}
}
