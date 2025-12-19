using UnityEngine;
using UnityEngine.InputSystem;

public class TouchHitTester : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.wasPressedThisFrame) return;

        Vector2 pos = touch.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(pos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            Debug.Log(">>> Hit = NOTHING");
            return;
        }

        GameObject obj = hit.collider.gameObject;

        // --------------------------
        // ① BasePlane
        // --------------------------
        if (obj.CompareTag("BasePlane"))
        {
            Debug.Log(">>> Hit = BASEPLANE");
            return;
        }

        // --------------------------
        // ② Gizmo 物件（XYZ 軸）
        // --------------------------
        // 只要屬於 Gizmo Layer 就算
        if (obj.layer == LayerMask.NameToLayer("ARObject"))
        {
            Debug.Log(">>> Hit = GIZMO ( " + obj.name + " )");
            return;
        }

        // --------------------------
        // ③ 其他物件
        // --------------------------
        Debug.Log(">>> Hit = OTHER: " + obj.name);
    }
}
