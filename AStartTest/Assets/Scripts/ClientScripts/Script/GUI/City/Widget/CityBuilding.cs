using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CityBuilding : MonoBehaviour
{
    public int _buildingCfgID;

    private BuildingInfo _currentInfo = null;

    [NonSerialized]
    public RectTransform Parent;
    [NonSerialized]
    public long EntityID = 0;

    private GameObject _buildingEffect = null;
    private GameObject _buildingWorkder = null;
    private GameObject _goSoldier = null;
    private bool _isLock = false;
    private bool _hasLock = false;

    private void Awake()
    {
    }

    void OnDestory()
    {
        if (_buildingEffect != null) {
            Destroy(_buildingEffect);
        }

        if (_goSoldier != null) {
            Destroy(_goSoldier);
        }
    }

    void OnDisable()
    {
        if (_buildingEffect != null) {
            Animator anim = _buildingEffect.GetComponent<Animator>();
            if (anim != null) {
                anim.enabled = false;
            }
        }
    }

    //    private void OnDrawGizmos()
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(transform.position, 1);
    //    }

    public void Refresh()
    {
        BuildingInfo info = CityManager.Instance.GetBuildingByConfigID(_buildingCfgID);
        _currentInfo = info;

        if (_currentInfo != null) {
            EntityID = _currentInfo.EntityID;
            if (_currentInfo.IsInBuilding()) {
                AddBuildingEffect();
            } else {
                RemoveBulidingEffect();
            }

            if (_hasLock) {
                MeshRenderer render = GetComponent<MeshRenderer>();
                if (render != null) {
                    render.material.shader = Shader.Find("Mobile/Diffuse");
                }
            }

            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null) {
                if (tbinfo.SoldierConfigID != 0 && tbinfo.SoldierCount > 0) {
                    if (_goSoldier == null) {
                        GameObject prefab = null;
                        if (tbinfo.SoldierConfigID == 1) {
                            prefab = Resources.Load<GameObject>("Effect/bingying_daobing");
                        } else if (tbinfo.SoldierConfigID == 2) {
                            prefab = Resources.Load<GameObject>("Effect/bingying_qibing");
                        } else if (tbinfo.SoldierConfigID == 3) {
                            prefab = Resources.Load<GameObject>("Effect/bingying_daobing");
                        } else if (tbinfo.SoldierConfigID == 4) {
                            prefab = Resources.Load<GameObject>("Effect/bingying_gongbing");
                        }
                        if (prefab != null) {
                            _goSoldier = Instantiate(prefab);
                            _goSoldier.transform.position = transform.position;
                        }
                    }
                } else {
                    if (_goSoldier != null) {
                        Destroy(_goSoldier);
                        _goSoldier = null;
                    }
                }
            }

        } else {
            if (!_hasLock) {
                _hasLock = true;
                MeshRenderer render = GetComponent<MeshRenderer>();
                if (render != null) {
                    Material mat = new Material(render.material);
                    mat.shader = Shader.Find("Shader/GrayScale");
                    render.material = mat;
                }
            }

        }
    }

    public void OnClick()
    {
        if (_currentInfo == null) {
            // 如果尚未获得此建筑，建筑尚未解锁，则提示解锁等级
            BuildingConstConfig cfg = BuildingConstConfigLoader.GetConfig(_buildingCfgID);
            if (cfg != null) {
                UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_UNLOCK", cfg.UnlockHomeLevelDemand);
            }
        } else {
            if (_currentInfo.IsInBuilding()) {
                UIManager.Instance.OpenWindow<UICityBuildingMenuView>(_currentInfo, this, Parent);
                return;
            }

            if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
                TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
                if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                    // 校场正在训练士兵
                    UIManager.Instance.OpenWindow<UICityTrainMenuView>(_currentInfo, this, Parent);
                    return;
                }
            } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
                // 兵营正在生产士兵
                TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
                if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                    UIManager.Instance.OpenWindow<UICityBuildingMenuView>(_currentInfo, this, Parent);
                    return;
                }
            }

            // 正常逻辑
            switch (_currentInfo.BuildingType) {
                case CityBuildingType.HOUSE:
                case CityBuildingType.WOOD:
                case CityBuildingType.STONE:
                    OnClickProduce();
                    break;
                case CityBuildingType.MONEY_STORAGE:
                case CityBuildingType.STONE_STORAGE:
                case CityBuildingType.WOOD_STORAGE:
                    OnClickBuilding();
                    break;
                case CityBuildingType.PALACE:
                    OnClickPalace();
                    break;
                case CityBuildingType.TRAIN:
                    OnClickTrain();
                    break;
                case CityBuildingType.TROOP:
                    OnClickTroop();
                    break;
                case CityBuildingType.SMITHY:
                    OnClickSmithy();
                    break;
                case CityBuildingType.COLLEGE:
                    OnClickCollege();
                    break;
            }
        }
    }

    public void OnClickProduceIcon()
    {
        ProduceBuildingInfo pbinfo = _currentInfo as ProduceBuildingInfo;
        if (pbinfo == null) return;

        if (!pbinfo.IsProduceFull() && pbinfo.GetCurrentProduceValue() > 0) {
            // 如果资源未满，并且有产出 则请求收货
            CityManager.Instance.RequestHarvest(_currentInfo.EntityID);
        } else {
            // 资源已满
            UIUtil.ShowMsgFormat("UI_MSG_RES_FULL", _currentInfo.GetContainerBuildingName(), _currentInfo.GetResName());
        }
    }

    private void OnClickBuilding()
    {
        // 这几个功能没有收获功能，只显示信息
        UIManager.Instance.OpenWindow<UICityBuildingUplevelView>(_currentInfo);
    }

    // 官邸
    private void OnClickPalace()
    {
        UIManager.Instance.OpenWindow<UICityPalaceTrainLevelUpView>(_currentInfo);
    }

    private void OnClickProduce()
    {
        ProduceBuildingInfo pbinfo = _currentInfo as ProduceBuildingInfo;
        if (pbinfo == null) return;

        if (!pbinfo.IsProduceFull() && pbinfo.GetCurrentProduceValue() > 0 && !pbinfo.IsInBuilding()) {
            // 如果资源未满，并且有产出 则请求收货
            Log.Info("收集资源： {0}", pbinfo.GetCurrentProduceValue());
            CityManager.Instance.RequestHarvest(_currentInfo.EntityID);

            if (pbinfo.BuildingType == CityBuildingType.WOOD) {
                EventDispatcher.TriggerEvent(EventID.EVENT_CITY_AWARD_WOOD);
            } else if (pbinfo.BuildingType == CityBuildingType.STONE) {
                EventDispatcher.TriggerEvent(EventID.EVENT_CITY_AWARD_STONE);
            }
        } else {
            // 点击界面，则隐藏资源收获图标
            EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_SHOW_PRODUCE_PANEL, pbinfo.EntityID, false);

            // 如果没有产出，则打开信息界面，此为升级等功能入口
            UIManager.Instance.OpenWindow<UICityBuildingUplevelView>(_currentInfo);
        }
    }

    // 点击校场
    private void OnClickTrain()
    {
        // 校场，显示士兵升级列表
        UIManager.Instance.OpenWindow<UICityTrainSelectView>();
    }

    // 点击兵营
    private void OnClickTroop()
    {
        TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
        if (tbinfo == null) return;

        if (tbinfo.SoldierCount <= 0) {
            // 没有士兵，直接打开选择士兵的界面
            UIManager.Instance.OpenWindow<UICitySoldierSelectView>(_currentInfo);
        } else {
            // 有士兵，打开切换士兵的界面
            UIManager.Instance.OpenWindow<UICitySoldierSwitchView>(_currentInfo);
        }
    }

    // 点击铁匠铺
    private void OnClickSmithy()
    {
        UIManager.Instance.OpenWindow<UICityBuildingSmithyView>(_currentInfo);
    }

    // 点击书院
    private void OnClickCollege()
    {
        //UIManager.Instance.OpenWindow<UICityBuildingSmithyView>(_currentInfo);
    }

    // 添加正在建筑的特效
    public void AddBuildingEffect()
    {
        if (_buildingEffect != null) {
            return;
        }

        _buildingEffect = Instantiate(Resources.Load<GameObject>("Effect/xiujian_Eff"));  // 添加木桩
        _buildingEffect.SetActive(true);
        _buildingEffect.transform.SetParent(transform, false);
        _buildingEffect.transform.localPosition = Vector3.zero;

        if (_currentInfo.BuildingType == CityBuildingType.PALACE) {  // 主城
            // 主城模型大
            _buildingEffect.transform.localScale = Vector3.one * (3 / transform.localScale.x);
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {  // 兵营
            _buildingEffect.transform.localPosition += new Vector3(0, 0, 5);
            _buildingEffect.transform.localScale = Vector3.one * (2 / transform.localScale.x);
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {  // 校场
            _buildingEffect.transform.localScale = Vector3.one * (2 / transform.localScale.x);
            _buildingEffect.transform.localPosition += new Vector3(0, 0, 5);
        } else {
            _buildingEffect.transform.localScale = Vector3.one * (1 / transform.localScale.x);
        }
        _buildingEffect.transform.localRotation = Quaternion.Euler(90, 0, 0);

        _buildingWorkder = Instantiate(Resources.Load<GameObject>("Effect/gongren_xiujian"));  // 自身带有 Animation 组件自动播放
        if (_buildingWorkder != null) {
            _buildingWorkder.transform.SetParent(_buildingEffect.transform, false);
            _buildingWorkder.transform.localPosition = Vector3.zero;
        }

        Animator anim = _buildingEffect.GetComponent<Animator>();
        if (anim != null) {
            anim.enabled = true;
        }
    }

    // 删除正在建筑的特效
    public void RemoveBulidingEffect()
    {
        if (_buildingEffect != null) {
            Destroy(_buildingEffect);
        }

        if (_buildingWorkder != null) {
            Destroy(_buildingWorkder);
        }
    }

    public void OnLevelUp()
    {
        ShowBuildingFinishEffect();
    }

    public void ShowBuildingFinishEffect()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Effect/eff_jianzuwancheng"));
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0, 0, 0);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
        Destroy(go, 3);
    }
}