using UnityEngine;

public class NitroParticleHandler : MonoBehaviour
{
    private float particleEmissionRate = 0;
    private CarController carController;
    private ParticleSystem[] particleSystemNitro;

    //ParticleSystem particleSystemSmoke;
    private ParticleSystem.EmissionModule particleSystemEmissionModule;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
        particleSystemNitro = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Reduce the particles over time. 
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        foreach (ParticleSystem particle in particleSystemNitro)
        {
            particleSystemEmissionModule = particle.emission;
            particleSystemEmissionModule.rateOverTime = particleEmissionRate;
        }

        if (carController.IsNitro())
        {
            particleEmissionRate = 30;
        }
        else
        {
            particleEmissionRate = 0;
        }
    }
}
