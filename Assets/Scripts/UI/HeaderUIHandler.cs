using UnityEngine;
using UnityEngine.SceneManagement;

public class HeaderUIHandler : MonoBehaviour
{
    [SerializeField] private string previousScene;

    public void OnReturn()
    {
        SceneManager.LoadScene(previousScene);
    }
}
