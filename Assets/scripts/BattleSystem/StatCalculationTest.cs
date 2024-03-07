using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCalculationTest : MonoBehaviour
{
	public FishBattleDNA dna;
	public Texture2D testfish;

	private void Start()
	{
		dna = new FishBattleDNA("test");


		FishColorReplacer.ReplaceFishColors(testfish, dna.colors);
		/*Test(5);
		Test(10);
		Test(20);
		Test(40);
		Test(50);
		Test(70);
		Test(100);*/
	}
	public void Test(int level = 10)
	{
		int[] calculatedStats = BattleStatCalculator.CalculateStats(level, dna);
		for(int i = 0; i < calculatedStats.Length; i++)
		{
			Debug.Log($"at level: {level} stat number {i} is: {calculatedStats[i]}");
		}


		

		
	}
}
