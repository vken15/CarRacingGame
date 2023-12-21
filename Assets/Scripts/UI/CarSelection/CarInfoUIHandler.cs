using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarInfoUIHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider accelerationBar;
    [SerializeField] private Slider speedBar;
    [SerializeField] private Slider offRoadSpeedBar;
    [SerializeField] private Slider driftBar;
    [SerializeField] private Slider turnBar;
    [SerializeField] private Slider nitroBar;

    private Canvas canvas;
    private List<SelectCarUIHandler> handlers = new();
    private SelectCarUIHandler selectCarUIHandler;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        selectCarUIHandler = GetComponent<SelectCarUIHandler>();
        selectCarUIHandler.OnSpawnCarChanged += OnSpawnCarChanged;
        selectCarUIHandler.enabled = false;
        SelectCarUIHandler[] selectCarUIHandlers = FindObjectsByType<SelectCarUIHandler>(FindObjectsSortMode.None);
        handlers.AddRange(selectCarUIHandlers);
        handlers.Remove(selectCarUIHandler);
    }

    private void Update()
    {
        if (InputManager.instance.Controllers.P1_Controls.ESC.IsPressed())
        {
            canvas.enabled = false;
            foreach (SelectCarUIHandler handler in handlers)
            {
                handler.enabled = true;
            }
            selectCarUIHandler.enabled = false;
        }
    }

    public void OnCarInfomationBTN()
    {
        canvas.enabled = !canvas.enabled;
        selectCarUIHandler.enabled = !selectCarUIHandler.enabled;
        foreach (SelectCarUIHandler handler in handlers)
        {
            handler.enabled = !handler.enabled;
        }
    }

    private void OnSpawnCarChanged(CarController car)
    {
        float speed = car.BaseSpeed;
        accelerationBar.value = car.AccelerationFactor;
        speedBar.value = speed;
        offRoadSpeedBar.value = car.OffRoadSpeedMulti * speed;
        driftBar.value = car.BaseDriftFactor;
        turnBar.value = car.BaseTurnFactor;
        nitroBar.value = car.NitroBoostMulti * speed;
    }

    private void OnDestroy()
    {
        selectCarUIHandler.OnSpawnCarChanged -= OnSpawnCarChanged;
    }
}
