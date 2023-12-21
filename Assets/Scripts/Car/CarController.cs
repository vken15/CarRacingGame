using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private float baseDriftFactor = 0.93f;
    [SerializeField] private float baseTurnFactor = 3.5f;
    [SerializeField] private float baseSpeed = 20.0f;
    [SerializeField] private float maxNitroFuel = 10;
    [Header("Car settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float driftFactor;
    [SerializeField] private float turnFactor;
    [SerializeField] private float accelerationFactor = 30.0f;
    [SerializeField] private float offRoadSpeedMulti = 0.9f;
    [SerializeField] private float nitroBoostMulti = 1.5f;
    [SerializeField] private float nitroFuel = 10;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer carSpriteRenderer;
    [SerializeField] private SpriteRenderer carShadowRenderer;

    [Header("Jumping")]
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private ParticleSystem landingParticleSystem;

    private float accelerationInput = 0;
    private float steeringInput = 0;
    private float rotationAngle;
    private float velocityVsUp = 0;
    private bool isJumping = false;
    private bool isPressingNitro = false;
    private bool isRechargeNitro = true;
    private Rigidbody2D carRigidbody2D;
    private Collider2D carCollider2D;
    private CarSoundEffectHandler carSoundEffectHandler;
    private CarLayerHandler carLayerHandler;

    public float AccelerationFactor { get { return accelerationFactor; } }
    public float BaseSpeed { get { return baseSpeed; } set { baseSpeed = value; } }
    public float OffRoadSpeedMulti { get { return offRoadSpeedMulti; } }
    public float BaseDriftFactor { get { return baseDriftFactor; } }
    public float BaseTurnFactor { get { return baseTurnFactor; } }
    public float NitroBoostMulti { get { return nitroBoostMulti; } }
    public float AccelerationInput { get { return accelerationInput; } }
    public float SteeringInput { get { return steeringInput; } }
    public float VelocityVsUp { get { return velocityVsUp; } }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carCollider2D = GetComponentInChildren<Collider2D>();
        carSoundEffectHandler = GetComponent<CarSoundEffectHandler>();
        carLayerHandler = GetComponent<CarLayerHandler>();
    }

    private void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
        driftFactor = baseDriftFactor;
        turnFactor = baseTurnFactor;
    }

    //Frame-rate independent for physics calculations
    private void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown)
        {
            return;
        }
        ApplySpeedChange();
        ApplyNitro();
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    private void ApplyEngineForce()
    {
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;
        //Caculate how much "forward" we are going in terms of the direction of our velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        //Apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
        if (accelerationInput == 0 || ((velocityVsUp > maxSpeed || velocityVsUp < -maxSpeed * 0.5f) && !carLayerHandler.IsDrivingOnRoad()))
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
            carRigidbody2D.drag = 0;

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        //Limit so we cannot go faster in any direction while accelerating
        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
            return;

        //Create a force for the engine
        Vector2 engineForceVector = accelerationFactor * accelerationInput * transform.up;
        //Apply force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        //Apply steering by rotating the car object
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private void ApplySpeedChange()
    {
        if (!carLayerHandler.IsDrivingOnRoad() && !isJumping)
            maxSpeed = baseSpeed * offRoadSpeedMulti;
        else
            maxSpeed = baseSpeed;
    }
    public void SetInputVector(Vector2 inputVector) 
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    private float GetLateralVelocity()
    {
        //Returns how fast the car is moving sideways. 
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }
    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;
        if (isJumping)
        {
            return false;
        }
        //Check if we are moving forward and if the player is hitting the brakes. In that case the tires should screech.
        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }
        //If we have a lot of side movement then the tires should be screeching
        if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
            return true;

        return false;
    }
    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }

    //Nitro system
    private void ApplyNitro()
    {
        if (isRechargeNitro)
            nitroFuel += Time.deltaTime / 2;

        if (nitroFuel >= maxNitroFuel)
            isRechargeNitro = false;

        if (isPressingNitro && !isRechargeNitro && accelerationInput > 0)
        {
            nitroFuel -= (nitroFuel <= 0) ? 0 : Time.deltaTime * 4;
            if (nitroFuel > 0) StartNitro();
            else StopNitro();
        }
    }
    public void ActiveNitro(bool input)
    {
        isPressingNitro = input;
    }
    public void StartNitro()
    {
        if (velocityVsUp > maxSpeed * nitroBoostMulti) return;
        Vector2 engineForceVector = accelerationFactor * accelerationInput * nitroBoostMulti * transform.up;
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }
    public void StopNitro()
    {
        isRechargeNitro = true;
    }
    public bool IsNitro()
    {
        return isPressingNitro && !isRechargeNitro;
    }
    public float GetNitroFuelPercent()
    {
        return nitroFuel/maxNitroFuel;
    }
    //Jump system
    public void Jump(float jumpHeightScale, float jumpPushScale)
    {
        if (!isJumping)
            StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale));
    }
    public bool IsJumping()
    {
        return isJumping;
    }
    private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidbody2D.velocity.magnitude * 0.05f;

        jumpHeightScale *= jumpDuration;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);

        carCollider2D.enabled = false;

        carSoundEffectHandler.PlayJumpSoundEffect();

        //Change sorting layer to flying
        carSpriteRenderer.sortingLayerName = "Flying";
        carShadowRenderer.sortingLayerName = "Flying";

        carRigidbody2D.AddForce(10 * jumpPushScale * carRigidbody2D.velocity.normalized, ForceMode2D.Impulse);

        while (isJumping)
        {
            //Percentage 0 - 1.0 of where we are in the jumping process
            float jumpCompletedPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);
            //Take the base scale of 1 and add how much we should increase the scale with
            carSpriteRenderer.transform.localScale = Vector3.one + jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale * Vector3.one;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;
            carShadowRenderer.transform.localPosition = 3 * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale * new Vector3(1, -1, 0.0f);
            if (jumpCompletedPercentage == 1.0f)
                break;
            yield return null;
        }
        //Check landing
        Collider2D overlapCollider = Physics2D.OverlapCircle(transform.position, 1.5f);
        if (overlapCollider != null && !overlapCollider.isTrigger)
        {
            isJumping = false;
            Jump(0.2f, 0.6f);
        }
        else
        {
            //Reset
            carSpriteRenderer.transform.localScale = Vector3.one;
            carShadowRenderer.transform.localPosition = Vector3.zero;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;
            //for now
            carCollider2D.enabled = true;
            //Change sorting layer to default
            carSpriteRenderer.sortingLayerName = "Default";
            carShadowRenderer.sortingLayerName = "Default";
            //Play the landing particle system if it is a bigger jump
            if (jumpHeightScale > 0.2f)
            {
                landingParticleSystem.Play();
                carSoundEffectHandler.PlayLandingSoundEffect();
            }

            //Change state
            isJumping = false;
        }
    }
    //Detect trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jump"))
        {
            JumpData jumpData = collision.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
        else if (collision.CompareTag("Pond"))
        {
            PondData pondData = collision.GetComponent<PondData>();
            driftFactor = pondData.driftFactor;
            turnFactor *= pondData.turnFactorScale;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pond"))
        {
            driftFactor = baseDriftFactor;
            turnFactor = baseTurnFactor;
        }
    }
}
