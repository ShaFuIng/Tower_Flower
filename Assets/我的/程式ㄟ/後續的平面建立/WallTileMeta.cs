using UnityEngine;

public class WallTileMeta : MonoBehaviour
{
    public FloorTileInfo ownerFloor;     // 這片牆屬於哪一顆 floor
    public WallDirection dir;            // 這片牆是 floor 的哪一面
    public int heightLevel = 1;          // 這片牆是第幾層（1,2,3...）
}
