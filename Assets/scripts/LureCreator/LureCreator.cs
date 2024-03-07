using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class LureCreator : EditorWindow
{
	[MenuItem("Window/LureCreator")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<LureCreator>("Lure Creator");
	}
	LureType type;
	[SerializeField]
	float weight;
	float aggravation;
	float action;
	float naturality;
	void OnGUI()
	{
		GUILayout.Label("Lure Creator", EditorStyles.boldLabel);
		name = EditorGUILayout.TextField("lure name", name);
		
		type = (LureType)EditorGUI.EnumPopup(new Rect(position.width - 68, 43, 64, 20), type);
		GUILayout.Label("Type");
		GUILayout.Space(5);
		weight = EditorGUILayout.Slider(value: weight, leftValue: 2f, rightValue: 40f, label: "lure weight");
		GUILayout.Label("Stats");
		GUILayout.Space(80);
		aggravation = GUI.VerticalSlider(new Rect(position.width - 99.99f, 85, 20, 50), aggravation, 1.0f, 0.0f);
		action = GUI.VerticalSlider(new Rect(position.width - 66.66f, 85, 20, 50), action, 1.0f, 0.0f);
		naturality = GUI.VerticalSlider(new Rect(position.width - 33.33f, 85, 20, 50), naturality, 1.0f, 0.0f);
		Vector3 vect = new Vector3(aggravation, action, naturality);
		vect = (vect.magnitude > 1f ? vect.normalized : vect);
		aggravation = vect.x;
		action = vect.y;
		naturality = vect.z;
		if (GUILayout.Button("Test"))
		{
			Debug.Log($"{name} {type} {weight}");
		}

	}
}
#endif