using UnityEngine;
using System;

/// <summary>
/// Manages the player's score during combat.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    private int currentScore;
    public int CurrentScore => currentScore;

    public event Action<int> OnScoreChanged;

    // Simple singleton for easy access
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Resets the score to zero, typically at the start of a level.
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// Adds a specified amount to the current score.
    /// (This will be used in the future when monsters are killed).
    /// </summary>
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }
}
