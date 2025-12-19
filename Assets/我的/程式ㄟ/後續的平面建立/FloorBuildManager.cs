using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloorBuildManager : MonoBehaviour
{
    public static FloorBuildManager Instance;

    public GameObject undoButton;
    public GameObject switchModeButton;
    public GameObject finishBuildButton;   // ★ 新增：完成建造按鈕

    // ------------------------------------------------------
    // Floor Settings
    // ------------------------------------------------------
    [Header("Floor Settings")]
    public Transform floorParent;
    public GameObject floorPrefab;

    // ------------------------------------------------------
    // Wall Settings
    // ------------------------------------------------------
    [Header("Wall Settings")]
    public Transform wallParent;
    public GameObject wallPrefab;

    // ------------------------------------------------------
    // References
    // ------------------------------------------------------
    [Header("References")]
    public BaseCaseManager baseCaseManager;
    public Transform basePlaneTransform;

    // ------------------------------------------------------
    // Data
    // ------------------------------------------------------
    private readonly List<Transform> allFloors = new List<Transform>();

    public readonly List<FloorTileInfo> allTiles = new List<FloorTileInfo>();

    private readonly List<Transform> allWalls = new List<Transform>();
    private readonly Stack<Transform> undoStack = new Stack<Transform>();

    // ------------------------------------------------------
    // ✅ 給 BFS 用的 BasePlane 代理 Tile (grid 0,0)【唯一一顆】
    // ------------------------------------------------------

    public List<Transform> GetAllWalls()
    {
        return allWalls;
    }

    private Camera cam;

    private float stepX = 1f;
    private float stepZ = 1f;

    private const float FLOOR_THICKNESS = 0.02f;
    private const float WALL_THICKNESS = 0.02f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = Camera.main;

        if (undoButton != null)
            undoButton.SetActive(false);
        if (switchModeButton != null)
            switchModeButton.SetActive(false);
        if (finishBuildButton != null)
            finishBuildButton.SetActive(false);
    }

    // ------------------------------------------------------
    // Build Mode
    // ------------------------------------------------------
    public void OnEnterBuildMode()
    {
        StartCoroutine(EnableButtonsNextFrame());
    }

    private IEnumerator EnableButtonsNextFrame()
    {
        yield return null;

        if (undoButton != null)
            undoButton.SetActive(true);
        if (switchModeButton != null)
            switchModeButton.SetActive(true);
        if (finishBuildButton != null)
            finishBuildButton.SetActive(true);

        ARBuildManager.Instance.currentBuildMode =
            ARBuildManager.BuildMode.Floor;

        Debug.Log("[BuildMode] Enter Build Mode. Default = Floor.");
    }

    // ------------------------------------------------------
    // Base Plane
    // ------------------------------------------------------
    public void SetBasePlane(Transform baseTf)
    {
        basePlaneTransform = baseTf;
        CacheBasePlaneWorldSize();

        var info = baseTf.GetComponent<FloorTileInfo>();
        if (info == null)
            info = baseTf.gameObject.AddComponent<FloorTileInfo>();

        info.gridX = 0;
        info.gridY = 0;
        info.occupied = OccupyType.Floor;

        if (!allTiles.Contains(info))
            allTiles.Add(info);

        if (!allFloors.Contains(baseTf))
            allFloors.Insert(0, baseTf);

        Debug.Log("[GRID] BasePlane registered as first FloorTile (0,0)");
    }


    private void CacheBasePlaneWorldSize()
    {
        stepX = basePlaneTransform.lossyScale.x;
        stepZ = basePlaneTransform.lossyScale.z;
    }

    public void UpdateBasePlaneScale(Transform baseTf)
    {
        if (baseTf == null) return;

        basePlaneTransform = baseTf;
        CacheBasePlaneWorldSize();

        // 確保基準面有被放在 allFloors 裡（避免重新偵測時遺失）
        if (!allFloors.Contains(baseTf))
        {
            allFloors.Insert(0, baseTf);
        }

    }

    // ------------------------------------------------------
    // ✅ 只負責維護「BFS 用的原點 Tile」
    // ------------------------------------------------------

    // ------------------------------------------------------
    // UI / Touch Filtering (new InputSystem safe method)
    // ------------------------------------------------------
    [Header("UI Blocker")]
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    private bool IsPointerOverUI(Vector2 screenPos)
    {
        if (uiRaycaster == null || eventSystem == null)
            return false;

        var data = new PointerEventData(eventSystem);
        data.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(data, results);

        return results.Count > 0;
    }

    // ------------------------------------------------------
    // Update Loop
    // ------------------------------------------------------
    private void Update()
    {
        if (!ARBuildManager.Instance.isBuildMode)
            return;

        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.wasReleasedThisFrame)
            return;

        Vector2 screenPos = touch.position.ReadValue();

        // UI 阻擋
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;   // 點到 UI → 不建造
        }

        if (IsPointerOverUI(screenPos))
            return;

        // 建造流程
        switch (ARBuildManager.Instance.currentBuildMode)
        {
            case ARBuildManager.BuildMode.Floor:
                TryPlaceFloor(screenPos);
                break;

            case ARBuildManager.BuildMode.Wall:
                TryPlaceWall(screenPos);
                break;
        }
    }

    // ======================================================
    // Floor Build
    // ======================================================
    private void TryPlaceFloor(Vector2 screenPos)
    {
        if (basePlaneTransform == null) return;

        if (!TryGetPointOnBuildPlane(screenPos, out Vector3 hitOnPlane))
            return;

        Transform pivotTile = FindNearestTile(hitOnPlane);
        if (pivotTile == null) return;

        Vector3 dir = GetBuildDirection(pivotTile, hitOnPlane);

        float step = (Mathf.Abs(Vector3.Dot(dir, basePlaneTransform.right)) > 0.5f)
                    ? stepX
                    : stepZ;

        Vector3 newPos = pivotTile.position + dir.normalized * step;
        newPos.y = basePlaneTransform.position.y;

        if (CheckFloorOverlap(newPos)) return;

        GameObject tile = Instantiate(
            floorPrefab, newPos,
            basePlaneTransform.rotation, floorParent
        );

        tile.transform.localScale = basePlaneTransform.localScale;

        // ============================
        // Grid 座標設定（沿用你原本的作法）
        // ============================
        var pivotInfo = pivotTile.GetComponent<FloorTileInfo>();
        var newInfo = tile.GetComponent<FloorTileInfo>();

        // ✅ NEW：註冊動態 floor tile
        if (newInfo != null && !allTiles.Contains(newInfo))
        {
            allTiles.Add(newInfo);
        }


        if (pivotInfo != null && newInfo != null)
        {
            if (Mathf.Abs(Vector3.Dot(dir, basePlaneTransform.right)) > 0.5f)
            {
                newInfo.gridX = pivotInfo.gridX + (dir.x > 0 ? 1 : -1);
                newInfo.gridY = pivotInfo.gridY;
            }
            else
            {
                newInfo.gridX = pivotInfo.gridX;
                newInfo.gridY = pivotInfo.gridY + (dir.z > 0 ? 1 : -1);
            }
        }

        allFloors.Add(tile.transform);
        undoStack.Push(tile.transform);
    }

    private bool TryGetPointOnBuildPlane(Vector2 screenPos, out Vector3 worldPoint)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(basePlaneTransform.up, basePlaneTransform.position);

        if (plane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        worldPoint = Vector3.zero;
        return false;
    }

    private Transform FindNearestTile(Vector3 worldPos)
    {
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (Transform t in allFloors)
        {
            float d = Vector3.Distance(worldPos, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        return best;
    }

    private bool CheckFloorOverlap(Vector3 pos)
    {
        foreach (var f in allFloors)
        {
            if (Vector3.Distance(f.position, pos) < 0.01f)
                return true;
        }
        return false;
    }

    private Vector3 GetBuildDirection(Transform pivot, Vector3 hit)
    {
        Vector3 local = pivot.InverseTransformPoint(hit);
        local.y = 0;

        if (Mathf.Abs(local.x) > Mathf.Abs(local.z))
            return local.x > 0 ? pivot.right : -pivot.right;
        else
            return local.z > 0 ? pivot.forward : -pivot.forward;
    }

    // ======================================================
    // Wall Build（世界投影版）
    // ======================================================
    private void TryPlaceWall(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (!FindNearestSurfaceAlongRay(ray, out Transform pivot, out bool isFloor, out Vector3 hit))
            return;

        if (isFloor)
            TryPlaceWallFromFloor(pivot, hit);
        else
            TryPlaceWallFromWall(pivot, hit);
    }

    // ======================================================
    // 🔥 牆優先
    // ======================================================
    private bool FindNearestSurfaceAlongRay(
        Ray ray,
        out Transform pivot,
        out bool isFloor,
        out Vector3 hitPoint)
    {
        pivot = null;
        isFloor = true;
        hitPoint = Vector3.zero;

        float bestEnter = float.MaxValue;
        const float padding = 0.0001f;

        // 1) Walls
        foreach (var w in allWalls)
        {
            Plane plane = new Plane(w.forward, w.position);

            if (!plane.Raycast(ray, out float enter)) continue;
            if (enter <= 0f) continue;

            Vector3 h = ray.GetPoint(enter);
            Vector3 local = w.InverseTransformPoint(h);

            float halfX = w.localScale.x * 0.5f + padding;
            float halfY = w.localScale.y * 0.5f + padding;

            if (Mathf.Abs(local.x) > halfX || Mathf.Abs(local.y) > halfY)
                continue;

            if (enter < bestEnter)
            {
                bestEnter = enter;
                pivot = w;
                isFloor = false;
                hitPoint = h;
            }
        }

        // 2) Floors
        foreach (var f in allFloors)
        {
            Plane plane = new Plane(f.up, f.position);

            if (!plane.Raycast(ray, out float enter)) continue;
            if (enter <= 0f) continue;

            Vector3 h = ray.GetPoint(enter);
            Vector3 local = f.InverseTransformPoint(h);

            float halfX = f.localScale.x * 0.5f + padding;
            float halfZ = f.localScale.z * 0.5f + padding;

            if (Mathf.Abs(local.x) > halfX || Mathf.Abs(local.z) > halfZ)
                continue;

            if (enter < bestEnter)
            {
                bestEnter = enter;
                pivot = f;
                isFloor = true;
                hitPoint = h;
            }
        }

        return pivot != null;
    }

    private bool CheckWallOverlap(Vector3 pos)
    {
        foreach (var w in allWalls)
        {
            if (Vector3.Distance(w.position, pos) < 0.01f)
                return true;
        }
        return false;
    }

    // Floor → Wall
    private void TryPlaceWallFromFloor(Transform floor, Vector3 hit)
    {
        Vector3 dir = GetBuildDirection(floor, hit);

        float tileWidth = floor.localScale.x;
        float tileDepth = floor.localScale.z;

        float step = Mathf.Abs(Vector3.Dot(dir, floor.right)) > 0.5f
            ? tileWidth
            : tileDepth;

        Vector3 wallPos =
            floor.position +
            dir.normalized * (step * 0.5f + WALL_THICKNESS * 0.5f);

        Quaternion rot =
            Quaternion.LookRotation(dir, basePlaneTransform.up);

        Vector3 scale = new Vector3(
            floor.localScale.x,
            floor.localScale.z,
            WALL_THICKNESS
        );

        wallPos.y = basePlaneTransform.position.y + scale.y * 0.5f;

        if (CheckWallOverlap(wallPos)) return;

        GameObject wallObj = Instantiate(wallPrefab, wallPos, rot, wallParent);
        wallObj.transform.localScale = scale;

        allWalls.Add(wallObj.transform);
        undoStack.Push(wallObj.transform);

        // ============================
        // ✅ NEW：牆阻擋 BFS 邏輯
        // ============================
        FloorTileInfo floorA = floor.GetComponent<FloorTileInfo>();
        FloorTileInfo floorB = null;

        foreach (var t in allFloors)
        {
            if (Vector3.Distance(t.position, wallPos + dir.normalized * step * 0.5f) < 0.01f)
            {
                floorB = t.GetComponent<FloorTileInfo>();
                break;
            }
        }

        RegisterWallBlockBetweenTiles(floorA, floorB);
    }

    // Wall → Wall
    private enum WallSide { Left, Right, Up, Down }

    private WallSide GetWallSide(Transform wall, Vector3 hit)
    {
        Vector3 local = wall.InverseTransformPoint(hit);

        float ax = Mathf.Abs(local.x);
        float ay = Mathf.Abs(local.y);

        if (ay > ax)
            return local.y > 0 ? WallSide.Up : WallSide.Down;
        else
            return local.x > 0 ? WallSide.Right : WallSide.Left;
    }

    private void TryPlaceWallFromWall(Transform pivotWall, Vector3 hit)
    {
        Vector3 pos = pivotWall.position;
        Vector3 scale = pivotWall.localScale;

        WallSide side = GetWallSide(pivotWall, hit);

        switch (side)
        {
            case WallSide.Right:
                pos += pivotWall.right * scale.x;
                break;

            case WallSide.Left:
                pos += -pivotWall.right * scale.x;
                break;

            case WallSide.Up:
                pos += pivotWall.up * scale.y;
                break;

            case WallSide.Down:
                {
                    float halfH = scale.y * 0.5f;

                    Vector3 cand = new Vector3(
                        pos.x,
                        pos.y - scale.y,
                        pos.z
                    );

                    float newBottom = cand.y - halfH;
                    float baseY = basePlaneTransform.position.y;
                    if (newBottom < baseY - 0.01f)
                        return;

                    pos = cand;
                    break;
                }
        }

        if (CheckWallOverlap(pos)) return;

        GameObject wallObj = Instantiate(wallPrefab, pos, pivotWall.rotation, wallParent);
        wallObj.transform.localScale = scale;

        allWalls.Add(wallObj.transform);
        undoStack.Push(wallObj.transform);
    }

    // ======================================================
    // Undo
    // ======================================================
    public void OnUndoButtonPressed()
    {
        if (undoStack.Count == 0) return;

        Transform last = undoStack.Pop();

        allFloors.Remove(last);
        allWalls.Remove(last);

        Destroy(last.gameObject);
    }

    public void ExitBuildMode()
    {
        Debug.Log("[FloorBuild] ExitBuildMode called");

        if (undoButton != null)
            undoButton.SetActive(false);

        if (switchModeButton != null)
            switchModeButton.SetActive(false);

        if (finishBuildButton != null)
            finishBuildButton.SetActive(false);

        ARBuildManager.Instance.isBuildMode = false;

        Debug.Log("[FloorBuild] Build Mode disabled");

        TowerDefenseUIManager.Instance.EnableCreateButton();
    }

    // ======================================================
    // ✅ NEW：在兩個 FloorTile 之間標記牆阻擋
    // ======================================================
    private void RegisterWallBlockBetweenTiles(
        FloorTileInfo a,
        FloorTileInfo b)
    {
        if (a == null || b == null)
            return;

        int dx = b.gridX - a.gridX;
        int dy = b.gridY - a.gridY;

        if (dx == 1 && dy == 0)
        {
            a.blockEast = true;
            b.blockWest = true;
        }
        else if (dx == -1 && dy == 0)
        {
            a.blockWest = true;
            b.blockEast = true;
        }
        else if (dx == 0 && dy == 1)
        {
            a.blockNorth = true;
            b.blockSouth = true;
        }
        else if (dx == 0 && dy == -1)
        {
            a.blockSouth = true;
            b.blockNorth = true;
        }
    }


    private WallDirection DirToWallDirection(Vector3 worldDir)
    {
        worldDir = Vector3.ProjectOnPlane(worldDir, basePlaneTransform.up).normalized;

        if (Vector3.Dot(worldDir, basePlaneTransform.forward) > 0.7f) return WallDirection.North;
        if (Vector3.Dot(worldDir, -basePlaneTransform.forward) > 0.7f) return WallDirection.South;
        if (Vector3.Dot(worldDir, basePlaneTransform.right) > 0.7f) return WallDirection.East;
        return WallDirection.West;
    }

    private FloorTileInfo FindTileByGrid(int gx, int gy)
    {
        foreach (var t in allTiles)
        {
            if (t != null && t.gridX == gx && t.gridY == gy) return t;
        }

        // basePlane 也可能不在 allTiles？你有加進去了，但保險
        if (basePlaneTransform != null)
        {
            var baseInfo = basePlaneTransform.GetComponent<FloorTileInfo>();
            if (baseInfo != null && baseInfo.gridX == gx && baseInfo.gridY == gy)
                return baseInfo;
        }
        return null;
    }

}
