//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DOTweenPath))]
public class DOTweenPathEditor : Editor
{
    DOTweenPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable(){
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (DOTweenPath)target;

	    if (_target.nodes == null) {
	        _target.nodes = new List<Vector3>();
	    }
	}
	
	public override void OnInspectorGUI(){		
		//path color:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");
		_target.pathColor = EditorGUILayout.ColorField(_target.pathColor);
		EditorGUILayout.EndHorizontal();
		
		//exploration segment count control:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Node Count");
		_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider(_target.nodeCount, 0, 10), 2,100);
		EditorGUILayout.EndHorizontal();
		
		//add node?
		if(_target.nodeCount > _target.nodes.Count){
			for (int i = 0; i < _target.nodeCount - _target.nodes.Count; i++) {
				_target.nodes.Add(Vector3.zero);	
			}
		}
	
		//remove node?
		if(_target.nodeCount < _target.nodes.Count){
            int removeCount = _target.nodes.Count - _target.nodeCount;
            _target.nodes.RemoveRange(_target.nodes.Count - removeCount, removeCount);
        }

	    EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel = 2;
        if (GUILayout.Button("Add")) {
            _target.nodes.Insert(0, _target.transform.position);
        }

        //node display:
        for (int i = 0; i < _target.nodes.Count; i++) {
		    //EditorGUILayout.BeginHorizontal();
            //EditorGUI.indentLevel = 1;
		    if (GUILayout.Button("S")) {
		        _target.nodes[i] = _target.transform.position;
		    }

            _target.nodes[i] = EditorGUILayout.Vector3Field("Node " + (i+1), _target.nodes[i]);
            //EditorGUILayout.EndHorizontal();
		}
        EditorGUILayout.EndVertical();
		
		//update and redraw:
		if(GUI.changed){
			EditorUtility.SetDirty(_target);			
		}
	}
	
	void OnSceneGUI(){
		if(_target.enabled) { // dkoontz
			if(_target.nodes.Count > 0){
				//allow path adjustment undo:
				Undo.RecordObject(_target,"Adjust iTween Path");
				
				//path begin and end labels:
				Handles.Label(_target.nodes[0], _target.gameObject.name + " Begin", style);
				Handles.Label(_target.nodes[_target.nodes.Count-1], _target.gameObject.name + " End", style);
				
				//node handle display:
				for (int i = 0; i < _target.nodes.Count; i++) {
					_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
				}	
			}
		} // dkoontz
	}
}