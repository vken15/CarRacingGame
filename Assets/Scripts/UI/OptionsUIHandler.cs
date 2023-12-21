using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsUIHandler : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenModeToggle;
    private Resolution[] resolutions;
    private List<Resolution> resolutionList;
    private double currentRefreshRate;
    private int currentResolutionIndex = 0;

    [Header("Sounds")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volMasterSlider;
    [SerializeField] private Slider volBGMSlider;
    [SerializeField] private Slider volSFXSlider;

    [Header("Controllers")]
    [SerializeField] private Canvas keyBindingsCanvas;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
    private void Start()
    {
        //Screens
        bool fullScreen = PlayerPrefs.GetInt("fullScreen") == 1;
        fullScreenModeToggle.isOn = fullScreen;

        resolutions = Screen.resolutions;
        resolutionList = new();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                resolutionList.Add(resolutions[i]);
            }
        }
        List<string> resolutionOptions = new();
        int currentresolutionWidth = PlayerPrefs.GetInt("resolutionWidth", -1);
        int currentresolutionHeight = PlayerPrefs.GetInt("resolutionHeight", -1);
        string currentOption = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString() + " " + currentRefreshRate + "Hz";
        if (currentresolutionWidth != -1 && currentresolutionHeight != -1)
        {
            currentOption = currentresolutionWidth + "x" + currentresolutionHeight + " " + currentRefreshRate + "Hz";
            Screen.SetResolution(currentresolutionWidth, currentresolutionHeight, fullScreen);
        }

        for (int i = 0; i < resolutionList.Count; i++)
        {
            string option = resolutionList[i].width.ToString() + "x" + resolutionList[i].height.ToString() + " " + resolutionList[i].refreshRateRatio.value + "Hz";
            resolutionOptions.Add(option);
            if (currentOption == option)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //Sounds
        float volMaster = PlayerPrefs.GetFloat("volMaster");
        float volBGM = PlayerPrefs.GetFloat("volBGM");
        float volSFX = PlayerPrefs.GetFloat("volSFX");
        volMasterSlider.value = volMaster;
        volBGMSlider.value = volBGM;
        volSFXSlider.value = volSFX;
        audioMixer.SetFloat("volMaster", Mathf.Log10(volMaster <= 0 ? 0.001f : volMaster) * 40f);
        audioMixer.SetFloat("volBGM", Mathf.Log10(volBGM <= 0 ? 0.001f : volBGM) * 40f);
        audioMixer.SetFloat("volSFX", Mathf.Log10(volSFX <= 0 ? 0.001f : volSFX) * 40f);
    }
    private void Update()
    {
        if (InputManager.instance.Controllers.P1_Controls.ESC.WasPressedThisFrame())
        {
            if (!keyBindingsCanvas.enabled)
                canvas.enabled = false;
            else
                keyBindingsCanvas.enabled = false;
        }
    }
    //Screens
    public void OnResolutionValueChange(int index)
    {
        Resolution resolution = resolutionList[index];
        bool fullScreen = PlayerPrefs.GetInt("fullScreen") == 1;
        PlayerPrefs.SetInt("resolutionWidth", resolution.width);
        PlayerPrefs.SetInt("resolutionHeight", resolution.height);
        Screen.SetResolution(resolution.width, resolution.height, fullScreen);
    }
    public void OnFullScreenToggleValueChange(bool value)
    {
        PlayerPrefs.SetInt("fullScreen", value ? 1 : 0);
        Screen.fullScreen = value;
    }
    //Sounds
    public void OnVolumeMasterChange(float vol)
    {
        float volMaster = Mathf.Log10(vol <= 0 ? 0.001f : vol) * 40f;
        PlayerPrefs.SetFloat("volMaster", vol);
        audioMixer.SetFloat("volMaster", volMaster);
    }
    public void OnVolumeBGMChange(float vol)
    {
        float volBGM = Mathf.Log10(vol <= 0 ? 0.001f : vol) * 40f;
        PlayerPrefs.SetFloat("volBGM", vol);
        audioMixer.SetFloat("volBGM", volBGM);
    }
    public void OnVolumeSFXChange(float vol)
    {
        float volSFX = Mathf.Log10(vol <= 0 ? 0.001f : vol) * 40f;
        PlayerPrefs.SetFloat("volSFX", vol);
        audioMixer.SetFloat("volSFX", volSFX);
    }
    //Controllers
    public void OnKeyBindings()
    {
        keyBindingsCanvas.enabled = true;
    }

    public void OnClose()
    {
        canvas.enabled = false;
    }
}
