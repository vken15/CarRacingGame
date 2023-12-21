using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum AIDifficult { easy, normal, hard, veryHard }

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followWayPoints, followPlayer, followMouse}

    [Header("AI settings")]
    public AIMode aiMode;
    [SerializeField] private AIDifficult aiDifficult;
    [SerializeField] private float baseSpeed = 45;
    public bool isAvoidingCars = true;
    [SerializeField] private float track = 20.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float aiSpeedLevel = 1.0f;

    private Vector3 targetPosition = Vector3.zero;
    private Transform targetTransform = null;
    private float orignalBaseSpeed = 0;
    private float randomTrack;

    //Avoidance
    private Vector2 avoidanceVectorLerped = Vector2.zero;
    private bool avoidToLeft = false;

    //Stuck handling
    private bool isRunningStuckCheck = false;
    private bool isFirstTemporaryWaypoint = false;
    private int stuckCheckCounter = 0;
    private List<Vector2> temporaryWaypoints = new();
    private float angleToTarget = 0;
    private WayPointNode currentWayPoint = null;
    private WayPointNode previousWayPoint = null;
    private WayPointNode nextWayPoint = null;
    private WayPointNode[] allWayPoints;

    private CarController carController;
    private CarLayerHandler carLayerHandler;
    private AStarLite aStarLite;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        carController = GetComponent<CarController>();
        carLayerHandler = GetComponent<CarLayerHandler>();
        aStarLite = GetComponent<AStarLite>();
        allWayPoints = FindObjectsByType<WayPointNode>(FindObjectsSortMode.None);
        orignalBaseSpeed = baseSpeed;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetBaseSpeedBasedOnSpeedLevel(baseSpeed);
    }

    // Update is called once per frame and is frame dependent
    private void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown || (GameManager.instance.networkStatus == NetworkStatus.online && !NetworkManager.Singleton.IsServer))
        {
            return;
        }

        Vector2 inputVector = Vector2.zero;

        if (temporaryWaypoints.Count == 0)
        {
            switch (aiMode)
            {
                case AIMode.followPlayer:
                    FollowPlayer();
                    break;
                case AIMode.followWayPoints:
                    FollowWayPoints();
                    break;
                case AIMode.followMouse:
                    FollowMouse();
                    break;
            }
        }
        else FollowTemporaryWayPoints();

        inputVector.x = TurnTowardTarget();
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);

        if (aiMode != AIMode.followMouse)
        {
            //If the AI is applying throttle but not manging to get any speed then lets run our stuck check.
            if (carController.GetVelocityMagnitude() < 0.5f && Mathf.Abs(inputVector.y) > 0.01f && !isRunningStuckCheck)
                StartCoroutine(StuckCheckCO());

            //Handle special case where the car has reversed for a while then it should check if it is still stuck. If it is not then it will drive forward again.
            if (stuckCheckCounter >= 4 && !isRunningStuckCheck)
                StartCoroutine(StuckCheckCO());
        }

        carController.SetInputVector(inputVector);
        carController.ActiveNitro(ActiveNitroCheck(inputVector.x));
    }

    private void FollowPlayer()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            targetPosition = targetTransform.position;
        }
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition = mousePosition;
    }

    private void FollowWayPoints()
    {
        if (currentWayPoint == null)
        {
            currentWayPoint = FindClosestWayPoint();
            previousWayPoint = currentWayPoint;
            nextWayPoint = currentWayPoint.nextWayPointNode[Random.Range(0, currentWayPoint.nextWayPointNode.Length)];
            randomTrack = Random.Range(track, 20.0f);
        }
        else
        {
            targetPosition = currentWayPoint.transform.position;
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;
            //Navigate towards nearest point on line
            if (distanceToWayPoint > track)
            {
                Vector3 nearestPointOnTheWayPointLine = FindNearestPointOnLine(previousWayPoint.transform.position, currentWayPoint.transform.position, transform.position);
                float segments = distanceToWayPoint / randomTrack;
                targetPosition = (targetPosition + nearestPointOnTheWayPointLine * segments) / (segments + 1);
                Debug.DrawLine(transform.position, targetPosition, Color.black);
            }
            
            float distanceToPreviousWayPoint = (previousWayPoint.transform.position - transform.position).magnitude;
            if (distanceToPreviousWayPoint <= previousWayPoint.minDistanceToReachWayPoint)
            {
                ReduceSpeedOnCorner();
            }
            else
            {
                SetBaseSpeedBasedOnSpeedLevel(100);
            }
            float currentMinDistanceToReachWayPoint = GetMinDistanceToReachWayPoint();
            if (distanceToWayPoint <= currentMinDistanceToReachWayPoint)
            {
                randomTrack = Random.Range(track, 20.0f);
                previousWayPoint = currentWayPoint;
                currentWayPoint = nextWayPoint;
                nextWayPoint = nextWayPoint.nextWayPointNode[Random.Range(0, currentWayPoint.nextWayPointNode.Length)];
            }
        }
    }

    private void FollowTemporaryWayPoints()
    {
        //Set the target position of for the AI. 
        targetPosition = temporaryWaypoints[0];

        //Store how close we are to the target
        float distanceToWayPoint = (targetPosition - transform.position).magnitude;

        //Drive a bit slower than usual
        SetBaseSpeedBasedOnSpeedLevel(10);

        //Check if we are close enough to consider that we have reached the waypoint
        float minDistanceToReachWaypoint = 1.5f;

        if (!isFirstTemporaryWaypoint)
            minDistanceToReachWaypoint = 3.0f;

        if (distanceToWayPoint <= minDistanceToReachWaypoint)
        {
            temporaryWaypoints.RemoveAt(0);
            isFirstTemporaryWaypoint = false;
        }
    }

    private WayPointNode FindClosestWayPoint()
    {
        return allWayPoints.OrderBy(w => Vector3.Distance(transform.position, w.transform.position)).FirstOrDefault();
    }

    private float TurnTowardTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();
        if (isAvoidingCars && !carController.IsJumping())
        {
            AvoidCars(vectorToTarget, out vectorToTarget);
        }
        angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;
        float steerAmount = angleToTarget / 45.0f;
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        return steerAmount;
    }

    private void ReduceSpeedOnCorner()
    {
        Vector2 vectorToNextTarget = currentWayPoint.transform.position - previousWayPoint.transform.position;
        float docProduct = Vector2.Dot(transform.up.normalized, vectorToNextTarget.normalized);
        if (docProduct < 0.9f)
        {
            float speedReduceOnCorner = orignalBaseSpeed * Random.Range(0.1f, 0.33f);
            SetBaseSpeedBasedOnSpeedLevel(speedReduceOnCorner);
        }
        else
        {
            SetBaseSpeedBasedOnSpeedLevel(100);
        }
    }

    private float ApplyThrottleOrBrake(float x)
    {
        if (carController.GetVelocityMagnitude() > baseSpeed)
        {
            return 0;
        }

        if (aiMode == AIMode.followMouse && InputManager.instance.Controllers.Mouse_Controls.Back.IsPressed())
        {
            return -1;
        }

        //Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to apply less speed forward. We store this as reduceSpeedDueToCornering so we can use it togehter with the speed level
        float reduceSpeedDueToCornering = Mathf.Abs(x) / 1.0f;

        //Apply throttle based on cornering.
        float throttle = 1.05f - reduceSpeedDueToCornering * aiSpeedLevel;

        //Handle throttle differently when we are following temp waypoints
        if (temporaryWaypoints.Count() != 0)
        {
            //If the angle is larger to reach the target the it is better to reverse. 
            if (angleToTarget > 70)
                throttle *= -1;
            else if (angleToTarget < -70)
                throttle *= -1;
            //If we are still stuck after a number of attempts then reverse for a number of attempts.
            else if (stuckCheckCounter > 3 && stuckCheckCounter < 10)
                throttle *= -1;
        }

        //Apply throttle based on cornering and speed level.
        return throttle;
    }

    private void SetBaseSpeedBasedOnSpeedLevel(float newSpeed)
    {
        baseSpeed = Mathf.Clamp(newSpeed, 0, orignalBaseSpeed);

        float speedMulti = Mathf.Clamp(aiSpeedLevel, 0.3f, 1.0f);
        baseSpeed *= speedMulti;
    }

    private Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint,lineHeadingVector);
        dotProduct = Mathf.Clamp(dotProduct, 0.0f, maxDistance);
        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    private bool IsCarsInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector)
    {
        
        //polygonCollider2D.enabled = false;
        RaycastHit2D raycastHit2D;
        if (carLayerHandler.IsDrivingOnOverpass())
        {
            raycastHit2D = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 1.2f, transform.up, 12, 1 << LayerMask.NameToLayer("ObjectOnOverpass"));
        }
        else
        {
            raycastHit2D = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 1.2f, transform.up, 12, 1 << LayerMask.NameToLayer("ObjectOnUnderpass"));
        }
        //polygonCollider2D.enabled = true;
        
        if (raycastHit2D.collider != null)
        {
            Debug.DrawRay(transform.position, transform.up * 12, Color.red);
            position = raycastHit2D.collider.transform.position;
            otherCarRightVector = raycastHit2D.collider.transform.right;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.up * 12, Color.blue);
        }
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;
        return false;
    }

    private void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarsInFrontOfAICar(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            float angleBetweenCarAndTarget = Vector2.SignedAngle(targetPosition - transform.position, targetPosition - previousWayPoint.transform.position);
            float angleBetweenOtherCarAndTarget = Vector2.SignedAngle(targetPosition - otherCarPosition, targetPosition - previousWayPoint.transform.position);
            if (angleBetweenOtherCarAndTarget < angleBetweenCarAndTarget)
            {
                avoidToLeft = true;
            }
            else
            {
                avoidToLeft = false;
            }
            //Calculate
            Vector2 avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);
            float distanceToTarget = (targetPosition - transform.position).magnitude;
            //Desire to reach waypoint / avoid other car
            float driveToTargetInfluence = 6.0f / distanceToTarget;
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.3f, 1.0f);
            float avoidanceInfluence = 1.0f - driveToTargetInfluence;
            //Reduce jittering
            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);
            //Avoid
            newVectorToTarget = vectorToTarget * driveToTargetInfluence + avoidanceVectorLerped * avoidanceInfluence;
            float angle = Vector2.SignedAngle(transform.up, newVectorToTarget);
            if ((avoidToLeft && angle < 0) || (!avoidToLeft && angle > 0))
            {
                newVectorToTarget = Vector2.Reflect(newVectorToTarget, transform.right);
            }
            newVectorToTarget.Normalize();
            //Debug.Log("Car" + angleBetweenCarAndTarget);
            //Debug.Log("Other" + angleBetweenOtherCarAndTarget);
            //Debug.Log("angle" + angle);
            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);
            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);
            Debug.DrawRay(otherCarPosition, targetPosition - otherCarPosition, Color.cyan);
            Debug.DrawRay(transform.position, targetPosition - transform.position, Color.white);
            return;
        }
        newVectorToTarget = vectorToTarget;
    }

    private IEnumerator StuckCheckCO()
    {
        Vector3 initialStuckPosition = transform.position;

        isRunningStuckCheck = true;

        yield return new WaitForSeconds(0.7f);

        //if we have not moved for a second then we are stuck
        if ((transform.position - initialStuckPosition).sqrMagnitude < 3)
        {
            //Get a path to the desired position
            temporaryWaypoints = aStarLite.FindPath(currentWayPoint.transform.position);

            //If there was no path found then it will be null so if that happens just make a new empty list.
            temporaryWaypoints ??= new List<Vector2>();

            stuckCheckCounter++;

            isFirstTemporaryWaypoint = true;
        }
        else stuckCheckCounter = 0;

        isRunningStuckCheck = false;
    }

    private bool ActiveNitroCheck(float x)
    {
        if (temporaryWaypoints.Count == 0 && aiMode == AIMode.followWayPoints)
        {
            targetPosition = currentWayPoint.transform.position;
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;
            if (aiDifficult == AIDifficult.veryHard || (aiDifficult == AIDifficult.hard && (Mathf.Abs(x) < 0.1) && (distanceToWayPoint > track)))
            {
                return true;
            }
        }
        if (aiMode == AIMode.followMouse && InputManager.instance.Controllers.Mouse_Controls.Nitro.IsPressed())
        {
            return true;
        }
        return false;
    }
    public float GetMinDistanceToReachWayPoint()
    {
        float scale = carController.VelocityVsUp > 20 ? carController.VelocityVsUp/20 : 1;
        float driftvalue = carController.BaseDriftFactor <= 0.93f ? 0 : (carController.BaseDriftFactor - 0.93f) * 100;
        return currentWayPoint.minDistanceToReachWayPoint * scale + driftvalue;
    }
    public void SetAIDifficult(AIDifficult difficult)
    {
        aiDifficult = difficult;
        if (aiDifficult == AIDifficult.easy)
        {
            track = 16;
            aiSpeedLevel = Random.Range(0.3f, 0.5f);
        }
        else if (aiDifficult == AIDifficult.normal)
        {
            track = 18;
            aiSpeedLevel = Random.Range(0.5f, 0.8f);
        }
        else if (aiDifficult == AIDifficult.hard)
        {
            track = 19;
            aiSpeedLevel = Random.Range(0.8f, 1.0f);
            carController.BaseSpeed += Random.Range(1, 4);
        }
        else
        {
            track = 20;
            aiSpeedLevel = 1.0f;
            carController.BaseSpeed += Random.Range(4, 8);
        }
    }
}
