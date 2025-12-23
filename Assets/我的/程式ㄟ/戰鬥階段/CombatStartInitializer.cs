using UnityEngine;

/// <summary>
/// This class is the central point for initializing the combat state.
/// It reads data from the data loaders and configures the various managers.
/// </summary>
public class CombatStartInitializer : MonoBehaviour
{
    [Header("Data Loaders")]
    [SerializeField] private LevelDataLoader levelDataLoader;

    [Header("Managers to Initialize")]
    [SerializeField] private GoalHealthManager goalHealthManager;
    [SerializeField] private EnemyKillCountManager enemyKillCountManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("UI Panels to Activate")]
    [SerializeField] private GameObject combatHudPanel;

    /// <summary>
    /// Initializes all combat systems based on the loaded level data.
    /// This should be called when the player confirms starting the battle.
    /// </summary>
    public void InitializeCombat()
    {
        if (!ValidateDependencies())
        {
            return;
        }

        LevelData data = levelDataLoader.CurrentLevelData;
        if (data == null)
        {
            Debug.LogError("LevelData is null. Cannot initialize combat.");
            return;
        }

        // Initialize all relevant managers with data from JSON
        goalHealthManager.SetInitialHealth(data.baseMaxHp);
        enemyKillCountManager.Initialize(data.totalEnemies);
        scoreManager.ResetScore();

        // Activate the combat UI
        if (combatHudPanel != null)
        {
            combatHudPanel.SetActive(true);
        }

        Debug.Log("Combat Initialized: Base HP, Enemy Count, and Score are set.");
    }

    private bool ValidateDependencies()
    {
        if (levelDataLoader == null)
        {
            Debug.LogError("LevelDataLoader is not assigned.", this);
            return false;
        }
        if (goalHealthManager == null)
        {
            Debug.LogError("GoalHealthManager is not assigned.", this);
            return false;
        }
        if (enemyKillCountManager == null)
        {
            Debug.LogError("EnemyKillCountManager is not assigned.", this);
            return false;
        }
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager is not assigned.", this);
            return false;
        }
        return true;
    }
}
