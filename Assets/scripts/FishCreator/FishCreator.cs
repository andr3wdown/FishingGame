using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class FishCreator : EditorWindow
{
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<LureCreator>("Lure Creator");
	}
	void OnGUI()
	{
		
	}
}
#endif
