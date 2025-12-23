using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Coordinates the flow between the preparation phase, battle confirmation, and the start of the battle.
/// This script acts as a mediator, connecting UI components and triggering game events.
/// </summary>
public class BattleStartFlowController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private LevelDataLoader levelDataLoader;
    [SerializeField] private BattleStartButtonController battleStartButtonController;
    [SerializeField] private BattleConfirmPanelController battleConfirmPanelController;
    [SerializeField] private CombatStartInitializer combatInitializer;

    public UnityEvent OnBattleStartConfirmed;

    private void Start()
    {
        if (!ValidateDependencies()) return;

        // Wire up the confirmation panel events via code
        battleConfirmPanelController.OnConfirm.AddListener(HandleConfirmation);
        battleConfirmPanelController.OnCancel.AddListener(HandleCancellation);

        // Set initial UI state
        battleConfirmPanelController.Hide();
        // The start button should be hidden at the beginning of the game.
        // It will be shown by another system (e.g., BuildPhaseManager) when appropriate.
        battleStartButtonController.Hide();
    }

    /// <summary>
    /// This method should be called by the "Battle Start" button's OnClick() event in the Unity Editor.
    /// </summary>
    public void OnBattleStartButtonClicked()
    {
        // First, check if the level data was loaded correctly.
        LevelData currentLevelData = levelDataLoader.CurrentLevelData;
        if (currentLevelData == null)
        {
            Debug.LogError("Cannot show confirmation panel because LevelData failed to load. Check for JSON errors in the console.", this);
            return;
        }

        battleStartButtonController.Hide();
        battleConfirmPanelController.Show(currentLevelData);
    }

    private void HandleConfirmation()
    {
        if (combatInitializer != null)
        {
            combatInitializer.InitializeCombat();
            Debug.Log("Battle Start Confirmed! Combat has been initialized.");
            OnBattleStartConfirmed?.Invoke();
        }
        else
        {
            Debug.LogError("CombatStartInitializer is not assigned! Cannot start combat.", this);
            HandleCancellation(); // Revert to the previous state
        }
    }

    private void HandleCancellation()
    {
        battleStartButtonController.Show();
    }

    private bool ValidateDependencies()
    {
        bool isValid = true;
        if (levelDataLoader == null) { Debug.LogError("LevelDataLoader not assigned.", this); isValid = false; }
        if (battleStartButtonController == null) { Debug.LogError("BattleStartButtonController not assigned.", this); isValid = false; }
        if (battleConfirmPanelController == null) { Debug.LogError("BattleConfirmPanelController not assigned.", this); isValid = false; }
        if (combatInitializer == null) { Debug.LogError("CombatStartInitializer not assigned.", this); isValid = false; }
        return isValid;
    }

    private void OnDestroy()
    {
        if (battleConfirmPanelController != null)
        {
            battleConfirmPanelController.OnConfirm.RemoveListener(HandleConfirmation);
            battleConfirmPanelController.OnCancel.RemoveListener(HandleCancellation);
        }
    }
}