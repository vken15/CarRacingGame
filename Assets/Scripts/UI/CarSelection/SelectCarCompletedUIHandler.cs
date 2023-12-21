using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCarOKButton : MonoBehaviour
{
    private CarUIData[] handlers;
    private CarData[] carDatas;
    private List<string> Listnames;
    [SerializeField] private Dropdown aiDifficultDropdown;

    private void Awake()
    {
        aiDifficultDropdown.ClearOptions();
        List<string> difficult = new();
        string[] text = { "Easy", "Normal", "Hard", "Very Hard" };
        difficult.AddRange(text);
        aiDifficultDropdown.AddOptions(difficult);
    }

    private void Start()
    {
        handlers = FindObjectsByType<CarUIData>(FindObjectsSortMode.None);
        carDatas = Resources.LoadAll<CarData>("CarData/");
        string[] names = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
        Listnames = names.ToList<string>();
        foreach (CarUIData handler in handlers)
        {
            string name = Listnames[Random.Range(0, Listnames.Count)];
            Listnames.Remove(name);
            handler.nameText.text = name;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnOK();
        }
    }
    public void OnOK()
    {
        GameManager.instance.ClearDriverList();
        int maxCars = GameManager.instance.GetMaxCars();
        List<CarData> Listcars = new(carDatas);
        foreach (CarUIData handler in handlers)
        {
            int id = handler.GetCarIDData();
            string name = handler.nameText.text;
            GameManager.instance.AddDriverToList(handler.playerNumber, name, id, handler.isAI.isOn, GetAIDifficult(), handler.currentInput, 0);
            Listnames.Remove(name);
            Listcars.Remove(carDatas[id-1]);
        }
        for (int i = 4; i <= maxCars; i++)
        {
            string name = Listnames[Random.Range(0, Listnames.Count)];
            Listnames.Remove(name);
            CarData carData = Listcars[Random.Range(0, Listcars.Count)];
            GameManager.instance.AddDriverToList(i, name, carData.CarID, true, GetAIDifficult(), InputType.keyboard, 0);
        }
        SceneManager.LoadScene(GameManager.instance.GetMapScene());
    }
    public AIDifficult GetAIDifficult()
    {
        if (aiDifficultDropdown.value == 0)
            return AIDifficult.easy;
        else if (aiDifficultDropdown.value == 1)
            return AIDifficult.normal;
        else if (aiDifficultDropdown.value == 2)
            return AIDifficult.hard;
        else return AIDifficult.veryHard;
    }
}
