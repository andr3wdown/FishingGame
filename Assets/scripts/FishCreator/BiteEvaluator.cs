using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BiteEvaluator : MonoBehaviour
{
	public Image hitBar;
	MapController map;
	List<FishData> allFish = new List<FishData>();
	bool incoming = false;
	public FishInformation[] allSpecies;
	List<string> speciesNames = new List<string>();
	public float hitDelay;
	private void Start()
	{
		
		for(int i = 0; i < allSpecies.Length; i++)
		{
			speciesNames.Add(allSpecies[i].fishName);
		}
		map = GetComponent<MapController>();
	}

	private void Update()
	{
		if(allFish.Count < 25)
		{
			allFish.Add(GetRandomFish());
		}
	}

	public FishData GetRandomFish()
	{
		//print(allSpecies.Length);
		return allSpecies[Random.Range(0, allSpecies.Length)].GetFish();
	}

	public void EvaluateBite(Lure lure)
	{
		if (!incoming)
		{
			for(int i = 0; i < allFish.Count; i++)
			{
				int speciesIndex = speciesNames.IndexOf(allFish[i].name);
				float tempEvaluation = allSpecies[speciesIndex].temp.Evaluate(MapController.GetTemp(lure.transform.position, lure.depth) / MapController.MAX_TEMP);
				float depthEvaluation = allSpecies[speciesIndex].depth.Evaluate(MapController.GetDepth(lure.transform.position) / MapController.Depth);
				float depthEvaluation2 = 1f - Mathf.Abs((lure.depth / MapController.GetDepth(lure.transform.position) - (((1f - allSpecies[speciesIndex].depthToAppearance.Evaluate(MapController.GetDepth(lure.transform.position))) * MapController.Depth) / MapController.Depth)));//allSpecies[speciesIndex].depth.Evaluate(MapController.GetDepth(lure.transform.position) / MapController.Depth);
				depthEvaluation2 = depthEvaluation2 * (MapController.GetDepth(lure.transform.position) / MapController.Depth) + (1f - (MapController.GetDepth(lure.transform.position) / MapController.Depth));
				float combinedEvaluation = tempEvaluation * depthEvaluation * depthEvaluation2 * allSpecies[speciesIndex].ratio * 0.02f; //10;
				//print($"{allFish[i].name} {tempEvaluation} {depthEvaluation} {depthEvaluation2} {combinedEvaluation}");                                                                                                                  //print($"{tempEvaluation} {depthEvaluation} {depthEvaluation2} {combinedEvaluation}");
																																												 //print($"{combinedEvaluation}");
				bool bite = Random.Range(0f, 1f) < combinedEvaluation;
				if (bite)
				{
					//print(allFish[i].strenght);
					StartCoroutine(BiteDelay(lure, allFish[i]));
					allFish.RemoveAt(i);				
					break;
				}
			}
		}
	}
	
	IEnumerator BiteDelay(Lure lure, FishData fish)
	{
		incoming = true;
		float delay = Random.Range(0f, 1.5f);
		yield return new WaitForSecondsRealtime(delay);
		
		if(lure != null)
		{ 
			hitBar.transform.parent.gameObject.SetActive(true);
			float cooldown = hitDelay;
			hitBar.fillAmount = 1f;
			CameraController.ScreenShake();
			bool secondShake = false;
			while (cooldown > 0f)
			{
				hitBar.fillAmount = 1f - (cooldown / hitDelay);
				cooldown -= Time.deltaTime;
				if(!secondShake && cooldown < hitDelay / 2f)
				{
					secondShake = true;
					CameraController.ScreenShake();
				}
				yield return null;
				if (Input.GetKeyDown(KeyCode.Mouse1))
				{
					if(lure != null) lure.FishOn(fish);
					cooldown = 0;
					break;
				}
			}
			hitBar.transform.parent.gameObject.SetActive(false);
		
		}
		incoming = false;
	}
}
