using UnityEngine;

public class PlacedTower : MonoBehaviour
{
    public string towerId;
    public int level = 1;

    public void OnSelected()
    {
        TowerUIManager.Instance?.ShowTowerInstance(towerId, level);
    }
}
