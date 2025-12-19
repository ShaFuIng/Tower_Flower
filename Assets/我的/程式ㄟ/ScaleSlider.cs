using UnityEngine;
using UnityEngine.UI;

public class ScaleSlider : MonoBehaviour
{
    public Transform target;   // BasePlane
    public Slider slider;

    private float baseSize = 1f;

    public void Initialize(Transform t)
    {
        target = t;

        // 以當下 BasePlane 的尺寸作為「1.0 倍」的基準
        baseSize = target.localScale.x;

        slider.minValue = 0.5f;   // 最小 50%
        slider.maxValue = 3.0f;   // 最大 300%
        slider.value = 1.0f;      // 初始為 100%

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float v)
    {
        if (target == null) return;

        float newSize = baseSize * v;

        // 等比縮放 XZ（保持高度不變）
        target.localScale = new Vector3(newSize, target.localScale.y, newSize);
    }
}
