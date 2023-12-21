using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIHandler : MonoBehaviour
{
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private Canvas gameFinishCanvas;

    private Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
    private void Update()
    {
        if (InputManager.instance.Controllers.P1_Controls.ESC.WasPressedThisFrame() && !optionsCanvas.enabled && !gameFinishCanvas.enabled)
        {
            canvas.enabled = !canvas.enabled;
            if (GameManager.instance.networkStatus == NetworkStatus.offline)
            {
                AudioListener.pause = canvas.enabled;
                Time.timeScale = canvas.enabled ? 0 : 1;
            }

        }
    }
    public void OnResume()
    {
        canvas.enabled = false;
        if (GameManager.instance.networkStatus == NetworkStatus.offline)
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
        }
    }
    public void OnOptions()
    {
        optionsCanvas.enabled = true;
    }
    public void OnBackToMenu()
    {
        OnResume();
        if (GameManager.instance.networkStatus == NetworkStatus.offline)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            GameManager.instance.networkStatus = NetworkStatus.offline;
            SceneTransitionHandler.sceneTransitionHandler.ExitAndLoadStartMenu();
        }
    }
}
