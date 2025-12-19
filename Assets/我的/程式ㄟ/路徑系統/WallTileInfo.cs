using UnityEngine;

public class WallTileInfo : MonoBehaviour
{
    public FloorTileInfo ownerFloor;   // 這片牆屬於哪一格 floor
    public WallDirection dir;          // North/South/East/West
    public int level = 1;              // 第幾層（從 1 開始：貼地那層 = 1）
}