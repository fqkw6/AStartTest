using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择公会旗帜的界面
public class UIGuildFlagView : UIWindow
{
    public const string Name = "Guild/UIGuildFlagView";
    public Text _txtGuildName;
    public Image _imgFlagView;
    public Text _txtFlagTextView;
    public InputField _inputFlagText;
    public Sprite[] _spriteFlag;
    public Toggle[] _toggleFlag;
    public GuildFlagColorWidget[] _imgFlagColor;
    public GuildFlagColorWidget[] _imgFlagTextColor;
    public Color[] _flagColor;

    private int _flagIndex;
    private int _flagColorIndex;
    private int _textColorIndex;
    private string _flagText;

    public override void OnRefreshWindow()
    {
        _imgFlagView.gameObject.SetActive(GuildManager.Instance.GuildFlagIndex >= 0);

        _flagIndex = GuildManager.Instance.GuildFlagIndex;
        _flagColorIndex = GuildManager.Instance.GuildFlagColorIndex;
        _textColorIndex = GuildManager.Instance.GuildFlagColorIndex;
        _flagText = GuildManager.Instance.GuildFlagText;

        SetFlag(_flagIndex);
        SetFlagColor(_flagColorIndex);
        SetTextColor(_textColorIndex);

        for (int i = 0; i < _imgFlagColor.Length; ++i) {
            _imgFlagColor[i].SetColor(_flagColor[i]);
            _imgFlagColor[i].Index = i;
            _imgFlagColor[i].OnClickCallback = OnClickFlagColor;

            _imgFlagTextColor[i].SetColor(_flagColor[i]);
            _imgFlagTextColor[i].Index = i;
            _imgFlagTextColor[i].OnClickCallback = OnClickTextColor;
        }
    }

    private void SetFlag(int index)
    {
        if (index < 0 || index >= _spriteFlag.Length) return;

        _flagIndex = index;
        _imgFlagView.sprite = _spriteFlag[index];
    }

    private void SetFlagColor(int index)
    {
        if (index < 0 || index >= _flagColor.Length) return;

        _flagColorIndex = index;
        _imgFlagView.color = _flagColor[index];
    }

    private void SetTextColor(int index)
    {
        if (index < 0 || index >= _flagColor.Length) return;

        _textColorIndex = index;
        _txtFlagTextView.color = _flagColor[index];
    }

    public void OnClickSave()
    {
        GuildManager.Instance.RequestModifyFlag(_flagIndex, _flagColorIndex, _textColorIndex, _flagText);
    }

    public void OnClickFlagColor(int index)
    {
        SetFlagColor(index);
    }

    public void OnClickTextColor(int index)
    {
        SetTextColor(index);
    }

    // 输入旗号完毕
    public void OnEndEdit(string text)
    {
        if (text.Length > 1) {
            _flagText = text.Substring(0, 1);
        } else {
            _flagText = text;
        }

        _txtFlagTextView.text = _flagText;
    }
}
