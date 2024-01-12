using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the saving and loading of game data and session data to a database
/// </summary>
public class SQLdatabase : MonoBehaviour
{

    private static SQLdatabase _instance;
    public static SQLdatabase Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // the name of the db, only change "RTS"
    // here so all the methods can access it
    private string dbName = "URI=file:RTS.db";

    /// <summary>
    /// Creates the Database if not already created, then loads the game
    /// </summary>
    void Start()
    {

        //create the table
        CreateDB();

        //Only load the game if its level one or two
        if (SceneManager.GetActiveScene().buildIndex != 1 || SceneManager.GetActiveScene().buildIndex != 2)
            LoadGame();

        LoadVolume();
    }

    /// <summary>
    /// Creates the tables for each saved data type, only if they dont exist already, using SQL lite
    /// </summary>
    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS buildings (name VARCHAR(20), health INT, xPos FLOAT, yPos FLOAT, zPos FLOAT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS units (name VARCHAR(20), health INT, xPos FLOAT, yPos FLOAT, zPos FLOAT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS enemies (name VARCHAR(20), health INT, xPos FLOAT, yPos FLOAT, zPos FLOAT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS waves (waveIndex INT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS population (maxPop INT, currentPop INT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS resources (gold INT, wood INT, food INT);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS sound (volume FLOAT);";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    /// <summary>
    /// Creates a new game, clears all the created tables and assign the default population and resource values
    /// </summary>
    public void NewGame()
    {
        //Clear the buildings table

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //DELETE FROM table_name
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM buildings;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM units;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM enemies;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM waves;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM population;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM resources;";
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO population (maxPop, currentPop) VALUES ('" + 9 + "', '" + 0 + "');";

                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO resources (gold, wood, food) VALUES ('" + 1000 + "', '" + 1000 + "', '" + 1000 + "');";

                command.ExecuteNonQuery();
            }



            


            connection.Close();
        }
    }

    /// <summary>
    /// Loads buildings, units, enemies and instantiates them, saved resources and wave index are reassigned to the gamemanager and wavespawner classes
    /// </summary>
    public void LoadGame()
    {
        for (int i = 1; i < BuildingSelection.Instance.buildingsList.Count; i++)
        {
            BuildingSelection.Instance.buildingsList.RemoveAt(i);
        }

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {

                //select what you want to get
                //this just sets the parameters of what will be returned

                //Building loading
                command.CommandText = "SELECT * FROM buildings;";

                //iterate through the recordset that was returned from the statement above
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        string buildingName = (string)reader["name"];
                        int savedBuildingHealth = (int)reader["health"];
                        double xPos = (double)reader["xPos"];
                        double yPos = (double)reader["yPos"];
                        double zPos = (double)reader["zPos"];
                        Vector3 buildingPosition = new Vector3((float) xPos, (float)yPos, (float)zPos);

                        GameObject prefab = FindPrefabInList(buildingName, GameManager.instance.buildingPrefabs);

                        if (prefab != null)
                        {
                            GameObject buildingObject = Instantiate(prefab, buildingPosition, Quaternion.identity);
                            Building building = buildingObject.GetComponent<Building>();
                            GameManager.instance.increaseBuildingCount(building.buildingSO);

                            building.buildingHealth = savedBuildingHealth;

                            if (building.buildingHealth != building.buildingSO.startingHealth)
                            {
                                building.buildingHealthbar.gameObject.SetActive(true);
                                building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                            }
                            else
                            {
                                building.buildingHealthbar.gameObject.SetActive(false);
                                building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Prefab not found for building: " + buildingName);
                        }
                        
                    }

                    reader.Close();
                }

                //unit loading
                command.CommandText = "SELECT * FROM units;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        string unitName = (string)reader["name"];
                        int savedUnitHealth = (int)reader["health"];
                        double xPos = (double)reader["xPos"];
                        double yPos = (double)reader["yPos"];
                        double zPos = (double)reader["zPos"];
                        Vector3 unitPosition = new Vector3((float)xPos, (float)yPos, (float)zPos);

                        GameObject prefab = FindPrefabInList(unitName, GameManager.instance.unitPrefabs);

                        if (prefab != null)
                        {

                            GameObject unitObject = Instantiate(prefab, unitPosition, Quaternion.identity);
                            Unit unit = unitObject.GetComponent<Unit>();


                            AstarAI myAstarAI = unitObject.GetComponent<AstarAI>();
                            myAstarAI.ai.destination = unitPosition;


                            unit.unitHealth = savedUnitHealth;

                            //If the unit has taken damage, we should see the healthbar
                            if (unit.unitHealth != unit.unitSO.startingHealth)
                            {
                                unit.unitHealthbar.gameObject.SetActive(true);
                                unit.unitHealthbar.UpdateHealthBar(unit.unitSO.startingHealth, unit.unitHealth);
                            }
                            else
                            {
                                unit.unitHealthbar.UpdateHealthBar(unit.unitSO.startingHealth, unit.unitHealth);
                                unit.unitHealthbar.gameObject.SetActive(false);
                            }

                        }
                        else
                        {
                            Debug.LogWarning("Prefab not found for unit: " + unitName);
                        }

                    }

                    reader.Close();
                }


                command.CommandText = "SELECT * FROM waves;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int savedWaveIndex = (int)reader["waveIndex"];
                        WaveSpawner.Instance.waveIndex = savedWaveIndex;

                    }

                    reader.Close();
                }

