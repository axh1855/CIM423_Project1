using UnityEngine;

[DisallowMultipleComponent]
public class HideOnButtonPress : MonoBehaviour
{
    [Header("What to hide")]
    [Tooltip("Leave empty to hide THIS GameObject. Or assign another object to hide.")]
    [SerializeField] private GameObject target;

    [Header("Keyboard shortcut")]
    [SerializeField] private bool listenForKey = true;
    [SerializeField] private KeyCode key = KeyCode.H;

    [Header("Behavior")]
    [Tooltip("If true, the first press hides it and later presses do nothing.")]
    [SerializeField] private bool onlyOnce = true;

    private bool hasHidden = false;
    private GameObject Target => target != null ? target : gameObject;

    private void Update()
    {
        if (!listenForKey) return;
        if (hasHidden && onlyOnce) return;

        if (Input.GetKeyDown(key))
        {
            HideNow();
        }
    }

    /// <summary>Call this from a UI Button's OnClick to hide immediately.</summary>
    public void HideNow()
    {
        if (hasHidden && onlyOnce) return;
        if (Target != null) Target.SetActive(false);
        hasHidden = true;
    }
}
