using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ETouch;

public class CameraBase : MonoBehaviour {
    public Transform target;                // 观察目标
    public float startingDistance = 10f; // Distance the camera starts from target object.
    public float maxDistance = 20f; // Max distance the camera can be from target object.
    public float minDistance = 3f; // Min distance the camera can be from target object.
    public float zoomSpeed = 20f; // The speed the camera zooms in.
    public float targetHeight = 1.0f; // The amount from the target object pivot the camera should look at.
    public float camRotationSpeed = 70;// The speed at which the camera rotates.
    public float camXAngle = 45.0f; // The camera x euler angle.
    public bool fadeObjects = false; // Enable objects of a certain layer to be faded.
    public List<int> layersToTransparent = new List<int>();	// The layers where we will allow transparency.
    public float alpha = 0.3f; // The alpha value of the material when player behind object.
    public bool pinchZoom = true; // If the camera is allowed to check for pichZoom.

    protected float y = 0.0f; // The camera y euler angle.
    protected Transform myTransform;
    protected Transform prevHit;
    protected float minCameraAngle = 0.0f; // The min angle on the camera's x axis.
    protected float maxCameraAngle = 90.0f; // The max angle on the camera's x axis.
    protected float prevDistance = 0.0f; // For pinch zoom to check prev distance between touches.
	
    protected List<GameObject> lastColliderObject;      //上次碰撞到的物体
    protected List<GameObject> colliderObject;          //本次碰撞到的物体

    protected void CheckFadeObjects()
    {
        if (!fadeObjects) {
            return;
        }

        /*射线可以从头部起始*/

        //这里是计算射线的方向，从主角发射方向是射线机方向
        Vector3 aim = target.position;
        //得到方向
        Vector3 ve = (target.position - transform.position).normalized;
        float an = transform.eulerAngles.y;
        aim -= an * ve;

        //在场景视图中可以看到这条射线
        //Debug.DrawLine(target.position, aim, Color.red);

        RaycastHit[] hit;
        hit = Physics.RaycastAll(target.position, aim, 100f);//起始位置、方向、距离

        //将 colliderObject 中所有的值添加进 lastColliderObject
        for (int i = 0; i < colliderObject.Count; i++)
            lastColliderObject.Add(colliderObject[i]);

        colliderObject.Clear();//清空本次碰撞到的所有物体
        for (int i = 0; i < hit.Length; i++)//获取碰撞到的所有物体
        {
            if (hit[i].collider.gameObject.name != "Editable Poly 1"//护栏
                && hit[i].collider.gameObject.name != "Editable Poly"//地面
                && hit[i].collider.gameObject.tag != "Player")//角色
            {
                //Debug.Log(hit[i].collider.gameObject.name);
                colliderObject.Add(hit[i].collider.gameObject);
                SetMaterialsColor(hit[i].collider.gameObject.GetComponent<Renderer>(), 0.4f);//置当前物体材质透明度
            }
        }

        //上次与本次对比，本次还存在的物体则赋值为null
        for (int i = 0; i < lastColliderObject.Count; i++) {
            for (int ii = 0; ii < colliderObject.Count; ii++) {
                if (colliderObject[ii] != null) {
                    if (lastColliderObject[i] == colliderObject[ii]) {
                        lastColliderObject[i] = null;
                        break;
                    }
                }
            }
        }

        //当值为null时则可判断当前物体还处于遮挡状态
        //值不为null时则可恢复默认状态(不透明)
        for (int i = 0; i < lastColliderObject.Count; i++) {
            if (lastColliderObject[i] != null)
                SetMaterialsColor(lastColliderObject[i].GetComponent<Renderer>(), 1f);//恢复上次物体材质透明度
        }
    }

    /// 置物体所有材质球颜色 <summary>
    /// 置物体所有材质球颜色
    /// </summary>
    /// <param name="_renderer">材质</param>
    /// <param name="Transpa">透明度</param>
    private void SetMaterialsColor(Renderer _renderer, float Transpa)
    {
        //获取当前物体材质球数量
        int materialsNumber = _renderer.sharedMaterials.Length;
        for (int i = 0; i < materialsNumber; i++) {
            //获取当前材质球颜色
            Color color = _renderer.materials[i].color;

            //设置透明度  取值范围：0~1;  0 = 完全透明
            color.a = Transpa;

            //置当前材质球颜色
            _renderer.materials[i].SetColor("_Color", color);
        }
    }

    private void OnEnable()
    {
        EasyTouch.On_Pinch += OnPinch;
    }

    private void OnDisable()
    {
        EasyTouch.On_Pinch -= OnPinch;
    }

    protected virtual void OnPinch(Gesture gesture)
    {
        
    }
}
