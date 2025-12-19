using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BaseCaseManager : MonoBehaviour
{
    [Header("AR Foundation")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header("Prefabs")]
    public GameObject basePlanePrefab;
    public GameObject gizmoControllerPrefab;

    public GizmoController gizmoController;

    private GameObject spawnedBase;
    private GameObject gizmoInstance;

    // Getter（外部唯讀）
    public GameObject SpawnedBase => spawnedBase;
    public GameObject GizmoInstance => gizmoInstance;

    public ModeCycleUI modeUI;

    private Camera cam;
    private readonly static List<ARRaycastHit> hits = new();

    private void Awake()
    {
        cam = Camera.main;
    }
    public void SwitchToMoveMode()
    {
        gizmoController.ShowMoveGizmo();
    }

    public void SwitchToScaleMode()
    {
        gizmoController.ShowScaleGizmo();
    }

    public void SwitchToRotateMode()
    {
        gizmoController.ShowRotateGizmo();
    }

    public static BaseCaseManager Instance;

    private void Update()
    {
        Instance = this;
        if (ARBuildManager.Instance != null && ARBuildManager.Instance.isBuildMode)
            return; // 已經進入建造模式 → 不再管 Gizmo

        var touch = Touchscreen.current;
        if (touch == null) return;
        var primary = touch.primaryTouch;

        // 手剛按下時判斷點到了什麼
        if (primary.press.wasPressedThisFrame)
        {
            Vector2 pos = primary.position.ReadValue();

            // STEP 1：建立 BasePlane
            if (spawnedBase == null)
            {
                TryPlaceBasePlane(pos);
                return;
            }

            // STEP 2：點 BasePlane → 生成 Gizmo
            if (gizmoInstance == null)
            {
                TryShowGizmo(pos);
                return;
            }

            // STEP 3：之後才會加入 Gizmo 互動
        }
    }

    // ======================================================
    // STEP 1：用 AR Raycast 建 BasePlane（只做一次）
    // ======================================================
    private void TryPlaceBasePlane(Vector2 pos)
    {

        Debug.Log("[DEBUG] TryPlaceBasePlane triggered at screen = " + pos);
        Debug.Log("[DEBUG] Current plane count = " + planeManager.trackables.count);

        if (!raycastManager.Raycast(pos, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("[DEBUG] Raycast did NOT hit a plane.");
            return;
        }

        Debug.Log("[DEBUG] Raycast hit plane: " + hits[0].trackableId);

        var hit = hits[0];
        ARPlane plane = planeManager.GetPlane(hit.trackableId);

        if (plane == null)
        {
            Debug.Log("[DEBUG] plane = null, impossible unless ARPlaneManager broken");
            return;
        }

        Debug.Log("[DEBUG] plane active = " + plane.gameObject.activeSelf);

        if (Vector3.Dot(plane.transform.up, Vector3.up) < 0.95f)
        {
            Debug.Log("[DEBUG] plane is not horizontal");
            return;
        }

        var boundary = plane.boundary;
        if (!boundary.IsCreated || boundary.Length < 3)
        {
            Debug.Log("[BaseCase] Invalid plane boundary");
            return;
        }

        // 取得 ARPlane 的偵測尺寸（原本邏輯）
        Vector2 size = ComputeBoundarySize(boundary);

        // 最大正方形
        float sq = Mathf.Min(size.x, size.y);

        // === ✔ 新增：水平朝向鏡頭 ===
        // 取得相機的前向，但不要有上下傾斜
        Vector3 flatForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Quaternion faceCameraRot = Quaternion.LookRotation(flatForward, Vector3.up);

        // 建立 BasePlane，但使用我們重新決定的 rotation
        spawnedBase = Instantiate(basePlanePrefab, hit.pose.position, faceCameraRot);


        // 設定正方形大小
        Vector3 oriScale = spawnedBase.transform.localScale;
        spawnedBase.transform.localScale = new Vector3(sq, oriScale.y, sq);

        Debug.Log("[BaseCase] Square BasePlane created & facing camera");



        DisableARPlanes();

        var fb = FloorBuildManager.Instance;
        if (fb != null)
        {
            Debug.Log("[BaseCase] Notify FloorBuildManager with spawned base = " + spawnedBase);

            fb.baseCaseManager = this;                     // ★ 新增
            fb.basePlaneTransform = spawnedBase.transform; // ★ 新增
            fb.SetBasePlane(spawnedBase.transform);
        }

    }


    // ======================================================
    // STEP 2：Physics Raycast 點 BasePlane → 顯示 Gizmo
    // ======================================================
    private void TryShowGizmo(Vector2 pos)
    {
        Ray ray = cam.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            if (hit.collider.CompareTag("BasePlane"))
            {
                gizmoInstance = Instantiate(gizmoControllerPrefab);

                gizmoInstance.transform.position = spawnedBase.transform.position;
                gizmoInstance.transform.rotation = spawnedBase.transform.rotation;

                gizmoController = gizmoInstance.GetComponent<GizmoController>();
                gizmoController.ShowMoveGizmo();

                foreach (var h in gizmoInstance.GetComponentsInChildren<AxisHandle>())
                    h.target = spawnedBase.transform;

                var rot = FindObjectOfType<RotateGesture>();
                if (rot != null) rot.gizmo = gizmoInstance.transform;

                // ⬇⬇⬇⬇⬇ 不要再用 FindObjectOfType！⬇⬇⬇⬇⬇
                modeUI.ShowUI();

                return;
            }
        }

        Debug.Log("[BaseCase] Touch did NOT hit BasePlane");
    }


    // ------------------------------------------------------
    // Utility
    // ------------------------------------------------------

    private Vector2 ComputeBoundarySize(NativeArray<Vector2> boundary)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var p in boundary)
        {
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minY = Mathf.Min(minY, p.y);
            maxY = Mathf.Max(maxY, p.y);
        }

        return new Vector2(maxX - minX, maxY - minY);
    }

    private void DisableARPlanes()
    {
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;
        foreach (var p in planeManager.trackables)
            p.gameObject.SetActive(false);
    }

}
