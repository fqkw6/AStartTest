using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 获得宝箱奖励的界面
public class UIPVEStarAwardView : UIWindow
{
    public const string Name = "PVE/UIPVEStarAwardView";

    public SimpleItemWidget[] _itemList;
    public Button[] _btnAward;
    public Text[] _txtNotAward;
    public Image[] _imgHasAward;
    public Text[] _txtAwardStar;

    private List<ChapterAwardConfig> _starAwardList = new List<ChapterAwardConfig>();
    private int _chapterID;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        _chapterID = (int) param[0];
    }

    public override void OnRefreshWindow()
    {
        _starAwardList.Clear();

        foreach (var item in ChapterAwardConfigLoader.Data) {
            if (item.Value.Degree == (int)PVEManager.Instance.ChapterType && item.Value.ChapterID == _chapterID) {
                _starAwardList.Add(item.Value);
            }
        }

        RefreshItem(0);
        RefreshItem(1);
        RefreshItem(2);
    }

    private void RefreshItem(int index)
    {
        if (index >= _starAwardList.Count) return;

        foreach (var item in ChapterAwardConfigLoader.Data) {
            if (item.Value.Degree == (int)PVEManager.Instance.ChapterType && item.Value.ChapterID == _chapterID) {
                _starAwardList.Add(item.Value);
            }
        }
        
        ChapterAwardConfig cfg = _starAwardList[index];

        List<AwardInfo> awardList = AwardManager.Instance.GetAwardList(cfg.Award);

        for (int i = 0; i < 3; ++i) {
            int newIndex = index*3 + i;
            if (i >= awardList.Count) {
                // 没有这个物品
                _itemList[newIndex].gameObject.SetActive(false);
            } else {
                _itemList[newIndex].gameObject.SetActive(true);
                _itemList[newIndex].SetInfo(awardList[i].ItemID, awardList[i].ItemCount);
            }
        }

        _txtAwardStar[index].text = cfg.Star.ToString();

        _imgHasAward[index].gameObject.SetActive(false);
        _txtNotAward[index].gameObject.SetActive(false);
        _btnAward[index].gameObject.SetActive(false);

        int curStar = PVEManager.Instance.GetChapterStar(_chapterID, PVEManager.Instance.ChapterType);
        if (PVEManager.Instance.HasChapterAward(_chapterID, PVEManager.Instance.ChapterType, index)) {
            // 不可领取
            if (curStar >= cfg.Star) {
                // 已经领取
                _imgHasAward[index].gameObject.SetActive(true);
            } else {
                // 未达成
                _txtNotAward[index].gameObject.SetActive(true);
            }
        } else {
            // 可以点击领取
            _btnAward[index].gameObject.SetActive(true);
        }
    }

    public void OnClickAward1()
    {
        PVEManager.Instance.RequestGetChapterAward(_starAwardList[0].Id, _chapterID, PVEManager.Instance.ChapterType);
    }

    public void OnClickAward2()
    {
        PVEManager.Instance.RequestGetChapterAward(_starAwardList[1].Id, _chapterID, PVEManager.Instance.ChapterType);
    }

    public void OnClickAward3()
    {
        PVEManager.Instance.RequestGetChapterAward(_starAwardList[2].Id, _chapterID, PVEManager.Instance.ChapterType);
    }
}
