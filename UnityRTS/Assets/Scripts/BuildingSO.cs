using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class BuildingSO : ScriptableObject
{
    public enum BuildingType
    {
        House,
        Barracks
    }

    public BuildingType buildingType;
    public new string name;
    public new string description;
}
