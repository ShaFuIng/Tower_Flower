using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;

    public ArrowPathRenderer arrowRenderer;
    public GameObject simulateMonsterButton;

    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("Monster Simulation")]
    public GameObject monsterPrefab;
    public Transform monsterParent;

    private GameObject currentMonsterInstance;
    private List<PathNode> cachedPathNodes;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetPathPreview()
    {
        if (currentMonsterInstance != null)
            Destroy(currentMonsterInstance);

        arrowRenderer?.Clear();
        simulateMonsterButton.SetActive(false);
        cachedPathNodes = null;

        TowerDefenseUIManager.Instance?.HideResetPathButton();
        TowerDefenseUIManager.Instance?.HideGameStartButton();
    }

    public void ComputePath()
    {
        StartCoroutine(ComputeCoroutine());
    }

    private IEnumerator ComputeCoroutine()
    {
        loadingPanel.SetActive(true);
        loadingSlider.value = 0;

        for (int i = 0; i <= 20; i++)
        {
            loadingSlider.value = i / 20f;
            yield return new WaitForSeconds(0.02f);
        }

        // ⭐ 改成 OccupyType 檢查
        if (!SpawnGoalPlacementManager.Instance.HasOccupyType(OccupyType.Spawn) ||
            !SpawnGoalPlacementManager.Instance.HasOccupyType(OccupyType.Goal))
        {
            loadingPanel.SetActive(false);
            yield break;
        }

        var fbm = FloorBuildManager.Instance;

        // 1️⃣ 先拿動態 floor tiles
        List<FloorTileInfo> tiles = new(
            fbm.floorParent.GetComponentsInChildren<FloorTileInfo>()
        );

        // 2️⃣ 補 BasePlane 本身（關鍵）
        if (fbm.basePlaneTransform != null)
        {
            var baseTile = fbm.basePlaneTransform.GetComponent<FloorTileInfo>();
            if (baseTile != null && !tiles.Contains(baseTile))
            {
                tiles.Add(baseTile);
                Debug.Log("[Pathfinding] BasePlane tile added to BFS tiles");
            }
        }

        FloorTileInfo startTile =
            SpawnGoalPlacementManager.Instance.GetFirstTileByOccupyType(OccupyType.Spawn);

        FloorTileInfo goalTile =
            SpawnGoalPlacementManager.Instance.GetFirstTileByOccupyType(OccupyType.Goal);

        var pathfinder = new WorldSpacePathfinder(tiles);
        cachedPathNodes = pathfinder.FindPath(startTile, goalTile);

        if (cachedPathNodes == null)
        {
            arrowRenderer.Clear();
            simulateMonsterButton.SetActive(false);
        }
        else
        {
            List<Vector3> pts = new();
            foreach (var n in cachedPathNodes)
                pts.Add(n.WorldCenter);

            arrowRenderer.ShowWorldPoints(pts);
            simulateMonsterButton.SetActive(true);
        }

        loadingPanel.SetActive(false);
        TowerDefenseUIManager.Instance?.ShowResetPathButton();
    }

    public void SimulateMonsterMovement()
    {
        if (cachedPathNodes == null || cachedPathNodes.Count < 2)
            return;

        if (currentMonsterInstance != null)
            Destroy(currentMonsterInstance);

        List<Vector3> path = new();
        foreach (var n in cachedPathNodes)
            path.Add(n.WorldCenter);


        currentMonsterInstance = Instantiate(
            monsterPrefab,
            path[0],
            Quaternion.identity
        );

        // 🔒 一開始先關掉 Renderer（所有子物件）
        foreach (var r in currentMonsterInstance.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        var movement = currentMonsterInstance.GetComponent<MonsterMovement>();

        // ✅ 關鍵：用起點 tile 設定視覺大小
        FloorTileInfo startTile =
            SpawnGoalPlacementManager.Instance.GetFirstTileByOccupyType(OccupyType.Spawn);

        movement.ApplySizeFromTile(startTile);

        movement.ApplySpeedFromTile(startTile); // ✅ 新增這行

        movement.SetPath(path);

        // ✅✅✅ 你漏掉的就是這行
        movement.ShowAfterScale();

        arrowRenderer.Clear();

        TowerDefenseUIManager.Instance?.ShowResetPathButton();
        TowerDefenseUIManager.Instance?.ShowGameStartButton();
    }
}
