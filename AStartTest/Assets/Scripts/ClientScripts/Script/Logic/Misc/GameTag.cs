using UnityEngine;
using System.Collections;

public class GameTag {
    static GameTag()
    {

    }

    public static readonly string Red = "Red";
    public static readonly string Blue = "Blue";


    public static void SetTag(Transform root, string tag)
    {
        root.gameObject.tag = tag;

        foreach (Transform tran in root) {
            SetTag(tran, tag);
        }
    }
}
