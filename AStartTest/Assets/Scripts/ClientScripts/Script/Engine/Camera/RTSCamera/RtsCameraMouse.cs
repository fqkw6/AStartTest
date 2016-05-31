using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

// Êó±ê¿ØÖÆÉãÏñ»ú
public class RtsCameraMouse : MonoBehaviour
{
    public bool AllowScreenEdgeMove;
    public bool ScreenEdgeMoveBreaksFollow;
    public int ScreenEdgeBorderWidth;
    public float MoveSpeed;

    public bool AllowPan;
    public bool PanBreaksFollow;
    public float PanSpeed;

    public bool AllowRotate;
    public float RotateSpeed;

    public bool AllowTilt;
    public float TiltSpeed;

    public bool AllowZoom;
    public float ZoomSpeed;

    public bool AllowDrag = true;

    private KeyCode MouseOrbitButton = KeyCode.Mouse1;
    private string RotateInputAxis = "Mouse X";
    private string TiltInputAxis = "Mouse Y";
    private string ZoomInputAxis = "Mouse ScrollWheel";
    private KeyCode PanKey1 = KeyCode.LeftShift;
    private KeyCode PanKey2 = KeyCode.RightShift;

    //

    private RtsCamera _rtsCamera;
    private Vector3 _lastPosition;

    //

    protected void Reset()
    {
        AllowScreenEdgeMove = true;
        ScreenEdgeMoveBreaksFollow = true;
        ScreenEdgeBorderWidth = 4;
        MoveSpeed = 30f;

        AllowPan = true;
        PanBreaksFollow = true;
        PanSpeed = 50f;
        PanKey1 = KeyCode.LeftShift;
        PanKey2 = KeyCode.RightShift;

        AllowRotate = true;
        RotateSpeed = 360f;

        AllowTilt = true;
        TiltSpeed = 200f;

        AllowZoom = true;
        ZoomSpeed = 500f;

        RotateInputAxis = "Mouse X";
        TiltInputAxis = "Mouse Y";
        ZoomInputAxis = "Mouse ScrollWheel";
    }

    protected void Start()
    {
        _rtsCamera = gameObject.GetComponent<RtsCamera>();
    }

    public bool IsTouchUI()
    {
        GameObject go = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        if (go != null) {
            return true;
        }

        return false;
    }

    protected void Update()
    {
        if (_rtsCamera == null)
            return; // no camera, bail!

        if (IsTouchUI()) {
            return;
        }

        if (AllowZoom)
        {
            var scroll = Input.GetAxisRaw(ZoomInputAxis);
            _rtsCamera.Distance -= scroll * ZoomSpeed * Time.deltaTime;
        }

        if (Input.GetKey(MouseOrbitButton))
        {
            if (AllowPan && (Input.GetKey(PanKey1) || Input.GetKey(PanKey2)))
            {
                // pan
                var panX = -1 * Input.GetAxisRaw("Mouse X") * PanSpeed * Time.deltaTime;
                var panZ = -1 * Input.GetAxisRaw("Mouse Y") * PanSpeed * Time.deltaTime;

                _rtsCamera.AddToPosition(panX, 0, panZ);

                if (PanBreaksFollow && (Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f))
                {
                    _rtsCamera.EndFollow();
                }
            }
            else
            {
                // orbit

                if (AllowTilt)
                {
                    var tilt = Input.GetAxisRaw(TiltInputAxis);
                    _rtsCamera.Tilt -= tilt * TiltSpeed * Time.deltaTime;
                }

                if (AllowRotate)
                {
                    var rot = Input.GetAxisRaw(RotateInputAxis);
                    _rtsCamera.Rotation += rot * RotateSpeed * Time.deltaTime;
                }
            }
        }

        if (AllowScreenEdgeMove && (!_rtsCamera.IsFollowing || ScreenEdgeMoveBreaksFollow))
        {
            var hasMovement = false;

            if (Input.mousePosition.y > (Screen.height - ScreenEdgeBorderWidth))
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(0, 0, MoveSpeed * Time.deltaTime);
            }
            else if (Input.mousePosition.y < ScreenEdgeBorderWidth)
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(0, 0, -1 * MoveSpeed * Time.deltaTime);
            }

            if (Input.mousePosition.x > (Screen.width - ScreenEdgeBorderWidth))
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(MoveSpeed * Time.deltaTime, 0, 0);
            }
            else if (Input.mousePosition.x < ScreenEdgeBorderWidth)
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(-1 * MoveSpeed * Time.deltaTime, 0, 0);
            }

            if (hasMovement && _rtsCamera.IsFollowing && ScreenEdgeMoveBreaksFollow)
            {
                _rtsCamera.EndFollow();
            }
        }

        // ÍÏ×§
        if (AllowDrag) {
            if (Input.GetMouseButtonDown(0)) {
                _lastPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0)) {
                Vector3 delta = Input.mousePosition - _lastPosition;
                _rtsCamera.AddToPosition(-delta.x * MoveSpeed * Time.deltaTime, 0, -delta.y * MoveSpeed * Time.deltaTime);
                _lastPosition = Input.mousePosition;
            }
        }
    }
}
