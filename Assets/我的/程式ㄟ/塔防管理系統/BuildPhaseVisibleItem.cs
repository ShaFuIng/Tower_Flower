using UnityEngine;

public class BuildPhaseVisibleItem : MonoBehaviour
{
    public BuildPhase visibleInPhase;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (BuildPhaseManager.Instance == null)
            return;

        bool visible =
            BuildPhaseManager.Instance.currentPhase == visibleInPhase;

        gameObject.SetActive(visible);
    }
}
