using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitSO unitSO;

    void Start()
    {
        
        Selections.Instance.unitList.Add(this.gameObject);


    }

    void OnDestroy()
    {
        Selections.Instance.unitList.Remove(this.gameObject);
    }
}
