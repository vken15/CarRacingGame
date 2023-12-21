using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSoundUIHandler : MonoBehaviour
{
    [SerializeField] private Text soundText;

    private Canvas canvas;
    private AudioClip[] bgmDatas;
    private bool isChangingSound = false;
    private int selectedSoundIndex = 0;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
    private void Start()
    {
        bgmDatas = Resources.LoadAll<AudioClip>("BGM/");
        StartCoroutine(SpawnSoundCO());
    }
    // Update is called once per frame
    private void Update()
    {
        float input = InputManager.instance.Controllers.P1_Controls.Move.ReadValue<Vector2>().x;
        if (input > 0)
            OnNext();
        if (input < 0)
            OnPrevious();
    }
    public void OnNext()
    {
        if (isChangingSound)
        {
            return;
        }
        selectedSoundIndex++;
        if (selectedSoundIndex > bgmDatas.Length - 1)
        {
            selectedSoundIndex = 0;
        }

        StartCoroutine(SpawnSoundCO());
    }
    public void OnPrevious()
    {
        if (isChangingSound)
        {
            return;
        }
        selectedSoundIndex--;
        if (selectedSoundIndex < 0)
        {
            selectedSoundIndex = bgmDatas.Length - 1;
        }

        StartCoroutine(SpawnSoundCO());
    }
    public void OnSelect()
    {
        GameManager.instance.BGM = bgmDatas[selectedSoundIndex];
        SceneManager.LoadScene("CarSelectionMenu");
    }
    public void OnCancel()
    {
        canvas.enabled = false;
    }

    private IEnumerator SpawnSoundCO()
    {
        isChangingSound = true;
        soundText.text = bgmDatas[selectedSoundIndex].name;
        yield return new WaitForSeconds(0.1f);
        isChangingSound = false;
    }
}
