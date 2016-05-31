using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// pvp最高排名奖励组件
public class PVPAwardListWidget : ListItemWidget
{
    public Text _txtRank;
    public SimpleItemWidget[] _itemWidgets;
    public Button _btnGet;

    private int _highID = 0;
    
    public override void SetInfo(object data)
    {
        _highID = (int) data;

        ArenaHistoryRankConfig cfg = ArenaHistoryRankConfigLoader.GetConfig(_highID);
        if (cfg == null) return;

        if (cfg.LowerHistoryRank == cfg.UpperHistoryRank) {
            _txtRank.text = Str.Format("UI_PVP_RANK", cfg.LowerHistoryRank);
        } else {
            _txtRank.text = Str.Format("UI_PVP_RANK_RANGE", cfg.UpperHistoryRank, cfg.LowerHistoryRank);
        }
        
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

        _btnGet.interactable = !PVPManager.Instance.HasGetHighAward(_highID);
        _btnGet.gameObject.SetActive(PVPManager.Instance.MyHighRank > 0 && PVPManager.Instance.MyHighRank <= cfg.LowerHistoryRank);
    }


    public void OnClickGet()
    {
        PVPManager.Instance.RequestGetHighAward(_highID);
    }
	
}
