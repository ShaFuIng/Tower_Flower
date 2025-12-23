using UnityEngine;
using TMPro;

/// <summary>
/// Updates and displays the enemy kill count UI.
/// It subscribes to EnemyKillCountManager to receive updates.
/// </summary>
public class EnemyKillCountUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI killCountText;

    private void Start()
    {
        if (EnemyKillCountManager.Instance != null)
        {
            // Subscribe to future updates
            EnemyKillCountManager.Instance.OnKillCountChanged += UpdateUI;
            
            // --- FIX ---
            // Immediately pull the current state to handle the initialization race condition.
            UpdateUI(EnemyKillCountManager.Instance.KilledCount, EnemyKillCountManager.Instance.TotalEnemies);
        }
        else
        {
            Debug.LogError("EnemyKillCountManager instance not found. UI will not be updated.", this);
            if (killCountText != null)
            {
                killCountText.text = "N/A";
            }
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (EnemyKillCountManager.Instance != null)
        {
            EnemyKillCountManager.Instance.OnKillCountChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Callback function to update the text display.
    /// </summary>
    private void UpdateUI(int killed, int total)
    {
        if (killCountText != null)
        {
            killCountText.text = $"{killed} / {total}";
        }
    }
}
