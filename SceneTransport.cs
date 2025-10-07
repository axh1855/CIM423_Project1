using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransport : MonoBehaviour
{
    // Name of the scene you want to load (must match Build Settings)
    public string sceneName;

    // Call this method to load the new scene
    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}












