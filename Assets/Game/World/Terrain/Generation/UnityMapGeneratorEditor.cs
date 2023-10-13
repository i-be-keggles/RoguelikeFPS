using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (UnityTerrainGenerator))]
public class UnityMapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
        UnityTerrainGenerator mapGen = (UnityTerrainGenerator)target;

		if (DrawDefaultInspector ()) {
            mapGen.GenerateMap();
        }

		if (GUILayout.Button ("Generate")) {
			mapGen.GenerateMap ();
		}
	}
}
