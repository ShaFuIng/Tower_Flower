using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Controls the battle confirmation panel UI.
/// Displays level preview information before entering combat.
/// This class is UI-only and does not contain gameplay logic.
/// </summary>
public class BattleConfirmPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text enemiesText;   // Displays total enemies
    [SerializeField] private TMP_Text titleText;     // Optional: level title
    [SerializeField] private TMP_Text descriptionText; // Optional: level description

    // Event invoked when the user confirms starting the battle.
    public UnityEvent OnConfirm;

    // Event invoked when the user cancels starting the battle.
    public UnityEvent OnCancel;

    private void Awake()
    {
        // Ensure the panel is hidden by default.
        gameObject.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(HandleConfirm);
        else
            Debug.LogError("Confirm Button is not assigned.", this);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(HandleCancel);
        else
            Debug.LogError("Cancel Button is not assigned.", this);
    }

    /// <summary>
    /// Shows the confirmation panel and populates it with level data.
    /// </summary>
    public void Show(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is null. Cannot show confirmation panel.", this);
            return;
        }

        // Display total enemies
        if (enemiesText != null)
            enemiesText.text = $"Total Enemies: {levelData.totalEnemies}";

        // Display optional UI preview text from Level.json
        if (levelData.uiPreview != null)
        {
            if (titleText != null)
                titleText.text = levelData.uiPreview.title;

            if (descriptionText != null)
                descriptionText.text = levelData.uiPreview.description;
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the confirmation panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HandleConfirm()
    {
        OnConfirm?.Invoke();
        Hide();
    }

    private void HandleCancel()
    {
        OnCancel?.Invoke();
        Hide();
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(HandleConfirm);

        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(HandleCancel);
    }
}
