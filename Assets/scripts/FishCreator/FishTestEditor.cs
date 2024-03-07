using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(FishTester))]
public class FishTestEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Test"))
		{
			FishTester tester = (FishTester)target;
			tester.Test();
		}
	}
}
#endif