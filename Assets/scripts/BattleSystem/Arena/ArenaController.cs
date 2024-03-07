using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
	public Vector2 bounds;
	private static ArenaController instance;
	private static List<FishBattleController> combatants;

	private void Start()
	{
		instance = this;
		combatants = new List<FishBattleController>();

	}

	private void Update()
	{
		if(instance != this)
		{
			Destroy(gameObject);
		}	
		//TODO: Check combatant state
	}
	public static void AddCombatant(FishBattleController combatant)
	{
		combatants.Add(combatant);
	}
	public static Vector2 GetRandomArenaPosition()
	{
		return new Vector2(Random.Range(-instance.bounds.x/2, instance.bounds.x/2), Random.Range(-instance.bounds.y/2, instance.bounds.y/2));
	}

	public static bool InArena(Vector2 pos)
	{
		if(pos.x < instance.bounds.x && pos.x > -instance.bounds.x && pos.y < instance.bounds.y && pos.y > -instance.bounds.y)
		{
			return true;
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Vector3.zero, bounds);
	}
}