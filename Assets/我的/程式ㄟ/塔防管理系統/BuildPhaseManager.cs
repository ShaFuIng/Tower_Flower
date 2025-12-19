using UnityEngine;

public enum BuildPhase
{
    PathSetup,   // Spawn / Goal + 路徑確認
    Gameplay     // 遊戲開始（塔防擺放）
}

public class BuildPhaseManager : MonoBehaviour
{
    public static BuildPhaseManager Instance;

    [Header("Runtime")]
    public BuildPhase currentPhase = BuildPhase.PathSetup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // 一開始進入 PathSetup
        SetPhase(BuildPhase.PathSetup);
    }

    public void SetPhase(BuildPhase phase)
    {
        currentPhase = phase;

        // 通知所有 UI item 刷新顯示狀態
        var items = FindObjectsOfType<BuildPhaseVisibleItem>(true);
        foreach (var item in items)
            item.Refresh();
    }

}
