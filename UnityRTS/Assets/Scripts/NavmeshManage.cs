using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshManage : MonoBehaviour
{
    private static NavmeshManage _instance;
    public static NavmeshManage Instance { get { return _instance; } }

    [SerializeField] private GameObject surfaceContainer; // GameObject containing NavMeshSurface components

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

        UpdateNavmesh();
    }

    public void UpdateNavmesh()
    {
        Debug.Log("Updating nav mesh");

        // Get all NavMeshSurface components in the surfaceContainer GameObject
        NavMeshSurface[] navMeshSurfaces = surfaceContainer.GetComponentsInChildren<NavMeshSurface>();

        // Build nav mesh for each NavMeshSurface
        foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
        {
            try
            {
                navMeshSurface.BuildNavMesh();
            } catch (Exception e)
            {

            }
            
        }

    }
}
