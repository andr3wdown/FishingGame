using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeepnetElement : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI[] texts;
	[SerializeField] private Image fishImg;

    public void Setup(FishData data)
	{
		texts[0].text = data.name;
		texts[1].text = System.Math.Round(data.lenght, 2).ToString()+"cm";
		texts[2].text = System.Math.Round(data.weight, 2).ToString()+"kg";
		fishImg.sprite = data.sprite;
	}
}
