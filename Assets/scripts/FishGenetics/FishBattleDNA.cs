using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishBattleDNA
{
	/* STAT INDEXES
	 0. HP
	 1. STR
	 2. DEF
	 3. STA
	 4. SPD
	*/
	public string species;
	public int level;
	public int[] stats;
	public int[] IV;

	/* Lettering Meanings 
	H. HP boost
	A. STR boost
	D. DEF boost
	G. STA boost
	S. SPD boost
	N. Ability boost
	*/
	public string bonusGenetics;
	public Color[] colors;

	public FishBattleDNA(string _species, int _level = 5)
	{
		//TODO: FETCH BASE STATS FROM SPECIES DICTIONARY SOMEWHERE
		stats = GenerateRandomStats();
		IV = GenerateRandomIVs();
		bonusGenetics = GenerateBonusGenetics();
		colors = ColorGenerator.GetFishColors();
		level = _level;
	}
	private string GenerateBonusGenetics(int genomicLenght = 6)
	{
		string letterings = "HADGSN";
		string genetics = "";
		for(int i = 0; i < genomicLenght; i++)
		{
			genetics += letterings[Random.Range(0, letterings.Length)];
		}
		return genetics;
	}
	private int[] GenerateRandomIVs(int maxIV = 30)
	{
		int[] IVs = new int[5];
		for(int i = 0; i < IVs.Length; i++)
		{
			IVs[i] = Random.Range(0, maxIV);
		}
		return IVs;
	}

	private int[] GenerateRandomStats(int minStat = 30, int maxStat = 120)
	{
		int[] nStats = new int[5];
		for (int i = 0; i < nStats.Length; i++)
		{
			nStats[i] = Random.Range(minStat, maxStat);
		}
		return nStats;
	}

}
[CreateAssetMenu(fileName = "newbasestats", menuName = "Battle/create new base stat file", order = 0)]
public class FishBaseStats : ScriptableObject
{
	public string speciesName = "";
	public int[] stats = new int[5];
}
