using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Loads monster reward data from MonsterDatabase.json and provides easy access.
/// Contains no gameplay logic.
/// </summary>
public class MonsterDatabaseLoader : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string databaseFileName = "MonsterDatabase.json";

    private Dictionary<string, MonsterRewardData> monsterRewardMap;

    private void Awake()
    {
        LoadMonsterData();
    }

    /// <summary>
    /// Loads and parses the monster database from the JSON file.
    /// </summary>
    private void LoadMonsterData()
    {
        string path = Path.Combine("DataBase", Path.GetFileNameWithoutExtension(databaseFileName));
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile != null)
        {
            MonsterRewardDataList dataList = JsonUtility.FromJson<MonsterRewardDataList>(jsonFile.text);
            // Convert the array to a dictionary for fast lookups by monster ID
            monsterRewardMap = dataList.monsters.ToDictionary(monster => monster.id, monster => monster);
            Debug.Log($"Successfully loaded {monsterRewardMap.Count} monster reward entries.");
        }
        else
        {
            Debug.LogError($"Failed to load monster database. Make sure '{databaseFileName}' exists in 'Assets/Resources/DataBase'.");
            monsterRewardMap = new Dictionary<string, MonsterRewardData>();
        }
    }

    /// <summary>
    /// Gets the reward data for a specific monster ID.
    /// </summary>
    /// <param name="monsterId">The ID of the monster (e.g., "slime_normal").</param>
    /// <returns>The MonsterRewardData for the given ID, or null if not found.</returns>
    public MonsterRewardData GetRewardData(string monsterId)
    {
        monsterRewardMap.TryGetValue(monsterId, out MonsterRewardData data);
        return data;
    }
}
