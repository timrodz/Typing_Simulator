using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI() {

        MapGenerator mapGen = (MapGenerator) target;

        // this if statement is true whenever the inspector changes its values
        if (DrawDefaultInspector()) {

            if (mapGen.autoUpdate) {
                mapGen.GenerateMap();
            }

        }

        if (GUILayout.Button("Generate")) {

            mapGen.GenerateMap();

        }

    }

}