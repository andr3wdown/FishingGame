using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Andr3wDown.Math;
using System.IO;
#if UNITY_EDITOR
[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
	private static Dictionary<Vector3Int, CellData> depthChart = null;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Generate"))
		{
			Generate();
		}

		if(GUILayout.Button("Generate Bitemap"))
		{
			if(depthChart == null)
			{
				Generate();
			}
			GenerateBitemap();
		}
		if (GUILayout.Button("Generate Depthmap"))
		{
			if (depthChart == null)
			{
				Generate();
			}
			GenerateDepthMap();
		}
		if (GUILayout.Button("Clear"))
		{
			MapController controller = (MapController)target;
			controller.Clear();
			depthChart = null;
		}
		
	}
	void Generate()
	{
		MapController controller = (MapController)target;
		depthChart = controller.Generate();
	}
	void GenerateBitemap()
	{
		MapController t = (MapController)target;
		int width = t.bounds.size.x;
		int height = t.bounds.size.y;
		foreach(FishInformation testFish in t.testFish)
		{
			List<Color> bitemap = new List<Color>();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{

					if (depthChart.ContainsKey(new Vector3Int(y - width / 2, x - width / 2, 0)))
					{
						CellData data;
						depthChart.TryGetValue(new Vector3Int(y - width / 2, x - width / 2, 0), out data);
						float depth = data.depth / t.depth;
						float depthEvaluation = testFish.depth.Evaluate(data.depth / t.depth);
						float tempEvaluation = testFish.temp.Evaluate(MathOperations.Map(testFish.depthToAppearance.Evaluate(depth), 0f, 1f, data.botTemp, data.topTemp) / MapController.MAX_TEMP);
						bitemap.Add(Color.Lerp(Color.black, Color.green, depthEvaluation * (t.includeTempInfo ? tempEvaluation : 1)));
					}
					else
					{
						bitemap.Add(Color.white);
					}
				}
			}
			Texture2D biteM = new Texture2D(width, height);
			biteM.SetPixels(bitemap.ToArray());
			biteM.filterMode = FilterMode.Point;
			File.WriteAllBytes(Application.dataPath + $"/SavedImages/{testFish.name}_bitemap.png", biteM.EncodeToPNG());
		}
		
	}
	void GenerateDepthMap()
	{
		MapController t = (MapController)target;
		int width = t.bounds.size.x;
		int height = t.bounds.size.y;

		List<Color> depthmap = new List<Color>();
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{

				if (depthChart.ContainsKey(new Vector3Int(y - width / 2, x - width / 2, 0)))
				{
					CellData data;
					depthChart.TryGetValue(new Vector3Int(y - width / 2, x - width / 2, 0), out data);
					float depth = data.depth / t.depth;
					depthmap.Add(Color.Lerp(Color.cyan, Color.blue, depth));
				}
				else
				{
					depthmap.Add(Color.black);
				}
			}
		}
		Texture2D depthM = new Texture2D(width, height);
		depthM.SetPixels(depthmap.ToArray());
		depthM.filterMode = FilterMode.Point;
		File.WriteAllBytes(Application.dataPath + $"/SavedImages/depthmap.png", depthM.EncodeToPNG());
	}
}
#endif