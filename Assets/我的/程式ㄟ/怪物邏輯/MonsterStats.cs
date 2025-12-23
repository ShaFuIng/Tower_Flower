using UnityEngine;
using System;

public enum ResolvedType
{
    Killed,
    ReachedGoal
}

// Handles monster health, damage processing, and resolution logic.
// This class owns HP state and determines when the monster is killed or reaches the goal.
// Towers should call TakeDamage(), and this class will handle death, events, and cleanup.
public class MonsterStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<int, int> OnHpChanged; // current, max
    public event Action<ResolvedType> OnResolved;

    private bool isResolved = false;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (isResolved) return;

        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }

        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void Die()
    {
        Resolve(ResolvedType.Killed);
    }

    public void Resolve(ResolvedType type)
    {
        if (isResolved) return;
        isResolved = true;

        OnResolved?.Invoke(type);
        
        // Per instructions, destroy immediately
        Destroy(gameObject);
    }
}
