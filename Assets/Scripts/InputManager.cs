using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType { keyboard, mouse}
public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    private KeyBoardControls controllers;

    public KeyBoardControls Controllers { get { return controllers; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        controllers = new();
        controllers.Enable();
        LoadRebindInputs();
    }

    private void LoadRebindInputs()
    {
        string rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            controllers.LoadBindingOverridesFromJson(rebinds);
        }
    }
    private void OnDisable()
    {
        var rebinds = controllers.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
