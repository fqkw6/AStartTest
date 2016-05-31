using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

// 卡牌组件
public class CardWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Image _imgCard;
    public Image _imgCardBgCover;
    public Text _txtName;
    public Text _txtCost;
    public bool _isPreview; // 预览卡牌，不能拖拽和选择，但是可以弹出tip提示
    public Image _imgMask;  // 圣水不足时的蒙版
    public RectTransform _panel;

    private bool _isSelect = false;
    private bool _isModel = false; // 标志是模型还是卡牌
    private Vector3 _originPos;
    private Sequence _currentSequence;
    private RectTransform _transform;
    private bool _isEnable = false;

    private const float YOFFSET = 5;
    private const float SCALE = 1.1f;
    private const float MAX_HEIGHT = 100;   // 卡牌选择区域的大小

    [NonSerialized]
    public CardInfo Info;

    void Awake()
    {
        _originPos = transform.localPosition;
        _transform = transform as RectTransform;
        _isEnable = true;
    }

    void Update()
    {
        if (Info.Cfg.Cost * GameConfig.MANA_MUL > BattleController.Instance.Mana)
        {
            // 圣水不足，卡牌灰化显示，并显示倒计时蒙版
            if (_isEnable)
            {
                SetDisable();
            }
            // 圣水进度条更新
            UpdateMask();
        }
        else
        {
            // 圣水足够
            if (!_isEnable)
            {
                SetEnable();
            }
        }
    }

    // 设置卡牌数据
    public void SetInfo(int index, CardInfo info)
    {
        Info = info;
        Info.Index = index;

        if (_txtName != null)
        {
            _txtName.text = Info.Cfg.Name;
        }

        if (_txtCost != null)
        {
            _txtCost.text = Info.Cfg.Cost.ToString();
        }
    }

    // 设置为灰化
    public void SetDisable()
    {

    }

    // 设置为正常态，并震动提示
    public void SetEnable()
    {
        ShakeByMana();
    }

    // 更新圣水蒙版
    private void UpdateMask()
    {
        // TODO 平滑过渡
        if (_imgMask != null)
        {
            _imgMask.fillAmount = Mathf.Min(1, 1f * BattleController.Instance.Mana / GameConfig.MANA_MUL / Info.Cfg.Cost);
        }
    }

    // 点击卡牌
    public void OnClick()
    {
        if (_isPreview)
        {
            // 预览卡牌无法点击
            return;
        }

        if (_isSelect)
        {
            // 已经选择了这张卡牌，点击卡牌会晃动
            const float TIME = 0.05f;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(_originPos, TIME));
            seq.Join(transform.DOScale(new Vector3(1, 1, 1), TIME));
            seq.Append(transform.DOLocalMoveY(_originPos.y + 5, TIME));
            seq.Join(transform.DOScale(new Vector3(1.1f, 1.1f, 1), TIME));
            seq.Append(transform.DORotate(new Vector3(0, 0, 2), TIME).SetLoops(4, LoopType.Yoyo));
            seq.Play();
        }
        else
        {
            // 通知父窗口选中这张卡牌
            EventDispatcher.TriggerEvent(EventID.UI_BATTLE_SELECT_CARD, Info.Index);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isPreview)
        {
            return;
        }

        Vector2 touchPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((transform.parent as RectTransform), eventData.position, eventData.pressEventCamera, out touchPos))
        {
            return;
        }

        if (touchPos.y < MAX_HEIGHT)
        {
            // 在ui区域内，卡牌还原到原格子，并且处于选中状态
            MoveBack();

            // 没有选择出牌，清理之前创建的预览模型
            BattleController.Instance.ClearCursorModel();
        }
        else
        {
            // 拖着卡牌松开手，出卡
            BattleController.Instance.UseCard(eventData.position);
        }
    }

    // 按下卡牌、缩小、标记已选、撤销之前已选
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isPreview)
        {
            return;
        }

        // 已经选择直接返回
        if (_isSelect) return;
        _isSelect = true;
        const float TIME = 0.2f;
        _currentSequence.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOScale(new Vector3(0.9f, 0.9f, 1), TIME));
        _currentSequence = seq;
        seq.Play();

        // 选中卡牌
        EventDispatcher.TriggerEvent(EventID.UI_BATTLE_SELECT_CARD, Info.Index);
    }

    // 选择卡牌会有一个提示，卡牌会向上移动，然后附加轻微抖动
    public void OnSelectCard()
    {
        if (_isPreview)
        {
            return;
        }

        _isSelect = true;

        const float TIME = 0.05f;
        transform.rotation = Quaternion.identity;
        _currentSequence.Kill();
        Sequence seq = DOTween.Sequence();
        _imgCardBgCover.gameObject.SetActive(true);
        seq.Join(transform.DOScale(new Vector3(1.1f, 1.1f, 1), TIME));
        seq.Join(transform.DOLocalMoveY(_originPos.y + 5, TIME));

        _currentSequence = seq;
        seq.Play();
    }

    // 卡牌还原到原始状态
    public void OnUnSelectCard()
    {
        if (_isPreview)
        {
            return;
        }

        _isSelect = false;
        const float TIME = 0.2f;
        _imgCardBgCover.gameObject.SetActive(false);
        _currentSequence.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOLocalMove(_originPos, TIME));
        seq.Join(transform.DOScale(Vector3.one, TIME));
        _currentSequence = seq;
        seq.Play();
    }

    // 圣水充足了，会有一个晃动提示
    public void ShakeByMana()
    {

    }

    // 圣水满了，所有的卡牌都会有持续晃动提示
    public void ShakeByManaFull()
    {

    }

    public void StopShakeByManaFull()
    {

    }

    // 新手牌动画
    public void OnPushNewCard(Vector3 src, float delay)
    {
        _panel.gameObject.SetActive(true);

        _isSelect = false;
        transform.localPosition = _originPos;
        transform.localScale = Vector3.one;
        _imgCardBgCover.gameObject.SetActive(false);

        transform.localScale = Vector3.one * 0.2f;
        transform.transform.position = src;

        const float TIME = 0.4f;
        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOScale(Vector3.one, TIME));
        seq.Join(transform.DOLocalMove(_originPos, TIME));
        seq.Play().SetDelay(delay);
    }

    // 当前拖动的卡牌返回到原来的卡牌格子
    public void MoveBack()
    {
        _panel.gameObject.SetActive(true);

        const float TIME = 0.2f;
        _currentSequence.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOLocalMove(new Vector3(_originPos.x, _originPos.y + 5, 0), TIME));
        seq.Join(transform.DOScale(new Vector3(1.1f, 1.1f, 1), TIME));
        _currentSequence = seq;
        seq.Play();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isPreview)
        {
            return;
        }

        // 先把选择动画停止，不然会出现缩放突然变回 0.9
        _currentSequence.Kill();
        // 使用世界坐标而不使用本地坐标，本地坐标会受缩放影响
        Vector2 touchPos;
        float y = transform.localPosition.y;    // 当前卡牌的位置
        float oriY = _originPos.y;  // 卡牌原始位置

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((transform.parent as RectTransform), eventData.position, eventData.pressEventCamera, out touchPos))
        {
            return;
        }

        if (touchPos.y >= MAX_HEIGHT)
        {
            // 超过上边沿，隐藏卡牌
            if (_panel.gameObject.activeInHierarchy)
            {
                _panel.gameObject.SetActive(false);
                BattleController.Instance.ShowCursorModel(eventData.position);
            }
            else
            {
                BattleController.Instance.SetCursorModelPosition(eventData.position);
            }
        }
        else
        {
            _transform.anchoredPosition = touchPos;
            // 在卡牌区域内，显示卡牌
            if (!_panel.gameObject.activeInHierarchy)
            {
                _panel.gameObject.SetActive(true);
                BattleController.Instance.HideCursorModel();
            }

            float scale = Mathf.Min(1, (MAX_HEIGHT - y) / (MAX_HEIGHT - oriY));
            transform.localScale = Vector3.one * scale;
        }
    }
}