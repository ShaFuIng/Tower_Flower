using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visibility of the "Battle Start" button.
/// This class is a simple UI controller. The click event is handled
/// directly in the Unity Editor via the Button's OnClick() event.
/// </summary>
[RequireComponent(typeof(Button))]
public class BattleStartButtonController : MonoBehaviour
{
    private Button battleStartButton;

    private void Awake()
    {
        battleStartButton = GetComponent<Button>();
        // The button should be visible by default, the flow controller will hide it.
    }

    /// <summary>
    /// Shows the battle start button.
    /// </summary>
    public void Show()
    {
        SetVisible(true);
    }

    /// <summary>
    /// Hides the battle start button.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Sets the visibility of the battle start button's GameObject.
    /// </summary>
    /// <param name="isVisible">True to show, false to hide.</param>
    public void SetVisible(bool isVisible)
    {
        if (gameObject.activeSelf != isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}