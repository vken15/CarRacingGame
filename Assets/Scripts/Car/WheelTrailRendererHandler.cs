using Unity.Netcode;
using UnityEngine;

public class WheelTrailRendererHandler : NetworkBehaviour
{
    [SerializeField] private bool isOverpassEmitter = false;
    private CarController carController;
    private TrailRenderer trailRenderer;
    private CarLayerHandler carLayerHandler;

    private NetworkVariable<bool> isEmittingTrailNetwork = new(false, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        EmittEffectServerRpc();
    }
    
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        trailRenderer = GetComponent<TrailRenderer>();
        //Set the trail renderer to not emit in the start. 
        trailRenderer.emitting = false;
    }

    // Update is called once per frame
    private void Update()
    {
        trailRenderer.emitting = false;

        //If the car tires are screeching then we'll emitt a trail.
        if (carController.IsTireScreeching(out _, out bool _))
        {
            if ((carLayerHandler.IsDrivingOnOverpass()  && isOverpassEmitter) || (!carLayerHandler.IsDrivingOnOverpass() && !isOverpassEmitter))
            {
                trailRenderer.emitting = true;
            }
        }

        if (IsHost || IsServer)
        {
            isEmittingTrailNetwork.Value = trailRenderer.emitting;
        }

        if (IsClient)
        {
            trailRenderer.emitting = isEmittingTrailNetwork.Value;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void EmittEffectServerRpc()
    {
        isEmittingTrailNetwork.OnValueChanged += (bool prevValue, bool newValue) =>
        {
            EmittEffectClientRpc();
        };
    }

    [ClientRpc]
    private void EmittEffectClientRpc()
    {
        EmittTrail(isEmittingTrailNetwork.Value);
    }

    private void EmittTrail(bool value)
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = !value;
    }
}
