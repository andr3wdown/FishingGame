using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	static List<AudioSource> sourcePool = new List<AudioSource>();
	public AudioClip[] clips;
	public AudioClip[] ui;
	static AudioController instance;
	private void Start()
	{
		instance = this;
		sourcePool = new List<AudioSource>();
	}
	public void PlayUIAudio(string clipName)
	{
		AudioClip clip = null;
		switch (clipName)
		{
			case "mouseOver":
				clip = ui[0];
				break;
		}
		AudioSource s = GetSource();
		s.gameObject.SetActive(true);
		s.gameObject.transform.position = Vector3.zero;
		s.pitch = 1.5f;
		s.PlayOneShot(clip, 0.4f);
	}
	public static void PlayClip(string clip, Vector2 pos, float vol = 0.7f, float pitch = 1f)
	{
		AudioClip c = null;
		switch (clip)
		{
			case "splash":
				c = instance.clips[0];
				break;
			case "cast":
				c = instance.clips[1];
				break;
		}
		AudioSource s = GetSource();
		s.gameObject.SetActive(true);
		s.gameObject.transform.position = pos;
		s.pitch = pitch;
		s.PlayOneShot(c, vol);
	}
	static AudioSource GetSource()
	{
		for (int i = 0; i < sourcePool.Count; i++)
		{
			if (!sourcePool[i].gameObject.activeInHierarchy)
			{
				return sourcePool[i];
			}
		}
		GameObject go = new GameObject("source");
		go.transform.SetParent(instance.transform);
		AudioSource s = go.AddComponent<AudioSource>();
		go.AddComponent<SourceDeactivator>().Init();
		sourcePool.Add(s);
		go.SetActive(false);
		return s;
	}



}
