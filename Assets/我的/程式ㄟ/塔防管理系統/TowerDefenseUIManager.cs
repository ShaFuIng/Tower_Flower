using UnityEngine;
using UnityEngine.UI;

public enum PlaceMode
{
    None, Spawn, Goal
}

public class    TowerDefenseUIManager : MonoBehaviour
{
    public static TowerDefenseUIManager Instance;
    
    [Header("Object Menu Buttons")]
    public Button spawnButton;
    public Button goalButton;
    public Button cancelButton;
    public Button createButton;

    [Header("Selection Highlight Boxes")]
    public GameObject spawnSelectionBox;
    public GameObject goalSelectionBox;

    [Header("Object Menu Root")]
    public GameObject objectMenu;

    [Header("Pathfinding UI")]
    public GameObject computePathButton;
    public GameObject simulateMonsterButton;
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public ArrowPathRenderer arrowRenderer;

    [Header("Path Control UI")]
    public GameObject resetPathButton;
    public GameObject gameStartButton;

    [Header("Money UI")]
    public MoneyUIController moneyUIController;

    public PlaceMode currentMode = PlaceMode.None;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnButton.onClick.AddListener(SelectSpawn);
        goalButton.onClick.AddListener(SelectGoal);

        // Cancel：關 objectMenu（與 preview / selection）
        cancelButton.onClick.AddListener(CloseMenu);

        // Create：打開 objectMenu
        createButton.onClick.AddListener(OpenMenu);

        computePathButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            PathfindingManager.Instance.ComputePath();
        });

        simulateMonsterButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            PathfindingManager.Instance.SimulateMonsterMovement();
        });

        if (PathfindingManager.Instance != null)
            PathfindingManager.Instance.arrowRenderer = arrowRenderer;

        HideResetPathButton();
        HideGameStartButton();

        objectMenu.SetActive(false);
        ClearSelectionUI();
        DisableCreateButton();

        computePathButton.SetActive(false);
        simulateMonsterButton.SetActive(false);
        loadingPanel.SetActive(false);
    }

    // =========================================================
    public void DisableCreateButton()
    {
        createButton.gameObject.SetActive(false);
    }

    public void EnableCreateButton()
    {
        createButton.gameObject.SetActive(true);
        UpdateComputePathButtonVisibility();
    }

    // =========================================================
    // OccupyType 判斷
    // =========================================================
    public void UpdateComputePathButtonVisibility()
    {
        if (SpawnGoalPlacementManager.Instance == null) return;

        bool hasSpawn = SpawnGoalPlacementManager.Instance.HasOccupyType(OccupyType.Spawn);
        bool hasGoal = SpawnGoalPlacementManager.Instance.HasOccupyType(OccupyType.Goal);

        computePathButton.SetActive(hasSpawn && hasGoal);
    }

    // =========================================================
    // 第一層：開 objectMenu
    // =========================================================
    public void OpenMenu()
    {
        objectMenu.SetActive(true);
    }

    // 第二層：關 objectMenu（不順便關 Tower UI）
    public void CloseMenu()
    {
        objectMenu.SetActive(false);
        ClearSelectionUI();
        SpawnGoalPlacementManager.Instance?.EndPreview();
    }


    // =========================================================
    // 進入放置/拖曳模式：關 Tower UI（但不關 objectMenu）
    // =========================================================
    public void SelectSpawn()
    {
        currentMode = PlaceMode.Spawn;
        UpdateSelectionUI();

        // ✅ 只關 Tower UI，不要動 ScrollView
        TowerUIManager.Instance?.Hide();
    }

    public void SelectGoal()
    {
        currentMode = PlaceMode.Goal;
        UpdateSelectionUI();

        // ✅ 只關 Tower UI，不要動 ScrollView
        TowerUIManager.Instance?.Hide();
    }

    // =========================================================
    public void ClearSelectionUI()
    {
        spawnSelectionBox.SetActive(false);
        goalSelectionBox.SetActive(false);
        currentMode = PlaceMode.None;
    }

    private void UpdateSelectionUI()
    {
        // ✅ 正確的邏輯：只控制高亮框的顯示/隱藏
        //    按鈕本身 (spawnButton, goalButton) 應該保持啟用狀態
        if (spawnSelectionBox != null)
            spawnSelectionBox.SetActive(currentMode == PlaceMode.Spawn);

        if (goalSelectionBox != null)
            goalSelectionBox.SetActive(currentMode == PlaceMode.Goal);
    }

    // =========================================================
    public void OnResetPathButtonPressed()
    {
        PathfindingManager.Instance?.ResetPathPreview();
        EnableCreateButton();
        ClearSelectionUI();
        UpdateComputePathButtonVisibility();
        HideResetPathButton();

        // ✅ 隱藏金錢 UI（回到建築模式）
        if (moneyUIController != null)
        {
            moneyUIController.HideUI();
        }
    }

    public void ShowResetPathButton()
    {
        resetPathButton.SetActive(true);
    }

    public void HideResetPathButton()
    {
        resetPathButton.SetActive(false);
    }

    public void ShowGameStartButton()
    {
        gameStartButton.SetActive(true);
    }

    public void HideGameStartButton()
    {
        gameStartButton.SetActive(false);
    }

    public void OnGameStartButtonPressed()
    {
        HideResetPathButton();
        HideGameStartButton();
        computePathButton.SetActive(false);

        objectMenu.SetActive(false);
        ClearSelectionUI();

        // 進入 gameplay 前也關掉塔資訊 UI
        TowerUIManager.Instance?.Hide();

        // ✅ 啟動金錢 UI
        if (moneyUIController != null)
        {
            moneyUIController.ShowUI();
        }

        PathfindingManager.Instance?.ResetPathPreview();
        BuildPhaseManager.Instance.SetPhase(BuildPhase.Gameplay);
    }
}
