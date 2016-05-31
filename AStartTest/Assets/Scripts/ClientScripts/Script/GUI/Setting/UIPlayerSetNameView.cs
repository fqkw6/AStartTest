using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 设置玩家名字界面
public class UIPlayerSetNameView : UIWindow
{
    public const string Name = "Setting/UIPlayerSetNameView";

    public Text _txtCost;
    public InputField _inputName;

    public override void OnOpenWindow()
    {
        _txtCost.text = GameConfig.CHANGE_NAME_COST.ToString();
    }

    public void OnClickOK()
    {
        string newName = _inputName.text;
        if (string.IsNullOrEmpty(newName)) {
            return;
        }

        if (UserManager.Instance.Gold < GameConfig.CHANGE_NAME_COST) {
            UIUtil.ShowErrMsg("MSG_CITY_BUILDING_GOLD_LIMIT");
            return;
        }

        UserManager.Instance.RequestChangeName(newName);
    }
}
