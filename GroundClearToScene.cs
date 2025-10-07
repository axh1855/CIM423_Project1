// GroundClearToScene.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class GroundClearToScene : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Tag applied to the falling objects to track. Leave blank to track ANY Rigidbody.")]
    [SerializeField] private string targetTag = "FallingAsset";

    [Tooltip("How many unique objects must have touched the ground at least once.")]
    [SerializeField] private int expectedAssetCount = 15;

    [Header("Timing")]
    [Tooltip("Seconds to wait after Play before monitoring (lets items fall first).")]
    [SerializeField] private float startDelay = 3f;

    [Header("Scene Loading")]
    [Tooltip("Name of the scene to load when ground is cleared. If empty, loads build index + 1.")]
    [SerializeField] private string nextSceneName = "";

    // Internals
    private readonly HashSet<int> currentlyOnGround = new HashSet<int>(); // which RBs are touching now
    private readonly HashSet<int> everTouched      = new HashSet<int>(); // which RBs have touched at least once
    private bool monitoring = false;
    private bool loaded = false;

    private void Reset()
    {
        // Ensure the ground collider is suitable for either collision or trigger callbacks.
        // (We support both OnCollision* and OnTrigger* below.)
        var col = GetComponent<Collider>();
        if (col != null)
        {
            // No forced changes here—user can choose trigger or not in Inspector.
            // Just a friendly default if it's a new BoxCollider.
            if (col is BoxCollider box && box.size == Vector3.zero)
                box.size = new Vector3(10f, 1f, 10f);
        }
    }

    private void Start()
    {
        StartCoroutine(BeginMonitoringAfterDelay());
    }

    private IEnumerator BeginMonitoringAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        monitoring = true;
        MaybeLoadNextScene(); // in case everything cleared during delay
    }

    // ----- Collision path (use if ground collider: Is Trigger = false) -----
    private void OnCollisionEnter(Collision collision)
    {
        TryRegisterTouch(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
        TryUnregisterTouch(collision.collider);
    }

    // ----- Trigger path (use if ground collider: Is Trigger = true) -----
    private void OnTriggerEnter(Collider other)
    {
        TryRegisterTouch(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TryUnregisterTouch(other);
    }

    private void TryRegisterTouch(Collider col)
    {
        int id = GetRigidbodyId(col, out Rigidbody rb);
        if (id == -1) return;
        if (!PassesTagFilter(rb)) return;

        currentlyOnGround.Add(id);
        everTouched.Add(id);

        if (monitoring) MaybeLoadNextScene();
    }

    private void TryUnregisterTouch(Collider col)
    {
        int id = GetRigidbodyId(col, out Rigidbody rb);
        if (id == -1) return;
        if (!PassesTagFilter(rb)) return;

        currentlyOnGround.Remove(id);

        if (monitoring) MaybeLoadNextScene();
    }

    private bool PassesTagFilter(Rigidbody rb)
    {
        if (string.IsNullOrEmpty(targetTag)) return true;
        return rb.gameObject.CompareTag(targetTag);
    }

    private int GetRigidbodyId(Collider col, out Rigidbody rb)
    {
        rb = col.attachedRigidbody;
        if (rb == null) rb = col.GetComponentInParent<Rigidbody>();
        if (rb == null) return -1; // not a rigidbody-backed object
        return rb.GetInstanceID();
    }

    private void MaybeLoadNextScene()
    {
        if (loaded) return;

        bool seenAll = everTouched.Count >= Mathf.Max(1, expectedAssetCount);
        bool groundIsClear = currentlyOnGround.Count == 0;

        if (seenAll && groundIsClear)
        {
            loaded = true;
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int i = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(i + 1);
        }
    }
}












