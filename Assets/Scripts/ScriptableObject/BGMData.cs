using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BGM Data", menuName = "BGM Data", order = 53)]
public class BGMData : ScriptableObject
{
    [SerializeField]
    private int bgmID = 0;

    [SerializeField]
    private GameObject bgmPrefab;
    public int BGMID
    {
        get { return bgmID; }
    }
    public GameObject BGMPrefab
    {
        get { return bgmPrefab; }
    }
}
