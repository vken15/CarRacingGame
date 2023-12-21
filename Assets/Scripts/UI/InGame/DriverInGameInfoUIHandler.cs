using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DriverInGameInfoUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject driverItemPrefab;

    private CarInputHandler[] carInputHandlers;
    private SetDriverItemInfo[] setDriverItemInfo;
    private bool isInitilized = false;

    // Start is called before the first frame update
    private void Start()
    {
        HorizontalLayoutGroup driverLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        carInputHandlers = FindObjectsByType<CarInputHandler>(FindObjectsSortMode.None).OrderBy(p => p.playerNumber).ToArray();
        setDriverItemInfo = new SetDriverItemInfo[carInputHandlers.Length];

        //Create the driver items
        for (int i = 0; i < carInputHandlers.Length; i++)
        {
            //Set the position
            GameObject driverInfoGameObject = Instantiate(driverItemPrefab, driverLayoutGroup.transform);

            setDriverItemInfo[i] = driverInfoGameObject.GetComponent<SetDriverItemInfo>();
            setDriverItemInfo[i].SetPositionText(GetPositionText(i + 1));
            setDriverItemInfo[i].SetDriverNameText(carInputHandlers[i].gameObject.name);
        }

        Canvas.ForceUpdateCanvases();
        isInitilized = true;
    }

    private void Update()
    {
        UpdateFuelBar();
    }
    public void UpdatePosition(List<CarLapCounter> lapCounters)
    {
        if (!isInitilized)
            return;

        for (int i = 0; i < lapCounters.Count; i++)
        {
            foreach (SetDriverItemInfo d in setDriverItemInfo)
                if (d.GetDriverName() == lapCounters[i].gameObject.name)
                {
                    d.SetPositionText(GetPositionText(i+1));
                    break;
                }
        }
    }
    /*
    public void UpdateTimer(CarLapCounter lapCounter , float time)
    {
        foreach (SetDriverItemInfo d in setDriverItemInfo)
            if (d.GetDriverName() == lapCounter.gameObject.name)
            {
                int raceTimeMinutes = (int)Mathf.Floor(time / 60);
                int raceTimeSeconds = (int)Mathf.Floor(time % 60);
                d.SetLastFinishTime($"{raceTimeMinutes:00}:{raceTimeSeconds:00}");
                break;
            }
    }
    */
    public void UpdateFuelBar()
    {
        for (int i = 0; i < setDriverItemInfo.Length; i++)
        {
            setDriverItemInfo[i].SetFuelBarValue(carInputHandlers[i].GetComponent<CarController>().GetNitroFuelPercent());
        }
    }

    private string GetPositionText(int position)
    {
        if (position == 1)
        {
            return position + "st";
        }
        else if (position == 2)
        {
            return position + "nd";
        }
        else if (position == 3)
        {
            return position + "rd";
        }
        else
        {
            return position + "th";
        }
    }
}
