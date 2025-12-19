using UnityEngine;

public class PlacementCostValidator : MonoBehaviour
{
    /// <summary>
    /// Check if a placement can start based on money affordability.
    /// Call this BEFORE initiating a drag preview.
    /// 
    /// If cost cannot be afforded:
    /// - Returns false
    /// - Shows "Not enough money" warning
    /// - Does NOT allow preview to start
    /// 
    /// If cost can be afforded:
    /// - Returns true
    /// - Drag preview can proceed
    /// </summary>
    public static bool CanStartPlacement(int cost)
    {
        // Validate managers exist
        if (MoneyManager.Instance == null)
        {
            Debug.LogError("[PlacementCostValidator] MoneyManager not found!");
            return false;
        }

        // Check if player can afford this placement
        if (MoneyManager.Instance.CanAfford(cost))
        {
            // Affordable - allow drag to start
            return true;
        }

        // Not affordable - show warning and block placement
        ShowNotEnoughMoneyWarning();
        return false;
    }

    /// <summary>
    /// Internal helper: Trigger the "Not enough money" warning UI.
    /// </summary>
    private static void ShowNotEnoughMoneyWarning()
    {
        var moneyUI = Object.FindFirstObjectByType<MoneyUIController>();
        if (moneyUI != null)
        {
            moneyUI.ShowNotEnoughMoneyWarning();
        }
        else
        {
            Debug.LogWarning("[PlacementCostValidator] MoneyUIController not found in scene!");
        }
    }

    /// <summary>
    /// Spend money from placement (called AFTER placement is confirmed).
    /// Returns true if spend was successful.
    /// Should be called after the object is actually placed.
    /// </summary>
    public static bool SpendPlacementCost(int cost)
    {
        if (MoneyManager.Instance == null)
        {
            Debug.LogError("[PlacementCostValidator] MoneyManager not found!");
            return false;
        }

        return MoneyManager.Instance.Spend(cost);
    }
}
