using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hidden : MonoBehaviour
{
    public LayerMask startingLayer;
    public LayerMask fogLayer;
    public LayerMask hiddenLayer;

    // Update is called once per frame
    void Update()
    {
        Vector3 boxSize = transform.localScale;

        Collider[] hitColliders = Physics.OverlapBox(transform.position, boxSize / 2.5f, Quaternion.identity, fogLayer);

        if (hitColliders.Length > 0)
        {
            //Is touching something on the fog layer
            //Hide Object
            if (gameObject.layer != LayerMask.NameToLayer("Hidden"))
            {
                Debug.Log("Hiding Object");
                gameObject.layer = LayerMask.NameToLayer("Hidden");
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Hidden");
                }
            }
            
        } else
        {
            //transform.gameObject.layer = startingLayer;

            if (gameObject.layer != LayerMask.NameToLayer("Default"))
            {
                Debug.Log("Showing Object");
                gameObject.layer = LayerMask.NameToLayer("Default");
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
                
        }
    }
}
