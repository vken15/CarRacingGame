using UnityEngine;
using UnityEngine.UI;

public class SetDriverItemInfo : MonoBehaviour
{
    [SerializeField] private Text driverNameText;
    [SerializeField] private Text driverPositionText;
    [SerializeField] private Text timerText;
    [SerializeField] private Slider fuelBarSlider;
    [SerializeField] private Image fuelBarFillImage;
    [SerializeField] private Image fuelBarBanImage;
    public string GetDriverName()
    {
        return driverNameText.text;
    }
    public void SetPositionText(string newPosition)
    {
        driverPositionText.text = newPosition;
    }
    public void SetDriverNameText(string newDriverName)
    {
        driverNameText.text = newDriverName;
    }
    public void SetLastFinishTime(string finishTime)
    {
        timerText.text = finishTime;
    }
    public void SetFuelBarValue(float newValue)
    {
        fuelBarSlider.value = newValue;
        if (newValue > 0.5f)
        {
            fuelBarFillImage.color = Color.green;
        }
        else if (newValue > 0.25f)
        {
            fuelBarFillImage.color = Color.yellow;
        }
        else
        {
            fuelBarFillImage.color = Color.red;
        }
        if (newValue >= 1)
        {
            fuelBarBanImage.enabled = false;
        }
        if (newValue <= 0)
        {
            fuelBarBanImage.enabled = true;
        }
    }
}
