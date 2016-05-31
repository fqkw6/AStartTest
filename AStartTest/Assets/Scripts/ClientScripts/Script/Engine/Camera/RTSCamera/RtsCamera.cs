using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

// 摄像机控制
public class RtsCamera : MonoBehaviour
{
    #region NOTES
    /*
     * ----------------------------------------------
     * Options for height calculation:
     * 
     * 1) Set GetTerrainHeight function (from another script) to provide x/z height lookup:
     * 
     *     _rtsCamera.GetTerrainHeight = MyGetTerrainHeightFunction;
     *     ...
     *     private float MyGetTerrainHeightFunction(float x, float z)
     *     {
     *         return ...;
     *     }
     * 
     * 2) Set "TerrainHeightViaPhysics = true;"
     * 
     * 3) For a "simple plain" style terrain (flat), set "LookAtHeightOffset" to base terrain height
     * 
     * See demo code for examples.
     * 
     * ----------------------------------------------
     * To "Auto Follow" a target:
     * 
     *     _rtsCamera.Follow(myGameObjectOrTransform)
     * 
     * See demo code for examples.
     * 
     * ----------------------------------------------
     * To be notified when Auto-Follow target changes (or is cleared):
     * 
     *     _rtsCamera.OnBeginFollow = MyBeginFollowCallback;
     *     _rtsCamera.OnEndFollow = MyEndFollowCallback;
     * 
     *     void OnBeginFollow(Transform followTransform) { ... }
     *     void OnEndFollow(Transform followTransform) { ... }
     * 
     * See demo code for examples.
     * 
     * ----------------------------------------------
     * To force the camera to follow "behind" (with optional degree offset):
     * 
     *      _rtsCamera.FollowBehind = true;
     *      _rtsCamera.FollowRotationOffset = 90;   // optional
     *      
     * See demo code for examples.
     * 
     * ----------------------------------------------
     * For target visibility checking via Physics:
     * 
     *      _rtsCamera.TargetVisbilityViaPhysics = true;
     *      _rtsCamera.TargetVisibilityIgnoreLayerMask = {your layer masks that should block camera};
     * 
     * See demo code for examples.
     * 
     */
    #endregion
    public Vector3 LookAt; // Desired lookat position
    public float _distance; // Desired distance (units, ie Meters)
    public float Rotation; // Desired rotation (degrees)
    public float Tilt; // Desired tilt (degrees)

    public bool Smoothing; // Should the camera "slide" between positions and targets?
    public float MoveDampening; // How "smooth" should the camera moves be?  Note: Smaller numbers are smoother
    public float ZoomDampening; // How "smooth" should the camera zooms be?  Note: Smaller numbers are smoother
    public float RotationDampening; // How "smooth" should the camera rotations be?  Note: Smaller numbers are smoother
    public float TiltDampening; // How "smooth" should the camera tilts be?  Note: Smaller numbers are smoother

    public Vector3 MinBoundsLow; // Minimum x,y,z world position for camera target
    public Vector3 MaxBoundsLow; // Maximum x,y,z world position for camera target
    public Vector3 MinBoundsHigh;
    public Vector3 MaxBoundsHigh;

    public float MinDistance; // Minimum distance of camera from target
    public float MaxDistance; // Maximum distance of camera from target

    public float MinTilt; // Minimum tilt (degrees)
    public float MaxTilt; // Maximum tilt (degrees)

    public Func<float, float, float> GetTerrainHeight; // Function taking x,z position and returning height (y).

    public bool TerrainHeightViaPhysics; // If set, camera will automatically raycast against terrain (using TerrainPhysicsLayerMask) to determine height 
    public LayerMask TerrainPhysicsLayerMask; // Layer mask indicating which layers the camera should ray cast against for height detection

    public float LookAtHeightOffset; // Y coordinate of camera target.   Only used if TerrainHeightViaPhysics and GetTerrainHeight are not set.

    public bool TargetVisbilityViaPhysics; // If set, camera will raycast from target out in order to avoid objects being between target and camera
    public float CameraRadius = 1f;
    public LayerMask TargetVisibilityIgnoreLayerMask; // Layer mask to ignore when raycasting to determine camera visbility

    public bool FollowBehind; // If set, keyboard and mouse rotation will be disabled when Following a target
    public float FollowRotationOffset; // Offset (degrees from zero) when forcing follow behind target

    public Action<Transform> OnBeginFollow; // "Callback" for when automatic target following begins
    public Action<Transform> OnEndFollow; // "Callback" for when automatic target following ends

