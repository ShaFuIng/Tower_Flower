using UnityEngine;

/// <summary>
/// Data structure for the UI preview section in Level.json.
/// </summary>
[System.Serializable]
public class LevelUIPreview
{
    public string title;
    public string description;
}

/// <summary>
/// A simple data container for level information, matching Level.json structure.
/// </summary>
[System.Serializable]
public class LevelData
{
    // The unique identifier for the level.
    public int levelId;
    // The starting health of the player's base for this level.
    public int baseMaxHp;
    // The total number of enemies in this level.
    public int totalEnemies;
    // UI-related text for the level confirmation panel.
    public LevelUIPreview uiPreview;
}
