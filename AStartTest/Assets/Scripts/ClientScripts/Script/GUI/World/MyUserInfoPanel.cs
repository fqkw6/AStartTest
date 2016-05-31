using UnityEngine;
using UnityEngine.UI;

public class MyUserInfoPanel : MonoBehaviour
{
    public Image _imageIcon;
    public Text _userLevel;

    void Start()
    {

    }

    public void Refresh()
    {
        _imageIcon.sprite = ResourceManager.Instance.GetPlayerIcon(UserManager.Instance.Icon);
        _userLevel.text = UserManager.Instance.Level.ToString();
    }

    public void OnClick()
    {

    }
}
