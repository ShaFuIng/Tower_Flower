using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnGoalPlacementManager : MonoBehaviour
{
    public static SpawnGoalPlacementManager Instance;

    [Header("Preview")]
    public Material previewMaterial;

    [Header("UI")]
    public ScrollRect scrollView;
    public CanvasGroup scrollGroup;
    public HorizontalLayoutGroup contentLayout;
    public ContentSizeFitter contentFitter;

    [Header("Delete UI")]
    public GameObject deleteButton;

    [Header("Raycast Mask")]
    public LayerMask placementMask;

    [Header("Preview Materials")]
    [SerializeField] private Material previewValidMaterial;
    [SerializeField] private Material previewInvalidMaterial;

    [Header("Snap Settings (Inspector 可調)")]
    [SerializeField, Tooltip("避免第一幀 prefab 超大閃一下")]
    private float previewInitialScale = 0.01f;

    [SerializeField, Tooltip("tile 尺寸縮小比例（避免 Z-fighting）")]
    private float tileShrinkRatio = 0.99f;

    [SerializeField, Tooltip("preview 從牆面推出的距離（避免穿牆）")]
    private float wallPushOut = 0.01f;

    [SerializeField, Tooltip("preview 是否要把 prefab 本身 collider 關掉（強烈建議開）")]
    private bool disablePreviewColliders = true;

    [SerializeField, Tooltip("需要時再開，log 會很多")]
    private bool debugLogs = false;

    [Header("Wall Orientation")]
    [SerializeField, Tooltip("prefab 哪個 local 法線代表『貼牆面』。你說底面當基準就 (0,-1,0)")]
    private Vector3 wallAttachLocalNormal = new Vector3(0f, -1f, 0f);

    [SerializeField, Tooltip("prefab 哪個 local 方向代表『上』。通常是 (0,1,0)，除非你的模型軸是歪的")]
    private Vector3 wallAttachLocalUp = new Vector3(0f, 1f, 0f);

    [SerializeField, Tooltip("若模型軸本身就差一點點，用這個做最後微調（度數）")]
    private float wallTwistOffsetDeg = 0f;

    // -------------------------
    // Runtime
    // -------------------------

    private WallDirection? currentPreviewWallDir;

    private PlaceableDefinition placingDefinition;
    private GameObject previewObject;
    private bool isPreviewing;

    private FloorTileInfo lastValidPreviewTile;

    private Camera cam;
    private FloorTileInfo previewTile;
    private SelectablePlacedObject selectedObject;

    private readonly Dictionary<PlaceableDefinition, List<GameObject>> placedObjects = new();

    // ================================
    // Rule system runtime tracking
    // ================================
    private struct WallSlot
    {
        public FloorTileInfo ownerFloor;
        public WallDirection dir;
        public int height; // 1,2,3...

        public WallSlot(FloorTileInfo ownerFloor, WallDirection dir, int height)
        {
            this.ownerFloor = ownerFloor;
            this.dir = dir;
            this.height = height;
        }
    }

    // 用來記錄「已放置的牆物件」對應到哪個 slot（不用改 SelectablePlacedObject）
    private readonly Dictionary<GameObject, WallSlot> wallSlotByObject = new();

    // Preview 當下的牆 slot 資訊（給驗證 / 放置使用）
    private FloorTileInfo previewWallOwnerFloor;
    private int previewWallHeightLevel;
    private WallDirection? previewWallDir;   // 你原本 currentPreviewWallDir 也會留著，我這裡只是更明確

    private Vector3 lastHitPoint;
    private Vector3 lastHitNormal;

    //（保留：你原本有用）
    private bool lastPreviewValid;
    private Vector3 lastValidPreviewPos;
    private Quaternion lastValidPreviewRot;

    private const string TAG_FLOOR = "FloorTile";
    private const string TAG_WALL = "WallTile";
    private const string TAG_BASEPLANE = "BasePlane";

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;

        if (deleteButton != null)
        {
            var btn = deleteButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(DeleteSelected);
        }
    }

    // =========================================================
    // Preview
    // =========================================================
    public void BeginPreview(PlaceableDefinition definition)
    {
        placingDefinition = definition;
        isPreviewing = true;

        previewTile = null;
        lastValidPreviewTile = null;
        currentPreviewWallDir = null;

        lastPreviewValid = false;
        lastValidPreviewPos = Vector3.zero;
        lastValidPreviewRot = Quaternion.identity;

        // ✅ 只禁用 raycast，不要改變 layout 或其他 properties
        if (scrollGroup != null)
            scrollGroup.blocksRaycasts = false;

        // ✅ 新增：確保 Layout 組件保持啟用（防止列表高度縮水）
        if (contentLayout != null)
            contentLayout.enabled = true;
        if (contentFitter != null)
            contentFitter.enabled = true;

        previewWallOwnerFloor = null;
        previewWallDir = null;
        previewWallHeightLevel = 1;

        if (previewObject != null)
            Destroy(previewObject);

        GameObject prefab =
            definition.previewPrefab != null
                ? definition.previewPrefab
                : definition.placePrefab;

        previewObject = Instantiate(prefab);

        // ✅ 避免第一幀超大
        previewObject.transform.localScale = Vector3.one * previewInitialScale;

        // ✅ 避免 preview 擋到 raycast（造成抖動 / hit 錯誤）
        if (disablePreviewColliders)
        {
            foreach (var c in previewObject.GetComponentsInChildren<Collider>(true))
                c.enabled = false;
        }

        // ✅ 先套預覽材質（之後會被 valid/invalid 覆蓋）
        foreach (var r in previewObject.GetComponentsInChildren<Renderer>(true))
            r.material = previewMaterial;

        previewObject.SetActive(true);
        DeselectObject();
    }

    public void EndPreview()
    {
        if (isPreviewing && placingDefinition != null && previewObject != null)
            PlaceObjectFromPreview();

        isPreviewing = false;

        // ✅ 恢復 ScrollView raycast（最重要！）
        if (scrollGroup != null)
            scrollGroup.blocksRaycasts = true;

        if (previewObject != null)
            previewObject.SetActive(false);
    }

    private void Update()
    {
        if (!isPreviewing) return;
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasReleasedThisFrame)
        {
            UpdatePreviewPosition(touch.position.ReadValue());
            EndPreview();
            return;
        }

        if (touch.press.isPressed)
        {
            UpdatePreviewPosition(touch.position.ReadValue());
        }
    }

    private void UpdatePreviewPosition(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        var hits = Physics.RaycastAll(ray, 20f, placementMask);
        if (hits == null || hits.Length == 0) return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i].collider;
            if (col == null) continue;

            // ✅ 只接受 Tile
            if (!col.CompareTag(TAG_FLOOR) && !col.CompareTag(TAG_WALL) && !col.CompareTag(TAG_BASEPLANE))
                continue;

            lastHitPoint = hits[i].point;
            lastHitNormal = hits[i].normal;

            if (debugLogs)
                Debug.Log($"[PreviewRay] HitTile: {col.name}, tag={col.tag}");

            MovePreviewOntoTile(col);
            return;
        }
    }

    private void MovePreviewOntoTile(Collider tileCollider)
    {
        if (previewObject == null) return;

        FloorTileInfo tileInfo =
            tileCollider.GetComponent<FloorTileInfo>() ??
            tileCollider.GetComponentInParent<FloorTileInfo>();

        bool isFloorTile =
            tileCollider.CompareTag(TAG_FLOOR) ||
            tileCollider.CompareTag(TAG_BASEPLANE);

        bool isWallTile = tileCollider.CompareTag(TAG_WALL);

        // 1) 顯示位置（先貼齊）
        if (placingDefinition.surface == PlacementSurface.Floor)
        {
            if (isWallTile) SnapPreviewToWall(tileCollider, forceUseHitPoint: false);
            else SnapPreviewToFloor(tileCollider);
        }
        else
        {
            if (isWallTile) SnapPreviewToWall(tileCollider, forceUseHitPoint: false);
            else SnapPreviewToFloor(tileCollider);
        }

        // 2) 基本命中判定
        bool baseValid =
            tileInfo != null &&
            (
                (placingDefinition.surface == PlacementSurface.Floor && isFloorTile) ||
                (placingDefinition.surface == PlacementSurface.Wall && isWallTile)
            );

        // 3) 決定 previewTile（Wall 一定要用 ownerFloor）
        if (placingDefinition.surface == PlacementSurface.Wall)
        {
            // 避免用到上一幀的牆資訊
            if (!baseValid || previewWallOwnerFloor == null || previewWallDir == null)
            {
                previewTile = null;
                currentPreviewWallDir = null;
                lastPreviewValid = false;
                SetPreviewInvalid();
                return;
            }

            previewTile = previewWallOwnerFloor;
            currentPreviewWallDir = previewWallDir; // 保持一致，給你其他地方用
        }
        else
        {
            if (!baseValid)
            {
                previewTile = null;
                currentPreviewWallDir = null;
                lastPreviewValid = false;
                SetPreviewInvalid();
                return;
            }

            previewTile = tileInfo;
        }

        // 4) 套規則 + 重疊檢查（要跟 PlaceObjectFromPreview 一致）
        bool valid = true;

        if (!PassExtraPlacementRules(placingDefinition.surface))
            valid = false;

        if (valid)
        {
            int h = (placingDefinition.surface == PlacementSurface.Wall) ? previewWallHeightLevel : 0;

            if (HasTowerConflict(
                    previewTile,
                    placingDefinition.surface,
                    placingDefinition.surface == PlacementSurface.Wall ? currentPreviewWallDir : null,
                    h))
            {
                valid = false;
            }
        }

        // 5) 上色 / 記錄
        if (valid)
        {
            lastValidPreviewTile = previewTile;
            lastValidPreviewPos = previewObject.transform.position;
            lastValidPreviewRot = previewObject.transform.rotation;
            lastPreviewValid = true;
            SetPreviewValid();
        }
        else
        {
            previewTile = null;
            currentPreviewWallDir = null;
            lastPreviewValid = false;
            SetPreviewInvalid();
        }
    }



    // =========================================================
    // Snap: Floor
    // =========================================================
    private void SnapPreviewToFloor(Collider tileCollider)
    {
        Bounds b = tileCollider.bounds;

        float baseSize = tileCollider.transform.localScale.x;
        float small = baseSize * tileShrinkRatio;
        previewObject.transform.localScale = Vector3.one * small;

        var rend = previewObject.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            // 沒 renderer 就至少放在 tile 表面中心
            Vector3 top0 = b.center + Vector3.up * b.extents.y;
            previewObject.transform.SetPositionAndRotation(top0, tileCollider.transform.rotation);
            return;
        }

        float h = rend.bounds.size.y;
        Vector3 top = b.center + Vector3.up * b.extents.y;

        previewObject.transform.SetPositionAndRotation(
            top + Vector3.up * h * 0.5f,
            tileCollider.transform.rotation
        );
    }

    // =========================================================
    // Snap: Wall (穩定版：面中心，不跟手抖跑)
    // =========================================================
    private WallDirection GetWallDirectionFromNormal(Vector3 normal)
    {
        // 你原本的方向判定我保留，但我會先把 normal 卡成主軸
        normal = GetCardinalNormal(normal);

        if (Vector3.Dot(normal, Vector3.forward) > 0.7f) return WallDirection.South;
        if (Vector3.Dot(normal, Vector3.back) > 0.7f) return WallDirection.North;
        if (Vector3.Dot(normal, Vector3.right) > 0.7f) return WallDirection.West;
        return WallDirection.East;
    }

    private static Vector3 GetCardinalNormal(Vector3 n)
    {
        // 取最大分量 → 卡成 (±1,0,0) / (0,±1,0) / (0,0,±1)
        float ax = Mathf.Abs(n.x);
        float ay = Mathf.Abs(n.y);
        float az = Mathf.Abs(n.z);

        if (ax >= ay && ax >= az) return new Vector3(Mathf.Sign(n.x), 0f, 0f);
        if (az >= ax && az >= ay) return new Vector3(0f, 0f, Mathf.Sign(n.z));
        return new Vector3(0f, Mathf.Sign(n.y), 0f);
    }

