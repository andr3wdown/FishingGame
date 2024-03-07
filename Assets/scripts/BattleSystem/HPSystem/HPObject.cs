using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPObject : MonoBehaviour
{

	private int maxHP;
	private int HP;
	private delegate void DisplayUpdateDelegate(float ratio);
	private DisplayUpdateDelegate displayUpdateDelegate;

	[SerializeField]private HPDisplay hpDisplay;
	public void Init(int _maxHP)
	{
		displayUpdateDelegate += hpDisplay.UpdateDisplay;
		maxHP = _maxHP;
		HP = maxHP;
	}
	public bool Damage(int damage)
	{
		HP -= damage;
		if(HP <= 0)
		{
			HP = 0;
			displayUpdateDelegate((float)HP / maxHP);
			GetComponent<FishBattleController>().Death();
			return true;
		}
		displayUpdateDelegate((float)HP/maxHP);
		return false;
	}
}
