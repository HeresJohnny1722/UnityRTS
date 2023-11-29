using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemyList = new List<GameObject>();
    
    public void changeTarget(GameObject unitRemoving)
    {
        Debug.Log("Changing target");
        foreach (var enemy in enemyList)
        {
            if (enemy.GetComponent<EnemyAI>().targetTransform == unitRemoving)
            {
                enemy.GetComponent<EnemyAI>().targetTransform = null;
                enemy.GetComponent<EnemyAI>().state = EnemyAI.State.Roaming;
            }
        }
    }

}
