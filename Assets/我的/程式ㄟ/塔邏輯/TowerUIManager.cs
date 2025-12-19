using UnityEngine;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TowerInfoPanelController infoPanel;

    [Header("Overlay (click to close Tower UI)")]
    [SerializeField] private GameObject overlay;   // MenuOverlay
    [SerializeField] private Button overlayButton; // MenuOverlay 上的 Button

    public bool IsVisible => infoPanel != null && infoPanel.IsOpen;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (overlay != null) overlay.SetActive(false);

        // ✅ 如果你沒拖 overlayButton，也幫你自動抓
        if (overlayButton == null && overlay != null)
            overlayButton = overlay.GetComponent<Button>();

        if (overlayButton != null)
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(Hide);
        }
    }

    public void ShowTowerPreview(string towerId)
    {
        if (infoPanel == null) return;
        ShowOverlay(true);
        infoPanel.ShowPreview(towerId);
    }

    public void ShowTowerInstance(string towerId, int level)
    {
        if (infoPanel == null) return;
        ShowOverlay(true);
        infoPanel.ShowInstance(towerId, level);
    }

    public void Hide()
    {
        if (infoPanel != null) infoPanel.Hide();
        ShowOverlay(false);
    }

    private void ShowOverlay(bool on)
    {
        if (overlay != null) overlay.SetActive(on);
    }
}
