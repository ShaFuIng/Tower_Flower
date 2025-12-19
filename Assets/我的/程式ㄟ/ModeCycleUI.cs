using UnityEngine;
using TMPro;

public enum EditMode
{
    Move,
    Scale,
    Rotate
}

public class ModeCycleUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI modeText;
    public GameObject uiRoot;              // ModeButton
    public GameObject finishButtonRoot;    // Exit button

    [Header("Managers / Controllers")]
    public BaseCaseManager baseManager;

    public GameObject scaleSliderRoot;
    public ScaleSlider scaleSlider;

    public RotateGesture rotateGesture;

    public EditMode currentMode = EditMode.Move;

    private void Start()
    {
        uiRoot.SetActive(false);
        scaleSliderRoot.SetActive(false);
        rotateGesture?.StopRotate();

        if (finishButtonRoot != null)
            finishButtonRoot.SetActive(false);
    }


    // -------------------------
    // Show UI when Gizmo created
    // -------------------------
    public void ShowUI()
    {
        uiRoot.SetActive(true);

        if (finishButtonRoot != null)
            finishButtonRoot.SetActive(true);

        currentMode = EditMode.Move;
        modeText.text = "移動模式";
        baseManager.SwitchToMoveMode();

        scaleSliderRoot.SetActive(false);
        rotateGesture?.StopRotate();

        Transform target = baseManager.SpawnedBase?.transform;

        if (scaleSlider != null)
            scaleSlider.Initialize(target);

        if (rotateGesture != null)
            rotateGesture.target = target;
    }


    // -------------------------
    // ⭐ 加回：Mode切換按鈕功能
    // -------------------------
    public void OnModeButtonPressed()
    {
        switch (currentMode)
        {
            case EditMode.Move:
                SetMode(EditMode.Scale);
                break;

            case EditMode.Scale:
                SetMode(EditMode.Rotate);
                break;

            case EditMode.Rotate:
                SetMode(EditMode.Move);
                break;
        }
    }


    // -------------------------
    // ⭐ 加回：模式切換的主要邏輯
    // -------------------------
    private void SetMode(EditMode mode)
    {
        currentMode = mode;

        // 統一先關閉所有 UI / 手勢
        scaleSliderRoot.SetActive(false);
        rotateGesture?.StopRotate();

        switch (mode)
        {
            case EditMode.Move:
                modeText.text = "移動模式";
                baseManager.SwitchToMoveMode();
                break;

            case EditMode.Scale:
                modeText.text = "縮放模式";
                baseManager.SwitchToScaleMode();

                Transform target = baseManager.SpawnedBase.transform;
                scaleSlider.Initialize(target);
                scaleSliderRoot.SetActive(true);
                break;

            case EditMode.Rotate:
                modeText.text = "旋轉模式";
                baseManager.SwitchToRotateMode();

                rotateGesture.target = baseManager.SpawnedBase.transform;
                rotateGesture.StartRotate();
                break;
        }
    }


    public void OnFinishBasePlanePressed()
    {
        Debug.Log("[UI] Finish BasePlane pressed");

        uiRoot.SetActive(false);

        if (finishButtonRoot != null)
            finishButtonRoot.SetActive(false);

        if (baseManager.GizmoInstance != null)
            baseManager.GizmoInstance.SetActive(false);

        // ✅ 1️⃣ 先更新 BasePlane 尺寸（你原本就有）
        FloorBuildManager.Instance.UpdateBasePlaneScale(baseManager.SpawnedBase.transform);

        // ✅ 3️⃣ 進入建造模式
        ARBuildManager.Instance.StartBuildMode();
    }

}
