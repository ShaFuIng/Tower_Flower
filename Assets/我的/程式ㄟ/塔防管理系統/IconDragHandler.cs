using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconDragHandler : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public ScrollRect parentScrollRect;

    private bool pointerDownOnIcon = false;
    private bool isDraggingPreview = false;

    private PlaceableDefinition definition;

    private void Awake()
    {
        // 🔑 每個 icon button 自己知道「我是什麼可放置物」
        definition = GetComponent<PlaceableDefinition>();

        if (definition == null)
        {
            Debug.LogError(
                $"[IconDragHandler] PlaceableDefinition missing on {gameObject.name}"
            );
        }
    }

    // 1️⃣ Pointer Down：只要點到自己或子物件，就視為點到 icon
    public void OnPointerDown(PointerEventData eventData)
    {
        var hitObj = eventData.pointerCurrentRaycast.gameObject;

        if (hitObj == null)
        {
            pointerDownOnIcon = false;
            return;
        }

        pointerDownOnIcon = hitObj.transform.IsChildOf(transform);
    }

    // 2️⃣ Begin Drag：決定是滾動 ScrollRect 還是進入預覽模式
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!pointerDownOnIcon || definition == null)
        {
            parentScrollRect?.OnBeginDrag(eventData);
            return;
        }

        // ✅ B 方案：直接把 definition 交出去
        SpawnGoalPlacementManager.Instance.BeginPreview(definition);
        isDraggingPreview = true;
    }

    // 3️⃣ Drag：預覽模式不處理拖曳，位置由 Manager.Update 控制
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingPreview)
        {
            parentScrollRect?.OnDrag(eventData);
            return;
        }
    }

    // 4️⃣ End Drag：結束預覽或交回 ScrollRect
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingPreview)
        {
            parentScrollRect?.OnEndDrag(eventData);
            pointerDownOnIcon = false;
            return;
        }

        SpawnGoalPlacementManager.Instance.EndPreview();

        isDraggingPreview = false;
        pointerDownOnIcon = false;
    }
}
