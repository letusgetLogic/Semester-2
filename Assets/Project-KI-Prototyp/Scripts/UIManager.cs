using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Switch to Simulation scene.
    /// </summary>
    public void OnSimulationScene()
    {
        SceneManager.LoadScene("SimulationScene");
    }

    /// <summary>
    /// OnQuitButton.
    /// </summary>
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(0);
#endif
    }
}
