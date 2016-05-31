using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 血条
public class BattleHealthBar : MonoBehaviour
{
    public Text _txtLevel;
    public Image _imgHP;
    public Text _txtNumber; // 血量数字，塔的prefab是有数字显示的，士兵的则没有

    private int _maxHealth;
    private Actor _actor;
    private Vector2 _offset;

    // Use this for initialization
    void Start()
    {

    }

    public void SetActor(Actor actor)
    {
        _actor = actor;
        _txtLevel.text = actor.Level.ToString();
        _maxHealth = actor.GetMaxHealth();
        _offset = new Vector2(0, _actor.HealthBarOffsetY);
    }

    // 当前血量
    public void SetHealth(int health)
    {
        _imgHP.fillAmount = 1f * health / _maxHealth;

        if (_txtNumber != null)
        {
            _txtNumber.text = health.ToString();
        }
    }

    // 使用LateUpdate，防止单位运动时血条偏移
    void LateUpdate()
    {
        if (_actor == null || Camera.main == null)
        {
            return;
        }

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null) return;

        Vector2 uiPos;
        Canvas canvas = UIManager.Instance.Canvas;
        Vector3 buildingPos = Camera.main.WorldToScreenPoint(_actor.transform.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, buildingPos, canvas.worldCamera, out uiPos))
        {
            rectTransform.anchoredPosition = uiPos + _offset;
        }
    }
}
