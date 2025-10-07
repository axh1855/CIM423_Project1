// RevealOnHit.cs
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class RevealOnHit : MonoBehaviour
{
    [Header("Who is allowed to trigger this? (assign up to 3)")]
    [Tooltip("Drag your three assets here. Child-collider hits are supported.")]
    [SerializeField] private GameObject[] triggerAssets = new GameObject[3];

    [Header("OR use a tag instead (optional)")]
    [Tooltip("If set, ANY object with this tag will trigger (in addition to specific assets above).")]
    [SerializeField] private string triggerTag = ""; // e.g. "KeyAsset"

    [Header("What should appear?")]
    [Tooltip("Object to unhide/show when a valid hit happens.")]
    [SerializeField] private GameObject revealObject;

    [Tooltip("Hide the revealObject on Start so it's invisible until triggered.")]
    [SerializeField] private bool hideOnStart = true;

    [Tooltip("Delay (seconds) before revealing after a valid hit.")]
    [SerializeField] private float revealDelay = 0f;

    [Tooltip("Only reveal once; ignore later hits.")]
    [SerializeField] private bool onlyOnce = true;

    private bool hasRevealed = false;

    private void Start()
    {
        if (hideOnStart && revealObject != null)
        {
            // It's safe to reference inactive objects in the Inspector.
            revealObject.SetActive(false);
        }

        // This script works whether the collider is a trigger or not.
        // If you want trigger-based detection, set this object's Collider -> IsTrigger = true.
    }

    // Use either path depending on collider setup.
    private void OnTriggerEnter(Collider other) => TryTrigger(other);
    private void OnCollisionEnter(Collision collision) => TryTrigger(collision.collider);

    private void TryTrigger(Collider col)
    {
        if (hasRevealed && onlyOnce) return;

        GameObject hitRoot = col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;

        // 1) Match by explicit asset references (supports children colliders)
        if (MatchesAnyTriggerAsset(hitRoot))
        {
            StartCoroutine(DoReveal());
            return;
        }

        // 2) Match by tag (optional)
        if (!string.IsNullOrEmpty(triggerTag) && hitRoot.CompareTag(triggerTag))
        {
            StartCoroutine(DoReveal());
            return;
        }
    }

    private bool MatchesAnyTriggerAsset(GameObject hit)
    {
        if (triggerAssets == null) return false;

        foreach (var target in triggerAssets)
        {
            if (target == null) continue;
            if (hit == target) return true;
            if (hit.transform.IsChildOf(target.transform)) return true; // child collider of the asset
        }
        return false;
    }

    private IEnumerator DoReveal()
    {
        hasRevealed = true;
        if (revealDelay > 0f)
            yield return new WaitForSeconds(revealDelay);

        if (revealObject != null)
        {
            // Unhide/enable the object
            revealObject.SetActive(true);

            // In case only renderers/colliders were disabled, ensure they’re enabled too:
            foreach (var r in revealObject.GetComponentsInChildren<Renderer>(true)) r.enabled = true;
            foreach (var c in revealObject.GetComponentsInChildren<Collider>(true)) c.enabled = true;
            foreach (var b in revealObject.GetComponentsInChildren<Behaviour>(true))
            {
                // Avoid re-enabling this script if it happens to be on the reveal object hierarchy
                if (b != null && b != this) b.enabled = true;
            }
        }
    }
}
