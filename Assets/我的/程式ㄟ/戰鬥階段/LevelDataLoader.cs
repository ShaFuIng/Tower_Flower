using UnityEngine;

/// <summary>
/// Loads level data.
/// In a real-world scenario, this might load from a file or a server.
/// For now, it provides hardcoded mock data for development.
/// </summary>
public class LevelDataLoader : MonoBehaviour
{
    /// <summary>
    /// Retrieves the data for the current level.
    /// This is a placeholder and returns a fixed LevelData object.
    /// </summary>
    /// <returns>A LevelData object with mock information.</returns>
    public LevelData LoadCurrentLevelData()
    {
        // In the future, this could be expanded to load data based on a level index
        // or from a configuration file (e.g., JSON, ScriptableObject).
        return new LevelData
        {
            levelId = 1,
            totalWaves = 10,
            totalEnemies = 50
        };
    }
}