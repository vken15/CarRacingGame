using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CarLapCounter : NetworkBehaviour
{
    [SerializeField] private Text carPositionText;
    private int passedCheckPointNumber = 0;
    private float timeAtLastPassedCheckPoint = 0;
    private int numberOfPassedCheckPoints = 0;
    private int lapsCompleted = 0;
    private int lapsToCompleted;
    private bool isRaceCompleted = false;
    private bool check = false;
    private int carPosition = 0;
    private bool isHideRoutineRunning = false;
    private float hideUIDelayTime;
    private LapCountUIHandler lapsCountUIHandler;

    public int CarPosition { get => carPosition; set => carPosition = value; }

    public event Action<CarLapCounter> OnPassCheckPoint;

    private void Start()
    {
        lapsToCompleted = GameManager.instance.GetNumberOfLaps();

        if (!IsServer && GameManager.instance.networkStatus == NetworkStatus.online) return;

        if (CompareTag("Player"))
        {
            lapsCountUIHandler = FindFirstObjectByType<LapCountUIHandler>();
        }
    }
    public int GetNumberOfCheckPointsPassed()
    {
        return numberOfPassedCheckPoints;
    }
    public float GetTimeAtLastPassedCheckPoint()
    {
        return timeAtLastPassedCheckPoint;
    }
    public bool IsRaceCompleted()
    {
        return isRaceCompleted;
    }

    private IEnumerator ShowPositionCO(float delayUntilHidePosition)
    {
        hideUIDelayTime = delayUntilHidePosition;
        carPositionText.text = carPosition.ToString();
        carPositionText.gameObject.SetActive(true);
        if (!isHideRoutineRunning)
        {
            isHideRoutineRunning = true;
            yield return new WaitForSeconds(hideUIDelayTime);
            carPositionText.gameObject.SetActive(false);
            isHideRoutineRunning = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            //Once a car has completed the race. Stop checking any checkpoints or laps.
            if (isRaceCompleted || GameManager.instance.GetGameState() == GameStates.raceOver)
            {
                return;
            }
            
            CheckPoint checkPoint = collision.GetComponent<CheckPoint>();
            //Make sure the car is passing the checkpoints in the correct order. 1 -> 2 -> 3 ...
            if (passedCheckPointNumber + 1 == checkPoint.checkPointNumber)
            {
                passedCheckPointNumber = checkPoint.checkPointNumber;
                numberOfPassedCheckPoints++;
                timeAtLastPassedCheckPoint = Time.time;
                if (checkPoint.isFinishLine)
                {
                    passedCheckPointNumber = 0;
                    lapsCompleted++;
                    if (lapsCompleted >= lapsToCompleted)
                    {
                        isRaceCompleted = true;
                    }
                    if (!isRaceCompleted && lapsCountUIHandler != null)
                    {
                        if (IsServer)
                        {
                            UpdateLapCountClientRpc(lapsCompleted, lapsToCompleted);
                        }
                        else if (GameManager.instance.networkStatus == NetworkStatus.offline)
                        {
                            lapsCountUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToCompleted}");
                        }
                    }
                }

                OnPassCheckPoint?.Invoke(this);

                //Show the cars position
                if (isRaceCompleted)
                {
                    StartCoroutine(ShowPositionCO(100));
                    if (CompareTag("Player"))
                    {
                        GetComponent<CarInputHandler>().enabled = false;
                        CarAIHandler carAIHandler = GetComponent<CarAIHandler>();
                        carAIHandler.enabled = true;
                        carAIHandler.aiMode = CarAIHandler.AIMode.followWayPoints;
                        GetComponent<AStarLite>().enabled = true;
                    }
                    if (!check && GameManager.instance.GetGameState() != GameStates.raceOver)
                    {
                        check = true;
                        GameManager.instance.OnRaceCompleted();
                    }
                }
                else
                {
                    StartCoroutine(ShowPositionCO(1.5f));
                }
            }
        }
    }

    [ClientRpc]
    private void UpdateLapCountClientRpc(int laps, int totalLaps)
    {
        if (IsOwner)
        {
            if (lapsCountUIHandler == null)
            {
                lapsCountUIHandler = FindFirstObjectByType<LapCountUIHandler>();
            }
            Debug.Log("Client: " + OwnerClientId + " lap: " + laps);
            lapsCountUIHandler.SetLapText($"LAP {laps + 1}/{lapsToCompleted}");
        }
    }
}
