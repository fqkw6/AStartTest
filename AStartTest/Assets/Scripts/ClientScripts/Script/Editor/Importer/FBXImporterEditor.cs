using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class FBXImporterEditor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
//        ModelImporter importer = assetImporter as ModelImporter;
//        string path = importer.assetPath;
//        path = path.Replace("\\", "/");
//        if (path.IndexOf("Model/") != -1
//           || path.IndexOf("Effect/particles") != -1) {
//            // 这些文件夹下的模型资源按照1的比例导出
//            importer.globalScale = 1;
//        }
        //importer.generateMaterials = ModelImporterGenerateMaterials.None;
    }

//     void OnPostprocessModel(GameObject go)
//     {
//         ModelImporter importer = assetImporter as ModelImporter;
//         string path = importer.assetPath;
//         path = path.Replace("\\", "/");
//         Debug.Log(path);
//         //importer.assetBundleName = path;
// 
//         DoCreateController(path);
//         DoCreatePrefab(path);
//     }
}