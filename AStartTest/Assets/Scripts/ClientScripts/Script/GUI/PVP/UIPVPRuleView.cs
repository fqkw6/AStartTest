using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// 竞技场规则说明界面
public class UIPVPRuleView : UIWindow
{
    public const string Name = "PVP/UIPVPRuleView";
    public SimpleItemWidget[] _itemWidget;
    public Text _txtHighRank;
    public Text _txtAward;
    public Text _txtRule;

    public override void OnRefreshWindow()
    {
        // 历史最高排名
        if (PVPManager.Instance.MyHighRank <= 0) {
            _txtHighRank.text = Str.Get("UI_PVP_NOT_IN_RANK");
        } else {
            _txtHighRank.text = PVPManager.Instance.MyHighRank.ToString();
        }

        _txtRule.text = Str.Get("UI_PVP_RULE");

        int myRank = PVPManager.Instance.MyRank;
        _txtAward.text = Str.Format("UI_PVP_DAILY_AWARD", myRank);

        ArenaDailyRankConfig cfg = null;
        foreach (var item in ArenaDailyRankConfigLoader.Data) {
            if (myRank <= item.Value.LowerRank && myRank >= item.Value.UpperRank) {
                cfg = item.Value;
                break;
            }   
        }

        if (cfg != null) {
            List<AwardInfo> list = AwardManager.Instance.GetAwardList(cfg.AwardId);
            for (int i = 0; i < _itemWidget.Length; ++i) {
                SimpleItemWidget item = _itemWidget[i];
                if (i < list.Count) {
                    _txtAward.gameObject.SetActive(true);
                    item.SetInfo(list[i].ItemID, list[i].ItemCount);
                } else {
                    item.gameObject.SetActive(false);
                }
            }
        } else {
            _txtAward.gameObject.SetActive(false);
            foreach (var item in _itemWidget) {
                item.gameObject.SetActive(false);
            }
        }
    }
}

