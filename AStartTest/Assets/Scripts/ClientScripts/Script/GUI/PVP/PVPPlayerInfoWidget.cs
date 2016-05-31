using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// pvp界面中对手玩家组件
public class PVPPlayerInfoWidget : MonoBehaviour
{
    public Image _imgIcon;
    public Text _txtName;
    public Text _txtLevel;
    public Text _txtRank;
    public Text _txtFightScore;

    private PVPPlayerInfo _info;

	void Start ()
    {
	}

    public void SetInfo(PVPPlayerInfo info)
    {
        _info = info;
        _imgIcon.sprite = ResourceManager.Instance.GetPlayerIcon(info.Icon);
        _txtName.text = info.Name;
        _txtLevel.text = "Lv" + info.Level;
        _txtRank.text = info.Rank.ToString();
        _txtFightScore.text = info.FightScore.ToString();
    }

    public void OnClick()
    {
        UIManager.Instance.OpenWindow<UIPVPPlayerInfoView>(_info);
    }


    public void OnClickAttack()
    {
        if (PVPManager.Instance.AttackCount <= 0) {
            int count = GameConfig.PVP_MAX_ATTACK_COUNT - PVPManager.Instance.AttackCount;
            UIUtil.ShowConfirm(Str.Format("UI_PVP_BUY_ATTACK_COUNT_CONFIRM", count * GameConfig.PVP_GOLD_PER_ATTACK_COUNT), "", () =>
            {
                PVPManager.Instance.RequestBuyAttackChance(count);
            });
            return;
        }

        PVPManager.Instance.RequestAttack(_info.EntityID);
    }
}
