using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// pvp积分列表组件
public class PVPScoreListWidget : ListItemWidget
{
    public SimpleItemWidget[] _itemWidgets;
    public Text _txtScore;
    public Button _btnGet;

    private int _scoreID;

    public override void SetInfo(object data)
    {
        _scoreID = (int) data;

        ArenaScoreConfig cfg = ArenaScoreConfigLoader.GetConfig(_scoreID);
        if (cfg == null) return;

        // 积分
        _txtScore.text = cfg.PointsDemand.ToString();

        List<AwardInfo> awardList = AwardManager.Instance.GetAwardList(cfg.AwardId);
        for (int i = 0; i < _itemWidgets.Length; ++i) {
            if (i < awardList.Count) {
                AwardInfo info = awardList[i];
                _itemWidgets[i].gameObject.SetActive(true);
                _itemWidgets[i].SetInfo(info.ItemID, info.ItemCount);
            } else {
                _itemWidgets[i].gameObject.SetActive(false);
            }
        }

        _btnGet.interactable = !PVPManager.Instance.HasGetScoreAward(_scoreID);
        _btnGet.gameObject.SetActive(PVPManager.Instance.MyScore >= cfg.PointsDemand);
    }

    public void OnClickGet()
    {
        PVPManager.Instance.RequestGetScoreAward(_scoreID);
    }
}
