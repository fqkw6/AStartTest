using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BagItemWidget : ItemWidget
{
    public Image _imgFlag;

    public override void SetInfo(object data)
    {
        base.SetInfo(data);

        ItemInfo info = data as ItemInfo;
        if (info == null) return;

        _imgFlag.gameObject.SetActive(info.IsNewItem);
    }

    public void ClearFlag()
    {
        _imgFlag.gameObject.SetActive(false);
    }
}
