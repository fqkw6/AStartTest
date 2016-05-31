using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// 列表中的元素
public class ListItemWidget : MonoBehaviour
{
    [NonSerialized] public int Index = 0;
    public virtual void Awake()
    {
        // 设置所有的listitem的anchor，否则修改listContainer大小时会造成元素位置的偏移
        RectTransform rc = GetComponent<RectTransform>();
        rc.anchorMin = new Vector2(0.5f, 1f);
        rc.anchorMax = new Vector2(0.5f, 1f);
    }

    // 设置数据，如果使用默认数据源，则必须实现此函数
    public virtual void SetInfo(object data)
    {
    }

    public virtual object GetInfo()
    {
        Debug.LogError("未实现GetInfo");
        return null;
    }

    // 点击这个widget(为了代码习惯的统一，尽量不要在子类中重载，而是用ListView的回调，在View中处理响应事件)
    public virtual void OnClick()
    {
        if (OnClickCallback != null) {
            OnClickCallback(Index, this);
        }
    }

    // 选中这个listitem(只处理控件表现，重载的此函数中不要涉及业务逻辑)
    public virtual void OnSelect()
    {
    }

    // 取消这个listitem的选中
    public virtual void OnUnselect()
    {
    }

    public Action<int, ListItemWidget> OnClickCallback;
}

// 列表控件
public class UIListView : MonoBehaviour
{
    public RectTransform _listContainer; // list的容器 任意大小
    public ListItemWidget[] _itemWidget; // list中每个元素的widget 由于有一些细节逻辑会关联到widget，所以widget统一定义在这里，不要放在外部ui逻辑里面

    public int _numberPerLine = 1; // 一行中有几个元素
    public float _xOffset = 0; // x偏移
    public float _yOffset = 100; // y偏移
    
    private ScrollRect _scrollView;

    [NonSerialized] public Func<int, ListItemWidget> OnListItemAtIndex; // 在这个回调函数里面创建widget并对其赋值
    [NonSerialized] public Action<int, ListItemWidget> OnClickListItem;

    // list的最大数目 如果没有设置数据源，就必须要设置最大数目
    private int _maxCount;
    public int MaxCount
    {
        get { return _maxCount; }

        set { _maxCount = value; }
    }

    // 数据源 如果列表中只有一种数据，可以直接设置数据源就可以了，否则需要实现OnGetListItemData
    private Array _data;
    public Array Data
    {
        set { _data = value; }
    }

    // 所有创建的列表组件
    private List<ListItemWidget> _listWidget = new List<ListItemWidget>();
    public List<ListItemWidget> ListWidget
    {
        get { return _listWidget;}
    }

    private void Awake()
    {
        _scrollView = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        if (_itemWidget.Length <= 0) {
            Log.Error("未设置ListItemWidget控件");
        }

        // 隐藏所有的prefab
        foreach (var item in _itemWidget) {
            item.gameObject.SetActive(false);
        }
    }

    // 获取原始（第一个）widget的坐标
    public Vector3 GetOriginPosition()
    {
        if (_itemWidget != null && _itemWidget.Length > 0) {
            // 保存第一个prefab的坐标作为列表中元素的起始位置
            return _itemWidget[0].transform.localPosition;
        } else {
            return Vector3.zero;
        }
    }

    // 创建指定索引的widget
    public T CreateListItemWidget<T>(int index) where T : ListItemWidget
    {
        return Instantiate(_itemWidget[index] as T);
    }

    // 滚动到指定控件
    public void ScrollTo(ListItemWidget widget)
    {
        if (widget == null) return;
        if (_itemWidget.Length <= 0) {
            return;
        }

        RectTransform rcWidget = widget.GetComponent<RectTransform>();
        RectTransform rcPrefab = _itemWidget[0].GetComponent<RectTransform>();
        RectTransform rcContainer = _listContainer.GetComponent<RectTransform>();
        if (_scrollView.horizontal) {
            rcContainer.anchoredPosition = new Vector2((rcWidget.anchoredPosition - rcPrefab.anchoredPosition).x, rcContainer.anchoredPosition.y);
        } else {
            rcContainer.anchoredPosition = new Vector2(rcContainer.anchoredPosition.x, -(rcWidget.anchoredPosition - rcPrefab.anchoredPosition).y);
        }
    }

