using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySplash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(DestroyThis());
    }
	IEnumerator DestroyThis()
	{
		yield return new WaitForSeconds(0.6f);
		Destroy(gameObject);
	}
}
