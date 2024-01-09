using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all the game data for saving and loading the game
/// </summary>
[System.Serializable]
public class GameData
{
    public int enemiesKilled;
    public int currentPopulation;
    public int maxPopulation;

    public int gold;
    public int wood;
    public int food;

    public int waveIndex;
    public int enemiesSpawned;

    public List<string> buildingsListName;
    public List<Vector3> buildingsListPositions;
    public List<int> buildingListHealth;

    public List<string> unitListName;
    public List<Vector3> unitListPositions;
    public List<int> unitListHealth;

    public List<string> enemyListName;
    public List<Vector3> enemyListPositions;
    public List<int> enemyListHealth;


    //public Vector3 playerPosition;
    //public SerializableDictionary<string, bool> coinsCollected;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        this.enemiesKilled = 0;
        this.currentPopulation = 0;
        this.maxPopulation = 9;

        this.gold = 1000;
        this.wood = 1000;
        this.food = 1000;

        this.waveIndex = 0;
        this.enemiesSpawned = 0;

    buildingsListName = new List<string>();
        buildingsListPositions = new List<Vector3>();
        buildingListHealth = new List<int>();

        unitListName = new List<string>();
        unitListPositions = new List<Vector3>();
        unitListHealth = new List<int>();

        enemyListName = new List<string>();
        enemyListPositions = new List<Vector3>();
        enemyListHealth = new List<int>();


        //buildings = new ArrayList();

        //playerPosition = Vector3.zero;
        //coinsCollected = new SerializableDictionary<string, bool>();
    }
}