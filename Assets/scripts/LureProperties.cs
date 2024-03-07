using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(order = 0, menuName = "Fishing/create new lure", fileName = "newlure")]
public class LureProperties : ScriptableObject
{
	public string lureName;
	public Sprite sprite;
	public LureType type;
	[Range(2f, 40f)]
	public float weight = 2;
	public LureFactor factor;

}
#if UNITY_EDITOR
[CustomEditor(typeof(LureProperties))]
public class LurePropertesEditor : Editor
{

	float aggravation;
	float action;
	float naturality;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.Label("Aggravation");
		aggravation = EditorGUILayout.Slider(aggravation, 0f, 1f);
		GUILayout.Label("Action");
		action = EditorGUILayout.Slider(action, 0f, 1f);
		GUILayout.Label("Naturality");
		naturality = EditorGUILayout.Slider(naturality, 0f, 1f);
		
		Vector3 normal = new Vector3(aggravation, action, naturality);
		if(normal.magnitude > 1f)
		{
			normal.Normalize();
			aggravation = normal.x;
			action = normal.y;
			naturality = normal.z;
		}
		LureProperties props = (LureProperties)target;
		props.factor.aggravation = aggravation;
		props.factor.action = action;
		props.factor.naturality = naturality;
		
	}
}
#endif
public enum LureType
{
	spinner = 0,
	spoon = 1,
	minnow = 2,
	jig = 3
}
[System.Serializable]
public struct LureFactor
{
	[HideInInspector]
	[Range(0f, 1f)]
	public float aggravation;
	[HideInInspector]
	[Range(0f, 1f)]
	public float action;
	[HideInInspector]
	[Range(0f, 1f)]
	public float naturality;
	public LureFactor(float ag = 0, float ac = 0, float na = 0)
	{
		aggravation = ag;
		action = ac;
		naturality = na;
	}

		
}
