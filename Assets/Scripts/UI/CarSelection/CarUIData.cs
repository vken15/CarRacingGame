using UnityEngine;
using UnityEngine.UI;

public class CarUIData : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text headerText;
    [Header("Player")]
    public InputField nameText;
    public int playerNumber = 1;
    public Text inputText;

    [Header("Ai settings")]
    public Toggle isAI;

    public InputType currentInput = InputType.keyboard;

    private SelectCarUIHandler selectCarUIHandler;

    private void Awake()
    {
        selectCarUIHandler = GetComponent<SelectCarUIHandler>();
        selectCarUIHandler.SetPlayerNumber(playerNumber);
    }

    public int GetCarIDData()
    {
        return selectCarUIHandler.GetCarIDData();
    }

    public void OnIsAIValueChange(bool value)
    {
        if (value)
        {
            headerText.text = "AI";
        }
        else
        {
            headerText.text = "PLAYER " + playerNumber;
        }
    }
    public void OnNextInputBTN()
    {
        if (currentInput == InputType.keyboard)
        {
            currentInput = InputType.mouse;
            inputText.text = "Mouse";
            CarUIData[] carUIDatas = FindObjectsByType<CarUIData>(FindObjectsSortMode.None);
            foreach (CarUIData carUIData in carUIDatas)
            {
                if (carUIData.currentInput == InputType.mouse && carUIData != this)
                {
                    carUIData.currentInput = InputType.keyboard;
                    carUIData.inputText.text = "Keyboard";
                }
            }
        }
        else
        {
            currentInput = InputType.keyboard;
            inputText.text = "Keyboard";
        }
    }
    /*
    public void OnPrevInputBTN()
    {
        if (currentInput == InputType.keyboard)
        {
            currentInput = InputType.mouse;
            inputText.text = "Mouse";
        }
        else
        {
            currentInput = InputType.keyboard;
            inputText.text = "Keyboard";
        }
    }
    */
}
