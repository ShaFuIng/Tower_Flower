using UnityEngine;

/// <summary>
/// Represents the reward data for a single monster type.
/// This is a plain data structure with no gameplay logic.
/// </summary>
[System.Serializable]
public class MonsterRewardData
{
    public string id;
    public int rewardMoney;
    public int rewardScore;
}

/// <summary>
/// A wrapper class to match the root structure of MonsterDatabase.json,
/// which contains an array of monsters.
/// </summary>
[System.Serializable]
public class MonsterRewardDataList
{
    public MonsterRewardData[] monsters;
}
