using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class FishTester : MonoBehaviour
{
	public FishInformation data;
    public void Test()
	{
		if(data != null)
		{
			float val = MathOperations.Map(data.weightDistribution.Evaluate(Random.Range(0f, 1f)), 0f, 1f, data.weightRange.x, data.weightRange.y) / 1000;
			val = (float)System.Math.Round(val, 2);
			print($"{val}kg");
		}
	}
}
