using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 世界地图上的空位
public class WorldSlot : MonoBehaviour {
    public int _index = 0;
    public bool _isResTown = false;
    private Image _image;
    void Awake()
    {
        _image = GetComponent<Image>();
        if (_image != null) {
            _image.enabled = false;
        }
    }
	
    void Start () {
	
	}
}
