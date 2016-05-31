using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 布阵界面
public class UIFormationView : UIWindow
{
    public const string Name = "Hero/UIFormationView";
    // 保存阵型
    public void OnClickSave()
    {
        PVPManager.Instance.RequestModify();
    }
}
