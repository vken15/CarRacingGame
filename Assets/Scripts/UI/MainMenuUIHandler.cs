using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    [SerializeField] private Canvas optionCanvas;

    //Play button
    public void OnPlay()
    {
        SceneManager.LoadScene("MapSelectionMenu");
    }
    public void OnTutorial()
    {
        //SceneManager.LoadScene("Tutorial");
        //SceneManager.LoadScene("MultiplayerMapTest");
        GameManager.instance.networkStatus = NetworkStatus.online;
        SceneManager.LoadScene("MultiplayerMenu");
    }
    //Options button
    public void OnOptions()
    {
        optionCanvas.enabled = true;
    }
    //Quit button
    public void OnQuit()
    {
        Application.Quit();
    }
}
