using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class KeyboardBindingsUIHandler : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private Text playerTxt;
    [Header("Input Text")]
    [SerializeField] private Text moveFowardTxt;
    [SerializeField] private Text moveBackTxt;
    [SerializeField] private Text driftLeftTxt;
    [SerializeField] private Text driftRightTxt;
    [SerializeField] private Text nitroTxt;
    [SerializeField] private Text escTxt;
    [Header("Canvas")]
    [SerializeField] private Canvas wattingCanvas;

    private KeyBoardControls controllers;
    private int currentPlayerInput = 1;
    private InputAction move;
    private InputAction nitro;
    private Canvas canvas;

    private RebindingOperation rebindingOperation;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        controllers = InputManager.instance.Controllers;
    }
    private void Start()
    {
        controllers = InputManager.instance.Controllers;
        ChangePlayerInput();
    }

    private void ChangeInputDisplay()
    {
        moveFowardTxt.text = InputControlPath.ToHumanReadableString(move.bindings[1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        moveBackTxt.text = InputControlPath.ToHumanReadableString(move.bindings[2].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        driftLeftTxt.text = InputControlPath.ToHumanReadableString(move.bindings[3].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        driftRightTxt.text = InputControlPath.ToHumanReadableString(move.bindings[4].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        nitroTxt.text = InputControlPath.ToHumanReadableString(nitro.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        escTxt.text = InputControlPath.ToHumanReadableString(controllers.P1_Controls.ESC.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void ChangePlayerInput()
    {
        switch (currentPlayerInput)
        {
            case 1:
                playerTxt.text = "Player 1";
                move = controllers.P1_Controls.Move;
                nitro = controllers.P1_Controls.Nitro;
                break;
            case 2:
                playerTxt.text = "Player 2";
                move = controllers.P2_Controls.Move;
                nitro = controllers.P2_Controls.Nitro;
                break;
            case 3:
                playerTxt.text = "Player 3";
                move = controllers.P3_Controls.Move;
                nitro = controllers.P3_Controls.Nitro;
                break;
        }
        ChangeInputDisplay();
    }

    public void OnMoveFowardBtn()
    {
        controllers.Disable();
        wattingCanvas.enabled = true;
        moveFowardTxt.enabled = false;
        rebindingOperation = move.PerformInteractiveRebinding()
            .WithTargetBinding(1)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .WithControlsExcluding(move.bindings[2].effectivePath)
            .WithControlsExcluding(move.bindings[3].effectivePath)
            .WithControlsExcluding(move.bindings[4].effectivePath)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(_ =>
            {
                moveFowardTxt.enabled = true;
                RebindComplete();
            })
            .OnComplete(operation => 
            {
                moveFowardTxt.enabled = true;
                RebindComplete();
                moveFowardTxt.text = InputControlPath.ToHumanReadableString(move.bindings[1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice); 
            }).Start();
    }
    public void OnMoveBackBtn()
    {
        controllers.Disable();
        wattingCanvas.enabled = true;
        moveBackTxt.enabled = false;
        rebindingOperation = move.PerformInteractiveRebinding()
            .WithTargetBinding(2)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .WithControlsExcluding(move.bindings[1].effectivePath)
            .WithControlsExcluding(move.bindings[3].effectivePath)
            .WithControlsExcluding(move.bindings[4].effectivePath)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(_ =>
            {
                moveBackTxt.enabled = true;
                RebindComplete();
            })
            .OnComplete(operation =>
            {
                moveBackTxt.enabled = true;
                RebindComplete();
                moveBackTxt.text = InputControlPath.ToHumanReadableString(move.bindings[2].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }).Start();
    }
    public void OnDriftLeftBtn()
    {
        controllers.Disable();
        wattingCanvas.enabled = true;
        driftLeftTxt.enabled = false;
        rebindingOperation = move.PerformInteractiveRebinding()
            .WithTargetBinding(3)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .WithControlsExcluding(move.bindings[2].effectivePath)
            .WithControlsExcluding(move.bindings[1].effectivePath)
            .WithControlsExcluding(move.bindings[4].effectivePath)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(_ =>
            {
                driftLeftTxt.enabled = true;
                RebindComplete();
            })
            .OnComplete(operation =>
            {
                driftLeftTxt.enabled = true;
                RebindComplete();
                driftLeftTxt.text = InputControlPath.ToHumanReadableString(move.bindings[3].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }).Start();
    }
    public void OnDriftRightBtn()
    {
        controllers.Disable();
        wattingCanvas.enabled = true;
        driftRightTxt.enabled = false;
        rebindingOperation = move.PerformInteractiveRebinding()
            .WithTargetBinding(4)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .WithControlsExcluding(move.bindings[2].effectivePath)
            .WithControlsExcluding(move.bindings[3].effectivePath)
            .WithControlsExcluding(move.bindings[1].effectivePath)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(_ =>
            {
                driftRightTxt.enabled = true;
                RebindComplete();
            })
            .OnComplete(operation =>
            {
                driftRightTxt.enabled = true;
                RebindComplete();
                driftRightTxt.text = InputControlPath.ToHumanReadableString(move.bindings[4].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }).Start();
    }
    public void OnNitroBtn()
    {
        controllers.Disable();
        wattingCanvas.enabled = true;
        nitroTxt.enabled = false;
        rebindingOperation = nitro.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(_ =>
            {
                nitroTxt.enabled = true;
                RebindComplete();
            })
            .OnComplete(operation => 
            {
                nitroTxt.enabled = true;
                RebindComplete();
                nitroTxt.text = InputControlPath.ToHumanReadableString(nitro.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }).Start();
    }
    public void OnESCBtn()
    {
        InputAction esc = controllers.P1_Controls.ESC;
        controllers.Disable();
        wattingCanvas.enabled = true;
        escTxt.enabled = false;
        rebindingOperation = esc.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<keyboard>/anyKey")
            .OnComplete(operation => 
            {
                escTxt.enabled = true;
                RebindComplete();
                escTxt.text = InputControlPath.ToHumanReadableString(esc.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }).Start();
    }
    private void RebindComplete()
    {
        rebindingOperation.Dispose();
        controllers.Enable();
        wattingCanvas.enabled = false;
    }
    public void OnResetBindings()
    {
        controllers.RemoveAllBindingOverrides();
        ChangeInputDisplay();
        PlayerPrefs.DeleteKey("rebinds");
    }

    public void OnNextPlayerInput()
    {
        if (currentPlayerInput < 3)
            currentPlayerInput++;
        else
            currentPlayerInput = 1;
        ChangePlayerInput();
    }
    public void OnPreviousPlayerInput()
    {
        if (currentPlayerInput > 1)
            currentPlayerInput--;
        else
            currentPlayerInput = 3;
        ChangePlayerInput();
    }

    public void OnClose()
    {
        canvas.enabled = false;
    }
}
