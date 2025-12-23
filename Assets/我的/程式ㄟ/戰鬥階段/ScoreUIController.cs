using UnityEngine;
using TMPro;

/// <summary>
/// Updates the score UI by listening to the ScoreManager.
/// </summary>
public class ScoreUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Formatting")]
    [SerializeField] private string scoreFormat = "D8"; // e.g., 00000000

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            // Set initial score display
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
        }
        else
        {
            Debug.LogError("ScoreManager instance not found. Score UI will not function.", this);
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString(scoreFormat);
        }
    }
}
