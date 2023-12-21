using UnityEngine;

[CreateAssetMenu(fileName = "New Car Data", menuName = "Car Data", order = 51)]
public class CarData : ScriptableObject
{
    [SerializeField]
    private int carID = 0;

    [SerializeField]
    private Sprite carUISprite;

    [SerializeField]
    private GameObject carPrefab;

    public int CarID
    {
        get { return carID; }
    }
    public Sprite CarUISprite
    {
        get { return carUISprite; }
    }
    public GameObject CarPrefab
    {
        get { return carPrefab; }
    }

}
