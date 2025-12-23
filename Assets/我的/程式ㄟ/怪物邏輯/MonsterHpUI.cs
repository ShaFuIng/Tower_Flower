using UnityEngine;
using UnityEngine.UI;

// Displays the monster's HP using a UI Slider.
// This component listens to MonsterStats HP change events
// and updates the slider accordingly.
// The HP bar is always visible.
[RequireComponent(typeof(Slider))]
public class MonsterHpUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonsterStats monsterStats;
    private Slider hpSlider;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();
        if (monsterStats == null)
        {
            monsterStats = GetComponentInParent<MonsterStats>();
        }
    }

    private void Start()
    {
        if (monsterStats != null)
        {
            monsterStats.OnHpChanged += UpdateHp;
            // Initialize HP bar
            UpdateHp(monsterStats.CurrentHp, monsterStats.MaxHp);
        }
        else
        {
            Debug.LogError("MonsterStats not found for MonsterHpUI.", this);
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (monsterStats != null)
        {
            monsterStats.OnHpChanged -= UpdateHp;
        }
    }

    private void UpdateHp(int current, int max)
    {
        if (max > 0)
        {
            hpSlider.value = (float)current / max;
        }
    }
}