private void SnapPreviewToWall(Collider tileCollider, bool forceUseHitPoint)
{
        Bounds b = tileCollider.bounds;

        // 每次 snap 都先清，避免沿用上一幀
        previewWallOwnerFloor = null;
        previewWallDir = null;
        previewWallHeightLevel = 1;

        // ✅ 只宣告一次，避免 out var 重複宣告炸掉
        FloorTileInfo owner;
        int hLevel;

        // 0) 必須是 BoxCollider 才能做到「用牆自身決定面」
        if (!(tileCollider is BoxCollider box))
        {
            Vector3 fallbackNormal = tileCollider.transform.forward;
            currentPreviewWallDir = GetWallDirectionFromNormal(fallbackNormal);

            Vector3 faceCenterFallback = tileCollider.ClosestPoint(b.center + fallbackNormal * 999f);

            float baseSize0 = tileCollider.transform.localScale.x;
            float small0 = baseSize0 * tileShrinkRatio;
            previewObject.transform.localScale = Vector3.one * small0;

            Vector3 pos0 = faceCenterFallback + fallbackNormal * wallPushOut;

            Quaternion rot0 = Quaternion.FromToRotation(
                wallAttachLocalNormal.normalized,
                (-fallbackNormal).normalized
            );

            previewObject.transform.SetPositionAndRotation(pos0, rot0);

            // ✅ 不用 meta：照樣嘗試推 owner/height
            if (TryResolveWallOwnerAndHeight(tileCollider, faceCenterFallback, fallbackNormal, out owner, out hLevel))
            {
                previewWallOwnerFloor = owner;
                previewWallHeightLevel = hLevel;
                previewWallDir = currentPreviewWallDir; // 由 fallbackNormal 推出來的
            }
            else
            {
                previewWallOwnerFloor = null;
                previewWallDir = null;
                previewWallHeightLevel = 1;
            }

            return;
        }

        // 1) hit point 只用來「判斷是哪個面」，不拿來定位/法線
        Transform tf = box.transform;

        Vector3 localP = tf.InverseTransformPoint(lastHitPoint);
        Vector3 rel = localP - box.center;
        Vector3 half = box.size * 0.5f;

        float distToX = half.x - Mathf.Abs(rel.x);
        float distToZ = half.z - Mathf.Abs(rel.z);

        // 2) 決定面：只看 X/Z（牆不看上下 Y 面）
        Vector3 localFaceNormal;
        if (distToX <= distToZ)
            localFaceNormal = new Vector3(Mathf.Sign(rel.x) == 0 ? 1f : Mathf.Sign(rel.x), 0f, 0f);
        else
            localFaceNormal = new Vector3(0f, 0f, Mathf.Sign(rel.z) == 0 ? 1f : Mathf.Sign(rel.z));

        // 3) 牆外法線（世界）
        Vector3 wallNormal = tf.TransformDirection(localFaceNormal).normalized;

        // 4) 記錄牆方向
        currentPreviewWallDir = GetWallDirectionFromNormal(wallNormal);

        // 5) 牆面中心（世界）
        Vector3 faceLocal = box.center + new Vector3(localFaceNormal.x * half.x, 0f, localFaceNormal.z * half.z);
        Vector3 wallFaceCenter = tf.TransformPoint(faceLocal);

        // ✅ 不用 meta：用幾何 probe 找 owner floor + height
        if (TryResolveWallOwnerAndHeight(tileCollider, wallFaceCenter, wallNormal, out owner, out hLevel))
        {
            previewWallOwnerFloor = owner;
            previewWallHeightLevel = hLevel;
            previewWallDir = currentPreviewWallDir; // 由 wallNormal 推
        }
        else
        {
            previewWallOwnerFloor = null;
            previewWallDir = null;
            previewWallHeightLevel = 1;
        }

        // 6) scale（保留你的）
        float baseSize = tileCollider.transform.localScale.x;
        float small = baseSize * tileShrinkRatio;
        previewObject.transform.localScale = Vector3.one * small;


    // ============================================================
    // ✅ 7) 四角對齊的關鍵：用「牆面基底」對齊「prefab 底面基底」
    //    然後用「prefab 底面中心」對齊「牆面中心」
    // ============================================================

    // 7-1) 牆面基底（world）
    Vector3 targetForward = (-wallNormal).normalized; // 向牆內（物件貼牆面時，底面法線朝牆內）
    Vector3 targetUp = ProjectOnPlaneSafe(tf.up, targetForward, Vector3.up);
    Vector3 targetRight = Vector3.Cross(targetUp, targetForward).normalized;

    // 7-2) prefab 底面基底（local）
    Vector3 attachN_L = GetCardinalAxisLocal(wallAttachLocalNormal); // 用 cardinal 軸避免你填歪一點就爆
    Vector3 attachU_L = ProjectOnPlaneSafe(wallAttachLocalUp, attachN_L, Vector3.up);
    Vector3 attachR_L = Vector3.Cross(attachU_L, attachN_L).normalized;

    // 7-3) 旋轉：把 prefab 基底映射到牆面基底（一次鎖住 yaw/pitch/roll）
    Quaternion prefabBasis = Quaternion.LookRotation(attachN_L, attachU_L);
    Quaternion wallBasis = Quaternion.LookRotation(targetForward, targetUp);

    Quaternion rot = wallBasis * Quaternion.Inverse(prefabBasis);

    // 7-4) 防止右軸反向（保留你原本 “確保 right 對齊” 的精神）
    Vector3 actualRightW = rot * attachR_L;
    if (Vector3.Dot(actualRightW, targetRight) < 0f)
    {
        targetUp = -targetUp;
        wallBasis = Quaternion.LookRotation(targetForward, targetUp);
        rot = wallBasis * Quaternion.Inverse(prefabBasis);
    }

    // 7-5) twist（保留你的微調，但注意這是繞貼牆法線做 roll）
    if (Mathf.Abs(wallTwistOffsetDeg) > 0.0001f)
    {
        rot = Quaternion.AngleAxis(wallTwistOffsetDeg, targetForward) * rot;
    }

    // 7-6) 位置：不是貼 pivot，而是貼「prefab 底面中心」→ 牆面中心（corner align 的前提）
    //      只要你的 prefab 尺寸本來就是 1 tile，四角會自然對齊牆四角
    Vector3 desiredFaceCenter = wallFaceCenter + wallNormal * wallPushOut; // 往牆外推一點避免穿牆

    if (!TryGetCombinedLocalBounds(previewObject, out Bounds prefabBoundsLocal))
    {
        // 沒 renderer → 退回中心貼
        previewObject.transform.SetPositionAndRotation(desiredFaceCenter, rot);
        return;
    }

    // prefab 底面（貼牆面）中心（local in previewObject space）
    Vector3 prefabAttachFaceCenter_L = GetFaceCenterFromBoundsLocal(prefabBoundsLocal, attachN_L);

    // 把 local 點轉成 world： pW = pos + rot * Scale(pL)
    // → pos = desired - rot * Scale(pL)
    Vector3 scaledLocal = Vector3.Scale(prefabAttachFaceCenter_L, previewObject.transform.localScale);
    Vector3 pos = desiredFaceCenter - (rot * scaledLocal);

    previewObject.transform.SetPositionAndRotation(pos, rot);
}




    // =========================================================
    // Materials
    // =========================================================
    private void SetPreviewValid()
    {
        if (previewObject == null) return;
        if (previewValidMaterial == null) return;

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>(true))
            r.material = previewValidMaterial;
    }

    private void SetPreviewInvalid()
    {
        if (previewObject == null) return;
        if (previewInvalidMaterial == null) return;

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>(true))
            r.material = previewInvalidMaterial;
    }

    // =========================================================
    // Place
    // =========================================================
    private void PlaceObjectFromPreview()
    {
        if (previewTile == null)
            return;   // ❗非法預覽，一律不放

        if (placingDefinition == null)
            return;

        var tileToUse = previewTile;

        if (placingDefinition.surface == PlacementSurface.Wall)
        {
            if (previewWallOwnerFloor == null || previewWallDir == null)
                return;

            tileToUse = previewWallOwnerFloor; // ✅ 關鍵：牆歸屬同一顆 floor
        }

        if (tileToUse == null)
            return;

        if (!placedObjects.ContainsKey(placingDefinition))
            placedObjects[placingDefinition] = new List<GameObject>();

        if (placingDefinition.maxCount >= 0 &&
            placedObjects[placingDefinition].Count >= placingDefinition.maxCount)
            return;

        // ==================================================
        // 🔑 放置條件分流（關鍵）
        // ==================================================
        if (placingDefinition.surface == PlacementSurface.Floor)
        {
            // 地板塔：一定要是空的 Floor
            if (tileToUse.occupied != OccupyType.Floor)
                return;
        }
        else if (placingDefinition.surface == PlacementSurface.Wall)
        {
            // 牆塔：不檢查地板佔用（牆是附著物，不佔地板）
        }

        // ==================================================
        // 🔒 Tower 重疊檢查（MVP）
        // ==================================================
        if (!PassExtraPlacementRules(placingDefinition.surface))
            return;

        int h = (placingDefinition.surface == PlacementSurface.Wall) ? previewWallHeightLevel : 0;

        if (HasTowerConflict(
                tileToUse,
                placingDefinition.surface,
                placingDefinition.surface == PlacementSurface.Wall ? currentPreviewWallDir : null,
                h))
        {
            return;
        }

        // ==================================================
        // Instantiate
        // ==================================================
        GameObject obj = Instantiate(placingDefinition.placePrefab);
        obj.transform.SetPositionAndRotation(
            previewObject.transform.position,
            previewObject.transform.rotation
        );
        obj.transform.localScale = previewObject.transform.localScale;

        var selectable = obj.GetComponent<SelectablePlacedObject>();
        if (selectable == null)
            selectable = obj.AddComponent<SelectablePlacedObject>();

        selectable.tile = tileToUse;
        selectable.definition = placingDefinition;

        // ✅ 用 occupyType 判斷是不是塔（Spawn/Goal 一律不是塔）
        selectable.isTower =
            placingDefinition.occupyType != OccupyType.Spawn &&
            placingDefinition.occupyType != OccupyType.Goal;

        if (placingDefinition.surface == PlacementSurface.Wall)
        {
            if (previewWallOwnerFloor == null || previewWallDir == null) return;

            // ✅ 這行是關鍵：牆塔一定要有 wallDirection，不能是 null
            selectable.wallDirection = previewWallDir.Value;

            // ✅ 記錄 slot（高度判斷靠這個）
            wallSlotByObject[obj] = new WallSlot(previewWallOwnerFloor, previewWallDir.Value, previewWallHeightLevel);

            // ✅ NEW：放牆就把 BFS 牆阻擋寫進 tile（只會作用於 height==1）
            ApplyMovementBlockForWallSlot(previewWallOwnerFloor, previewWallDir.Value, previewWallHeightLevel, add: true);

        }
        else
        {
            // Floor 物件一定是 null
            selectable.wallDirection = null;
        }


        // ==================================================
        // 🔑 更新 tile 佔用（牆不要改）
        // ==================================================
        if (placingDefinition.surface == PlacementSurface.Floor)
        {
            tileToUse.occupied = placingDefinition.occupyType;
        }

        placedObjects[placingDefinition].Add(obj);

        // ✅ Spend money after successful placement
        PlacementCostValidator.SpendPlacementCost(placingDefinition.cost);

        // ✅ 只有 Spawn/Goal 會影響 compute 按鈕顯示，Tower 放置不刷新
        if (placingDefinition.occupyType == OccupyType.Spawn ||
            placingDefinition.occupyType == OccupyType.Goal)
        {
            TowerDefenseUIManager.Instance?.UpdateComputePathButtonVisibility();
        }

    }

    // =========================================================
    // ✅ NEW：Wall -> FloorTileInfo.block* 同步（只做 height==1 的牆）
    // =========================================================
    private void ApplyMovementBlockForWallSlot(FloorTileInfo owner, WallDirection dir, int height, bool add)
    {
        if (owner == null) return;

        // ✅ 只有 height==1 的牆才會擋 BFS 移動（你現在的規則就是這樣）
        if (height != 1) return;

        // 找 neighbour（用你自己的 grid 規則）
        if (TryGetNeighbourFloorByGrid(owner, dir, out var neighbour) && neighbour != null)
        {
            SetBlockBetween(owner, neighbour, add);
        }
        else
        {
            // 地圖邊界：沒有 neighbour 就只設 owner 這邊（其實 BFS 也不會走出去）
            // 但為了 Debug 清楚，仍保留
            // 這邊可選擇不做任何事也行
        }
    }

    // ✅ 用 dx/dy 決定要改哪個 flag（完全對齊你 FloorTileInfo.IsBlockedTowards 的定義）
    private static void SetBlockBetween(FloorTileInfo a, FloorTileInfo b, bool blocked)
    {
        int dx = b.gridX - a.gridX;
        int dy = b.gridY - a.gridY;

        // a -> b
        if (dx == 1 && dy == 0) a.blockEast = blocked;
        else if (dx == -1 && dy == 0) a.blockWest = blocked;
        else if (dx == 0 && dy == 1) a.blockNorth = blocked;
        else if (dx == 0 && dy == -1) a.blockSouth = blocked;

        // b -> a（反向也要設）
        dx = -dx;
        dy = -dy;

        if (dx == 1 && dy == 0) b.blockEast = blocked;
        else if (dx == -1 && dy == 0) b.blockWest = blocked;
        else if (dx == 0 && dy == 1) b.blockNorth = blocked;
        else if (dx == 0 && dy == -1) b.blockSouth = blocked;
    }

    // ✅ 刪除時要用：確認這條邊上還有沒有其他 height==1 的牆
    private bool HasAnyHeight1WallOnEdge(FloorTileInfo owner, WallDirection dir)
    {
        if (owner == null) return false;

        // 先找 neighbour
        if (!TryGetNeighbourFloorByGrid(owner, dir, out var neighbour) || neighbour == null)
            return false;

        WallDirection opp = OppositeDir(dir);

        foreach (var kv in wallSlotByObject)
        {
            if (kv.Key == null) continue;
            var s = kv.Value;
            if (s.height != 1) continue;

            // 同一條邊可能由 owner 或 neighbour 當 owner 被記錄到，所以兩邊都算
            bool sameEdgeA = (s.ownerFloor == owner && s.dir == dir);
            bool sameEdgeB = (s.ownerFloor == neighbour && s.dir == opp);

            if (sameEdgeA || sameEdgeB)
                return true;
        }

        return false;
    }

    // =========================================================
    // Select / Delete
    // =========================================================
    public void SelectObject(SelectablePlacedObject obj)
    {
        selectedObject = obj;

        // ✅ 只要有選到任何物件就顯示 XX，沒選到就不顯示
        if (deleteButton != null)
            deleteButton.SetActive(obj != null);

        // 先關一次塔資訊，避免殘留上一個塔的面板
        TowerUIManager.Instance?.Hide();

        // 不是塔（Spawn/Goal）→ 只顯示 XX
        if (obj == null || !obj.isTower)
            return;

        // 是塔 → 嘗試抓 PlacedTower 資料
        var tower = obj.GetComponent<PlacedTower>() ?? obj.GetComponentInParent<PlacedTower>();

        Debug.Log($"[SelectObject] hit={obj.name}, isTower={obj.isTower}, placedTower={(tower != null)}, id={(tower != null ? tower.towerId : "null")}");

        if (tower != null && !string.IsNullOrEmpty(tower.towerId))
            TowerUIManager.Instance?.ShowTowerInstance(tower.towerId, tower.level);
    }

    public void DeselectObject()
    {
        selectedObject = null;

        if (deleteButton != null)
            deleteButton.SetActive(false);

        // ✅ 點空地取消選取時，同步關掉塔資訊
        TowerUIManager.Instance?.Hide();
    }

    // ✅ 你原本遺漏會報錯的兩個方法：完整保留
    public bool HasOccupyType(OccupyType type)
    {
        if (FloorBuildManager.Instance == null)
            return false;

        // 1️⃣ 掃 floorParent 底下的 tiles
        var tiles = FloorBuildManager.Instance.floorParent
            .GetComponentsInChildren<FloorTileInfo>();

        foreach (var t in tiles)
        {
            if (t.occupied == type)
                return true;
        }

        // 2️⃣ 補掃 BasePlane 本身（關鍵）
        var baseTf = FloorBuildManager.Instance.basePlaneTransform;
        if (baseTf != null)
        {
            var baseTile = baseTf.GetComponent<FloorTileInfo>();
            if (baseTile != null && baseTile.occupied == type)
                return true;
        }

        return false;
    }

    public FloorTileInfo GetFirstTileByOccupyType(OccupyType type)
    {
        if (FloorBuildManager.Instance == null)
            return null;

        // 1️⃣ 先掃 floor tiles
        var tiles = FloorBuildManager.Instance.floorParent
            .GetComponentsInChildren<FloorTileInfo>();

        foreach (var t in tiles)
        {
            if (t.occupied == type)
                return t;
        }

        // 2️⃣ 再看 BasePlane
        var baseTf = FloorBuildManager.Instance.basePlaneTransform;
        if (baseTf != null)
        {
            var baseTile = baseTf.GetComponent<FloorTileInfo>();
            if (baseTile != null && baseTile.occupied == type)
                return baseTile;
        }

        return null;
    }

    public void DeleteSelected()
    {
        if (selectedObject == null) return;

        // ✅ Refund money before destroying the object
        if (selectedObject.definition != null)
        {
            MoneyManager.Instance?.RefundMoney(selectedObject.definition.cost);
        }

        // ✅ 先記下類型（Destroy 之後就不好拿了）
        OccupyType deletedType = OccupyType.None;
        if (selectedObject.definition != null)
            deletedType = selectedObject.definition.occupyType;

        // =========================================================
        // ✅ NEW：刪牆後重建所有 height==1 的 block（最穩）
        // =========================================================
        wallSlotByObject.Remove(selectedObject.gameObject);
        RebuildAllHeight1MovementBlocks();


        // =========================================================
        // ✅ 只有「地板物件」才清 occupied
        // =========================================================
        if (selectedObject.definition != null && selectedObject.definition.surface == PlacementSurface.Floor)
        {
            if (selectedObject.tile != null)
                selectedObject.tile.occupied = OccupyType.Floor;
        }

        // ✅ 從 placedObjects 清掉
        if (selectedObject.definition != null &&
            placedObjects.TryGetValue(selectedObject.definition, out var list))
        {
            list.Remove(selectedObject.gameObject);
        }

        Destroy(selectedObject.gameObject);

        // ✅ 只有刪 Spawn/Goal 才刷新 compute
        if (deletedType == OccupyType.Spawn || deletedType == OccupyType.Goal)
        {
            TowerDefenseUIManager.Instance?.UpdateComputePathButtonVisibility();
        }

        DeselectObject();
    }


    // =========================================================
    // ✅ NEW：重建所有 height==1 牆的移動阻擋（最穩，不怕方向 mapping 出錯）
    // =========================================================
    private void RebuildAllHeight1MovementBlocks()
    {
        var fbm = FloorBuildManager.Instance;
        if (fbm == null) return;

        // 1) 收集全部 tiles（含 BasePlane）
        var tiles = new List<FloorTileInfo>(
            fbm.floorParent.GetComponentsInChildren<FloorTileInfo>()
        );

        if (fbm.basePlaneTransform != null)
        {
            var baseTile = fbm.basePlaneTransform.GetComponent<FloorTileInfo>();
            if (baseTile != null && !tiles.Contains(baseTile)) tiles.Add(baseTile);
        }

        // 2) 全清 block
        foreach (var t in tiles)
        {
            t.blockNorth = t.blockSouth = t.blockEast = t.blockWest = false;
        }

        // 3) 依目前所有已放置的牆（只做 height==1）重建 block
        foreach (var kv in wallSlotByObject)
        {
            var s = kv.Value;
            if (s.ownerFloor == null) continue;
            if (s.height != 1) continue;

            ApplyMovementBlockForWallSlot(s.ownerFloor, s.dir, s.height, add: true);
        }

        if (debugLogs) Debug.Log("[WallBlock] RebuildAllHeight1MovementBlocks done");
    }

    // =========================================================
    // Overlap Check (MVP) 
    // =========================================================
    private bool HasTowerConflict(
     FloorTileInfo tile,
     PlacementSurface surface,
     WallDirection? wallDir,
     int wallHeight // ✅ 你現在呼叫時傳 previewWallHeightLevel
 )
    {
        foreach (var pair in placedObjects)
        {
            foreach (var obj in pair.Value)
            {
                if (obj == null) continue;

                var placed = obj.GetComponent<SelectablePlacedObject>();
                if (placed == null) continue;
                if (!placed.isTower) continue;

                // 只檢查同一顆 owner floor tile
                if (placed.tile != tile)
                    continue;

                bool isPlacedFloorObj = (placed.wallDirection == null);

                // 這顆已放置的牆高度（沒記到就當 1）
                int placedWallH = 1;
                if (!isPlacedFloorObj && wallSlotByObject.TryGetValue(obj, out var slot))
                    placedWallH = slot.height;

                // -----------------------------
                // Case 1：要放的是 Floor
                // -----------------------------
                if (surface == PlacementSurface.Floor)
                {
                    // ✅ Floor 物件彼此永遠互斥
                    if (isPlacedFloorObj)
                        return true;

                    // ✅ 只有「height=1 的牆」會擋 Floor（你的 Rule 2）
                    if (!isPlacedFloorObj && placedWallH == 1)
                        return true;

                    // ❌ height>=2 的牆不要擋 Floor
                    continue;
                }

                // -----------------------------
                // Case 2：要放的是 Wall
                // -----------------------------
                if (surface == PlacementSurface.Wall)
                {
                    // ✅ Floor 只會擋 height=1 的牆（你的 Rule 1）
                    if (isPlacedFloorObj)
                    {
                        if (wallHeight == 1) return true;
                        else continue; // height>=2 不被 floor 擋
                    }

                    // ✅ 牆只擋「同一面 + 同一高度」(slot overlap)
                    if (placed.wallDirection == wallDir && placedWallH == wallHeight)
                        return true;

                    // 不同面或不同高度 → 允許
                    continue;
                }
            }
        }

        return false;
    }




    // ===============================
    // Helpers for Wall Face Alignment
    // ===============================

    private static Vector3 GetCardinalAxisLocal(Vector3 n)
    {
        // 取最大分量 → 卡成 (±1,0,0)/(0,±1,0)/(0,0,±1)
        n = n.normalized;
        float ax = Mathf.Abs(n.x);
        float ay = Mathf.Abs(n.y);
        float az = Mathf.Abs(n.z);

        if (ax >= ay && ax >= az) return new Vector3(Mathf.Sign(n.x) == 0 ? 1f : Mathf.Sign(n.x), 0f, 0f);
        if (ay >= ax && ay >= az) return new Vector3(0f, Mathf.Sign(n.y) == 0 ? 1f : Mathf.Sign(n.y), 0f);
        return new Vector3(0f, 0f, Mathf.Sign(n.z) == 0 ? 1f : Mathf.Sign(n.z));
    }

    private static Vector3 ProjectOnPlaneSafe(Vector3 v, Vector3 planeNormal, Vector3 fallback)
    {
        Vector3 p = Vector3.ProjectOnPlane(v, planeNormal);
        if (p.sqrMagnitude < 1e-8f) p = Vector3.ProjectOnPlane(fallback, planeNormal);
        if (p.sqrMagnitude < 1e-8f) p = Vector3.up; // 最後保底
        return p.normalized;
    }

    private bool TryGetCombinedLocalBounds(GameObject root, out Bounds localBounds)
    {
        // 把所有 Renderer 的 localBounds 統一換算到 root local 空間後 Encapsulate
        var rs = root.GetComponentsInChildren<Renderer>(true);
        if (rs == null || rs.Length == 0)
        {
            localBounds = new Bounds(Vector3.zero, Vector3.zero);
            return false;
        }

        bool inited = false;
        localBounds = new Bounds(Vector3.zero, Vector3.zero);

        for (int i = 0; i < rs.Length; i++)
        {
            Renderer r = rs[i];
            Bounds b = r.localBounds;

            // localBounds 的 8 個角（在該 renderer 的 local space）
            Vector3 c = b.center;
            Vector3 e = b.extents;

            Vector3[] corners =
            {
            c + new Vector3( e.x,  e.y,  e.z),
            c + new Vector3( e.x,  e.y, -e.z),
            c + new Vector3( e.x, -e.y,  e.z),
            c + new Vector3( e.x, -e.y, -e.z),
            c + new Vector3(-e.x,  e.y,  e.z),
            c + new Vector3(-e.x,  e.y, -e.z),
            c + new Vector3(-e.x, -e.y,  e.z),
            c + new Vector3(-e.x, -e.y, -e.z),
        };

            for (int k = 0; k < corners.Length; k++)
            {
                // renderer local -> world -> root local
                Vector3 pWorld = r.transform.TransformPoint(corners[k]);
                Vector3 pLocalToRoot = root.transform.InverseTransformPoint(pWorld);

                if (!inited)
                {
                    localBounds = new Bounds(pLocalToRoot, Vector3.zero);
                    inited = true;
                }
                else
                {
                    localBounds.Encapsulate(pLocalToRoot);
                }
            }
        }

        return inited;
    }

    private static Vector3 GetFaceCenterFromBoundsLocal(Bounds b, Vector3 faceNormalLocalCardinal)
    {
        // faceNormalLocalCardinal 是 (±1,0,0)/(0,±1,0)/(0,0,±1)
        Vector3 e = b.extents;
        Vector3 c = b.center;
        return c + new Vector3(faceNormalLocalCardinal.x * e.x, faceNormalLocalCardinal.y * e.y, faceNormalLocalCardinal.z * e.z);
    }

    // ================================
    // Rule helpers
    // ================================
    private static WallDirection OppositeDir(WallDirection d)
    {
        return d switch
        {
            WallDirection.North => WallDirection.South,
            WallDirection.South => WallDirection.North,
            WallDirection.East => WallDirection.West,
            WallDirection.West => WallDirection.East,
            _ => d
        };
    }

    // ⚠️ 這個 offset 依照你原本 GetWallDirectionFromNormal 的 mapping：
    // normal +X => West, normal -X => East, normal -Z => North, normal +Z => South
    private static Vector2Int DirToGridOffset(WallDirection d)
    {
        return d switch
        {
            WallDirection.West => new Vector2Int(+1, 0),
            WallDirection.East => new Vector2Int(-1, 0),
            WallDirection.North => new Vector2Int(0, -1),
            WallDirection.South => new Vector2Int(0, +1),
            _ => Vector2Int.zero
        };
    }

    private bool TryGetNeighbourFloorByGrid(FloorTileInfo floor, WallDirection dir, out FloorTileInfo neighbour)
    {
        neighbour = null;
        if (floor == null) return false;

        // 建一次 cache：grid -> tile
        // （你的 FloorTileInfo 有 gridX/gridY，所以可以用這個）
        var tiles = FloorBuildManager.Instance?.floorParent?.GetComponentsInChildren<FloorTileInfo>();
        if (tiles == null) return false;

        // 小量就直接線性找，避免你還要維護 cache（數量大再優化）
        Vector2Int off = DirToGridOffset(dir);
        int nx = floor.gridX + off.x;
        int ny = floor.gridY + off.y;

        foreach (var t in tiles)
        {
            if (t != null && t.gridX == nx && t.gridY == ny)
            {
                neighbour = t;
                return true;
            }
        }

        // 補 BasePlane（如果它不在 floorParent 裡）
        var baseTf = FloorBuildManager.Instance?.basePlaneTransform;
        if (baseTf != null)
        {
            var baseTile = baseTf.GetComponent<FloorTileInfo>();
            if (baseTile != null && baseTile.gridX == nx && baseTile.gridY == ny)
            {
                neighbour = baseTile;
                return true;
            }
        }

        return false;
    }

    // 從「牆 tile collider」推導：owner floor + heightLevel（不改你的 floor prefab）
    // 從「牆 tile collider」推導：owner floor + heightLevel（雙向 probe，內外側都能抓到）
    private bool TryResolveWallOwnerAndHeight(
        Collider wallCollider,
        Vector3 wallFaceCenter,
        Vector3 wallNormalOutward,
        out FloorTileInfo ownerFloor,
        out int heightLevel
    )
    {
        ownerFloor = null;
        heightLevel = 1;

        Bounds wb = wallCollider.bounds;
        float tileSize = Mathf.Max(wb.size.x, wb.size.z);

        float sideOffset = tileSize * 0.55f;
        float upOffset = tileSize * 0.5f;
        float maxDown = tileSize * 5f;

        // ✅ 兩邊都試：-normal 與 +normal
        Vector3 probeA = wallFaceCenter - wallNormalOutward.normalized * sideOffset + Vector3.up * upOffset;
        Vector3 probeB = wallFaceCenter + wallNormalOutward.normalized * sideOffset + Vector3.up * upOffset;

        bool hitA = TryRayDownFindFloor(probeA, maxDown, out Collider floorColA, out float distA);
        bool hitB = TryRayDownFindFloor(probeB, maxDown, out Collider floorColB, out float distB);

        Collider bestCol = null;

        if (hitA && hitB) bestCol = (distA <= distB) ? floorColA : floorColB;
        else if (hitA) bestCol = floorColA;
        else if (hitB) bestCol = floorColB;
        else return false;

        ownerFloor =
            bestCol.GetComponent<FloorTileInfo>() ??
            bestCol.GetComponentInParent<FloorTileInfo>();

        if (ownerFloor == null) return false;

        // ✅ 計算 heightLevel（用牆的 centerY，比用 bottomY 穩）
        // Level 1 的中心 ≈ floorTopY + 0.5*tileSize
        Bounds fb = bestCol.bounds;
        float floorTopY = fb.center.y + fb.extents.y;

        float wallCenterY = wb.center.y;
        float level1CenterY = floorTopY + tileSize * 0.5f;

        float t = (wallCenterY - level1CenterY) / tileSize;
        heightLevel = Mathf.Max(1, Mathf.RoundToInt(t) + 1);

        return true;
    }

    // 幫手：往下找最近的 floor/baseplane（只回傳符合 tag 的）
    private bool TryRayDownFindFloor(
        Vector3 origin,
        float maxDist,
        out Collider bestCol,
        out float bestDist
    )
    {
        bestCol = null;
        bestDist = float.PositiveInfinity;

        Ray down = new Ray(origin, Vector3.down);
        var hits = Physics.RaycastAll(down, maxDist, placementMask);

        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i].collider;
            if (c == null) continue;

            if (!c.CompareTag(TAG_FLOOR) && !c.CompareTag(TAG_BASEPLANE))
                continue;

            if (hits[i].distance < bestDist)
            {
                bestDist = hits[i].distance;
                bestCol = c;
            }
        }

        return bestCol != null;
    }

    // 判斷：這個 floor tile 上是否已有「地板物件」（plane prefab）
    // 你現在系統裡只要 occupied != Floor，就視為 floor slot 被占（包含 Spawn/Goal 你要不要算，下面我用最嚴格：都算）
    private bool IsFloorSlotOccupied(FloorTileInfo floor)
    {
        if (floor == null) return false;
        return floor.occupied != OccupyType.Floor;
    }

    // 判斷：某 floor 的任一 height=1 牆 slot 是否已被占
    private bool HasAnyHeight1WallOnFloor(FloorTileInfo ownerFloor)
    {
        foreach (var kv in wallSlotByObject)
        {
            if (kv.Key == null) continue;
            var s = kv.Value;
            if (s.ownerFloor == ownerFloor && s.height == 1)
                return true;
        }
        return false;
    }

    // 判斷：某個 wall slot 是否被占（同 ownerFloor+dir+height）
    private bool IsWallSlotOccupied(FloorTileInfo ownerFloor, WallDirection dir, int height)
    {
        foreach (var kv in wallSlotByObject)
        {
            if (kv.Key == null) continue;
            var s = kv.Value;
            if (s.ownerFloor == ownerFloor && s.dir == dir && s.height == height)
                return true;
        }
        return false;
    }

    private bool PassExtraPlacementRules(PlacementSurface surface)
    {
        // =========================
        // Floor rules
        // =========================
        if (surface == PlacementSurface.Floor)
        {
            // Rule 2（前半）：任一 height=1 wall 有東西 → floor 不能放
            
            
            if (HasAnyHeight1WallOnFloor(previewTile))
                return false;

            // 你原本的基本規則：tile.occupied 不是 Floor → 不能放
            if (IsFloorSlotOccupied(previewTile))
                return false;

            return true;
        }

        // =========================
        // Wall rules
        // =========================
        if (previewWallOwnerFloor == null || previewWallDir == null)
            return false;

        int h = previewWallHeightLevel;
        WallDirection d = previewWallDir.Value;
        FloorTileInfo owner = previewWallOwnerFloor;

        if (h == 1)
        {
            // Rule 1：floor 上有放「平面 prefab / Floor 物件」→ 四面 height=1 wall 全非法
            if (HasAnyFloorPlacedObjectOnTile(owner))
                return false;

            // 基本：同一個 wall slot（owner+dir+height）不能重疊
            if (IsWallSlotOccupied(owner, d, 1))
                return false;

            // ⚠️ Rule 2（後半）要不要做「其餘三面也不能放」，看你要哪個版本（下面我給你兩種）
            return true;
        }

        // h >= 2
        // Rule 3：對面（一格 floor 距離）不能放（同高度、對向面）
        if (TryGetNeighbourFloorByGrid(owner, d, out var neighbour))
        {
            // 基本：同一個 wall slot 不能重疊
            if (IsWallSlotOccupied(owner, d, h))
                return false;

            WallDirection opp = OppositeDir(d);
            if (IsWallSlotOccupied(neighbour, opp, h))
                return false;
        }

        return true;
    }


    // ✅ 真正的「floor 上是否有放置平面 prefab（Floor 物件）」判定
    private bool HasAnyFloorPlacedObjectOnTile(FloorTileInfo tile)
    {
        if (tile == null) return false;

        foreach (var pair in placedObjects)
        {
            foreach (var obj in pair.Value)
            {
                if (obj == null) continue;

                var placed = obj.GetComponent<SelectablePlacedObject>();
                if (placed == null) continue;
                if (!placed.isTower) continue;

                if (placed.tile != tile) continue;

                // Floor 物件：wallDirection == null（你目前的邏輯就是這樣區分）
                if (placed.wallDirection == null)
                    return true;
            }
        }
        return false;
    }


    private bool TryGetWallHeight(GameObject obj, out int h)
    {
        h = 1;
        if (obj == null) return false;
        if (wallSlotByObject.TryGetValue(obj, out var slot))
        {
            h = slot.height;
            return true;
        }
        return false; // 沒記到就當作 height=1（保守）
    }
}

