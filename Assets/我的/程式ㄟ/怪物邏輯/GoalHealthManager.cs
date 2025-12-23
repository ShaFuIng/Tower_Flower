using UnityEngine;
using System;

// Manages the goal/base health.
// This is the only class allowed to modify base HP.
// It emits events when HP changes or reaches zero.
public class GoalHealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHp = 20;
    private int currentHp;
    private int lostHp; // Accumulated damage this level

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public int LostHp => lostHp;

    // Events
    public event Action<int, int> OnHpChanged; // current, max
    public event Action<int> OnLostHpChanged; // new lostHp value
    public event Action OnGoalDead;

    // Note: OnHpDelta is removed as OnLostHpChanged is more specific to the new design.

    private void Awake()
    {
        // Initialize with default values, can be overwritten by a level loader.
        SetInitialHealth(maxHp);
    }

    /// <summary>
    /// Sets the initial health values for the start of a level.
    /// </summary>
    public void SetInitialHealth(int newMaxHp)
    {
        maxHp = newMaxHp;
        currentHp = maxHp;
        lostHp = 0;

        // Notify listeners of the initial state
        OnHpChanged?.Invoke(currentHp, maxHp);
        OnLostHpChanged?.Invoke(lostHp);
    }

    /// <summary>
    /// Applies damage to the goal, updating both current and lost HP.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        int damageToTake = Mathf.Min(damage, currentHp);
        currentHp -= damageToTake;
        lostHp += damageToTake;

        OnHpChanged?.Invoke(currentHp, maxHp);
        OnLostHpChanged?.Invoke(lostHp);

        if (currentHp <= 0)
        {
            OnGoalDead?.Invoke();
            Debug.Log("Goal has been destroyed!");
        }
    }
}
