using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;



//Saving
//Each building in the building will add to a table called "buildings"
//5 fields, name, health, x pos, y pos, z pos,

public class SQLdatabase : MonoBehaviour
{

    //public static SimpleDB instance { get; private set; }



    // the name of the db, only change "RTS"
    // here so all the methods can access it
    private string dbName = "URI=file:RTS.db";

    void Start()
    {

        //create the table
        CreateDB();

        LoadGame();

        //Add entry
        //AddWeapon("Silver Sword", 30);

        //Display Recrods to console
        //DisplayBuildings();

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

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

                command.CommandText = "CREATE TABLE IF NOT EXISTS waves (waveIndex INT);";
            }

            

            connection.Close();
        }
    }

    public void AddWeapon(string weaponName, int weaponDamage)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO weapons (name, damage) VALUES ('" + weaponName + "', '" + weaponDamage + "');";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void DisplayBuildings()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {

                //select what you want to get
                //this just sets the parameters of what will be returned
                command.CommandText = "SELECT * FROM buildings;";

                //iterate through the recordset that was returned from the statement above

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Name: " + reader["name"] + "\tHealth: " + reader["health"]);
                    }

                    reader.Close();
                }

            }

            connection.Close();
        }
    }

    /// <summary>
    /// Loads buildings, units, enemies, resources
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

                //iterate through the recordset that was returned from the statement above

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
            }

            connection.Close();
        }

    }

    /// <summary>
    /// Finds the prefab associated with the provided building name.
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
    /// 
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
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM units;";
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



            connection.Close();
        }
    }

}
