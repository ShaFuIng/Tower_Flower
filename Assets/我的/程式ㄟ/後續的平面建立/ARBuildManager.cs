using UnityEngine;

public class ARBuildManager : MonoBehaviour
{
    public static ARBuildManager Instance;

    [Header("Build Mode")]
    public bool isBuildMode = false;

    [Tooltip("建造模式時要顯示的物件（例如：UI、Panel、提示文字）")]
    public GameObject buildModeRoot;

    public enum BuildMode
    {
        Floor,
        Wall
    }

    public BuildMode currentBuildMode = BuildMode.Floor;
    private void Awake()
    {
        Instance = this;

        // 建議：一開始隱藏建造模式物件（避免場景啟動就亮）
        if (buildModeRoot != null)
            buildModeRoot.SetActive(false);
    }


    public void StartBuildMode()
    {
        isBuildMode = true;

        if (buildModeRoot != null)
            buildModeRoot.SetActive(true);

        // 通知 FloorBuildManager 進入建造模式
        FloorBuildManager.Instance?.OnEnterBuildMode();
    }

}