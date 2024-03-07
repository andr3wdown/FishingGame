using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAnimate : MonoBehaviour
{
	public int start = 0;
	public float speed;
	float counter = 0;
	SpriteRenderer rend;
	private void Start()
	{
		rend = GetComponent<SpriteRenderer>();
		counter = start / 3f;
	}
	private void Update()
	{
		counter += Time.deltaTime * speed;
		if(counter >= 1f)
		{
			counter = 0;
		}
		Color c = rend.color;
		c.a = 1f - counter;
		rend.color = c;
		transform.localScale = new Vector3(counter, counter, counter);
	}
}
