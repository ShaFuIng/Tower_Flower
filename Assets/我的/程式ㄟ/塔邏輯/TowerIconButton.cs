using UnityEngine;
using UnityEngine.UI;

public class TowerIconButton : MonoBehaviour
{
    [SerializeField] private string towerId; // 這個要跟 towers.json 的 id 一樣

    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        TowerUIManager.Instance?.ShowTowerPreview(towerId);
    }
}
