using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TileFish))]
public class TileFishEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Randomize"))
		{
			TileFish tile = (TileFish)target;
			tile.Randomize();
		}
	}
}
#endif
