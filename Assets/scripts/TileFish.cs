using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

[CreateAssetMenu(order = 0, fileName ="newTile", menuName ="Fishing/create new tile")]
public class TileFish : ScriptableObject
{	
	public AnimationCurve curve;
	
	public void Randomize()
	{
		curve = MathOperations.GenerateRandomCurve();
	}
	
}
