using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class InputDetectUIHandler : MonoBehaviour
{
    [SerializeField] private Text headerText;
    [Header("Image")]
    [SerializeField] private Image upImage;
    [SerializeField] private Image downImage;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private Image nitroImage;
    [Header("Car")]
    [SerializeField] private CarController carController;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnRaceStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (headerText.text.Contains("Player"))
        {
            Vector2 inputMove = InputManager.instance.Controllers.P1_Controls.Move.ReadValue<Vector2>();
            InputDetect(inputMove, InputManager.instance.Controllers.P1_Controls.Nitro.IsPressed());
        }
        else
        {
            Vector2 inputMove = new(Mathf.Abs(carController.SteeringInput) < 0.1 ? 0 : carController.SteeringInput, carController.AccelerationInput);
            InputDetect(inputMove, carController.IsNitro());
        }
    }

    private void InputDetect(Vector2 inputMove, bool inputNitro)
    {
        
        if (inputMove.x > 0)
        {
            leftImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            rightImage.color = new Color(0.6698113f, 0.4815575f, 0.3254272f);
        }
        else if (inputMove.x < 0)
        {
            rightImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            leftImage.color = new Color(0.6698113f, 0.4815575f, 0.3254272f);
        }
        else
        {
            leftImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            rightImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
        }
        if (inputMove.y > 0)
        {
            downImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            upImage.color = new Color(0.6698113f, 0.4815575f, 0.3254272f);
        }
        else if (inputMove.y < 0)
        {
            upImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            downImage.color = new Color(0.6698113f, 0.4815575f, 0.3254272f);
        }
        else
        {
            upImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
            downImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
        }
        if (inputNitro)
        {
            nitroImage.color = new Color(0.6698113f, 0.4815575f, 0.3254272f);
        }
        else
        {
            nitroImage.color = new Color(0.8235295f, 0.7176471f, 0.572549f);
        }
    }
}