                command.CommandText = "SELECT * FROM population;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int currentPop = (int)reader["currentPop"];
                        int maxPop = (int)reader["maxPop"];
                        GameManager.instance.currentPopulation = currentPop;
                        GameManager.instance.maxedPopulation = maxPop;

                    }

                    reader.Close();
                }

                command.CommandText = "SELECT * FROM resources;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        int savedGold = (int)reader["gold"];

                        Debug.Log("Gold: " + savedGold);
                        int savedWood = (int)reader["wood"];
                        int savedFood = (int)reader["food"];
                        GameManager.instance.gold = savedGold;
                        GameManager.instance.wood = savedWood;
                        GameManager.instance.food = savedFood;

                    }

                    reader.Close();
                }

                //enemy loading
                command.CommandText = "SELECT * FROM enemies;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string enemyName = (string)reader["name"];
                        int savedEnemyHealth = (int)reader["health"];
                        double xPos = (double)reader["xPos"];
                        double yPos = (double)reader["yPos"];
                        double zPos = (double)reader["zPos"];
                        Vector3 enemyPosition = new Vector3((float)xPos, (float)yPos, (float)zPos);

                        GameObject prefab = FindPrefabInList(enemyName, GameManager.instance.enemyPrefabs);

                        if (prefab != null)
                        {
                            // Instantiate the prefab and add it to the buildingsList and set its health to the saved health
                            GameObject enemyObject = Instantiate(prefab, enemyPosition, Quaternion.identity);
                            EnemyAI enemy = enemyObject.GetComponent<EnemyAI>();

                            enemy.enemyHealth = savedEnemyHealth;

                            //If the enemy has taken damage, we should see the healthbar
                            if (enemy.enemyHealth != enemy.enemyAISO.startingHealth)
                            {
                                enemy.enemyHealthbar.gameObject.SetActive(true);
                                enemy.enemyHealthbar.UpdateHealthBar(enemy.enemyAISO.startingHealth, enemy.enemyHealth);
                            }
                            else
                            {
                                enemy.enemyHealthbar.gameObject.SetActive(false);
                                enemy.enemyHealthbar.UpdateHealthBar(enemy.enemyAISO.startingHealth, enemy.enemyHealth);
                            }

                        }
                        else
                        {
                            Debug.LogWarning("Prefab not found for enemy: " + enemyName);
                        }

                    }

                    reader.Close();
                }
            }

            connection.Close();
        }

        GameManager.instance.UpdateTextFields();

    }

    /// <summary>
    /// Finds the prefab associated with the provided name.
    /// </summary>
    private GameObject FindPrefabInList(string name, List<GameObject> objectList)
    {
        foreach (var prefab in objectList)
        {
            if (prefab.name.Length >= 5 && prefab.name.Substring(0, 5) == name.Substring(0, 5))
            {
                return prefab;
            }
        }

        return null;
    }

    /// <summary>
    /// Clears all the tables, then assigns the values you want to save to each corresponding slot
    /// </summary>
    public void SaveGame()
    {
        //Clear the buildings table

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //DELETE FROM table_name
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM buildings;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM units;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM enemies;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM waves;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM population;";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM resources;";
                command.ExecuteNonQuery();
            }

            for (int i = 1; i < BuildingSelection.Instance.buildingsList.Count; i++)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO buildings (name, health, xPos, yPos, zPos) VALUES ('" + BuildingSelection.Instance.buildingsList[i].name + "', '" + BuildingSelection.Instance.buildingsList[i].GetComponent<Building>().buildingHealth + "', '" + BuildingSelection.Instance.buildingsList[i].transform.position.x + "', '" + BuildingSelection.Instance.buildingsList[i].transform.position.y + "', '" + BuildingSelection.Instance.buildingsList[i].transform.position.z + "'); ";
                    command.ExecuteNonQuery();
                }
            }

            for (int i = 0; i < UnitSelection.Instance.unitList.Count; i++)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO units (name, health, xPos, yPos, zPos) VALUES ('" + UnitSelection.Instance.unitList[i].name + "', '" + UnitSelection.Instance.unitList[i].GetComponent<Unit>().unitHealth + "', '" + UnitSelection.Instance.unitList[i].transform.position.x + "', '" + UnitSelection.Instance.unitList[i].transform.position.y + "', '" + UnitSelection.Instance.unitList[i].transform.position.z + "'); ";
                    command.ExecuteNonQuery();
                }
            }

            for (int i = 0; i < GameManager.instance.enemies.Count; i++)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO enemies (name, health, xPos, yPos, zPos) VALUES ('" + GameManager.instance.enemies[i].name + "', '" + GameManager.instance.enemies[i].GetComponent<EnemyAI>().enemyHealth + "', '" + GameManager.instance.enemies[i].transform.position.x + "', '" + GameManager.instance.enemies[i].transform.position.y + "', '" + GameManager.instance.enemies[i].transform.position.z + "'); ";
                    command.ExecuteNonQuery();
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO waves (waveIndex) VALUES ('" + WaveSpawner.Instance.waveIndex + "');";
                command.ExecuteNonQuery();

            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO population (maxPop, currentPop) VALUES ('" + GameManager.instance.maxedPopulation + "', '" + GameManager.instance.currentPopulation + "');";

                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO resources (gold, wood, food) VALUES ('" + GameManager.instance.gold + "', '" + GameManager.instance.wood + "', '" + GameManager.instance.food + "');";

                command.ExecuteNonQuery();
            }






            connection.Close();
        }
    }

    public void SaveVolume(float volume)
    {
        //Clear the buildings table

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            //DELETE FROM table_name
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM sound;";
                command.ExecuteNonQuery();

            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO sound (volume) VALUES ('" + volume + "');";

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void LoadVolume()
    {
        
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {

                //enemy loading
                command.CommandText = "SELECT * FROM sound;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double volumeValue = (double) reader["volume"];
                        AudioListener.volume = (float) volumeValue;

                    }

                    reader.Close();
                }
            }

            connection.Close();
        }


    }
}
