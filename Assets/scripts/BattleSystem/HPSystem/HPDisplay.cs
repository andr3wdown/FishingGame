using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPDisplay : MonoBehaviour
{
	public Image bar;
	public void UpdateDisplay(float ratio)
	{
		bar.fillAmount = ratio;
	}
}
