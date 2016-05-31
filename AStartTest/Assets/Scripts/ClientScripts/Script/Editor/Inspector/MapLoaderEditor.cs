using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapLoader))]
public class MapLoaderEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("生成地图")) {
            MapLoader loader = (MapLoader) target;
            if (loader != null) {
                loader.LoadMap();
            }
        }

        if (GUILayout.Button("清理地图")) {
            MapLoader loader = (MapLoader) target;
            if (loader != null) {
                loader.CleanMap();
            }
        }
    }
}
