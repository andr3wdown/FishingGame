using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleOnEnd : MonoBehaviour
{
	public void Destroy()
	{
		Destroy(gameObject);
	}
}
