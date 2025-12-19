using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SelectablePlacedObject : MonoBehaviour, IPointerDownHandler
{
    [Header("Runtime Binding")]
    public FloorTileInfo tile;
    public PlaceableDefinition definition;   // ✅ 保留：系統會用到
    public bool isTower = true;              // Spawn/Goal 請設 false
    public WallDirection? wallDirection;     // 牆塔用

    public void OnPointerDown(PointerEventData eventData)
    {
        // ✅ 只做「選取」：讓 XX 出現 / 丟給管理器統一決策
        SpawnGoalPlacementManager.Instance?.SelectObject(this);

        eventData.Use();
    }
}
