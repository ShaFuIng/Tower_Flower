using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class AxisHandle : MonoBehaviour
{
    public enum AxisType { X, Y, Z }
    public AxisType axis;

    public Transform target;   // BasePlane
    private Camera cam;

    private bool dragging = false;
    private Vector2 lastFingerPos;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var ts = Touchscreen.current;
        if (ts == null) return;

        var touch = ts.primaryTouch;

        // 剛開始按
        if (touch.press.wasPressedThisFrame)
        {
            Vector2 pos = touch.position.ReadValue();
            TryBeginDrag(pos);
        }
        // 放開
        else if (touch.press.wasReleasedThisFrame)
        {
            dragging = false;
        }
        // 持續拖曳中
        else if (dragging)
        {
            Vector2 pos = touch.position.ReadValue();
            Drag(pos);
        }
    }

    // ======================================================
    // 嘗試開始拖曳
    // ======================================================
    private void TryBeginDrag(Vector2 pos)
    {
        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                dragging = true;
                lastFingerPos = pos;

                Debug.Log("[AxisHandle] Drag begin Axis=" + axis);
            }
        }
    }

    // ======================================================
    // 平滑拖曳（螢幕位移 → 世界位移）
    // ======================================================
    private void Drag(Vector2 fingerPos)
    {
        Vector2 fingerDelta = fingerPos - lastFingerPos;
        lastFingerPos = fingerPos;

        if (fingerDelta.sqrMagnitude < 0.0001f)
            return;

        // 軸向世界方向
        Vector3 axisWorld =
            axis == AxisType.X ? target.right :
            axis == AxisType.Y ? target.up :
                                 target.forward;

        // 把指移動量轉向軸方向
        float scale = 0.0015f;   // ← 可調整拖曳速度
        Vector3 move = axisWorld * (fingerDelta.x + fingerDelta.y) * scale;

        target.position += move;
        transform.root.position += move;
    }
}
