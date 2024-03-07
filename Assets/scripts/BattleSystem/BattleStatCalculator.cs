using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleStatCalculator
{
	public static int[] CalculateStats(int level, FishBattleDNA dna)
	{
		int[] calulatedBonuses = CalculateBonuses(dna.bonusGenetics);
		int[] calculatedStats = new int[5];
		for (int i = 0; i < calculatedStats.Length; i++)
		{
			int stat = dna.stats[i];
			int iv = dna.IV[i];
			int bonus = calulatedBonuses[i];

			float adjusted = (stat + iv + bonus) * 2f * (level / 100f);

			int floored = Mathf.FloorToInt(adjusted);

			if (i == 0) calculatedStats[i] = floored * 10;
			else calculatedStats[i] = floored;
		}
		return calculatedStats;
	}
	private static int[] CalculateBonuses(string bonusGenes)
	{
		string letterings = "HADGSN";
		int[] bonuses = new int[6];
		for(int i = 0; i < bonusGenes.Length; i++)
		{
			int index = letterings.IndexOf(bonusGenes[i]);

			bonuses[index]++;
		}
		for(int i = 0; i < bonuses.Length; i++)
		{
			if(i == bonuses.Length - 1)
			{
				continue;
			}
			if(bonuses[i] < 2)
			{
				bonuses[i] = 0;
			}
			else if(bonuses[i] == 2)
			{
				bonuses[i] = 10;
			}
			else
			{
				bonuses[i] = 15;
			}	
		}
		return bonuses;
	}
}
