using System;
using System.Collections;
using UnityEngine;

public class SelectCarUIHandler : MonoBehaviour
{
    [Header("Car prefab")]
    [SerializeField] private GameObject carPrefab;

    [Header("Spawn on")]
    [SerializeField] private Transform spawnOnTransform;

    private int playerNumber = 1;
    private bool isChangingCar = false;
    private CarData[] carDatas;
    private int selectedCarIndex = 0;
    private CarUIHandler carUIHandler = null;

    //Events
    public event Action<CarController> OnSpawnCarChanged;

    // Start is called before the first frame update
    private void Start()
    {
        //Load the car Data
        carDatas = Resources.LoadAll<CarData>("CarData/");
        StartCoroutine(SpawnCarCO(true));
    }

    // Update is called once per frame
    private void Update()
    {
        float input;
        switch (playerNumber)
        {
            case 1:
                input = InputManager.instance.Controllers.P1_Controls.Move.ReadValue<Vector2>().x;
                if (input > 0)
                {
                    OnNextCar();
                }
                if (input < 0)
                {
                    OnPreviousCar();
                }
                break;
            case 2:
                input = InputManager.instance.Controllers.P2_Controls.Move.ReadValue<Vector2>().x;
                if (input > 0)
                {
                    OnNextCar();
                }
                if (input < 0)
                {
                    OnPreviousCar();
                }
                break;
            case 3:
                input = InputManager.instance.Controllers.P3_Controls.Move.ReadValue<Vector2>().x;
                if (input > 0)
                {
                    OnNextCar();
                }
                if (input < 0)
                {
                    OnPreviousCar();
                }
                break;
        }
    }
    public void OnNextCar()
    {
        if (isChangingCar)
        {
            return;
        }
        selectedCarIndex++;
        if (selectedCarIndex > carDatas.Length - 1)
        {
            selectedCarIndex = 0;
        }
        
        StartCoroutine(SpawnCarCO(false));
    }
    public void OnPreviousCar()
    {
        if (isChangingCar)
        {
            return;
        }
        selectedCarIndex--;
        if (selectedCarIndex < 0)
        {
            selectedCarIndex = carDatas.Length - 1;
        }

        StartCoroutine(SpawnCarCO(true));
    }

    private IEnumerator SpawnCarCO(bool isCarAppearingOnRightSide)
    {
        isChangingCar = true;
        if (carUIHandler != null)
        {
            carUIHandler.StartCarExitAnimation(!isCarAppearingOnRightSide);
        }
        GameObject intantiatedCar = Instantiate(carPrefab, spawnOnTransform);
        carUIHandler = intantiatedCar.GetComponent<CarUIHandler>();
        carUIHandler.SetupCar(carDatas[selectedCarIndex]);
        carUIHandler.StartCarEntranceAnimation(isCarAppearingOnRightSide);
        OnSpawnCarChanged?.Invoke(carDatas[selectedCarIndex].CarPrefab.GetComponent<CarController>());
        yield return new WaitForSeconds(0.4f);
        isChangingCar = false;
    }

    public int GetCarIDData()
    {
        return carDatas[selectedCarIndex].CarID;
    }

    public void SetPlayerNumber(int number)
    {
        playerNumber = number;
    }
}   
