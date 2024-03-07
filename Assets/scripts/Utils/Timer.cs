using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
	public float delay;
	private float countdown;

	public Timer(float _delay)
	{
		delay = _delay;
		countdown = delay;
	}
	public bool CountDown(bool reset = true, bool randomize = false, float min = 0f, float max = 2f)
	{
		countdown -= Time.deltaTime;
		return Check(reset, randomize, min, max);
	}
	public bool Check(bool reset = true, bool randomize = false, float min = 0f, float max = 2f)
	{
		if (countdown <= 0f)
		{
			if (reset)
			{
				if (randomize) countdown = Random.Range(min, max);
				else countdown = delay;
			}
			return true;
		}
		return false;
	}

}
