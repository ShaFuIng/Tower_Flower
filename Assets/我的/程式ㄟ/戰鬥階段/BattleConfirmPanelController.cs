using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; // Using TextMeshPro for modern UI text.

public class BattleConfirmPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text wavesText; // Text to display total waves
    [SerializeField] private TMP_Text enemiesText; // Text to display total enemies

    // Event invoked when the user confirms the battle start.
    public UnityEvent OnConfirm;

    // Event invoked when the user cancels the battle start.
    public UnityEvent OnCancel;

    private void Awake()
    {
        // Ensure the panel is hidden by default.
        gameObject.SetActive(false);

        // Add listeners to the buttons.
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(HandleConfirm);
        }
        else
        {
            Debug.LogError("Confirm Button is not assigned in the BattleConfirmPanelController.");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(HandleCancel);
        }
        else
        {
            Debug.LogError("Cancel Button is not assigned in the BattleConfirmPanelController.");
        }
    }

    /// <summary>
    /// Shows the confirmation panel and populates it with level data.
    /// </summary>
    /// <param name="levelData">The data for the level to be displayed.</param>
    public void Show(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is null. Cannot show panel.");
            return;
        }

        // Update UI text elements with data from the LevelData object.
        if (wavesText != null)
        {
            wavesText.text = $"Total Waves: {levelData.totalWaves}";
        }
        if (enemiesText != null)
        {
            enemiesText.text = $"Total Enemies: {levelData.totalEnemies}";
        }

        // Show the panel.
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the confirmation panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the confirm button click.
    /// Invokes the OnConfirm event and then hides the panel.
    /// </summary>
    private void HandleConfirm()
    {
        OnConfirm?.Invoke();
        Hide();
    }

    /// <summary>
    /// Handles the cancel button click.
    /// Invokes the OnCancel event and then hides the panel.
    /// </summary>
    private void HandleCancel()
    {
        OnCancel?.Invoke();
        Hide();
    }

    private void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks.
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(HandleConfirm);
        }
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(HandleCancel);
        }
    }
}