using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

// 角色头顶血条信息显示
// 小兵、建筑显示正常血条
// 英雄显示分段血条
public class HUDComponent : ActorComponent
{
    private BattleHealthBar _healthBar;
    private Actor _actor;
    private Text _txtDmgPrefab;

    public override void OnInit(Actor actor)
    {
        _actor = actor;
        //_txtDmgPrefab = Resources.Load<Text>("Misc/DmgText");
    }

    public override void OnUpdate(float dt)
    {
    }

    public override void OnDestory()
    {
        // 对象销毁的时候，血条一并销毁
        if (_healthBar != null) {
            Destroy(_healthBar.gameObject);
        }
    }

    public void CreateHealthBar(string prefabName)
    {
        var prefab = Resources.Load<BattleHealthBar>(prefabName);
        _healthBar = Instantiate(prefab);

        // 一开始隐藏血条
        _healthBar.gameObject.SetActive(false);

        Transform root = UIManager.Instance.Canvas.transform;
        if (root != null) {
            _healthBar.transform.SetParent(root);
            _healthBar.transform.localScale = Vector3.one;
            _healthBar.transform.localPosition = Vector3.zero;

            // 设置数据
            _healthBar.SetActor(_actor);
        }
    }


    // 显示血条（在受击的时候显示，敌方公主塔一开始就显示）
    public void ShowHealthBar()
    {
        if (_healthBar != null) {
            _healthBar.gameObject.SetActive(true);
        }
    }

    // 隐藏血条
    public void HideHealthBar()
    {
        if (_healthBar != null) {
            _healthBar.gameObject.SetActive(false);
        }
    }

    // 设置血量数值
    public void SetHealth(int health)
    {
        if (_healthBar != null) {
            _healthBar.SetHealth(health);
        }
    }

    // 掉血
    public void FloatingBlood(int number)
    {
        if (_txtDmgPrefab == null) {
            return;
        }

        Transform root = UIManager.Instance.Canvas.transform;
        Text txt = Instantiate(_txtDmgPrefab);
        txt.text = "-" + number;
        txt.color = Color.red;
        txt.transform.SetParent(root, false);
        Vector2 uiPos;
        Vector3 buildingPos = Camera.main.WorldToScreenPoint(_actor.transform.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(root as RectTransform, buildingPos, UIManager.Instance.Canvas.worldCamera, out uiPos)) {
            txt.rectTransform.anchoredPosition = uiPos + new Vector2(0, _actor.HealthBarOffsetY);
        }

        txt.rectTransform.DOAnchorPosY(txt.rectTransform.anchoredPosition.y + 50, 1);
        txt.DOFade(0, 1.2f).OnComplete(() => {
            Destroy(txt.gameObject);
        });
    }

    // 加血
    public void FloatingHealth(int number)
    {
        if (_txtDmgPrefab == null) {
            return;
        }

        Transform root = UIManager.Instance.Canvas.transform;
        Text txt = Instantiate(_txtDmgPrefab);
        txt.text = "+" + number;
        txt.color = Color.green;
        txt.transform.SetParent(root, false);
        Vector2 uiPos;
        Vector3 buildingPos = Camera.main.WorldToScreenPoint(_actor.transform.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(root as RectTransform, buildingPos, UIManager.Instance.Canvas.worldCamera, out uiPos)) {
            txt.rectTransform.anchoredPosition = uiPos + new Vector2(0, _actor.HealthBarOffsetY);
        }

        txt.rectTransform.DOAnchorPosY(txt.rectTransform.anchoredPosition.y + 50, 1);
        txt.DOFade(0, 1.2f).OnComplete(() => {
            Destroy(txt.gameObject);
        });
    }

    public void FloatingCritical(int number)
    {

    }

    public void FloatingMiss()
    {

    }

    // 头顶漂字
    public void FloatingText(string text, Color color)
    {

    }
}
