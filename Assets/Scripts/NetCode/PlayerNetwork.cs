using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<Vector2> inputVector = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> inputNitro = new(writePerm: NetworkVariableWritePermission.Owner);
    /*
    private NetworkVariable<PlayerNetworkData> inputValue = new(writePerm: NetworkVariableWritePermission.Owner);
    struct PlayerNetworkData : INetworkSerializable
    {
        private float x, y;
        private bool nitro;

        internal Vector2 InputVector
        {
            get => new(x, y);
            set { x = value.x; y = value.y; }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref nitro);
        }
    }
    */
    private CarController carController;
    private KeyBoardControls control;

    public override void OnNetworkSpawn()
    {
        PlayerClientRPC();
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponent<CarController>();
        control = InputManager.instance.Controllers;
    }

    private void Update()
    {
        if (!IsOwner) return;
        inputVector.Value = control.P1_Controls.Move.ReadValue<Vector2>();
        inputNitro.Value = control.P1_Controls.Nitro.IsPressed();

        if (IsClient) return;
        carController.SetInputVector(inputVector.Value);
        carController.ActiveNitro(inputNitro.Value);
    }

    [ClientRpc]
    private void PlayerClientRPC()
    {
        inputVector.OnValueChanged += (Vector2 prevValue, Vector2 newValue) =>
        {
            carController.SetInputVector(inputVector.Value);
        };
        inputNitro.OnValueChanged += (bool prevValue, bool newValue) =>
        {
            carController.ActiveNitro(inputNitro.Value);
        };
    }
}
