using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class Fish : MonoBehaviour
{
	float hookRatio = 1f;
	float rotZ = 0;
	Lure hookedLure;
	bool hooked = false;
	Transform child;
	float strenght = 1f;
	[HideInInspector]
	public FishData data;

	float cooldown = 2f;
	float delay = 2f;
	Vector2 currentDir;


	private void Start()
	{
		child = transform.GetChild(0);
		rotZ = transform.rotation.eulerAngles.z;
		child.transform.localPosition = new Vector3(0, -child.localScale.y * 0.23f, 0);
	}
	public Fish Hook(Lure hook, FishData fish)
	{
		data = fish;
		strenght = fish.strenght;
		hookedLure = hook;
		hooked = true;
		return this;
	}
	private void LateUpdate()
	{
		if(hooked && hookedLure != null)
		{
			cooldown -= Time.deltaTime;
			if(cooldown <= 0)
			{
				cooldown = delay;
				currentDir = SetDir();
			}
			
			if(transform.position != hookedLure.transform.position)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, MathOperations.LookAt2D(hookedLure.transform.position, transform.position, -90), 6 * Time.deltaTime);
			}
			
			transform.position = hookedLure.transform.position;
			float pullStrenght = (1.5f + 1.4f * Mathf.Clamp(Mathf.Sin(Time.time * 0.4f), 0f, 1f)) * strenght;
			hookedLure.FishPull((child.position - (Vector3)LureCaster.position).normalized * 1.1f + ((transform.position + (Vector3)currentDir) - transform.position).normalized, pullStrenght);
		}
	}
	Vector2 SetDir()
	{
		return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
	}
}
