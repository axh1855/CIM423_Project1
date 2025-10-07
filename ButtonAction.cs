using UnityEngine;
using System.Collections;

public class ButtonActionSmooth : MonoBehaviour
{
    [Header("Objects to Hide (disable renderers only)")]
    public GameObject itemToHide1;
    public GameObject itemToHide2;
    public GameObject itemToHide3;

    [Header("Objects to Move Up (to Y = 0.015)")]
    public GameObject item1ToMove;
    public GameObject item2ToMove;

    [Header("Third Object to Move (to Y = 0.877)")]
    public GameObject item3ToMove;

    [Header("Movement Settings")]
    public float move015Duration = 3f;  // seconds to reach Y = 0.015
    public float move0877Duration = 5f; // seconds to reach Y = 0.877

    public void OnButtonClick()
    {
        // Hide the three objects (just disable their renderers)
        HideRenderer(itemToHide1);
        HideRenderer(itemToHide2);
        HideRenderer(itemToHide3);

        // Move the two objects to Y = 0.015
        if (item1ToMove != null)
            StartCoroutine(MoveY(item1ToMove.transform, 0.015f, move015Duration));
        if (item2ToMove != null)
            StartCoroutine(MoveY(item2ToMove.transform, 0.015f, move015Duration));

        // Move the third object to Y = 0.877
        if (item3ToMove != null)
            StartCoroutine(MoveY(item3ToMove.transform, 0.877f, move0877Duration));
    }

    private void HideRenderer(GameObject obj)
    {
        if (obj == null) return;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        // Hide all child renderers as well
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }

    private IEnumerator MoveY(Transform obj, float targetY, float time)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos   = new Vector3(startPos.x, targetY, startPos.z);
        float elapsed    = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            obj.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        obj.position = endPos;
    }
}












