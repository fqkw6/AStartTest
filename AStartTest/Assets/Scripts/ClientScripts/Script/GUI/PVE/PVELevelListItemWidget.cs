using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 选择关卡的关卡
public class PVELevelListItemWidget : ListItemWidget
{
    public Image _imgFrame;
    public Image _imgLock;
    public Image[] _imgStars;
    public Text _txtTitle;
    public Image _imgNew;
    public Sprite _sprStarGray;

    public System.Action<int> OnClickLevelCallback;

    public int _levelID;
    private LevelInfo _info;
    public override void SetInfo(object data)
    {
        _levelID = (int) data;
        _info = PVEManager.Instance.GetLevelInfo(_levelID);

        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(_levelID);
        _txtTitle.text = cfg.MissionName;

        for (int i = 0; i < _imgStars.Length; ++i) {
            if (_info == null || i >= _info.star) {
                _imgStars[i].sprite = _sprStarGray;
            }
        }

        if (PVEManager.Instance.IsLevelEnable(_levelID)) {
            _imgLock.gameObject.SetActive(false);
        } else {
            _imgLock.gameObject.SetActive(true);
        }

    }

    public void Select()
    {
        _imgFrame.gameObject.SetActive(true);
    }

    public void UnSelect()
    {
        _imgFrame.gameObject.SetActive(false);
    }

    public override void OnClick()
    {
        if (OnClickLevelCallback != null) {
            OnClickLevelCallback(_levelID);
        }
    }
}
