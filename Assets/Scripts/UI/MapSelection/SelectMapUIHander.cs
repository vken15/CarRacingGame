using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMapUIHander : MonoBehaviour
{
    [Header("Map prefab")]
    [SerializeField] private GameObject mapPrefab;

    [Header("Spawn on")]
    [SerializeField] private Transform spawnOnTransform;

    [Header("Sound Canvas")]
    [SerializeField] private Canvas soundSelectionCanvas;

    [Header("Map infomation")]
    [SerializeField] private Text maxCarsText;
    [SerializeField] private Text difficultyText;
    [SerializeField] private Text discriptionText;
    [SerializeField] private Text timerText;

    private bool isChangingMap = false;
    private MapData[] mapDatas;
    private int selectedMapIndex = 0;
    private MapUIHandler mapUIHandler = null;
    private int numberOfLaps = 2;

    // Start is called before the first frame update
    private void Start()
    {
        //Load the map Data
        mapDatas = Resources.LoadAll<MapData>("MapData/");
        StartCoroutine(SpawnMapCO(true));
    }

    // Update is called once per frame
    private void Update()
    {
        float input = InputManager.instance.Controllers.P1_Controls.Move.ReadValue<Vector2>().x;
        if (input > 0)
            OnNextMap();
        if (input < 0)
            OnPreviousMap();
    }
    public void OnNextMap()
    {
        if (isChangingMap)
        {
            return;
        }
        selectedMapIndex++;
        if (selectedMapIndex > mapDatas.Length - 1)
        {
            selectedMapIndex = 0;
        }

        StartCoroutine(SpawnMapCO(false));
    }
    public void OnPreviousMap()
    {
        if (isChangingMap)
        {
            return;
        }
        selectedMapIndex--;
        if (selectedMapIndex < 0)
        {
            selectedMapIndex = mapDatas.Length - 1;
        }

        StartCoroutine(SpawnMapCO(true));
    }
    public void OnSelectMap()
    {
        GameManager.instance.SetMap(mapDatas[selectedMapIndex]);
        GameManager.instance.SetNumberOfLaps(numberOfLaps);
        soundSelectionCanvas.enabled = true;
    }
    public MapData GetMapData()
    {
        return mapDatas[selectedMapIndex];
    }

    private IEnumerator SpawnMapCO(bool isMapAppearingOnRightSide)
    {
        isChangingMap = true;
        if (mapUIHandler != null)
        {
            mapUIHandler.StartMapExitAnimation(!isMapAppearingOnRightSide);
        }
        GameObject intantiatedMap = Instantiate(mapPrefab, spawnOnTransform);
        mapUIHandler = intantiatedMap.GetComponent<MapUIHandler>();
        mapUIHandler.SetupMap(mapDatas[selectedMapIndex]);
        mapUIHandler.StartMapEntranceAnimation(isMapAppearingOnRightSide);

        maxCarsText.text = "Number of cars: " + mapDatas[selectedMapIndex].MaxCars.ToString();
        difficultyText.text = "Difficulty: " + mapDatas[selectedMapIndex].Difficulty.ToString();
        discriptionText.text = "Discription: " + mapDatas[selectedMapIndex].Discription;
        string key = mapDatas[selectedMapIndex].Scene + "Best Time";
        string bestTime = PlayerPrefs.GetString(key);
        if (bestTime.Length == 5)
            timerText.text = bestTime;
        else
            timerText.text = "--:--";

        yield return new WaitForSeconds(0.4f);
        isChangingMap = false;
    }

    public void NumberOfLapsValueChange(string value)
    {
        numberOfLaps = int.Parse(value);
    }
}
