using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

[CreateAssetMenu(order = 0, fileName = "newFish", menuName = "Fishing/create new fish")]
[System.Serializable]
public class FishInformation : ScriptableObject
{
	public string fishName;
	[Range(0.0f, 1.7f)]
	public float strenghtRatio;
	[Range(0.0f, 1f)]
	public float baseStrenght;
	public Sprite fishSprite;
	public Vector2 weightRange;
	public Vector2 lenghtRange;
	public AnimationCurve weightDistribution;
	public AnimationCurve depth;
	public AnimationCurve temp;
	public AnimationCurve depthToAppearance;
	public AnimationCurve lenghtToWeight;
	public AnimationCurve strenghtCurve;

	public float ratio;
	public float maxMoney = 0f;
	public FishData GetFish()
	{
		float size = Random.Range(0f, 1f);
		size = weightDistribution.Evaluate(size);
		float weight = MathOperations.Map(size, 0f, 1f, weightRange.x, weightRange.y);
		float lenght = MathOperations.Map(lenghtToWeight.Evaluate(size), 0f, 1f, lenghtRange.x, lenghtRange.y);
		float strenght = baseStrenght + (strenghtRatio * strenghtCurve.Evaluate(size));
		int money = Mathf.RoundToInt((weight / weightRange.y) * maxMoney);
		return new FishData(fishName, size, weight, lenght, strenght, money, fishSprite);
	}
	
}
public struct FishData
{
	public string name;
	public float size;
	public float weight;
	public float lenght;
	public float strenght;
	public int money;
	public Sprite sprite;
	
	public FishData(string name, float size, float weight, float lenght, float strenght, int money, Sprite sprite)
	{
		this.name = name;
		this.size = size;
		this.weight = weight;
		this.lenght = lenght;
		this.strenght = strenght;
		this.money = money;
		this.sprite = sprite;
	}
}