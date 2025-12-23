using UnityEngine;
using System.IO;

/// <summary>
/// Loads level data from a JSON file.
/// This class is responsible for reading and parsing the file,
/// but contains no gameplay logic itself.
/// </summary>
public class LevelDataLoader : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string levelFileName = "Level.json";

    private LevelData currentLevelData;

    public LevelData CurrentLevelData => currentLevelData;

    private void Awake()
    {
        LoadLevelDataFromFile();
    }

    /// <summary>
    /// Loads the level data from the specified JSON file in the Resources/DataBase folder.
    /// </summary>
    private void LoadLevelDataFromFile()
    {
        string path = Path.Combine("DataBase", Path.GetFileNameWithoutExtension(levelFileName));
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile != null)
        {
            currentLevelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
            Debug.Log($"Successfully loaded level data for Level ID: {currentLevelData.levelId}");
        }
        else
        {
            Debug.LogError($"Failed to load level data. Make sure '{levelFileName}' exists in 'Assets/Resources/DataBase'.");
            // Provide a default fallback to prevent null reference errors
            currentLevelData = new LevelData();
        }
    }

    // This method is kept for compatibility if other parts of the code still use it,
    // but the recommended way is to access the property.
    public LevelData LoadCurrentLevelData()
    {
        if (currentLevelData == null)
        {
            LoadLevelDataFromFile();
        }
        return currentLevelData;
    }
}