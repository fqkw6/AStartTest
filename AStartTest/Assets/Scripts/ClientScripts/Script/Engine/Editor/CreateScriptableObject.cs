using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateScriptableObject
{
    private static void CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof (T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
    }

//    [MenuItem("Assets/Create/CreateProjectile")]
//    public static void CreateProjectile()
//    {
//        CreateAsset<ProjectileConfigEx>("Assets/Resources/Projectile");
//    }
}