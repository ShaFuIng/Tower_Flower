using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyUIController : MonoBehaviour
{

    [Header("Money Display")]
    [SerializeField]
    private TextMeshProUGUI moneyText;

    [Header("Warning")]
    [SerializeField]
    private TextMeshProUGUI warningText;  // TMP_Text for warning

    [SerializeField]
    private float warningFadeDuration = 0.3f;

    [SerializeField]
    private float warningDisplayDuration = 2f;

    private CanvasGroup warningCanvasGroup;
    private CanvasGroup moneyCanvasGroup;
    private Coroutine warningFadeCoroutine;

    private void Awake()
    {

        
        // Get or create CanvasGroup for warning fade effect
        if (warningText != null)
        {
            warningCanvasGroup = warningText.GetComponent<CanvasGroup>();
            if (warningCanvasGroup == null)
            {
                warningCanvasGroup = warningText.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Get or create CanvasGroup for money text fade effect
        if (moneyText != null)
        {
            moneyCanvasGroup = moneyText.GetComponent<CanvasGroup>();
            if (moneyCanvasGroup == null)
            {
                moneyCanvasGroup = moneyText.gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        // 預設先不顯示金錢（但 UI 仍然存在）
       HideUI(); 
        // Hide warning by default
        HideWarning();
    }

    private void Start()
    {


        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged += OnMoneyChanged;
            OnMoneyChanged(MoneyManager.Instance.GetCurrentMoney());
        }
    }


    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    /// <summary>
    /// Called when money amount changes.
    /// Updates the displayed money value.
    /// </summary>
    private void OnMoneyChanged(int newAmount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {newAmount}";
        }
    }

    /// <summary>
    /// Show the money display UI (called by TowerDefenseUIManager).
    /// </summary>
    public void ShowUI()
    {
        if (moneyText != null)
        {
            moneyText.gameObject.SetActive(true);
        }
    }

    public void HideUI()
    {
        if (moneyText != null)
        {
            moneyText.gameObject.SetActive(false);
        }

        HideWarning();
    }
    /// <summary>
    /// Show the "Not enough money" warning with fade in/out.
    /// </summary>
    public void ShowNotEnoughMoneyWarning()
    {
        if (warningText == null || warningCanvasGroup == null)
        {
            Debug.LogWarning("[MoneyUIController] Warning text or canvas group not configured!");
            return;
        }

        // Stop any existing fade coroutine
        if (warningFadeCoroutine != null)
        {
            StopCoroutine(warningFadeCoroutine);
        }

        // Start new fade sequence
        warningFadeCoroutine = StartCoroutine(FadeWarningSequence());
    }

    /// <summary>
    /// Coroutine: Fade in, hold, fade out warning.
    /// </summary>
    private IEnumerator FadeWarningSequence()
    {
        warningText.gameObject.SetActive(true);

        // Fade in
        yield return FadeWarning(from: 0f, to: 1f, duration: warningFadeDuration);

        // Hold
        yield return new WaitForSeconds(warningDisplayDuration);

        // Fade out
        yield return FadeWarning(from: 1f, to: 0f, duration: warningFadeDuration);

        // Hide
        HideWarning();
    }

    /// <summary>
    /// Coroutine: Fade CanvasGroup alpha from 'from' to 'to' over duration.
    /// </summary>
    private IEnumerator FadeWarning(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            warningCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        warningCanvasGroup.alpha = to;
    }

    /// <summary>
    /// Immediately hide the warning (called when done fading).
    /// </summary>
    private void HideWarning()
    {
        if (warningText != null)
        {
            warningCanvasGroup.alpha = 0f;
            warningText.gameObject.SetActive(false);
        }
    }
}
