using System;
using UnityEngine;
using System.Collections.Generic;
using DynamicShadowProjector;
using Pathfinding.RVO;

// 人物换装 主角可以有丰富的换装系统  追随者可以更换武器  怪物不能换装
public class AvatarComponent : ActorComponent
{
    // 换装的部件信息
    public class AvatarInfo
    {
        public string partName;
        public BodyPart part;
        public GameObject defaultPart;
        public GameObject avatarPart;
    }
    
    protected int _bodyModelId;
    protected GameObject _body;         // 基础模型动画
    protected GameObject _shadow;       // 阴影
    protected Dictionary<BodyPart, AvatarInfo> _avatarInfo = new Dictionary<BodyPart, AvatarInfo>();    // 换装信息
    protected Dictionary<AttachPoint, GameObject> _attachedGameObject = new Dictionary<AttachPoint, GameObject>(); 

    protected Dictionary<BodyPart, string> _defaultPartName = new Dictionary<BodyPart, string>();       // 默认的部件索引关键字
    protected Dictionary<AttachPoint, string> _defaultAttachPoint = new Dictionary<AttachPoint, string>();  // 默认绑点名

    private Actor _actor;
    private Transform _transform;
    private GameObject _gameObject;
    
    public override void OnInit(Actor actor)
    {
        _actor = actor;

        // 身体部件的名字后缀
        _defaultPartName[BodyPart.HAIR] = "helmet";
        _defaultPartName[BodyPart.HELMET] = "helmet";
        _defaultPartName[BodyPart.SHOULDERS] = "shoulders";
        _defaultPartName[BodyPart.GLOVES] = "gloves";
        _defaultPartName[BodyPart.CHEST] = "chest";
        _defaultPartName[BodyPart.BOOTS] = "boots";
        _defaultPartName[BodyPart.PANTS] = "pants";

        // 绑点名字
        _defaultAttachPoint[AttachPoint.RIGHT_HAND] = "tag_righthand";
        _defaultAttachPoint[AttachPoint.LEFT_HAND] = "tag_lefthand";
        _defaultAttachPoint[AttachPoint.LEFT_ARM] = "tag_leftarm";
    }

    public override void OnUpdate(float dt)
    {
    }

    public override void OnDestory()
    {
    }

    // 获取默认部件名字
    protected string GetDefaultPartName(BodyPart part)
    {
        string partName;
        if (_defaultPartName.TryGetValue(part, out partName)) {
            return partName;
        }

        return null;
    }

    // 创建模型
    public void CreateModel(string modelPath, System.Action<GameObject> callback = null)
    {
        // 创建模型时，不要设置scale，scale统一由prefab决定
        GameObject prefab = Resources.Load<GameObject>(modelPath);
        _body = Instantiate(prefab);
        _body.SetActive(true);
        _body.transform.SetParent(transform);
        _body.transform.localPosition = Vector3.zero;

        if (callback != null) {
            callback(_body);
        }
    }
    
    // 创建模型 带动画 怪物一般只需要创建模型就够了，人物还需要换装
    public void CreateModel(int modelId, System.Action<GameObject> callback = null)
    {
        if (_bodyModelId == modelId) {
            return;
        }
    }
    
    // 添加阴影
    public void AddShadow()
    {
        Transform body = _body != null ? _body.transform : transform;
        _shadow = ResourceManager.Instance.GetShadowProjector();
        _shadow.transform.SetParent(body);
        _shadow.transform.localPosition = new Vector3(0, 2.2f, -1);
        _shadow.transform.localRotation = Quaternion.Euler(45, 0, 0);
        _shadow.transform.localScale = Vector3.one;

        DrawTargetObject draw = _shadow.GetComponent<DrawTargetObject>();
        if (draw != null) {
            draw.target = body;
        }
    }

    // 移除阴影
    public void RemoveShadow()
    {
        if (_shadow != null) {
            Destroy(_shadow);
            _shadow = null;
        }
    }

    // 模型换装（主要是怪物，一个基础模型包含动画，其他的模型不包含动画）
    private void ChangeModelAvatar(GameObject baseModel, GameObject newModel)
    {
        SkinnedMeshRenderer[] baseSR = baseModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] newSR = newModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        // TODO 考虑新模型增加或减少一些部件的情况

