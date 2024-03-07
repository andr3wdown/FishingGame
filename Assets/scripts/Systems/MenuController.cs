using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	public Image fishPict;
	public Text[] texts;
	static bool menuing = false;
	public Animator fishAnim;
	static MenuController instance;
    void Start()
    {
		instance = this;
		menuing = false;
    }
	public static void Open(FishData fish)
	{
		instance.OpenFishMenu(fish);
	}
	public void OpenFishMenu(FishData fish)
	{
		menuing = true;
		fishPict.sprite = fish.sprite;
		texts[0].text = fish.name;
		texts[1].text = System.Math.Round(fish.weight/1000, 2).ToString() + "kg";
		texts[2].text = System.Math.Round(fish.lenght, 2).ToString() + "cm";
		texts[3].text = fish.money.ToString() + "$";
		fishAnim.SetBool("MenuOpen", true);
	}
	public void CloseFishMenu()
	{
		menuing = false;
		//fishPict.sprite = null;
		texts[0].text = "";
		texts[1].text = "";
		texts[2].text = "";
		texts[3].text = "";
		fishAnim.SetBool("MenuOpen", false);
	}
   
}