    public bool ShowDebugCameraTarget; // If set, "small green sphere" will be shown indicating camera target position (even when Following)
    //
    // PRIVATE VARIABLES
    //

    private Vector3 _initialLookAt;
    private float _initialDistance;
    private float _initialRotation;
    private float _initialTilt;

    private float _currDistance; // actual distance
    private float _currRotation; // actual rotation
    private float _currTilt; // actual tilt

    private Vector3 _moveVector;

    private GameObject _target;
    private MeshRenderer _targetRenderer;

    private Transform _followTarget;
    private Vector3 _lastLookAtInBounds = Vector3.zero;


    public float Distance
    {
        get { return _distance; }
        set
        {
            _distance = value;
            CalcBoundingBox();
        }
    }

    protected void Reset()
    {
        Smoothing = true;

        LookAtHeightOffset = 0f;
        TerrainHeightViaPhysics = false;
        TerrainPhysicsLayerMask = ~0; // "Everything" by default!
        GetTerrainHeight = null;

        TargetVisbilityViaPhysics = false;
        CameraRadius = 1f;
        TargetVisibilityIgnoreLayerMask = 0; // "Nothing" by default!

        LookAt = new Vector3(0, 0, 0);
        MoveDampening = 5f;

        MinBoundsLow = new Vector3(-100, -100, -100);
        MaxBoundsLow = new Vector3(100, 100, 100);
        MinBoundsHigh = new Vector3(-100, -100, -100);
        MaxBoundsHigh = new Vector3(100, 100, 100);

        Distance = 16f;
        MinDistance = 8f;
        MaxDistance = 32f;
        ZoomDampening = 5f;

        Rotation = 0f;
        RotationDampening = 5f;

        Tilt = 45f;
        MinTilt = 30f;
        MaxTilt = 85f;
        TiltDampening = 5f;

        FollowBehind = false;
        FollowRotationOffset = 0;
    }

