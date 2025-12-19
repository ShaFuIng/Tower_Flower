using UnityEngine;
using UnityEngine.InputSystem;

public class RotateGesture : MonoBehaviour
{
    public Transform target;
    public Transform gizmo;        // ⭐ 新增：讓 Gizmo 同步旋轉

    private bool rotateActive = false;

    private Vector2 lastP0;
    private Vector2 lastP1;
    private bool rotating = false;

    // ⭐ 幅度 × 1.5 → 0.3 * 1.5 ≈ 0.45
    private const float ROTATE_SPEED = 0.45f;

    private void Update()
    {
        if (!rotateActive || target == null) return;

        var ts = Touchscreen.current;
        if (ts == null) return;
        if (ts.touches.Count < 2)
        {
            rotating = false;
            return;
        }

        var t0 = ts.touches[0];
        var t1 = ts.touches[1];

        if (t0.press.wasPressedThisFrame || t1.press.wasPressedThisFrame)
        {
            lastP0 = t0.position.ReadValue();
            lastP1 = t1.position.ReadValue();
            rotating = true;
            return;
        }

        if (rotating)
        {
            Vector2 curP0 = t0.position.ReadValue();
            Vector2 curP1 = t1.position.ReadValue();

            Vector2 lastDir = (lastP1 - lastP0).normalized;
            Vector2 curDir = (curP1 - curP0).normalized;

            // ⭐ 修正方向 → 取負號
            float angle = -Vector2.SignedAngle(lastDir, curDir);

            // ⭐ 放大旋轉幅度
            float finalAngle = angle * ROTATE_SPEED;

            // ⭐ 只旋轉 Y 軸（水平旋轉）
            target.Rotate(Vector3.up, finalAngle, Space.World);

            // ⭐ Gizmo 也要跟著旋轉
            if (gizmo != null)
                gizmo.rotation = target.rotation;

            // 更新手勢位置
            lastP0 = curP0;
            lastP1 = curP1;
        }
    }

    public void StartRotate()
    {
        rotateActive = true;
        rotating = false;
    }

    public void StopRotate()
    {
        rotateActive = false;
        rotating = false;
    }
}
