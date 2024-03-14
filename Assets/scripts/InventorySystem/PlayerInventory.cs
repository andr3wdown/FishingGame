using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	private List<FishData> currentKeepnet = new List<FishData>();
	private static PlayerInventory instance;
	
	private void Awake()
	{
		instance = this;
		
	}
	private void Start()
	{
		if (instance != this)
		{
			Destroy(gameObject);
		}
	}


	public static void AddFish(FishData data)
	{
		
		NotificationsController.AddNotification(new Notification($"{data.name} ({data.weight}kg) was added to the keepnet!"));
		instance.currentKeepnet.Add(data);
	}
	public static float GetTotalWeight(bool rounded = true)
	{
		float total = 0;
		if(instance.currentKeepnet.Count <= 0)
		{
			return 0;
		}
		for(int i = 0; i < instance.currentKeepnet.Count; i++)
		{
			total += instance.currentKeepnet[i].weight;
		}
		if (rounded)
		{
			total = (float)System.Math.Round((double)total, 2);
		}
		
		return total;
	}

	public static FishData[] GetFishInKeepnet
	{
		get
		{
			return instance.currentKeepnet.ToArray();
		}
	}
}
