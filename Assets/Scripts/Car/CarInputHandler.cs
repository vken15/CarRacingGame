using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public int playerNumber = 1;

    private CarController carController;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 inputVector = Vector2.zero;
        bool inputNitro = false;
        KeyBoardControls control = InputManager.instance.Controllers;

        switch (playerNumber)
        {
            case 1:
                /*
                inputVector.x = Input.GetAxis("Horizontal_P1");
                inputVector.y = Input.GetAxis("Vertical_P1");
                inputNitro = Input.GetAxis("Nitro_P1");
                */
                inputVector = control.P1_Controls.Move.ReadValue<Vector2>();
                inputNitro = control.P1_Controls.Nitro.IsPressed();
                break;
            case 2:
                inputVector = control.P2_Controls.Move.ReadValue<Vector2>();
                inputNitro = control.P2_Controls.Nitro.IsPressed();
                break;
            case 3:
                inputVector = control.P3_Controls.Move.ReadValue<Vector2>();
                inputNitro = control.P3_Controls.Nitro.IsPressed();
                break;
        }

        /*
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        */
        
        carController.SetInputVector(inputVector);
        carController.ActiveNitro(inputNitro);
        /*
        if (Input.GetButtonDown("Jump"))
        {
            carController.Jump(1.0f, 0.0f);
        }
        */
    }
}
