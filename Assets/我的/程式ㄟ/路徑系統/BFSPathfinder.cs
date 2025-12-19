using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ✅ 純 Tile BFS 轉接器
/// - 不再使用世界座標
/// - 不再使用 Raycast
/// - 不再使用 tileLayer
/// - 唯一資料來源：FloorTileInfo
/// </summary>
public class BFSPathfinder : IPathfinder
{
    public List<PathNode> FindPath(PathNode start, PathNode goal)
    {
        Debug.Log("BFSPathfinder.FindPath called");

        // ---------------------------
        // ✅ 防呆檢查
        // ---------------------------
        if (start == null || goal == null)
        {
            Debug.LogWarning("BFSPathfinder aborted: start or goal node is null");
            return null;
        }

        if (start.tile == null || goal.tile == null)
        {
            Debug.LogWarning("BFSPathfinder aborted: start.tile or goal.tile is null");
            return null;
        }

        FloorTileInfo startTile = start.tile;
        FloorTileInfo goalTile = goal.tile;

        Debug.Log($"[BFS] StartTile = {startTile.name}, GoalTile = {goalTile.name}");

        // ---------------------------
        // ✅ 只從官方邏輯層取得 Tile
        // ---------------------------
        FloorBuildManager fbm = FloorBuildManager.Instance;

        if (fbm == null || fbm.floorParent == null)
        {
            Debug.LogError("[BFS] FloorBuildManager or floorParent is null");
            return null;
        }

        List<FloorTileInfo> tiles = new List<FloorTileInfo>(
            fbm.floorParent.GetComponentsInChildren<FloorTileInfo>()
        );

        // 🔥 補 BasePlane Tile
        if (fbm.basePlaneTransform != null)
        {
            var baseTile = fbm.basePlaneTransform.GetComponent<FloorTileInfo>();
            if (baseTile != null && !tiles.Contains(baseTile))
            {
                tiles.Add(baseTile);
                Debug.Log("[BFS] BasePlane tile added to tile list");
            }
        }

        Debug.Log("[BFS] Official Tile count = " + tiles.Count);

        // ✅✅✅ 新增：完整列出每一顆 tile 的來源
        Debug.Log("[BFS] ---- TILE LIST START ----");

        foreach (var t in tiles)
        {
            Debug.Log(
                $"[BFS] Tile = {t.name} | Grid = ({t.gridX}, {t.gridY}) | Parent = {t.transform.parent.name}"
            );
        }

        Debug.Log("[BFS] ---- TILE LIST END ----");

        // ---------------------------
        // ✅ 建立新版 WorldSpacePathfinder（只吃 tiles）
        // ---------------------------
        var worldPathfinder = new WorldSpacePathfinder(tiles);

        Debug.Log("[BFS] Delegating to WorldSpacePathfinder.FindPath(startTile, goalTile)");

        // ✅ 核心：不再用世界座標
        List<PathNode> nodePath = worldPathfinder.FindPath(startTile, goalTile);

        if (nodePath == null)
        {
            Debug.LogWarning("[BFS]  Received null path result");
        }
        else
        {
            Debug.Log("[BFS]  Path received. Node count = " + nodePath.Count);
        }

        return nodePath;
    }
}
