using UnityEngine;
using TMPro;

/// <summary>
/// Displays goal health information, including remaining HP and accumulated lost HP.
/// Subscribes to GoalHealthManager for updates.
/// </summary>
public class GoalHealthUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI remainingHpText; // e.g., "20 / 20"
    [SerializeField] private TextMeshProUGUI lostHpText;      // e.g., "-3"

    [Header("Dependencies")]
    [SerializeField] private GoalHealthManager goalHealthManager;

    private void Start()
    {
        if (goalHealthManager == null)
        {
            // Try to find it in the scene if not assigned
            goalHealthManager = FindObjectOfType<GoalHealthManager>();
        }

        if (goalHealthManager != null)
        {
            // Subscribe to events
            goalHealthManager.OnHpChanged += UpdateRemainingHp;
            goalHealthManager.OnLostHpChanged += UpdateLostHp;

            // Initial UI setup to reflect the starting state
            UpdateRemainingHp(goalHealthManager.CurrentHp, goalHealthManager.MaxHp);
            UpdateLostHp(goalHealthManager.LostHp);
        }
        else
        {
            Debug.LogError("GoalHealthManager not found in the scene. This UI will be disabled.", this);
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent issues
        if (goalHealthManager != null)
        {
            goalHealthManager.OnHpChanged -= UpdateRemainingHp;
            goalHealthManager.OnLostHpChanged -= UpdateLostHp;
        }
    }

    /// <summary>
    /// Updates the text for remaining HP (e.g., "17 / 20").
    /// </summary>
    private void UpdateRemainingHp(int current, int max)
    {
        if (remainingHpText != null)
        {
            remainingHpText.text = $"{current} / {max}";
        }
    }

    /// <summary>
    /// Updates the text for lost HP (e.g., "-3").
    /// Hides the text if no HP has been lost.
    /// </summary>
    private void UpdateLostHp(int lostHp)
    {
        if (lostHpText != null)
        {
            // Per requirements, only show the text if HP has actually been lost
            if (lostHp > 0)
            {
                lostHpText.gameObject.SetActive(true);
                lostHpText.text = $"-{lostHp}";
            }
            else
            {
                lostHpText.gameObject.SetActive(false);
            }
        }
    }
}
