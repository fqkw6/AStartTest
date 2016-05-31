using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 战斗数据分析界面
public class UIPVEBattleDataView : UIWindow
{
    public const string Name = "PVE/UIPVEBattleDataView";
    //    public Image _attackHeroImage;
    //    public Text _attackHeroLevel;
    //    public Image _defendHeroImage;
    //    public Text _defendHeroLevel;

    public HeroBattleDataWidget _heroWidget;
    public HeroBattleDataWidget _enemyWidget;

    public Sprite[] _soldierSprite;
    public Image[] _imgCost;
    public Text[] _txtCost;

    public float _yOffset = 100;

    public override void OnOpenWindow()
    {
        BattleDataInfo data = PVEManager.Instance.BattleData;
        if (data == null) return;

        // 没有英雄数据
        if (data.heroInfo.Count <= 0) {
            _heroWidget.gameObject.SetActive(false);
//            _attackHeroImage.gameObject.SetActive(false);
//            _attackHeroLevel.gameObject.SetActive(false);
        }

        if (data.enemyHeroInfo.Count <= 0) {
            _enemyWidget.gameObject.SetActive(false);
//            _defendHeroImage.gameObject.SetActive(false);
//            _defendHeroLevel.gameObject.SetActive(false); 
        }

        if (data.heroInfo.Count >= 1) {
           // 需要额外实例化
            for (int i = 1; i < data.heroInfo.Count; ++i) {
                HeroBattleDataWidget widget = Instantiate(_heroWidget);
                widget.transform.SetParent(_heroWidget.transform.parent);
                Vector3 pos = _heroWidget.transform.localPosition;
                widget.transform.localPosition = new Vector3(pos.x, pos.y + _yOffset * i, pos.z);
                widget.SetInfo(data.heroInfo[i], data.totalDamage, data.totalDamageGet, data.totalKill, data.totalTime);
            }
            
            // 第一个英雄
            _heroWidget.SetInfo(data.heroInfo[0], data.totalDamage, data.totalDamageGet, data.totalKill, data.totalTime);

//            _attackHeroImage.sprite = ResourceManager.Instance.GetHeroIcon(data.heroInfo[0].heroID);
//            _attackHeroLevel.text = data.heroInfo[0].level.ToString();

        }

        if (data.enemyHeroInfo.Count >= 1) {
            // 需要额外实例化
            for (int i = 1; i < data.enemyHeroInfo.Count; ++i) {
                HeroBattleDataWidget widget = Instantiate(_enemyWidget);
                widget.transform.SetParent(_enemyWidget.transform.parent);
                Vector3 pos = _enemyWidget.transform.localPosition;
                widget.transform.localPosition = new Vector3(pos.x, pos.y + _yOffset * i, pos.z);
                widget.SetInfo(data.enemyHeroInfo[i], data.totalEnemyDamage, data.totalEnemyDamageGet, data.totalEnemyKill, data.totalEnemyTime);
            }

            // 第一个敌人
            _enemyWidget.SetInfo(data.enemyHeroInfo[0], data.totalEnemyDamage, data.totalEnemyDamageGet, data.totalEnemyKill, data.totalEnemyTime);

//            _defendHeroImage.sprite = ResourceManager.Instance.GetHeroIcon(data.enemyHeroInfo[0].heroID);
//            _defendHeroLevel.text = data.enemyHeroInfo[0].level.ToString();
        }

        for (int i = 0; i < 6; ++i) {
            if (i < data.costInfo.Count) {
                // TODO 根据兵种职业获取对应图片
                //_imgCost[i].sprite = null;
                _txtCost[i].text = data.costInfo[i].costCount.ToString();
            } else {
                _imgCost[i].gameObject.SetActive(false);
                _txtCost[i].gameObject.SetActive(false);
            }
        }
    }
}

