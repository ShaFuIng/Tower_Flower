using System;
using System.Collections.Generic;

[Serializable]
public class TowerDatabaseRoot
{
    public List<TowerDefinition> towers;
}

[Serializable]
public class TowerDefinition
{
    public string id;
    public TowerUIData ui;
    public List<TowerLevelStats> levels;
}

[Serializable]
public class TowerUIData
{
    public string name;
    public string type;
    public string description;
}

[Serializable]
public class TowerLevelStats
{
    public int level;
    public int cost;
    public float damage;
    public float cooldown;
    public int rangeTiles;
}
