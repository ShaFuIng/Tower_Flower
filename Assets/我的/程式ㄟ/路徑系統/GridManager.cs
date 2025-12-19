using UnityEngine;

/// <summary>
/// 在純 A 方案下，GridManager 不再負責「格子座標 / PathNode[,]」。
/// 這裡只保留一個 Singleton 殼與相容用的空方法，
/// 目前的路徑規劃完全由 WorldSpacePathfinder / PathfindingManager 負責。
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;
        Debug.Log("[GridManager] World-space pathfinding is active. Grid-based APIs are deprecated.");
    }

    /// <summary>
    /// 兼容舊程式碼的空實作。
    /// 在純 A 方案中，Tile 不再需要註冊到格子系統。
    /// </summary>
    public void RegisterTile(FloorTileInfo tile)
    {
        // 不再做任何事，只保留介面以避免其他腳本爆錯。
    }

    /// <summary>
    /// 兼容舊程式碼的空實作。
    /// </summary>
    public void UnregisterTile(FloorTileInfo tile)
    {
        // 不再做任何事。
    }
}
