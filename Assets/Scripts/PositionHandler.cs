using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    [SerializeField] private List<CarLapCounter> carLapCounters = new();
    private LeaderboardUIHandler leaderboardUIHandler;
    private DriverInGameInfoUIHandler driverInGameInfoUIHandler;

    // Start is called before the first frame update
    private void Start()
    {
        CarLapCounter[] carLapCounterArray = FindObjectsByType<CarLapCounter>(FindObjectsSortMode.None);
        carLapCounters = carLapCounterArray.ToList();
        foreach (CarLapCounter lapCounters in carLapCounters)
        {
            lapCounters.OnPassCheckPoint += OnPassCheckPoint;
        }
        leaderboardUIHandler = FindFirstObjectByType<LeaderboardUIHandler>();
        if (leaderboardUIHandler != null)
        {
            leaderboardUIHandler.UpdateList(carLapCounters);
        }
        driverInGameInfoUIHandler = FindFirstObjectByType<DriverInGameInfoUIHandler>();
        if (driverInGameInfoUIHandler != null)
        {
            driverInGameInfoUIHandler.UpdatePosition(carLapCounters);
        }
    }

    private void OnPassCheckPoint(CarLapCounter carLapCounter)
    {
        //Sort
        carLapCounters = carLapCounters.OrderByDescending(s => s.GetNumberOfCheckPointsPassed()).ThenBy(s => s.GetTimeAtLastPassedCheckPoint()).ToList();
        //Get car position
        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;
        carLapCounter.CarPosition = carPosition;

        if (carLapCounter.IsRaceCompleted())
        {
            //Set player last position
            int playerNumber = carLapCounter.GetComponent<CarInputHandler>().playerNumber;

            if (GameManager.instance.networkStatus == NetworkStatus.offline || NetworkManager.Singleton.IsServer)
            {
                GameManager.instance.SetDriverLastRacePosition(playerNumber, carPosition);
                int pointReward = FindFirstObjectByType<SpawnCars>().GetNumberOfCarsSpawned() - carPosition;
                GameManager.instance.AddPoints(playerNumber, pointReward);
            }
            if (playerNumber == 1)
            {
                int numberOfLaps = GameManager.instance.GetNumberOfLaps();
                float time = GameManager.instance.GetRaceTime() / numberOfLaps;
                int raceTimeMinutes = (int)Mathf.Floor(time / 60);
                int raceTimeSeconds = (int)Mathf.Floor(time % 60);
                string key = GameManager.instance.GetMapScene() + "Best Time";
                string bestTime = $"{raceTimeMinutes:00}:{raceTimeSeconds:00}";
                string oldBestTime = PlayerPrefs.GetString(key);
                int oldTime = int.MaxValue;
                if (oldBestTime.Length == 5)
                    oldTime = int.Parse(oldBestTime[..2]) * 60 + int.Parse(oldBestTime[3..]);

                if (oldTime > time)
                    PlayerPrefs.SetString(key, bestTime);
            }
        }

        if (leaderboardUIHandler != null)
        {
            leaderboardUIHandler.UpdateList(carLapCounters);
            if (carLapCounter.IsRaceCompleted())
            {
                leaderboardUIHandler.UpdateTimer(carLapCounter, GameManager.instance.GetRaceTime());
            }
        }
        if (driverInGameInfoUIHandler != null)
        {
            driverInGameInfoUIHandler.UpdatePosition(carLapCounters);
            /*
            if (carLapCounter.IsCrossedFinishLine())
            {
                driverInGameInfoUIHandler.UpdateTimer(carLapCounter, GameManager.instance.GetRaceTime());
            }
            */
        }
    }
}