    protected void Start()
    {
        if (GetComponent<Rigidbody>()) {
            // don't allow camera to rotate
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        //
        // store initial values so that we can reset them using ResetToInitialValues method
        //
        _initialLookAt = LookAt;
        _initialDistance = Distance;
        _initialRotation = Rotation;
        _initialTilt = Tilt;
        _lastLookAtInBounds = LookAt;

        //
        // set our current values to the desired values so that we don't "slide in"
        //
        _currDistance = Distance;
        _currRotation = Rotation;
        _currTilt = Tilt;

        //
        // create a target sphere, hidden by default
        //
        CreateTarget();

        CalcBoundingBox();
    }

    public void ResetPosition()
    {

    }

    public struct Param
    {
        public float a;
        public float b;
        public float c;

        public Param(float pa, float pb, float pc)
        {
            a = pa;
            b = pb;
            c = pc;
        }
    }
    public static Param CalParam(Vector3 p1, Vector3 p2)
    {
        float a, b, c;
        float x1 = p1.x, y1 = p1.z, x2 = p2.x, y2 = p2.z;
        a = y2 - y1;
        b = x1 - x2;
        c = (x2 - x1) * y1 - (y2 - y1) * x1;
        if (b < 0) {
            a *= -1; b *= -1; c *= -1;
        } else if (Mathf.Abs(b) <= 0.01f && a < 0) {
            a *= -1; c *= -1;
        }
        return new Param(a, b, c);
    }

    public static Vector3 getIntersectPoint(float a1, float b1, float c1, float a2, float b2, float c2)
    {
        Vector3 p = Vector3.zero;
        float m = a1 * b2 - a2 * b1;
        if (Mathf.Abs(m) <= 0.01f) {
            return Vector3.zero;
        }
        float x = (c2 * b1 - c1 * b2) / m;
        float y = (c1 * a2 - c2 * a1) / m;
        p = new Vector3(x, 0, y);
        return p;
    }

    public Vector3 GetIntersectPoint(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 pt4)
    {
        Param param1 = CalParam(pt1, pt2);
        Param param2 = CalParam(pt3, pt4);
        Vector3 pt = getIntersectPoint(param1.a, param1.b, param1.c, param2.a, param2.b, param2.c);
        
        return pt;
    }

    // 点是否在线段上
    public bool IsPointInSegment(Vector3 pt, Vector3 pt1, Vector3 pt2)
    {
        // 判断是否在一条直线上
        if (Mathf.Abs((pt1.x - pt.x)*(pt2.z - pt.z) - (pt2.x - pt.x)*(pt1.z - pt.z)) >= 0.01f) {
            return false;
        }

        // 是否在线段上
        if (pt.x > pt1.x && pt.x > pt2.x || pt.x < pt1.x && pt.x < pt2.x) {
            return false;
        }

        if (pt.z > pt1.z && pt.z >= pt2.z || pt.z < pt1.z && pt.z < pt2.z) {
            return false;
        }

        return true;

    }
    private Vector3[] _intersectoinPoints = new Vector3[4];
    private Vector3 _nearPoint;
    private Vector3 CalcNearPoint()
    {
        Vector3 pt0 = LookAt;
        pt0.y = 0;

        // 恰好在边界点上面
        for (int i = 0; i < _newBounds.Length; ++i) {
            if (Vector3.Distance(pt0, _newBounds[i]) <= 0.1f) {
                return _newBounds[i];
            }
        }
        
        Vector3 pt1 = GetIntersectPoint(pt0, Vector3.zero, _newBounds[0], _newBounds[1]);
        Vector3 pt2 = GetIntersectPoint(pt0, Vector3.zero, _newBounds[1], _newBounds[2]);
        Vector3 pt3 = GetIntersectPoint(pt0, Vector3.zero, _newBounds[2], _newBounds[3]);
        Vector3 pt4 = GetIntersectPoint(pt0, Vector3.zero, _newBounds[3], _newBounds[0]);
//        Log.Info("CalcNearPoint:  {0}  {1}  {2}  {3}", pt1, pt2, pt3, pt4);

        _intersectoinPoints[0] = IsPointInSegment(pt1, _newBounds[0], _newBounds[1]) ? pt1 : Vector3.zero;
        _intersectoinPoints[1] = IsPointInSegment(pt2, _newBounds[1], _newBounds[2]) ? pt2 : Vector3.zero;
        _intersectoinPoints[2] = IsPointInSegment(pt3, _newBounds[2], _newBounds[3]) ? pt3 : Vector3.zero;
        _intersectoinPoints[3] = IsPointInSegment(pt4, _newBounds[3], _newBounds[0]) ? pt4 : Vector3.zero;

        _nearPoint = pt0;
        float distance = 1000000;
        foreach (var item in _intersectoinPoints) {
            if (item.sqrMagnitude > 0.01f) {
                float d = Vector3.Distance(pt0, item);
                if (d < distance) {
                    _nearPoint = item;
                    distance = d;
                }
            }
        }
//        Log.Info("CalcNearPoint:  {0}    {1}", LookAt, _nearPoint);
        return _nearPoint;
    }

    protected void LateUpdate()
    {
        //
        // update desired target position
        //
        if (IsFollowing) {
            LookAt = _followTarget.position;
        } else {
            _moveVector.y = 0;
            LookAt += Quaternion.Euler(0, Rotation, 0)*_moveVector;
            LookAt.y = GetHeightAt(LookAt.x, LookAt.z);
        }
        LookAt.y += LookAtHeightOffset;

        CalcNearPoint();
        //
        // clamp values
        //
        Tilt = Mathf.Clamp(Tilt, MinTilt, MaxTilt);
        Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        
        //
        // move from "desired" to "target" values
        //
        bool isOutofBounds = !CheckBoundingBox(LookAt);
        if (!isOutofBounds) {
            _lastLookAtInBounds = LookAt;
            if (Smoothing) {
                _currRotation = Mathf.LerpAngle(_currRotation, Rotation, Time.deltaTime*RotationDampening);
                _currDistance = Mathf.Lerp(_currDistance, Distance, Time.deltaTime*ZoomDampening);
                _currTilt = Mathf.LerpAngle(_currTilt, Tilt, Time.deltaTime*TiltDampening);
                Vector3 dest = Vector3.Lerp(_target.transform.position, LookAt, Time.deltaTime*MoveDampening);
                _target.transform.position = dest;
            } else {
                _currRotation = Rotation;
                _currDistance = Distance;
                _currTilt = Tilt;
                _target.transform.position = LookAt;
            }
        } else {
            _currRotation = Rotation;
            _currDistance = Distance;
            _currTilt = Tilt;
            LookAt = CalcNearPoint();
            _target.transform.position = LookAt;
        }
        
        _moveVector = Vector3.zero;

        //
        // if we're following AND forcing behind, override the rotation to point to target (with offset)
        //
        if (IsFollowing && FollowBehind) {
            ForceFollowBehind();
        }

        //
        // optionally, we'll check to make sure the target is visible
        // Note: we only do this when following so that we don't "jar" when moving manually
        //
        if (IsFollowing && TargetVisbilityViaPhysics && DistanceToTargetIsLessThan(1f)) {
            EnsureTargetIsVisible();
        }

        //
        // recalculate the actual position of the camera based on the above
        //
        var rotation = Quaternion.Euler(_currTilt, _currRotation, 0);
        var v = new Vector3(0.0f, 0.0f, -_currDistance);
        var position = rotation*v + _target.transform.position;

        if (GetComponent<Camera>().orthographic) {
            GetComponent<Camera>().orthographicSize = _currDistance;
        }

        // update position and rotation of camera
        transform.rotation = rotation;
        transform.position = position;
    }

    private bool PtInPolygon(Vector3 p, Vector3[] ptPolygon, int nCount)
    {
        int nCross = 0;
        for (int i = 0; i < nCount; i++) {
            Vector3 p1 = ptPolygon[i];
            Vector3 p2 = ptPolygon[(i + 1)%nCount];
            // 求解 y=p.y 与 p1p2 的交点
            if (Math.Abs(p1.z - p2.z) < 0.01f) // p1p2 与 y=p0.y平行 
                continue;
            if (p.z < Mathf.Min(p1.z, p2.z)) // 交点在p1p2延长线上 
                continue;
            if (p.z >= Mathf.Max(p1.z, p2.z)) // 交点在p1p2延长线上 
                continue;
            // 求交点的 X 坐标 -------------------------------------------------------------- 
            float x = (p.z - p1.z)*(p2.x - p1.x)/(p2.z - p1.z) + p1.x;
            if (x > p.x)
                nCross++; // 只统计单边交点 
        }
        // 单边交点为偶数，点在多边形之外 --- 
        return (nCross%2 == 1);
    }

    private bool CheckBoundingBox(Vector3 pos)
    {
        return PtInPolygon(new Vector3(pos.x, 0, pos.z), _newBounds, 4);
    }

    private void CalcBoundingBox()
    {
        // 边界大小
        float percent = (_currDistance - MinDistance) / (MaxDistance - MinDistance);
        float minX = MinBoundsLow.x + (MinBoundsHigh.x - MinBoundsLow.x) * percent;
        float minZ = MinBoundsLow.z + (MinBoundsHigh.z - MinBoundsLow.z) * percent;
        float maxX = MaxBoundsLow.x + (MaxBoundsHigh.x - MaxBoundsLow.x) * percent;
        float maxZ = MaxBoundsLow.z + (MaxBoundsHigh.z - MaxBoundsLow.z) * percent;

        Vector3 ptLD = new Vector3(minX, 0, minZ);
        Vector3 ptRD = new Vector3(maxX, 0, minZ);
        Vector3 ptRU = new Vector3(maxX, 0, maxZ);
        Vector3 ptLU = new Vector3(minX, 0, maxZ);

        // 旋转角度
        float degree = Rotation/180*Mathf.PI;
        float cos = Mathf.Cos(degree);
        float sin = Mathf.Sin(degree);
        Vector3 ptLD2 = new Vector3(ptLD.x*cos + ptLD.z*sin, 0, ptLD.z*cos - ptLD.x*sin);
        Vector3 ptRD2 = new Vector3(ptRD.x*cos + ptRD.z*sin, 0, ptRD.z*cos - ptRD.x*sin);
        Vector3 ptRU2 = new Vector3(ptRU.x*cos + ptRU.z*sin, 0, ptRU.z*cos - ptRU.x*sin);
        Vector3 ptLU2 = new Vector3(ptLU.x*cos + ptLU.z*sin, 0, ptLU.z*cos - ptLU.x*sin);

        _newBounds[0] = ptLD2;
        _newBounds[1] = ptRD2;
        _newBounds[2] = ptRU2;
        _newBounds[3] = ptLU2;
        _newBounds[4] = ptLD;
        _newBounds[5] = ptRD;
        _newBounds[6] = ptRU;
        _newBounds[7] = ptLU;
    }
    
    private Vector3[] _newBounds = new Vector3[8];

    private void OnDrawGizmos()
    {
        if (!ShowDebugCameraTarget) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_newBounds[0], _newBounds[1]);
        Gizmos.DrawLine(_newBounds[1], _newBounds[2]);
        Gizmos.DrawLine(_newBounds[2], _newBounds[3]);
        Gizmos.DrawLine(_newBounds[3], _newBounds[0]);

        Gizmos.DrawSphere(_newBounds[0], 1);
        Gizmos.DrawSphere(_newBounds[1], 1);
        Gizmos.DrawSphere(_newBounds[2], 1);
        Gizmos.DrawSphere(_newBounds[3], 1);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_newBounds[4], _newBounds[5]);
        Gizmos.DrawLine(_newBounds[5], _newBounds[6]);
        Gizmos.DrawLine(_newBounds[6], _newBounds[7]);
        Gizmos.DrawLine(_newBounds[7], _newBounds[4]);

