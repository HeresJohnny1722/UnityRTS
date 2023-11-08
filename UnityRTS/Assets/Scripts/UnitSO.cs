using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class UnitSO : ScriptableObject
{
    public enum UnitType
    {
        Gunner,
        Pikeman,
        Worker,
        Healer
    }

    public GameObject prefab;
    public UnitType unitType;
    public new string name;
    public float trainingTime;
    //public new string description;
}
