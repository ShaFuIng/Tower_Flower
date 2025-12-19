using System.Collections.Generic;
using UnityEngine;

public class TowerDatabase : MonoBehaviour
{
    public static TowerDatabase Instance { get; private set; }

    [Header("Resources 路徑（不含 .json）")]
    [SerializeField] private string resourcesPath = "DataBase/towers";

    private TowerDatabaseRoot root;
    private Dictionary<string, TowerDefinition> byId;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        TextAsset json = Resources.Load<TextAsset>(resourcesPath);

        if (json == null)
        {
            Debug.LogError($"[TowerDatabase] 找不到 JSON：Resources/{resourcesPath}.json");
            return;
        }

        root = JsonUtility.FromJson<TowerDatabaseRoot>(json.text);

        if (root == null || root.towers == null)
        {
            Debug.LogError("[TowerDatabase] JSON 解析失敗");
            return;
        }

        byId = new Dictionary<string, TowerDefinition>();

        foreach (var def in root.towers)
        {
            if (!string.IsNullOrEmpty(def.id))
                byId[def.id] = def;
        }

        Debug.Log($"[TowerDatabase] Loaded {byId.Count} towers");
    }

    // ========= 給 UI / 系統使用的查詢 =========

    public TowerDefinition GetTower(string towerId)
    {
        if (byId == null) return null;
        byId.TryGetValue(towerId, out var def);
        return def;
    }

    public TowerLevelStats GetLevel(string towerId, int level)
    {
        var def = GetTower(towerId);
        if (def == null) return null;

        foreach (var lv in def.levels)
            if (lv.level == level)
                return lv;

        return null;
    }

    public TowerLevelStats GetNextLevel(string towerId, int currentLevel)
    {
        return GetLevel(towerId, currentLevel + 1);
    }
}
