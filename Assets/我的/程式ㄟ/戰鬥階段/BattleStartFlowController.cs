using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Coordinates the flow between the preparation phase, battle confirmation, and the start of the battle.
/// This script acts as a mediator, connecting UI components and triggering game events
/// without containing any gameplay logic itself.
/// </summary>
public class BattleStartFlowController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private LevelDataLoader levelDataLoader;
    [SerializeField] private BattleStartButtonController battleStartButtonController; // Re-enabled this reference
    [SerializeField] private BattleConfirmPanelController battleConfirmPanelController;

    /// <summary>
    /// This event is invoked when the user confirms that they want to start the battle.
    /// Other systems (like an enemy spawner or game phase manager) can listen to this event
    /// to begin the actual battle gameplay.
    /// </summary>
    public UnityEvent OnBattleStartConfirmed;

    // OnCancelConfirmation is no longer needed as the flow is self-contained again.

    private void Start()
    {
        // Ensure all components are assigned in the editor to prevent null reference errors.
        if (levelDataLoader == null || battleConfirmPanelController == null || battleStartButtonController == null)
        {
            Debug.LogError("One or more required components are not assigned in the BattleStartFlowController inspector.");
            return;
        }

        // --- Wire up the event listeners ---

        // 1. When the battle start button is clicked, begin the confirmation flow.
        battleStartButtonController.OnBattleStartClicked.AddListener(HandleBattleStartClicked);

        // 2. When the user confirms on the panel, trigger the final battle start event.
        battleConfirmPanelController.OnConfirm.AddListener(HandleConfirmation);

        // 3. When the user cancels on the panel, return to the initial state.
        battleConfirmPanelController.OnCancel.AddListener(HandleCancellation);

        // --- Set initial UI state ---
        // The panel should be hidden by default. The button's visibility will be controlled externally.
        battleConfirmPanelController.Hide();
    }

    /// <summary>
    /// Called when the main "Battle Start" button is clicked.
    /// </summary>
    private void HandleBattleStartClicked()
    {
        // Hide the start button immediately to prevent double-clicks.
        battleStartButtonController.SetVisible(false);

        // Load the data for the current level.
        LevelData currentLevelData = levelDataLoader.LoadCurrentLevelData();

        // Show the confirmation panel, populated with the loaded data.
        battleConfirmPanelController.Show(currentLevelData);
    }

    /// <summary>
    /// Called when the user clicks the "Confirm" button on the confirmation panel.
    /// </summary>
    private void HandleConfirmation()
    {
        // The panel hides itself, so we don't need to manage that here.

        // Invoke the final event to signal that the battle should begin.
        Debug.Log("Battle Start Confirmed! Firing OnBattleStartConfirmed event.");
        OnBattleStartConfirmed?.Invoke();

        // The button remains hidden as we are now proceeding to the battle phase.
    }

    /// <summary>
    /// Called when the user clicks the "Cancel" button on the confirmation panel.
    /// </summary>
    private void HandleCancellation()
    {
        // The panel hides itself.

        // Show the battle start button again, returning the UI to its initial state.
        battleStartButtonController.SetVisible(true);
    }

    private void OnDestroy()
    {
        // It's good practice to remove listeners when the object is destroyed
        if (battleStartButtonController != null)
        {
            battleStartButtonController.OnBattleStartClicked.RemoveListener(HandleBattleStartClicked);
        }
        if (battleConfirmPanelController != null)
        {
            battleConfirmPanelController.OnConfirm.RemoveListener(HandleConfirmation);
            battleConfirmPanelController.OnCancel.RemoveListener(HandleCancellation);
        }
    }
}