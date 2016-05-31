using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UINewPVEEntranceView : UIWindow
{
    public const string Name = "PVE/UINewPVEEntranceView";

    public Text _txtLevelTitle;
    public Text _txtLevelDesc;
    public Text _txtFightScore;
    public Image _imgLevelPic;

    public Toggle _toggleNormal;
    public Toggle _toggleElite;
    
    public UIListView _listChapter;
    public UIListView _listLevel;

    private int _currentChapter = 1;
    private int _currentLevel = 0;

    private List<PVEChapterListItemWidget> _listChapterWidgets = new List<PVEChapterListItemWidget>();
    private List<PVELevelListItemWidget> _listLevelWidgets = new List<PVELevelListItemWidget>();  

    public override void OnOpenWindow()
    {
        if (PVEManager.Instance.ChapterType == ChapterType.NORMAL) {
            _toggleNormal.isOn = true;
        } else if (PVEManager.Instance.ChapterType == ChapterType.ELITE) {
            _toggleElite.isOn = true;
        }
    }

    public override void OnRefreshWindow()
    {
        RefreshChapterList();
        RefreshLevelList();

        if (PVEManager.Instance.CurrentSelectChapterID == 0) {
            SelectChapterByIndex(0);
        } else {
            OnSelectChapter(PVEManager.Instance.CurrentSelectChapterID);
        }

        if (PVEManager.Instance.CurrentSelectLevelID == 0) {
            SelectLevelByIndex(0);
        } else {
            OnSelectLevel(PVEManager.Instance.CurrentSelectLevelID);
            ScrollToLevel(PVEManager.Instance.CurrentSelectLevelID);
        }
    }

    private void RefreshChapterList()
    {
        List<int> chapterList = new List<int>();
        foreach (var item in ChapterConfigLoader.Data) {
            chapterList.Add(item.Key);
        }
        _listChapter.Data = chapterList.ToArray();
        _listChapter.Refresh();

        _listChapterWidgets.Clear();
        List<ListItemWidget> widgets = _listChapter.ListWidget;
        foreach (var item in widgets) {
            PVEChapterListItemWidget widget = item as PVEChapterListItemWidget;
            if (widget) {
                widget.OnClickChaperCallback = OnSelectChapter;
            }

            _listChapterWidgets.Add(widget);
        }
    }

    private void RefreshLevelList()
    {
        List<int> levelList = new List<int>();
        foreach (var item in MissionConstConfigLoader.Data) {
            if (item.Value.Chapter == _currentChapter && item.Value.MissionDegree == (int)PVEManager.Instance.ChapterType) {
                levelList.Add(item.Key);
            }
        }

        _listLevel.Data = levelList.ToArray();
        _listLevel.Refresh();

        _listLevelWidgets.Clear();
        List<ListItemWidget> widgets = _listLevel.ListWidget;
        foreach (var item in widgets) {
            PVELevelListItemWidget widget = item as PVELevelListItemWidget;
            if (widget) {
                widget.OnClickLevelCallback = OnSelectLevel;
            }

            _listLevelWidgets.Add(widget);
        }
    }

    private void SelectChapterByIndex(int index)
    {
        for (int i = 0; i < _listChapterWidgets.Count; ++i) {
            var item = _listChapterWidgets[i];
            if (i == index) {
                _currentChapter = item._chapterID;
                item.Select();
                SelectLevelByIndex(0);
            } else {
                item.UnSelect();
            }
        }
    }

    private void SelectLevelByIndex(int index)
    {
        for (int i = 0; i < _listLevelWidgets.Count; ++i) {
            var item = _listLevelWidgets[i];
            if (i == index) {
                item.Select();
                MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(item._levelID);
                _txtLevelTitle.text = cfg.MissionName;
                _txtLevelDesc.text = cfg.MissionDescription;
                _txtFightScore.text = cfg.RecommendStrength.ToString();
                _currentLevel = item._levelID;
            } else {
                item.UnSelect();
            }
        }
    }

    private void OnSelectChapter(int chapterID)
    {
        _currentChapter = chapterID;
        PVEManager.Instance.CurrentSelectChapterID = chapterID;

        foreach (var item in _listChapterWidgets) {
            if (chapterID == item._chapterID) {
                item.Select();
            } else {
                item.UnSelect();
            }
        }

        RefreshLevelList();
        SelectLevelByIndex(0);
    }

    private void OnSelectLevel(int levelID)
    {
        _currentLevel = levelID;
        PVEManager.Instance.CurrentSelectLevelID = levelID;

        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        _txtLevelTitle.text = cfg.MissionName;
        _txtLevelDesc.text = cfg.MissionDescription;
        _txtFightScore.text = cfg.RecommendStrength.ToString();

        ListItemWidget widget = null;
        foreach (var item in _listLevelWidgets) {
            if (levelID == item._levelID) {
                item.Select();
                widget = item;
            } else {
                item.UnSelect();
            }
        }
    }

    private void ScrollToLevel(int levelID)
    {
        ListItemWidget widget = null;
        foreach (var item in _listLevelWidgets) {
            if (levelID == item._levelID) {
                widget = item;
            }
        }
        _listLevel.ScrollTo(widget);
    }

    public void OnToggleNormal()
    {
        if (_toggleNormal.isOn) {
            PVEManager.Instance.ChapterType = ChapterType.NORMAL;
            OnRefreshWindow();
        }
    }

    public void OnToggleElite()
    {
        if (_toggleElite.isOn) {
            PVEManager.Instance.ChapterType = ChapterType.ELITE;
            OnRefreshWindow();
        }
    }

    public void OnClickGO()
    {
        if (!PVEManager.Instance.IsLevelEnable(_currentLevel)) {
            return;
        }

        UIManager.Instance.OpenWindow<UINewPVELevelInfoView>(_currentLevel);
    }
}

