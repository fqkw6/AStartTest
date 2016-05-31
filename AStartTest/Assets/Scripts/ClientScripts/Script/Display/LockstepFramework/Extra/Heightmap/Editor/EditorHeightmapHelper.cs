﻿using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Lockstep.Extra
{
    [CustomEditor (typeof (HeightmapHelper))]
    public class EditorHeightmapHelper : Editor
    {
        SerializedProperty Size;
        SerializedProperty BottomLeft;

        SerializedProperty HeightBounds;

        SerializedProperty Interval;

        void GenerateProperties (SerializedObject so) {
            Size = so.FindProperty("_size");
            BottomLeft = so.FindProperty("_bottomLeft");
            Interval = so.FindProperty("_interval");
            HeightBounds = so.FindProperty("_heightBounds");
        }
        public override void OnInspectorGUI()
        {

            HeightmapHelper hh = (HeightmapHelper)target;

            SerializedObject so = new SerializedObject(hh);
            GenerateProperties (so);
            Size.Draw();
            BottomLeft.Draw();
            HeightBounds.Draw();

            Interval.Draw();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Maps",EditorStyles.boldLabel);
            SerializedProperty Maps = so.FindProperty("_maps");

            int size = EditorGUILayout.IntField ("Map Count", Maps.arraySize);
            if (size != Maps.arraySize)
                Maps.arraySize = size;

            so.ApplyModifiedProperties();

            for (int i = 0; i < hh.Maps.Length; i++) {
                SerializedProperty heightMapProp = Maps.GetArrayElementAtIndex(i);          
                EditorGUILayout.PropertyField(heightMapProp, new GUIContent("Map " + i.ToString()),true);
                so.ApplyModifiedProperties();
                HeightMap hm = hh.Maps[i];

                if (GUILayout.Button("Scan")) {
                    short[,] Scan = hh.Scan(hh.Maps[i].ScanLayers.value);
                    hm.Map.LocalClone (Scan);
                }
            }

            EditorGUILayout.Space();

            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(hh);
        }
    }
}