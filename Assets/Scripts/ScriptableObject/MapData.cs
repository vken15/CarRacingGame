using UnityEngine;

[CreateAssetMenu(fileName = "New Map Data", menuName = "Map Data", order = 52)]
public class MapData : ScriptableObject
{
    [SerializeField] private int mapID = 0;
    [SerializeField] private Sprite mapUISprite;
    [SerializeField] private string scene;
    [SerializeField] private int maxCars;
    [SerializeField] private int difficulty;
    [SerializeField] private string discription = "None";

    public int MapID
    {
        get { return mapID; }
    }
    public Sprite MapUISprite
    {
        get { return mapUISprite; }
    }
    public string Scene
    {
        get { return scene; }
    }
    public int MaxCars
    { 
        get { return maxCars; } 
    }
    public int Difficulty
    {
        get { return difficulty; } 
    }
    public string Discription
    {
        get { return discription; } 
    }
}
