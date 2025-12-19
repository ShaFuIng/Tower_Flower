using UnityEngine;

public class PathNode
{
    // 對應的實體 Tile
    public FloorTileInfo tile;

    // --- BFS / 評分用資料 ---
    public int stepCount;                 // 從起點走到這裡幾步
    public float distanceFromStart;       // 世界距離（未來可用）
    public float totalScore;              // 總評分（未來 A* 用）

    // 父節點（回溯路徑）
    public PathNode parent;

    public PathNode(FloorTileInfo tile)
    {
        this.tile = tile;
        Reset();
    }

    public Vector3 WorldCenter => tile != null ? tile.WorldCenter : Vector3.zero;
    public bool Walkable => tile != null && tile.Walkable;

    public void Reset()
    {
        stepCount = 0;
        distanceFromStart = 0f;
        totalScore = 0f;
        parent = null;
    }
}
