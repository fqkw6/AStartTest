using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//  公会申请人列表界面
public class UIGuildApplyView : UIWindow
{
    public const string Name = "Guild/UIGuildApplyView";
    public RectTransform _listContainer;
    public GuildApplyInfoWidget _itemPrefab;

    public float _xStart = 20;
    public float _yStart = -20;
    public float _xOffset = 290;
    public float _yOffset = 150;

    private List<GuildApplyInfoWidget> _goList = new List<GuildApplyInfoWidget>(); 

    public override void OnRefreshWindow()
    {
        
    }

    private void RefreshList()
    {
        _goList.Clear();
        foreach (Transform item in _listContainer) {
            Destroy(item.gameObject);
        }

        List<GuildApplyInfo> list = GuildManager.Instance.ApplyList;

        int count = list.Count;
        float maxHeight = Mathf.RoundToInt(1.0f * count) * _yOffset;
        _listContainer.sizeDelta = new Vector2(_listContainer.sizeDelta.x, maxHeight);

        float x = _xStart;
        float y = _yStart;

        for (int i = 0; i < count; ++i) {
            GuildApplyInfoWidget go = Instantiate(_itemPrefab);
            go.transform.SetParent(_listContainer);
            go.transform.localPosition = new Vector3(x, y, 0);
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(true);

            x += _xOffset;
            y -= _yOffset;
            go.SetInfo(list[i]);

            _goList.Add(go);
        }
    }

    // 获取勾选上的列表
    private List<long> GetCheckList()
    {
        List<long> ret = new List<long>();

        foreach (var item in _goList) {
            if (item._toggleCheck.isOn) {
                ret.Add(item._info.EntityID);
            }
        }

        return ret;
    } 

    public void OnClickAgree()
    {
        GuildManager.Instance.RequestGuildApplyAgree(GetCheckList());
    }

    public void OnClickRefuse()
    {
        GuildManager.Instance.RequestGuildApplyRefuse(GetCheckList());

    }

    public void OnClickClear()
    {
        GuildManager.Instance.RequestGuildApplyClear();
    }
}
