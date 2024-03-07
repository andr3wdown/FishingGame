using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceDeactivator : MonoBehaviour
{
	AudioSource s;
	public void Init()
	{
		s = GetComponent<AudioSource>();
	}
	private void Update()
	{
		if (!s.isPlaying)
		{
			gameObject.SetActive(false);
		}
	}
}
