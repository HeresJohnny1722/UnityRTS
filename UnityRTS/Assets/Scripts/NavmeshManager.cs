using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavmeshManager : MonoBehaviour
{
    private static NavmeshManager _instance;
    public static NavmeshManager Instance { get { return _instance; } }

    [SerializeField] private NavMeshSurface surface;

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

    public void UpdateNavmesh()
    {
        Debug.Log("Updating nav mesh");
        surface.BuildNavMesh();
    }
}
