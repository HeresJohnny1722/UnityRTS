using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField]
    private PlacementSystem placementSystem; // Reference to your PlacementSystem

    void Start()
    {
        if (placementSystem == null)
        {
            Debug.LogError("PlacementSystem reference not set in ObstacleManager. Please assign it in the Inspector.");
            return;
        }

        // Assuming buildings are tagged with "Building" (adjust the tag accordingly)
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (var building in buildings)
        {
            // Get the grid position of the building
            Vector3Int gridPosition = placementSystem.grid.WorldToCell(building.transform.position);

            // Add the building to the buildingData
            placementSystem.buildingData.AddObjectAt(gridPosition, Vector2Int.one, 0, -1); // Assuming ID is 0, and -1 for placedObjectIndex
        }
    }
}
