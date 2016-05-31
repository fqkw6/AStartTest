using UnityEngine;
using System.Collections.Generic;

// 扫荡10次结果界面
public class UIPVEQuickFight10ResultView : UIWindow
{
    public const string Name = "PVE/UIPVEQuickFight10ResultView";
    public PVEQuickFightWidget _fightWidget;
    public SimpleItemWidget[] _itemWidgetExtra;
    public RectTransform _itemExtra;
    public RectTransform _imgFightOver;
    public RectTransform _listContainer;

    public float _offset = 100;
    public float _yStart = 100;

    public override void OnOpenWindow()
    {
        List<BattleResultInfo> result = PVEManager.Instance.QuickFightResult;

        foreach (Transform item in _listContainer) {
            Destroy(item.gameObject);
        }

        int count = result.Count;
        float maxHeight = Mathf.RoundToInt(1.0f * count) * _offset + _offset;
        _listContainer.sizeDelta = new Vector2(_listContainer.sizeDelta.x, maxHeight);
        
        float y = _yStart;

        // 每一战的结果
        for (int i = 0; i < count; ++i) {
            PVEQuickFightWidget go = Instantiate<PVEQuickFightWidget>(_fightWidget);
            go.transform.SetParent(_listContainer);
            go.transform.localPosition = new Vector3(0, y, 0);
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(true);

            go.SetInfo(result[i], i);
            y -= _offset;
        }

        // 扫荡完成的图片
        _imgFightOver.gameObject.SetActive(false);

        // 额外奖励
        for (int i = 0; i < _itemWidgetExtra.Length; ++i) {
            SimpleItemWidget itemWidget = _itemWidgetExtra[i];
            itemWidget.gameObject.SetActive(false);
        }
        
        _itemExtra.transform.SetParent(_listContainer);
        _itemExtra.transform.localPosition = new Vector3(0, y, 0);
    }

    // 再次扫荡
    public void OnClickQuickFight10()
    {
        PVEManager.Instance.RequestQuickFight(PVEManager.Instance.CurrentSelectLevelID,10);
        CloseWindow();
    }
}