        if (newSR.Length <= 0) return;
        baseSR[0].sharedMesh = newSR[0].sharedMesh;
        baseSR[0].materials = newSR[0].materials;
    }

    private void ChangeTexture(GameObject go, Texture tex)
    {
        // 没有对应纹理
        if (go == null || tex == null) {
            return;
        }

        Renderer[] rs = go.GetComponentsInChildren<Renderer>();
        foreach (var item in rs) {
            Material[] mats = item.materials;
            foreach (var mat in mats) {
                mat.mainTexture = tex;
            }
        }
    }

    private string GetFullPath(string path)
    {
        return "model/" + path;
    }

    // 添加一个模型到绑点上，如武器 翅膀
    public void AttachModel(int modelId, AttachPoint ap)
    {
    }

    // 给人物换装 avatarId对应avatar配表中id的后两位
    public void AddPart(int avatarId, BodyPart bodyPart)
    {   
    }
    
    // 删除当前部件
    public void RemovePart(BodyPart partType)
    {
        AvatarInfo curAvatar;

        // 确定需要删除的部件
        if (_avatarInfo.TryGetValue(partType, out curAvatar)) {
            UnityEngine.Object.Destroy(curAvatar.avatarPart);
            curAvatar.avatarPart = null;

            if (curAvatar.defaultPart != null) {
                curAvatar.defaultPart.SetActive(true);
            }
        }
    }

    // 替换部件
    protected void ChangePart(BodyPart partType, GameObject avatarModel, Texture texture)
    {
        if (avatarModel == null) {
            return;
        }

        // 先移除当前的部件
        RemovePart(partType);

        string partName = _defaultPartName[partType];
        
        // 需要替换的部件
        Transform avatarPart = GetPart(avatarModel.transform, partName);
        if (avatarPart == null) {
            Log.Error("Avatar Part Not Found: " + partName);
            return;
        }

        SkinnedMeshRenderer avatarRender = avatarPart.GetComponent<SkinnedMeshRenderer>();
        if (avatarRender == null) {
            return;
        }

        // 将原始部件隐藏
        Transform bodyPart = GetPart(_body.transform, partName);
        

        // 设置到body上的新物件
        GameObject newPart = new GameObject(partName);
        newPart.transform.parent = _body.transform;
        SkinnedMeshRenderer newPartRender = newPart.AddComponent<SkinnedMeshRenderer>();
        if (bodyPart != null) {
            bodyPart.gameObject.SetActive(false);
            // 刷新骨骼模型数据
            UpdateBones(newPartRender, avatarRender, bodyPart.GetComponent<SkinnedMeshRenderer>(), _body);
        } else {
            UpdateBones(newPartRender, avatarRender, null, _body);
        }
        newPartRender.sharedMesh = avatarRender.sharedMesh;
        newPartRender.sharedMaterials = avatarRender.sharedMaterials;

        // 记录换装信息
        AvatarInfo info = new AvatarInfo();
        info.partName = partName;
        if (bodyPart != null) {
            info.defaultPart = bodyPart.gameObject;
        } else {
            info.defaultPart = null;
        }
        info.avatarPart = newPart;

        // 更换对应纹理
        foreach (var mat in newPartRender.materials) {
            mat.mainTexture = texture;
        }

        _avatarInfo[partType] = info;
    }
    
    // 递归遍历子物体
    public Transform GetPart(Transform t, string searchName)
    {
        foreach (Transform c in t) {
            string partName = c.name.ToLower();

            if (partName.IndexOf(searchName, StringComparison.Ordinal) != -1) {
                return c;
            } else {
                Transform r = GetPart(c, searchName);
                if (r != null) {
                    return r;
                }
            }
        }
        return null;
    }
    
    // 查找子物体（名称严格匹配）
    public Transform FindChild(Transform t, string searchName)
    {
        foreach (Transform c in t) {
            string partName = c.name;
            if (partName == searchName) {
                return c;
            } else {
                Transform r = FindChild(c, searchName);
                if (r != null) {
                    return r;
                }
            }
        }
        return null;
    }

    // 刷新骨骼数据
    public void UpdateBones(SkinnedMeshRenderer newRenderer, SkinnedMeshRenderer avatarRender, SkinnedMeshRenderer defaultRenderer, GameObject root)
    {
        // newRenderer是新创建的模型（avatar模型的实例，之所以不直接实例化avatar模型是考虑到后期的模型合并）
        // newRenderer的骨骼引用的是角色模型的骨骼并不是新加载的换装模型的骨骼，但是骨骼顺序是换装部件的骨骼顺序 换装模型必须跟角色模型是一套骨骼
        // 换装部件的骨骼数组中元素顺序跟旧部件可以不一致，事实上虽然共用一套骨骼，但是每个部件的骨骼数组中的元素顺序都可能不一致，这个跟美术做模型、刷权重有关
        // 动画驱动骨骼运动，最终带动蒙皮模型运动
        int length = avatarRender.bones.Length;
        var myBones = new Transform[length];
        for (var i = 0; i < length; i++) {
            myBones[i] = FindChild(root.transform, avatarRender.bones[i].name);
        }
        
        newRenderer.bones = myBones;
    }
}
