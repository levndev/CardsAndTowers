using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
    private bool active;

    public void Exit()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void ToggleActive()
    {
        active = !active;
        gameObject.SetActive(active);
    }
}
