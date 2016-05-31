using UnityEngine;
using System.Collections;

public class GameLayer
{
    static GameLayer()
    {
        Ground = LayerMask.NameToLayer("Ground");
        GroundMask = LayerMask.GetMask("Ground");

        Object = LayerMask.NameToLayer("Object");
        ObjectMask = LayerMask.GetMask("Object");
    }

    public static readonly int Ground;
    public static readonly int GroundMask;
    public static readonly int Object;
    public static readonly int ObjectMask;

    public static void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;

        foreach (Transform tran in root) {
            SetLayer(tran, layer);
        }
    }
}
