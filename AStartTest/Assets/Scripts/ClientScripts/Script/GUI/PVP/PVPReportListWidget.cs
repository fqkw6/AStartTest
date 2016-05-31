using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// pvp战报列表组件
public class PVPReportListWidget : ListItemWidget
{
    public Image _imgWin;
    public Image _imgArrow;
    public Text _txtNumber;

    public Image _imgIcon;
    public Text _txtLevel;
    public Text _txtName;
    public Text _txtTime;
    
    public Sprite _sprLose;
    public Sprite _sprLoseArrow;
    public Color _winColor;
    public Color _loseColor;

    private PVPReportInfo _info;

    public override void SetInfo(object data)
    {
        _info = (PVPReportInfo)data;

        if (_info.Win) {
            // 赢了
            // 向上箭头
            _txtNumber.color = _winColor;
        } else {
            // 输了
            _imgWin.sprite = _sprLose;
            _imgArrow.sprite = _sprLoseArrow;
            _txtNumber.color = _loseColor;
        }

        _txtNumber.text = _info.Number.ToString();
        _imgIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_info.Icon);
        _txtLevel.text = "Lv" + _info.Level;
        _txtName.text = _info.Name;
        _txtTime.text = PVPManager.Instance.GetElapseTimeString(_info.BattleTime.GetTime());
    }

    public void OnClickReplay()
    {
        
    }
}
