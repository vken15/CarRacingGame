using Unity.Netcode;
using UnityEngine;

public class WheelParticleHandler : NetworkBehaviour
{
    private float particleEmissionRate = 0;
    private CarController carController;
    private ParticleSystem particleSystemSmoke;
    private ParticleSystem.EmissionModule particleSystemEmissionModule;

    private NetworkVariable<float> particleEmissionRateNetwork = new(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        ParticleEmittRateServerRpc();
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
        particleSystemSmoke = GetComponent<ParticleSystem>();
        particleSystemEmissionModule = particleSystemSmoke.emission;
        particleSystemEmissionModule.rateOverTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        //Reduce the particles over time. 
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (carController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            //If the car tires are screeching then we'll emitt smoke. If the player is braking then emitt a lot of smoke.
            if (isBraking)
                particleEmissionRate = 30;
            //If the player is drifting we'll emitt smoke based on how much the player is drifting. 
            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 2;

            if (IsHost || IsServer)
            {
                particleEmissionRateNetwork.Value = particleEmissionRate;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ParticleEmittRateServerRpc()
    {
        particleEmissionRateNetwork.OnValueChanged += (float prevValue, float newValue) =>
        {
            ParticleEmittRateClientRpc();
        };
    }

    [ClientRpc]
    private void ParticleEmittRateClientRpc()
    {
        particleEmissionRate = particleEmissionRateNetwork.Value;
        if (particleEmissionRateNetwork.Value == 30) particleEmissionRateNetwork.Value = 29;
    }
}
