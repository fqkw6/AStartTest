using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class UICreateRoleView : UIWindow
{
    public const string Name = "Login/UICreateRoleView";
    public InputField _edtName;

    public override void OnOpenWindow()
    {
        OnCreateRandomName();
    }

    public void OnCreateRandomName()
    {
        _edtName.text = UserManager.Instance.GetRandomName();
    }

    public void OnClickCreate()
    {
        string text = _edtName.text;

        // 这里可能为中文，所以要转换为utf8来判断长度
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        if (bytes.Length > GameConfig.MAX_NAME_SIZE || bytes.Length < GameConfig.MIN_NAME_SIZE) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_CHAR_NAME_SIZE_LIMIT", GameConfig.MIN_NAME_SIZE, GameConfig.MAX_NAME_SIZE);
            return;
        }

        ServerManager.Instance.RequestCreateNewRole(_edtName.text, 0, 0);
    }
}
