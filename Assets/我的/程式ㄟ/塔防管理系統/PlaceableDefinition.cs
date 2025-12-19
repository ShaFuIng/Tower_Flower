using UnityEngine;
using static IconDragHandler;

public enum PlacementSurface
{
    Floor,
    Wall
}
public class PlaceableDefinition : MonoBehaviour
{

    [Header("Placement")]
    public PlacementSurface surface;

    [Header("Prefabs")]
    public GameObject placePrefab;     // 真正放下去的
    public GameObject previewPrefab;   // 預覽用（可為 null）

    [Header("Rules")]
    public int maxCount = -1;           // -1 = 不限制
    public OccupyType occupyType;       // 放下去 tile 變成什麼

    [Header("Cost")]
    public int cost = 100;              // Cost to place this object (from tower database level 1)
}
