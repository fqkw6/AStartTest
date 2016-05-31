using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 选择关卡的章节
public class PVEChapterListItemWidget : ListItemWidget
{
    public Text _txtUnlock;
    public Image _imgUnlock;
    public Image _imgMask;
    public Button _btnAward;
    public Text _txtStar;
    public Text _txtTitle;
    public Image _imgPic;
    public Image _imgAward;
    public Image _imgFrame;

    public GameObject _goEffect;

    public System.Action<int> OnClickChaperCallback;

    public int _chapterID;

    public override void SetInfo(object data)
    {
        _chapterID = (int) data;

        ChapterConfig cfg = ChapterConfigLoader.GetConfig(_chapterID);
        _txtTitle.text = cfg.ChapterName;

        _goEffect.gameObject.SetActive(false);

        if (PVEManager.Instance.IsChapterEnable(_chapterID, PVEManager.Instance.ChapterType)) {
            _imgUnlock.gameObject.SetActive(false);
            _txtUnlock.gameObject.SetActive(false);
            _imgMask.gameObject.SetActive(false);

            _btnAward.gameObject.SetActive(true);
            _txtStar.gameObject.SetActive(true);

            _txtStar.text = string.Format("{0}/{1}", PVEManager.Instance.GetChapterStar(_chapterID, PVEManager.Instance.ChapterType), PVEManager.Instance.GetFullChapterStar(_chapterID, PVEManager.Instance.ChapterType));

            if (ShowEffect()) {
                _goEffect.gameObject.SetActive(true);
            }
        } else {
            _imgUnlock.gameObject.SetActive(true);
            _txtUnlock.gameObject.SetActive(true);
            _imgMask.gameObject.SetActive(true);

            _btnAward.gameObject.SetActive(false);
            _txtStar.gameObject.SetActive(false);
        }
    }

    private bool ShowEffect()
    {
        List<ChapterAwardConfig> starAwardList = new List<ChapterAwardConfig>();
        foreach (var item in ChapterAwardConfigLoader.Data) {
            if (item.Value.Degree == (int) PVEManager.Instance.ChapterType && item.Value.ChapterID == _chapterID) {
                starAwardList.Add(item.Value);
            }
        }

        for (int i = 0; i < 3; ++i) {
            if (i >= starAwardList.Count) {
                continue;
            }
            ChapterAwardConfig cfg = starAwardList[i];
            int curStar = PVEManager.Instance.GetChapterStar(_chapterID, PVEManager.Instance.ChapterType);
            if (!PVEManager.Instance.HasChapterAward(_chapterID, PVEManager.Instance.ChapterType, i) && curStar >= cfg.Star) {
                // 有可领取的奖励
                return true;
            }
        }

        return false;
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
        if (OnClickChaperCallback != null) {
            OnClickChaperCallback(_chapterID);
        }
    }

    public void OnClickAward()
    {
        UIManager.Instance.OpenWindow<UIPVEStarAwardView>(_chapterID);
    }
}
