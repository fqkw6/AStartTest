using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStarPanel : MonoBehaviour {
    public Image[] _stars;

    public Sprite _sprNormal;
    public Sprite _sprDisable;

    void Awake()
    {
    }

    public void SetStar(int star)
    {
        for (int i = 0; i < _stars.Length; ++i) {
            Image img = _stars[i];
            if (img == null) {
                continue;
            }

            if (_sprDisable != null) {
                if (i < star) {
                    if (_sprNormal != null) {
                        _stars[i].sprite = _sprNormal;
                    }
                } else {
                    _stars[i].sprite = _sprDisable;
                }
            } else {
                _stars[i].gameObject.SetActive(i < star);
            }
        }
    }
}
