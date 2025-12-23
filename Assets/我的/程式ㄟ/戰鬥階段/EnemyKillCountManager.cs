using UnityEngine;
using System;

/// <summary>
/// Tracks the number of enemies killed in the current level.
/// This class is responsible for the logic of counting kills, not for UI.
/// </summary>
public class EnemyKillCountManager : MonoBehaviour
{
    private int killedCount;
    private int totalEnemies;

    // Public properties for UI controllers to pull initial state
    public int KilledCount => killedCount;
    public int TotalEnemies => totalEnemies;

    public event Action<int, int> OnKillCountChanged; // killed, total

    // A simple singleton for easy access.
    public static EnemyKillCountManager Instance { get; private set; }

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
    /// Sets the total number of enemies for the level and resets the kill count.
    /// This should be called at the start of a level.
    /// </summary>
    public void Initialize(int total)
    {
        totalEnemies = total;
        killedCount = 0;
        OnKillCountChanged?.Invoke(killedCount, totalEnemies);
    }

    /// <summary>
    /// Increments the kill count. This method should be called when an enemy is confirmed killed.
    /// </summary>
    public void RecordKill()
    {
        if (killedCount < totalEnemies)
        {
            killedCount++;
            OnKillCountChanged?.Invoke(killedCount, totalEnemies);
        }
    }
}
