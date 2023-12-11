using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public void updateMesh()
    {
        NavmeshManage.Instance.UpdateNavmesh();
    }
}
