using UnityEngine;

public enum OccupyType
{
    None,
    Spawn,
    Goal,
    Floor,
    Wall
}

public enum WallDirection
{
    North,
    South,
    East,
    West
}

public class FloorTileInfo : MonoBehaviour
{
    [Header("Occupy Info")]
    public OccupyType occupied = OccupyType.Floor;

    [Header("Grid Position")]
    public int gridX;
    public int gridY;

    // ======================================================
    // ✅ NEW：牆阻擋資訊（只影響「移動方向」，不影響 Walkable）
    // ======================================================
    [Header("Wall Block Flags")]
    public bool blockNorth;
    public bool blockSouth;
    public bool blockEast;
    public bool blockWest;

    public bool Walkable
    {
        get
        {
            return occupied != OccupyType.None && occupied != OccupyType.Wall;
        }
    }

    // ======================================================
    // ✅ NEW：查詢「往某 tile 是否被牆擋」
    // ======================================================
    public bool IsBlockedTowards(FloorTileInfo other)
    {
        int dx = other.gridX - gridX;
        int dy = other.gridY - gridY;

        if (dx == 1 && dy == 0) return blockEast;
        if (dx == -1 && dy == 0) return blockWest;
        if (dx == 0 && dy == 1) return blockNorth;
        if (dx == 0 && dy == -1) return blockSouth;

        return false;
    }

    public Vector3 WorldCenter
    {
        get
        {
            Renderer r = GetComponentInChildren<Renderer>();
            if (r == null)
                return transform.position;

            return new Vector3(
                r.bounds.center.x,
                r.bounds.max.y,
                r.bounds.center.z
            );
        }
    }
}
