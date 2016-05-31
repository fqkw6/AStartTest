using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * 
 *          4
 * 
 *   5      0       3
 * 
 *      1        2
 *      
 * 
*/

public class UIPropWidget : MaskableGraphic
{
    private enum AnimationStatus
    {
        NOT_START,
        ANIMATING,
        FINISH,
    }
    public List<Vector2> _maxPropVector;
    public bool _withAnimation = true;
    public const int MAX_PROP_COUNT = 5;
    private const float ANIMATION_TIME = 0.8f;
    private const float MAX_PROP_VALUE = 100.0f;

    private Vector2[] _propList = new Vector2[MAX_PROP_COUNT];
    private Vector2[] _currentList = new Vector2[MAX_PROP_COUNT];

    private bool _isStartAnimation = false;
    private bool _isAnimationFinish = false;
    private bool _isSetValue = false;

    private float _startTime = 0;

    protected override void Awake()
    {
        base.Awake();

        _isStartAnimation = false;
        _isAnimationFinish = false;
        _isSetValue = false;
    }

    // 设置五个属性值
    public void SetPropList(List<float> list, bool withAnimation = false)
    {
        if (list.Count < MAX_PROP_COUNT) {
            Log.Error("必须提供5个属性");
            return;
        }
        
        var r = GetPixelAdjustedRect();
        var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
        // v.x,v.y为左下角的点   v.z,v.w为右上角的点
        _propList[0] = new Vector3(v.x * _maxPropVector[0].x, v.y * _maxPropVector[0].y) * list[0] / MAX_PROP_VALUE; // 1
        _propList[1] = new Vector3(v.z * _maxPropVector[1].x, v.y * _maxPropVector[1].y) * list[1] / MAX_PROP_VALUE; // 2
        _propList[2] = new Vector3(v.z * _maxPropVector[2].x, v.w * _maxPropVector[2].y) * list[2] / MAX_PROP_VALUE; // 3
        _propList[3] = new Vector3(v.x * _maxPropVector[3].x, v.w * _maxPropVector[3].y) * list[3] / MAX_PROP_VALUE; // 4
        _propList[4] = new Vector3(v.x * _maxPropVector[4].x, v.w * _maxPropVector[4].y) * list[4] / MAX_PROP_VALUE; // 5

        _isSetValue = true;

        if (withAnimation) {
            PlayAnimation();
        } else {
            for (int i = 0; i < MAX_PROP_COUNT; ++i) {
                _currentList[i] = _propList[i];
            }
        }

        SetVerticesDirty();
    }

    // 开始播放动画
    public void PlayAnimation()
    {
        _isAnimationFinish = false;
        _isStartAnimation = true;
        _startTime = Time.time;
    }

    void Update()
    {
        if (_isAnimationFinish || !_isSetValue || !_isStartAnimation) {
            return;
        }

        // 动画播放完毕
        if (Time.time - _startTime >= ANIMATION_TIME) {
            for (int i = 0; i < MAX_PROP_COUNT; ++i) {
                _currentList[i] = _propList[i];
            }

            _isAnimationFinish = true;
            return;
        }

        // 更新当前动画的数据
        float percent = (Time.time - _startTime) / ANIMATION_TIME;
        for (int i = 0; i < MAX_PROP_COUNT; ++i) {
            _currentList[i] = _propList[i] * percent;
        }

        SetVerticesDirty();
    }

    protected override  void OnPopulateMesh(VertexHelper vh)
    {
        // 尚未赋值，不用绘制
        if (!_isSetValue) {
            return;
        }

        Color32 color32 = color;
        vh.Clear();
        vh.AddVert(new Vector3(0, 0), color32, new Vector2(0f, 0f)); // 0
        vh.AddVert(_currentList[0], color32, new Vector2(0f, 0f)); // 1
        vh.AddVert(_currentList[1], color32, new Vector2(0f, 1f)); // 2
        vh.AddVert(_currentList[2], color32, new Vector2(1f, 1f)); // 3
        vh.AddVert(_currentList[3], color32, new Vector2(1f, 0f)); // 4
        vh.AddVert(_currentList[4], color32, new Vector2(1f, 0f)); // 5

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
        vh.AddTriangle(0, 3, 4);
        vh.AddTriangle(0, 4, 5);
        vh.AddTriangle(0, 5, 1);
    }
}
