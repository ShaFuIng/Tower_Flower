using System.Collections.Generic;
using UnityEngine;

public class WorldSpacePathfinder
{
    private readonly List<FloorTileInfo> tiles;

    // ✅ NEW：用 grid 快速查找 tile
    private readonly Dictionary<Vector2Int, FloorTileInfo> gridMap = new();

    public WorldSpacePathfinder(List<FloorTileInfo> tiles)
    {
        this.tiles = tiles;

        // ✅ 建立 gridMap（後面找鄰居不必掃整個 tiles）
        gridMap.Clear();
        if (tiles != null)
        {
            foreach (var t in tiles)
            {
                if (t == null) continue;
                gridMap[new Vector2Int(t.gridX, t.gridY)] = t;
            }
        }
    }

    // ============================
    // ✅ 主入口：吃 Grid Tile
    // ============================
    public List<PathNode> FindPath(FloorTileInfo startTile, FloorTileInfo goalTile)
    {
        if (startTile == null || goalTile == null)
        {
            Debug.LogWarning("[PATH] Start or Goal is NULL");
            return null;
        }

        // ✅ 防呆：起點/終點不可走就直接 fail（避免你以為 BFS 壞了）
        if (!startTile.Walkable || !goalTile.Walkable)
        {
            Debug.LogWarning($"[PATH] Start or Goal not walkable. StartWalkable={startTile.Walkable}, GoalWalkable={goalTile.Walkable}");
            return null;
        }

        return BFS(startTile, goalTile);
    }

    // ============================
    // ✅ Grid BFS 主體
    // ============================
    private List<PathNode> BFS(FloorTileInfo start, FloorTileInfo goal)
    {
        Queue<FloorTileInfo> queue = new();
        Dictionary<FloorTileInfo, FloorTileInfo> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = null;

        Debug.Log("========== [GRID DEBUG START] ==========");

        Debug.Log($"[GRID] Start = {start.name} ({start.gridX},{start.gridY}) Walkable={start.Walkable} " +
                  $"Blocks(N,S,E,W)=({start.blockNorth},{start.blockSouth},{start.blockEast},{start.blockWest})");

        Debug.Log($"[GRID] Goal  = {goal.name} ({goal.gridX},{goal.gridY}) Walkable={goal.Walkable} " +
                  $"Blocks(N,S,E,W)=({goal.blockNorth},{goal.blockSouth},{goal.blockEast},{goal.blockWest})");

        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                if (tile == null) continue;
                Debug.Log($"[GRID] Tile {tile.name} => ({tile.gridX},{tile.gridY}) Walkable={tile.Walkable} " +
                          $"Blocks(N,S,E,W)=({tile.blockNorth},{tile.blockSouth},{tile.blockEast},{tile.blockWest})");
            }
        }

        Debug.Log("========== [GRID DEBUG END] ==========");

        // ✅ 正式 BFS 開始
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            Debug.Log($"[GRID] Current = {current.name} ({current.gridX},{current.gridY})");

            if (current == goal)
            {
                Debug.Log("[PATH] Find Goal，start reback");
                return ReconstructPath(cameFrom, goal);
            }

            foreach (var neighbour in FindGridNeighbours(current))
            {
                if (neighbour == null) continue;

                bool edgeBlocked = IsEdgeBlocked(current, neighbour);

                Debug.Log($"[GRID]   Neighbour {neighbour.name} ({neighbour.gridX},{neighbour.gridY}) " +
                          $"Walkable={neighbour.Walkable} EdgeBlocked={edgeBlocked}" +
                          (edgeBlocked ? 
                              $" (blocked: current.IsBlockedTowards={current.IsBlockedTowards(neighbour)}, " +
                              $"neighbour.IsBlockedTowards={neighbour.IsBlockedTowards(current)})" : ""));

                if (!cameFrom.ContainsKey(neighbour) && neighbour.Walkable && !edgeBlocked)
                {
                    cameFrom[neighbour] = current;
                    queue.Enqueue(neighbour);
                }   
                else
                {
                    Debug.Log($"[GRID]   Skip {neighbour.name} (reason: " +
                              $"{(cameFrom.ContainsKey(neighbour) ? "visited " : "")}" +
                              $"{(!neighbour.Walkable ? "not-walkable " : "")}" +
                              $"{(edgeBlocked ? "blocked-by-wall " : "")})");
                }
            }
        }

        Debug.LogWarning("[PATH]  BFS not find");
        return null;
    }

    // ============================
    // ✅ NEW：雙向牆阻擋判定（很重要）
    // ============================
    private bool IsEdgeBlocked(FloorTileInfo a, FloorTileInfo b)
    {
        // ✅ 雙向都檢查：只要任一邊說「被擋」就算被擋
        // （避免你只在其中一顆 tile 設 block flag，BFS 還是鑽過去）
        return a.IsBlockedTowards(b) || b.IsBlockedTowards(a);
    }

    // ============================
    // ✅ Grid 鄰居搜尋（只看四向，直接用 gridMap 找）
    // ============================
    private List<FloorTileInfo> FindGridNeighbours(FloorTileInfo from)
    {
        List<FloorTileInfo> neighbours = new();

        int x = from.gridX;
        int y = from.gridY;

        // 右、左、上、下（dx,dy）
        TryAdd(neighbours, x + 1, y);
        TryAdd(neighbours, x - 1, y);
        TryAdd(neighbours, x, y + 1);
        TryAdd(neighbours, x, y - 1);

        return neighbours;
    }

    private void TryAdd(List<FloorTileInfo> neighbours, int x, int y)
    {
        if (gridMap.TryGetValue(new Vector2Int(x, y), out var t) && t != null)
            neighbours.Add(t);
    }

    // ============================
    // ✅ 回推路徑
    // ============================
    private List<PathNode> ReconstructPath(
        Dictionary<FloorTileInfo, FloorTileInfo> cameFrom,
        FloorTileInfo end)
    {
        List<PathNode> path = new();

        var current = end;
        while (current != null)
        {
            path.Add(new PathNode(current));
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }
}
