using UnityEngine;

public class CarSoundEffectHandler : MonoBehaviour
{
    [Header("Audio sources")]
    [SerializeField] private AudioSource tiresScreechingAudioSource;
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource carHitAudioSource;
    [SerializeField] private AudioSource carJumpAudioSource;
    [SerializeField] private AudioSource carJumpLandingAudioSource;
    private float desiredEnginePitch = 0.5f;
    private float tireScreechPitch = 0.5f;
    private CarController carController;

    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateEngineSoundEffect();
        UpdateTiresScreechingSoundEffect();
    }

    private void UpdateEngineSoundEffect()
    {
        //Handle engine sound effect
        float velocityMagnitude = carController.GetVelocityMagnitude();

        //Increase the engine volume as the car goes faster
        float desiredEngineVolume = velocityMagnitude * 0.05f;

        //But keep a minimum level so it playes even if the car is idle
        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);

        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);

        //To add more variation to the engine sound we also change the pitch
        desiredEnginePitch = velocityMagnitude * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    private void UpdateTiresScreechingSoundEffect()
    {
        //Handle tire screeching SFX
        if (carController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            //If the car is braking we want the tire screech to be louder and also change the pitch.
            if (isBraking)
            {
                tiresScreechingAudioSource.volume = Mathf.Lerp(tiresScreechingAudioSource.volume, 1.0f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                //If we are not braking we still want to play this screech sound if the player is drifting.
                tiresScreechingAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        //Fade out the tire screech sound effect if we are not screeching. 
        else tiresScreechingAudioSource.volume = Mathf.Lerp(tiresScreechingAudioSource.volume, 0, Time.deltaTime * 10);
    }
    public void PlayJumpSoundEffect()
    {
        carJumpAudioSource.Play();
    }
    public void PlayLandingSoundEffect()
    {
        carJumpLandingAudioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        //Get the relative velocity of the collision
        float relativeVelocity = collision2D.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
        carHitAudioSource.volume = volume;

        if (!carHitAudioSource.isPlaying)
            carHitAudioSource.Play();
    }
}
