using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int enemiesKilled;
    public int buildingCount;

    public int gold;
    public int wood;
    public int food;

    public List<string> buildingsList;
    public List<Vector3> buildingsListPositions;


    //public Vector3 playerPosition;
    //public SerializableDictionary<string, bool> coinsCollected;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        this.enemiesKilled = 0;
        this.buildingCount = 0;
        this.gold = 1000;
        this.wood = 1000;
        this.food = 1000;

        buildingsList = new List<string>();
        buildingsListPositions = new List<Vector3>();
        //buildings = new ArrayList();

        //playerPosition = Vector3.zero;
        //coinsCollected = new SerializableDictionary<string, bool>();
    }
}