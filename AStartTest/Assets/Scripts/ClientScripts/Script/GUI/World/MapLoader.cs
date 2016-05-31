using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapLoader : MonoBehaviour {
    public Sprite[] _mapRes;
    public int _widthCount;
    
	void Start () {
	}

    public void LoadMap()
    {
        if (_mapRes.Length <= 0) {
            return;
        }

        float yOffset = _mapRes[0].rect.height;

        float x = 0;
        float y = 0;
        for (int i = 0; i < _mapRes.Length; ++i) {
            if (i > 0 && i % _widthCount == 0) {
                x = 0;
                y -= yOffset;
            }

            GameObject go = new GameObject("Bg" + i);
            go.AddComponent<CanvasRenderer>();
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.SetParent(transform, false);
            rt.localPosition = new Vector3(x, y, 0);
            rt.localScale = Vector3.one;
            rt.SetSiblingIndex(0);

            // 左上角对齐
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);

            Sprite sprite = _mapRes[i];
            
            rt.sizeDelta = sprite.rect.size;
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            x += sprite.rect.width;
        }
    }

    public void CleanMap()
    {
        List<GameObject> ret = new List<GameObject>();
        foreach (Transform item in transform) {
            if (item.gameObject.name.StartsWith("Bg")) {
                ret.Add(item.gameObject);
            }
        }

        foreach (var item in ret) {
            DestroyImmediate(item);
        }
    }
}