    // 滚动到指定索引位置
    public void ScrollTo(int index)
    {
        if (index >= _listWidget.Count) return;
        ScrollTo(_listWidget[index]);
    }

    // 重新加载list中的一项
    public void ReloadListItem(int index)
    {
        _itemWidget[index].SetInfo(_data.GetValue(index));
    }

    // 选中widget(如果singleChoice为true，则同时取消其他widget的选中)
    public void SelectWidget(ListItemWidget widget, bool singleChoice = true)
    {
        foreach (var item in _listWidget) {
            if (item == widget) {
                item.OnSelect();
            } else {
                if (singleChoice) {
                    item.OnUnselect();
                }
            }
        }
    }

    // 选中list中的一项
    public void SelectListItem(int index, bool singleChoice = true)
    {
        for (int i = 0; i < _listWidget.Count; ++i) {
            if (i == index) {
                _listWidget[i].OnSelect();
            } else {
                if (singleChoice) {
                    _listWidget[i].OnUnselect();
                }
            }
        }
    }
    
    // 清理旧数据
    private void Clear()
    {
        // 没有任何widget
        if (_itemWidget.Length <= 0) return;

        _listWidget.Clear();

        foreach (Transform item in _listContainer) {
            // 跳过prefab
            bool find = false;
            foreach (var itemWidget in _itemWidget) {
                if (item.gameObject == itemWidget.gameObject) {
                    find = true;
                    break;
                }
            }

            if (find) continue;

            // 删除旧的widget
            if (item.GetComponent<ListItemWidget>()) {
                Destroy(item.gameObject);
            }
        }
    }

    // 刷新
    public virtual void Refresh()
    {
        Clear();

        // 没有数据
        if (_itemWidget.Length <= 0) {
            return;
        }

        if ((_data == null && _maxCount == 0) || (_data != null && _data.Length <= 0)) {
            return;
        }
        
        // 计算list的大小
        Vector3 originPosition = GetOriginPosition();
        int count = _data != null ? _data.Length : _maxCount;

        if (_scrollView.vertical) {
            float maxHeight = Mathf.CeilToInt(1.0f * count / _numberPerLine) * _yOffset - originPosition.y;
            _listContainer.sizeDelta = new Vector2(_listContainer.sizeDelta.x, maxHeight);
        } else if (_scrollView.horizontal) {
            float maxWidth = Mathf.CeilToInt(1.0f * count) * _xOffset;
            _listContainer.sizeDelta = new Vector2(maxWidth, _listContainer.sizeDelta.y);
        }

        float x = originPosition.x;
        float y = originPosition.y;

        for (int i = 0; i < count; ++i) {
            // 如果本行元素已经排满了，切换到下一行
            if (i%_numberPerLine == 0) {
                x = originPosition.x;
                if (i != 0) {
                    y -= _yOffset;
                }
            }

            Vector3 pos = new Vector3(x, y, originPosition.z);

            ListItemWidget widget;
            if (OnListItemAtIndex != null) {
                // 外部ui逻辑控制创建哪个widget，并对其进行赋值
                widget = OnListItemAtIndex(i);
                if (widget.OnClickCallback == null) {
                    // 如果自定义widget的时候没有指定回调，则使用ListView的回调
                    widget.OnClickCallback = OnClickListItem;
                }
            } else {
                // 默认情况，取第一个widget，赋予数据源的数据
                widget = Instantiate(_itemWidget[0]);
                if (_data != null) {
                    widget.SetInfo(_data.GetValue(i));
                }
                widget.OnClickCallback = OnClickListItem;
            }
            
            widget.transform.SetParent(_listContainer, false);
            widget.gameObject.SetActive(true);
            widget.transform.localPosition = pos;
            widget.transform.localScale = Vector3.one;
            widget.Index = i;
            _listWidget.Add(widget);

            x += _xOffset;
        }
    }
}