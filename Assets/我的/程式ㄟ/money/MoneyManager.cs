using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    // Singleton
    public static MoneyManager Instance { get; private set; }

    [SerializeField]
    private int currentMoney = 1000; // Starting amount (adjustable in inspector)

    /// <summary>
    /// Event fired when money amount changes.
    /// Passes the new money amount as parameter.
    /// </summary>
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Notify UI controllers of initial money state
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// Check if player can afford a given cost.
    /// Does NOT spend money - just checks.
    /// </summary>
    public bool CanAfford(int cost)
    {
        return currentMoney >= cost;
    }

    /// <summary>
    /// Attempt to spend money.
    /// Returns true if successful (money was spent).
    /// Returns false if insufficient funds (money unchanged).
    /// </summary>
    public bool Spend(int cost)
    {
        if (!CanAfford(cost))
            return false;

        currentMoney -= cost;
        OnMoneyChanged?.Invoke(currentMoney);               
        return true;
    }

    /// <summary>
    /// Add money to player account.
    /// </summary>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// Refund money (e.g., when a tower is sold).
    /// This is an alias for AddMoney to make the intent clearer.
    /// </summary>
    public void RefundMoney(int amount)
    {
        AddMoney(amount);
    }

    /// <summary>
    /// Get current money amount (for UI or other queries).
    /// </summary>
    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}
