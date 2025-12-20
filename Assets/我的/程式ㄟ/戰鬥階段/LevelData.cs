using UnityEngine;

/// <summary>
/// A simple data container for level information.
/// Can be used as a ScriptableObject or a plain C# class.
/// </summary>
[System.Serializable]
public class LevelData
{
    // The unique identifier for the level.
    public int levelId;
    // The total number of enemy waves in this level.
    public int totalWaves;
    // The total number of enemies in this level.
    public int totalEnemies;
}
