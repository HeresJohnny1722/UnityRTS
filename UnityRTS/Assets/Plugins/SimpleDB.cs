using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;



//Saving
//Each building in the building will add to a table called "buildings"
//5 fields, name, health, x pos, y pos, z pos,

public class SimpleDB : MonoBehaviour
{

    //public static SimpleDB instance { get; private set; }

    

    // the name of the db, only change "RTS"
    // here so all the methods can access it
    private string dbName = "URI=file:RTS.db";

    void Start()
    {
        
        
        //create the table
        CreateDB();

        //Add entry
        //AddWeapon("Silver Sword", 30);

        //Display Recrods to console
        //DisplayWeapons();

    }

    private void OnApplicationQuit()
    {
//        SaveGame();

        
    }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS buildings (name VARCHAR(20), health INT, xPos FLOAT, yPos FLOAT, zPos FLOAT,);";
                command.ExecuteNonQuery();
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

    public void DisplayWeapons()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {

                //select what you want to get
                //this just sets the parameters of what will be returned
                command.CommandText = "SELECT * FROM weapons;";

                //iterate through the recordset that was returned from the statement above

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Name: " + reader["name"] + "\tDamage: " + reader["damage"]);
                    }

                    reader.Close();
                }

            }

            connection.Close();
        }
    }


    /*public void SaveGame()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            /*for (int i = 1; i < this.buildingsList.Count; i++)
        {
            data.buildingsListName.Add(this.buildingsList[i].name);
            data.buildingsListPositions.Add(this.buildingsList[i].transform.position);
            data.buildingListHealth.Add((int)this.buildingsList[i].GetComponent<Building>().buildingHealth);
        }*/
            /*Buoi.Instance.SelectBuilding(hit.transform);

            for (int i = 1; i < BuildingSelection.instance; i++)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO weapons (name, damage) VALUES ('" + weaponName + "', '" + weaponDamage + "');";
                    command.ExecuteNonQuery();
                }
            }
            

            connection.Close();
        }
    }
            */


}
