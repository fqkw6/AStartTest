using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

// 吃经验界面的经验书
public class ExpItemWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image _imgBg;
    public Image _imgBgCover;
    public Image _imgIcon;
    public Text _txtCount;
    public Text _txtName;
    public Text _txtAddExp;
    public PanelHeroExpBook _panelHeroExpBook;

    private int _itemID = 0;
    private HeroInfo _info;
    private ItemInfo _itemInfo;
    private float _curBooksCostNum = 0;  // 本次点击或者按压所消耗的书本
    private int _leftBooksNum = 0;  // 当前剩余多少本经验书
    private float _holdDownTime = 0;  // 按压时间
    private int _currentLevel = 0;  // 当前等级
    private int _currentExp = 0;  // 当前经验,相对于当前等级
    private int _bookAddExp = 0;  // 一本书添加多少经验值
    private int _totalBooksCostNumb = 0;  //  总共花费多少本经验书

    void Start()
    {
    }

    public void SetInfo(int itemID, HeroInfo heroInfo)
    {
        _itemID = itemID;
        _info = heroInfo;
        _currentLevel = heroInfo.Level;
        _currentExp = heroInfo.Exp;

        ItemsConfig cfg = ItemsConfigLoader.GetConfig(itemID);
        if (cfg == null)
        {
            gameObject.SetActive(false);
            return;
        }

        ItemInfo itemInfo = UserManager.Instance.GetItemByConfigID(itemID);
        _itemInfo = itemInfo;

        _imgBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
        _imgBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(cfg.Quality);  // 设置新加的遮挡 icon 背景
        _imgIcon.sprite = ResourceManager.Instance.GetItemIcon(itemID);

        if (itemInfo != null && itemInfo.Number > 0)
        {
            _txtCount.color = Color.white;
            _txtCount.text = "x" + itemInfo.Number;
            _leftBooksNum = itemInfo.Number;
        }
        else
        {
            _txtCount.color = Color.red;
            _txtCount.text = "x0";
        }
        _txtName.text = cfg.Name;
        _txtAddExp.text = "+" + cfg.ExpIncrease;
        _bookAddExp = cfg.ExpIncrease;
    }

    public void OnClick()
    {
        ItemInfo itemInfo = UserManager.Instance.GetItemByConfigID(_itemID);
        if (itemInfo == null || itemInfo.Number == 0)
        {
            return;
        }
        //UserManager.Instance.RequestUseExpBook(_info.EntityID, itemInfo.EntityID, 1);
    }

    void Update()
    {

    }

    private void UpadateTimer()
    {
        if (_info.Level >= UserManager.Instance.Level)
        {
            return;
        }
        ItemInfo itemInfo = UserManager.Instance.GetItemByConfigID(_itemID);
        if (itemInfo == null)
        {
            return;
        }

        if (itemInfo.Number > 0)
        {
            UserManager.Instance.UseItemByConfigID(_itemID);
            _txtCount.color = Color.white;
            _txtCount.text = "x" + itemInfo.Number;
        }
        else
        {
            _txtCount.color = Color.red;
            _txtCount.text = "x0";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 当前没有该经验书或者不能继续升级时直接返回
        if (_itemInfo == null || _itemInfo.Number == 0 || GetMaxCostBooks() <= 0)
        {
            return;
        }
        _holdDownTime = 0;
        _totalBooksCostNumb = 0;
        // 至少消耗 1 本书
        _curBooksCostNum = 1;
        // 一秒之后开始统计
        InvokeRepeating("BookCostUpdateTimer", 1, 0.1f);
    }

    // 计算当前能花费的经验书数目
    public int GetMaxCostBooks()
    {
        HeroLevelConfig curHeroExpCfg = HeroLevelConfigLoader.GetConfig(_currentLevel);
        int curTotalExp = curHeroExpCfg.TotalExp + _currentExp;
        // 计算英雄能达到的最大等级（不超过玩家等级）
        int maxLevel = UserManager.Instance.Level;
        HeroLevelConfig maxLevelExpCfg = HeroLevelConfigLoader.GetConfig(maxLevel);
        int needExpToMaxLevel = maxLevelExpCfg.TotalExp - curTotalExp;
        // 计算达到最大等级需要的经验数目，需注意，该经验书数目为 0 时 _bookAddExp 为 0
        int needBookToMaxLevel = needExpToMaxLevel / (_bookAddExp > 0 ? _bookAddExp : 1);
        // 负数表示英雄已达到最大等级
        if (needBookToMaxLevel < 0) needBookToMaxLevel = 0;

        return needBookToMaxLevel > _leftBooksNum ? _leftBooksNum : needBookToMaxLevel;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_itemInfo == null || _itemInfo.Number == 0)
        {
            return;
        }
        if (IsInvoking("BookCostUpdateTimer"))
        {
            CancelInvoke("BookCostUpdateTimer");
        }

        // 至少要消耗一本
        _totalBooksCostNumb = _totalBooksCostNumb > 0 ? _totalBooksCostNumb : 1;
        // 向服务器发消息
        //AddExpByCurExp(_currentLevel, _totalAddBooks * _bookAddExp);
        UserManager.Instance.RequestUseExpBook(_info.EntityID, _itemInfo.EntityID, _totalBooksCostNumb);
    }

    // 持续按压累加消耗经验书
    private void BookCostUpdateTimer()
    {
        if (_leftBooksNum <= 0) return;
        _holdDownTime += 0.1f;
        // 1s 内，每秒钟消耗 5 本
        if (_holdDownTime > 0 && _holdDownTime <= 1)
        {
            _curBooksCostNum += 0.5f;
        }
        else if (_holdDownTime > 1 && _holdDownTime <= 2)
        {
            _curBooksCostNum += 1f;
        }
        else if (_holdDownTime > 2)
        {
            _curBooksCostNum += 2f;
        }

        int booksCostNum = Mathf.FloorToInt(_curBooksCostNum) > GetMaxCostBooks() ? GetMaxCostBooks() : Mathf.FloorToInt(_curBooksCostNum);
        _leftBooksNum -= booksCostNum;
        _txtCount.text = "x" + (_leftBooksNum);
        _totalBooksCostNumb += booksCostNum;
        AddExpBaseOnCurExp(_currentLevel, booksCostNum * _bookAddExp);
    }


    // 在当前经验（相对于当前等级）的基础上加经验
    public void AddExpBaseOnCurExp(int curLevel, int booksAddExp)
    {
        int totalAddExp = booksAddExp + _currentExp;
        int requiredExp = 0;
        int reduceExp = 0;
        for (int i = curLevel; i <= HeroLevelConfigLoader.Data.Count; i++)
        {
            HeroLevelConfig config = HeroLevelConfigLoader.GetConfig(i);
            requiredExp += config.ExpRequire;
            // 一直累加直到能再升级
            if (totalAddExp < requiredExp)
            {
                _currentLevel = config.Level;
                _currentExp = totalAddExp - reduceExp;
                break;
            }

            //超过90级后 经验不增加
            if (i == HeroLevelConfigLoader.Data.Count)
            {
                _currentLevel = config.Level;
                _currentExp = 0;
            }

            // 累加用于计算升级后对应的当前经验
            reduceExp += config.ExpRequire;
            //_panelHeroExpBook.SetExpProgress(_currentLevel, _currentExp);

            EventDispatcher.TriggerEvent<int, int>(EventID.EVENT_EXPBOOK_COST, _currentLevel, _currentExp);

        }
        // 设置经验进度条,break 出来之后
        EventDispatcher.TriggerEvent<int, int>(EventID.EVENT_EXPBOOK_COST, _currentLevel, _currentExp);
        //_panelHeroExpBook.SetExpProgress(_currentLevel, _currentExp);
    }
}
