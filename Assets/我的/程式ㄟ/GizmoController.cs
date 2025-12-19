using UnityEngine;

public class GizmoController : MonoBehaviour
{
    [Header("Gizmo Models")]
    public GameObject moveGizmo;    // XYZ 軸
    public GameObject scaleGizmo;   // 縮放專用的 handle（之後會加）
    public GameObject rotateGizmo;  // 旋轉圓環（之後會加）

    // ===== 顯示 Move Gizmo =====
    public void ShowMoveGizmo()
    {
        moveGizmo.SetActive(true);
        if (scaleGizmo != null) scaleGizmo.SetActive(false);
        if (rotateGizmo != null) rotateGizmo.SetActive(false);
    }

    // ===== 顯示 Scale Gizmo =====
    public void ShowScaleGizmo()
    {
        moveGizmo.SetActive(false);
        if (scaleGizmo != null) scaleGizmo.SetActive(true);
        if (rotateGizmo != null) rotateGizmo.SetActive(false);
    }

    // ===== 顯示 Rotate Gizmo =====
    public void ShowRotateGizmo()
    {
        moveGizmo.SetActive(false);
        if (scaleGizmo != null) scaleGizmo.SetActive(false);
        if (rotateGizmo != null) rotateGizmo.SetActive(true);
    }
}
