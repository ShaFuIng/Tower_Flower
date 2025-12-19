using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerInfoPanelController : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Panels (Only 3)")]
    public GameObject previewPanel;   // Icon 用
    public GameObject currentPanel;   // 場上塔 Current
    public GameObject nextPanel;      // 場上塔 Next

    // =========================
    // PreviewPanel (Icon 用) - Basic Info + Stats
    // =========================
    [Header("Preview Panel - Basic Info (Icon 用)")]
    public TextMeshProUGUI previewName;
    public TextMeshProUGUI previewType;
    public TextMeshProUGUI previewDescription;

    [Header("Preview Panel - Stats (Icon 用)")]
    public TextMeshProUGUI previewCost;
    public TextMeshProUGUI previewDamage;
    public TextMeshProUGUI previewCooldown;
    public TextMeshProUGUI previewRange;

    // =========================
    // CurrentPanel (Instance Current)
    // =========================
    [Header("Current Panel - Stats (Instance Current)")]
    public TextMeshProUGUI currentCost;
    public TextMeshProUGUI currentDamage;
    public TextMeshProUGUI currentCooldown;
    public TextMeshProUGUI currentRange;

    // =========================
    // NextPanel (Instance Next)
    // =========================
    [Header("Next Panel - Stats (Instance Next)")]
    public TextMeshProUGUI nextCost;
    public TextMeshProUGUI nextDamage;
    public TextMeshProUGUI nextCooldown;
    public TextMeshProUGUI nextRange;

    [Header("Upgrade (Optional)")]
    public Button upgradeButton;

    public bool IsOpen => root != null && root.activeSelf;

    private void SetAllPanelsOff()
    {
        if (previewPanel) previewPanel.SetActive(false);
        if (currentPanel) currentPanel.SetActive(false);
        if (nextPanel) nextPanel.SetActive(false);
        if (upgradeButton) upgradeButton.gameObject.SetActive(false);
    }

    // =========================
    // Icon：只開 PreviewPanel（顯示 Name/Type/Desc + Lv1 stats）
    // =========================
    public void ShowPreview(string towerId)
    {
        if (root == null) return;
        root.SetActive(true);

        SetAllPanelsOff();
        if (previewPanel) previewPanel.SetActive(true);

        if (TowerDatabase.Instance == null) { Hide(); return; }

        var def = TowerDatabase.Instance.GetTower(towerId);
        if (def == null) { Hide(); return; }

        // Preview 的 Basic Info
        if (previewName) previewName.text = def.ui.name;
        if (previewType) previewType.text = def.ui.type;
        if (previewDescription) previewDescription.text = def.ui.description;

        // Preview 的 stats (Lv1)
        var lv1 = TowerDatabase.Instance.GetLevel(towerId, 1);
        ApplyStatsToTexts(lv1, previewCost, previewDamage, previewCooldown, previewRange);
    }

    // =========================
    // Instance：只開 CurrentPanel + (NextPanel 若有) + Upgrade(若有 next)
    // 不動 Preview 的 Name/Type/Desc（因為那是 PreviewPanel 的排版）
    // =========================
    public void ShowInstance(string towerId, int level)
    {
        if (root == null) return;
        root.SetActive(true);

        SetAllPanelsOff();
        if (currentPanel) currentPanel.SetActive(true);

        if (TowerDatabase.Instance == null) { Hide(); return; }

        var def = TowerDatabase.Instance.GetTower(towerId);
        if (def == null) { Hide(); return; }

        // Current
        var cur = TowerDatabase.Instance.GetLevel(towerId, level);
        ApplyStatsToTexts(cur, currentCost, currentDamage, currentCooldown, currentRange);

        // Next
        var next = TowerDatabase.Instance.GetNextLevel(towerId, level);
        if (next != null)
        {
            if (nextPanel) nextPanel.SetActive(true);
            ApplyStatsToTexts(next, nextCost, nextDamage, nextCooldown, nextRange);
            if (upgradeButton) upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            if (nextPanel) nextPanel.SetActive(false);
            if (upgradeButton) upgradeButton.gameObject.SetActive(false);
        }
    }

    private void ApplyStatsToTexts(
        TowerLevelStats s,
        TextMeshProUGUI tCost,
        TextMeshProUGUI tDamage,
        TextMeshProUGUI tCooldown,
        TextMeshProUGUI tRange)
    {
        if (tCost) tCost.text = $"Cost: {(s != null ? s.cost.ToString() : "N/A")}";
        if (tDamage) tDamage.text = $"Damage: {(s != null ? s.damage.ToString() : "N/A")}";
        if (tCooldown) tCooldown.text = $"Cooldown: {(s != null ? s.cooldown.ToString("0.0") : "N/A")}";
        if (tRange) tRange.text = $"Range: {(s != null ? s.rangeTiles.ToString() : "N/A")}";
    }

    public void Hide()
    {
        SetAllPanelsOff();
        if (root != null) root.SetActive(false);
    }
}