        Gizmos.color = Color.green;
        if (_intersectoinPoints[0].sqrMagnitude >= 0.01f) {
            Gizmos.DrawSphere(_intersectoinPoints[0], 3);
        }


        if (_intersectoinPoints[1].sqrMagnitude >= 0.01f) {
            Gizmos.DrawSphere(_intersectoinPoints[1], 3);
        }


        if (_intersectoinPoints[2].sqrMagnitude >= 0.01f) {
            Gizmos.DrawSphere(_intersectoinPoints[2], 3);
        }


        if (_intersectoinPoints[3].sqrMagnitude >= 0.01f) {
            Gizmos.DrawSphere(_intersectoinPoints[3], 3);
        }

        if (ShowDebugCameraTarget) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(LookAt, 3);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_nearPoint, 3);
    }

    //
    // PUBLIC METHODS
    //
    #region PUBLIC_METHODS
    /// <summary>
    /// Current transform of camera target (NOTE: should not be set directly)
    /// </summary>
    public Transform CameraTarget
    {
        get { return _target.transform; }
    }

    /// <summary>
    /// True if the current camera auto-follow target is set.  Else, false.
    /// </summary>
    public bool IsFollowing
    {
        get { return FollowTarget != null; }
    }

    /// <summary>
    /// Current auto-follow target
    /// </summary>
    public Transform FollowTarget
    {
        get { return _followTarget; }
    }

    /// <summary>
    /// Reset camera to initial (startup) position, distance, rotation, tilt, etc.
    /// </summary>
    /// <param name="includePosition">If true, position will be reset as well.  If false, only distance/rotation/tilt.</param>
    /// <param name="snap">If true, camera will snap instantly to the position.  If false, camera will slide smoothly back to initial values.</param>
    public void ResetToInitialValues(bool includePosition, bool snap = false)
    {
        if (includePosition)
            LookAt = _initialLookAt;

        Distance = _initialDistance;
        Rotation = _initialRotation;
        Tilt = _initialTilt;

        if (snap) {
            _currDistance = Distance;
            _currRotation = Rotation;
            _currTilt = Tilt;
            _target.transform.position = LookAt;
        }
    }

    /// <summary>
    /// Manually set target position (snap or slide).
    /// </summary>
    /// <param name="toPosition">Vector3 position</param>
    /// <param name="snap">If true, camera will "snap" to the position, else will "slide"</param>
    public void JumpTo(Vector3 toPosition, bool snap = false)
    {
        EndFollow();

        LookAt = toPosition;

        if (snap) {
            _target.transform.position = toPosition;
        }
    }

    /// <summary>
    /// Manually set target position (snap or slide).
    /// </summary>
    /// <param name="toTransform">Transform to which the camera target will be moved</param>
    /// <param name="snap">If true, camera will "snap" to the position, else will "slide"</param>
    public void JumpTo(Transform toTransform, bool snap = false)
    {
        JumpTo(toTransform.position, snap);
    }

    /// <summary>
    /// Manually set target position (snap or slide).
    /// </summary>
    /// <param name="toGameObject">GameObject to which the camera target will be moved</param>
    /// <param name="snap">If true, camera will "snap" to the position, else will "slide"</param>
    public void JumpTo(GameObject toGameObject, bool snap = false)
    {
        JumpTo(toGameObject.transform.position, snap);
    }

    /// <summary>
    /// Set current auto-follow target (snap or slide).
    /// </summary>
    /// <param name="followTarget">Transform which the camera should follow</param>
    /// <param name="snap">If true, camera will "snap" to the position, else will "slide"</param>
    public void Follow(Transform followTarget, bool snap = false)
    {
        if (_followTarget != null) {
            if (OnEndFollow != null) {
                OnEndFollow(_followTarget);
            }
        }

        _followTarget = followTarget;

        if (_followTarget != null) {
            if (snap) {
                LookAt = _followTarget.position;
            }

            if (OnBeginFollow != null) {
                OnBeginFollow(_followTarget);
            }
        }
    }

    /// <summary>
    /// Set current auto-follow target (snap or slide).
    /// </summary>
    /// <param name="followTarget">GameObject which the camera should follow</param>
    /// <param name="snap">If true, camera will "snap" to the position, else will "slide"</param>
    public void Follow(GameObject followTarget, bool snap = false)
    {
        Follow(followTarget.transform);
    }

    /// <summary>
    /// Break auto-follow.   Camera will now be manually controlled by player input.
    /// </summary>
    public void EndFollow()
    {
        Follow((Transform) null, false);
    }

    /// <summary>
    /// Adds movement to the camera (world coordinates).
    /// </summary>
    /// <param name="dx">World coordinate X distance to move</param>
    /// <param name="dy">World coordinate Y distance to move</param>
    /// <param name="dz">World coordinate Z distance to move</param>
    public void AddToPosition(float dx, float dy, float dz)
    {
        _moveVector += new Vector3(dx, dy, dz);
    }
    #endregion

    //
    // PRIVATE METHODS
    //
    #region PRIVATE_METHODS
    /// <summary>
    /// If "GetTerrainHeight" function set, will call to obtain desired camera height (y position).
    /// Else, if TerrainHeightViaPhysics is true, will use Physics.RayCast to determine camera height.
    /// Else, will assume flat terrain and will return "0" (which will later be offset by LookAtHeightOffset)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private float GetHeightAt(float x, float z)
    {
        //
        // priority 1:  use supplied function to get height at point
        //
        if (GetTerrainHeight != null) {
            return GetTerrainHeight(x, z);
        }

        //
        // priority 2:  use physics ray casting to get height at point
        //
        if (TerrainHeightViaPhysics) {
            var y = MaxBoundsHigh.y;
            var maxDist = MaxBoundsHigh.y - MinBoundsHigh.y + 1f;

            RaycastHit hitInfo;
            if (Physics.Raycast(new Vector3(x, y, z), new Vector3(0, -1, 0), out hitInfo, maxDist, TerrainPhysicsLayerMask)) {
                return hitInfo.point.y;
            }
            return 0; // no hit!
        }

        //
        // assume flat terrain
        //
        return 0;
    }

    /// <summary>
    /// Creates the camera's target, initially not visible.
    /// </summary>
    private void CreateTarget()
    {
        _target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _target.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _target.transform.SetParent(transform.parent);

        _target.GetComponent<Renderer>().material.color = Color.green;

        var targetCollider = _target.GetComponent<Collider>();
        if (targetCollider != null) {
            targetCollider.enabled = false;
        }

        _targetRenderer = _target.GetComponent<MeshRenderer>();
        _targetRenderer.enabled = ShowDebugCameraTarget;

        _target.name = "CameraTarget";
        _target.transform.position = LookAt;
    }

    private bool DistanceToTargetIsLessThan(float sqrDistance)
    {
        if (!IsFollowing)
            return true; // our distance is technically zero

        var p1 = _target.transform.position;
        var p2 = _followTarget.position;
        p1.y = p2.y = 0; // ignore height offset
        var v = p1 - p2;
        var vd = v.sqrMagnitude; // use sqr for performance

        return vd < sqrDistance;
    }

    private void EnsureTargetIsVisible()
    {
        var direction = (transform.position - _target.transform.position);
        direction.Normalize();

        var distance = Distance;

        RaycastHit hitInfo;

        //if (Physics.Raycast(_target.transform.position, direction, out hitInfo, distance, ~TargetVisibilityIgnoreLayerMask))
        if (Physics.SphereCast(_target.transform.position, CameraRadius, direction, out hitInfo, distance, ~TargetVisibilityIgnoreLayerMask)) {
            if (hitInfo.transform != _target) // don't collide with outself!
            {
                _currDistance = hitInfo.distance - 0.1f;
            }
        }
    }

    private void ForceFollowBehind()
    {
        var v = _followTarget.transform.forward*-1;
        var angle = Vector3.Angle(Vector3.forward, v);
        var sign = (Vector3.Dot(v, Vector3.right) > 0.0f) ? 1.0f : -1.0f;
        _currRotation = Rotation = 180f + (sign*angle) + FollowRotationOffset;
    }
    #endregion
}