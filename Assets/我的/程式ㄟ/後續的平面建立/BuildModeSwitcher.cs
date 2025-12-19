using UnityEngine;
using TMPro;

public class BuildModeSwitcher : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI modeButtonText;       // Floor / Wall 顯示文字
    public GameObject finishBuildButton;          // 完成建造按鈕
    public GameObject opacitySwitchButton;        // ⭐ 透明度切換按鈕

    [Header("Reference")]
    public WallMaterialSwitcher wallMaterialSwitcher;

    private void Start()
    {
        // 初始化 Floor 模式
        SetMode(ARBuildManager.BuildMode.Floor);

        // 一開始隱藏完成建造按鈕
        if (finishBuildButton != null)
            finishBuildButton.SetActive(false);

        // ⭐ 一開始隱藏透明切換按鈕（BaseCase 尚未完成）
        if (opacitySwitchButton != null)
            opacitySwitchButton.SetActive(false);
    }


    // ===========================
    //   Floor <-> Wall 模式切換
    // ===========================
    public void OnSwitchModeButtonPressed()
    {
        if (ARBuildManager.Instance.currentBuildMode == ARBuildManager.BuildMode.Floor)
            SetMode(ARBuildManager.BuildMode.Wall);
        else
            SetMode(ARBuildManager.BuildMode.Floor);
    }

    private bool opacityButtonInitialized = false;
    private void SetMode(ARBuildManager.BuildMode mode)
    {
        ARBuildManager.Instance.currentBuildMode = mode;

        modeButtonText.text = (mode == ARBuildManager.BuildMode.Floor)
            ? "地板模式" : "牆面模式";

        if (finishBuildButton != null)
            finishBuildButton.SetActive(true);

        // ⭐ 第一次進入 Floor/Wall 模式 → 顯示透明按鈕
        if (opacitySwitchButton != null)
        {
            opacitySwitchButton.SetActive(true);

            // ⭐⭐ 初始化按鈕文字（只做一次）
            if (!opacityButtonInitialized)
            {
                var txt = opacitySwitchButton.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                    txt.text = "預設";

                opacityButtonInitialized = true;
            }
        }
    }



    // ===========================
    //     按下「完成建造」
    // ===========================
    public void OnFinishBuildButtonPressed()
    {
        // 關閉建造 UI（由 FloorBuildManager 負責）
        FloorBuildManager.Instance.ExitBuildMode();

        // ⭐ 透明度按鈕保持存在，不要關閉它
        // 👉 所以什麼都不用做，只要不關它就好

        // （如果你想切換成透明模式，可放這裡）
        // wallMaterialSwitcher.SwitchToTransparent();
    }


    // ===========================
    //      透明度模式切換按鈕
    // ===========================
    public void OnOpacitySwitchButtonPressed()
    {
        int mode = wallMaterialSwitcher.SwitchMode(
            FloorBuildManager.Instance.GetAllWalls()
        );

        // 更新按鈕文字
        string text = mode switch
        {
            0 => "預設",
            1 => "半透明",
            2 => "透明",
            _ => "牆模式"
        };

        opacitySwitchButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
