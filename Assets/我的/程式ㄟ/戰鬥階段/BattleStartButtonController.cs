using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Controls the visibility and interaction of the "Battle Start" button.
/// This class is UI-only and does not handle any game logic or phase changes.
/// </summary>
[RequireComponent(typeof(Button))]
public class BattleStartButtonController : MonoBehaviour
{
    private Button battleStartButton;

    // UnityEvent to be invoked when the button is clicked.
    // This allows connecting the button click to any other system in the Unity Editor.
    public UnityEvent OnBattleStartClicked;

    private void Awake()
    {
        battleStartButton = GetComponent<Button>();
        if (battleStartButton == null)
        {
            Debug.LogError("BattleStartButtonController requires a Button component on the same GameObject.");
            return;
        }

        // Add a listener to the button's onClick event.
        battleStartButton.onClick.AddListener(HandleClick);

        // Ensure the button is hidden at the start of the game.
        Hide();
    }

    /// <summary>
    /// Handles the button click event.
    /// Invokes the OnBattleStartClicked event.
    /// </summary>
    private void HandleClick()
    {
        // When the button is clicked, invoke the public UnityEvent.
        // The BattleStartFlowController will listen to this event.
        OnBattleStartClicked?.Invoke();
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
    /// Sets the visibility of the battle start button.
    /// </summary>
    /// <param name="isVisible">True to show the button, false to hide it.</param>
    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    private void OnDestroy()
    {
        // Clean up the listener when the object is destroyed.
        if (battleStartButton != null)
        {
            battleStartButton.onClick.RemoveListener(HandleClick);
        }
    }
}