using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask friendlyUnitsLayerMask;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("The Bullet hit something");
        Destroy(this.gameObject);
    }


}
